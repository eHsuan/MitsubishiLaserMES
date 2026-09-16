using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Nodes;
using MitsubishiLaserOpc.Models;
using MitsubishiLaserOpc.Services;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace MitsubishiLaserOpc.Clients
{

/// <summary>
/// Production OPC UA transport implementation. It deliberately accepts catalog descriptors,
/// so callers never need to embed a raw NodeId in business or UI code.
/// </summary>
public sealed class MitsubishiOpcUaClient : IOpcUaClient, IDisposable
{
    // OPC UA SDK 的內部診斷管道；目前不額外輸出，日後可在此接入 Serilog / NLog。
    private static readonly ITelemetryContext Telemetry = DefaultTelemetry.Create(_ => { });
    private readonly MitsubishiOpcUaOptions _options;
    private readonly IOpcAuditLogger _auditLogger;

    // 非同步版本的互斥鎖：同一時間只能有一個工作建立或關閉 Session。
    // 例如 ReadAsync 與 WriteAsync 同時發現斷線時，可避免建立兩條 OPC UA Session。
    // 不能用 lock，因為連線過程包含 await；lock 不適合跨 await 持有。
    private readonly SemaphoreSlim _connectionGate = new SemaphoreSlim(1, 1);
    private ApplicationConfiguration _configuration;
    private ISession _session;

    public MitsubishiOpcUaClient(MitsubishiOpcUaOptions options)
        : this(options, null)
    {
    }

    /// <summary>
    /// The optional logger constructor is useful for unit tests or a site-specific logging implementation.
    /// A normal production client automatically uses CsvOpcAuditLogger.
    /// </summary>
    public MitsubishiOpcUaClient(MitsubishiOpcUaOptions options, IOpcAuditLogger auditLogger)
    {
        if (options == null)
            throw new ArgumentNullException("options");

        _options = options;
        _auditLogger = auditLogger ?? new CsvOpcAuditLogger(GetAuditLogDirectory(options));
    }

    public string AuditLogFilePath { get { return _auditLogger.CurrentLogFilePath; } }

    public bool IsConnected
    {
        // 已建立且仍處於 Connected 的 Session 才視為已連線。
        // 後續 Read / Write 會重用同一個 Session，不會每次操作都重新連線。
        get { return _session != null && _session.Connected; }
    }

    /// <summary>
    /// 確保 OPC UA Session 已連線。
    /// 若 Session 已存在且仍連線，會立刻返回，不會重建連線。
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // 正常讀寫時會走這裡直接返回；不是「每次讀寫都斷線再重連」。
        if (IsConnected) return;
        if (string.IsNullOrWhiteSpace(_options.EndpointUrl))
            throw new InvalidOperationException("OPC UA EndpointUrl is required.");

        // 等待其他可能正在連線或斷線的工作完成。
        // ConfigureAwait(false) 代表函式庫不需要回到 WPF UI 執行緒再繼續執行。
        await _connectionGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 取得 gate 後要再確認一次：等待期間可能已由另一個工作完成連線。
            if (IsConnected) return;

            // 建立 Client 的憑證、安全性與 Session timeout 等設定。
            _configuration = await CreateConfigurationAsync().ConfigureAwait(false);

            // EndpointUrl 只是 Server 位址，例如 opc.tcp://192.168.1.10:4840。
            // OPC UA Server 可在同一位址提供多種 Security Policy / 加密方式。
            // SelectEndpointAsync 會詢問 Server 的可用 Endpoint，並依 UseSecurity 選出一種。
            // UseSecurity=false：選未加密 Endpoint（常用於設備測試）。
            // UseSecurity=true ：選安全 Endpoint（通常需憑證與加密）。
            var endpoint = await CoreClientUtils.SelectEndpointAsync(
                _configuration,          // Client 憑證與安全設定
                _options.EndpointUrl,    // OPC UA Server 位址
                _options.UseSecurity,    // 是否要求安全連線
                15_000,                  // 搜尋 Endpoint 最長等待 15 秒
                Telemetry,               // OPC UA SDK 診斷管道
                cancellationToken)       // 允許外部取消連線流程
                .ConfigureAwait(false);

            // 將選出的 Endpoint 套用到 Client 設定，準備建立 Session。
            var configuredEndpoint = new ConfiguredEndpoint(null, endpoint, EndpointConfiguration.Create(_configuration));
            var identity = CreateIdentity();

            // 真正建立 OPC UA Session；此後 Read / Write / Subscribe 都共用 _session。
            // 兩個 bool 分別是 updateBeforeConnect=false 與 checkDomain=false。
            // DeviceXPlorer 的 Server certificate 沒有將現場 IP (192.168.5.3) 列在 DNS/IP SAN 中；
            // 使用 UAExpert 已確認的 SecurityPolicy=None Endpoint 時，若維持 checkDomain=true，
            // SDK 會在建立 Session 前拒絕連線："domain is not listed in the server certificate"。
            // 若日後切換成正式憑證與加密 Endpoint，應改回 true 並在 Server certificate 加入正確 DNS/IP。
            _session = await new DefaultSessionFactory(Telemetry).CreateAsync(
                _configuration,
                configuredEndpoint,
                false,
                false,
                _options.ApplicationName,
                _options.SessionTimeoutMs,
                identity,
                null,
                cancellationToken).ConfigureAwait(false);

            WriteAudit("Connect", null, null, null, "Good", true, "OPC UA session connected. Endpoint=" + _options.EndpointUrl);
        }
        catch (Exception ex)
        {
            WriteAudit("Connect", null, null, null, "Bad", false, ex.Message);
            throw;
        }
        finally
        {
            // 不論連線成功、失敗或取消，都必須釋放 gate，否則後續無法再連線。
            _connectionGate.Release();
        }
    }

    public async Task DisconnectAsync()
    {
        await _connectionGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_session != null)
            {
                await _session.CloseAsync().ConfigureAwait(false);
                _session.Dispose();
            }
            _session = null;
            WriteAudit("Disconnect", null, null, null, "Good", true, "OPC UA session disconnected.");
        }
        catch (Exception ex)
        {
            WriteAudit("Disconnect", null, null, null, "Bad", false, ex.Message);
            throw;
        }
        finally
        {
            _connectionGate.Release();
        }
    }

    public async Task<OpcReadResult> ReadAsync(NodeDescriptor node, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var session = await GetConnectedSessionAsync(cancellationToken).ConfigureAwait(false);
            var value = await session.ReadValueAsync(ResolveNodeId(node), cancellationToken).ConfigureAwait(false);
            OpcReadResult result = StatusCode.IsGood(value.StatusCode)
                ? new OpcReadResult(true, value.Value, value.StatusCode.ToString())
                : new OpcReadResult(false, value.Value, value.StatusCode.ToString(), value.StatusCode.ToString());
            WriteAudit("Read", node, null, value.Value, result.StatusCode, result.Succeeded, result.Error);
            return result;
        }
        catch (Exception ex)
        {
            WriteAudit("Read", node, null, null, "Bad", false, ex.Message);
            return new OpcReadResult(false, null, "Bad", ex.Message);
        }
    }

    public async Task<OpcWriteResult> WriteAsync(NodeDescriptor node, object value, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var session = await GetConnectedSessionAsync(cancellationToken).ConfigureAwait(false);
            var request = new WriteValue
            {
                NodeId = ResolveNodeId(node),
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(value)),
            };

            var response = await session.WriteAsync(null, new WriteValueCollection { request }, cancellationToken).ConfigureAwait(false);
            var statusCode = response.Results[0];
            OpcWriteResult result = StatusCode.IsGood(statusCode)
                ? new OpcWriteResult(true, statusCode.ToString())
                : new OpcWriteResult(false, statusCode.ToString(), statusCode.ToString());
            WriteAudit("Write", node, value, null, result.StatusCode, result.Succeeded, result.Error);
            return result;
        }
        catch (Exception ex)
        {
            WriteAudit("Write", node, value, null, "Bad", false, ex.Message);
            return new OpcWriteResult(false, "Bad", ex.Message);
        }
    }

    public IDisposable Subscribe(IReadOnlyCollection<NodeDescriptor> nodes, Action<OpcSubscriptionUpdate> onUpdate)
    {
        if (nodes.Count == 0) throw new ArgumentException("At least one node is required.", nameof(nodes));
        if (onUpdate == null) throw new ArgumentNullException("onUpdate");
        var session = GetConnectedSessionAsync(CancellationToken.None).GetAwaiter().GetResult();
        var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 500 };
        session.AddSubscription(subscription);
        foreach (var descriptor in nodes)
        {
            var item = new MonitoredItem(subscription.DefaultItem)
            {
                StartNodeId = ResolveNodeId(descriptor),
                AttributeId = Attributes.Value,
                DisplayName = descriptor.SpecTag,
                SamplingInterval = 500,
            };
            item.Notification += (sender, args) =>
            {
                foreach (var value in item.DequeueValues())
                {
                    WriteAudit("Subscribe", descriptor, null, value.Value, value.StatusCode.ToString(), StatusCode.IsGood(value.StatusCode), null);
                    onUpdate(new OpcSubscriptionUpdate(descriptor, value.Value, value.StatusCode.ToString(), value.SourceTimestamp));
                }
            };
            subscription.AddItem(item);
        }
        subscription.CreateAsync().GetAwaiter().GetResult();
        foreach (NodeDescriptor descriptor in nodes)
            WriteAudit("Subscribe", descriptor, null, null, "Good", true, "Subscription started.");
        return new OpcUaSubscription(session, subscription);
    }

    /// <summary>
    /// Recursively browses hierarchical references below the OPC UA Objects folder.
    /// Attribute read failures are retained in each row instead of aborting the export.
    /// </summary>
    public async Task<IReadOnlyList<OpcAddressSpaceNode>> BrowseAddressSpaceAsync(
        IProgress<int> progress = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var session = await GetConnectedSessionAsync(cancellationToken).ConfigureAwait(false);
        var browser = new Browser(session, new BrowserOptions());
        browser.BrowseDirection = BrowseDirection.Forward;
        browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;
        browser.IncludeSubtypes = true;
        browser.NodeClassMask = 0;
        browser.ResultMask = (uint)BrowseResultMask.All;
        browser.ContinueUntilDone = true;

        var rows = new List<OpcAddressSpaceNode>();
        var rowsByNodeId = new Dictionary<NodeId, OpcAddressSpaceNode>();
        var visited = new HashSet<NodeId>();
        var pending = new Queue<BrowseWorkItem>();
        NodeId rootNodeId = ObjectIds.ObjectsFolder;
        var root = new OpcAddressSpaceNode
        {
            BrowsePath = "Objects",
            NodeId = rootNodeId.ToString(),
            NamespaceIndex = rootNodeId.NamespaceIndex,
            NamespaceUri = session.NamespaceUris.GetString(rootNodeId.NamespaceIndex) ?? string.Empty,
            NodeClass = NodeClass.Object.ToString(),
            BrowseName = "Objects",
            DisplayName = "Objects",
        };
        rows.Add(root);
        rowsByNodeId[rootNodeId] = root;
        visited.Add(rootNodeId);
        pending.Enqueue(new BrowseWorkItem(rootNodeId, root.BrowsePath));

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            BrowseWorkItem current = pending.Dequeue();
            try
            {
                ReferenceDescriptionCollection references = await browser
                    .BrowseAsync(current.NodeId, cancellationToken)
                    .ConfigureAwait(false);
                foreach (ReferenceDescription reference in references)
                {
                    NodeId childNodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);
                    if (childNodeId == null || !visited.Add(childNodeId)) continue;

                    string browseName = reference.BrowseName == null ? string.Empty : reference.BrowseName.Name;
                    string browsePath = current.BrowsePath + "/" + browseName;
                    var row = new OpcAddressSpaceNode
                    {
                        BrowsePath = browsePath,
                        NodeId = childNodeId.ToString(),
                        ParentNodeId = current.NodeId.ToString(),
                        NamespaceIndex = childNodeId.NamespaceIndex,
                        NamespaceUri = session.NamespaceUris.GetString(childNodeId.NamespaceIndex) ?? string.Empty,
                        NodeClass = reference.NodeClass.ToString(),
                        BrowseName = reference.BrowseName == null ? string.Empty : reference.BrowseName.ToString(),
                        DisplayName = reference.DisplayName == null ? string.Empty : reference.DisplayName.Text,
                        TypeDefinition = reference.TypeDefinition == null ? string.Empty : reference.TypeDefinition.ToString(),
                    };
                    rows.Add(row);
                    rowsByNodeId[childNodeId] = row;
                    pending.Enqueue(new BrowseWorkItem(childNodeId, browsePath));
                }
            }
            catch (Exception ex)
            {
                OpcAddressSpaceNode row;
                if (rowsByNodeId.TryGetValue(current.NodeId, out row))
                    row.Error = AppendError(row.Error, "Browse: " + ex.Message);
            }

            if (progress != null && rows.Count % 100 == 0) progress.Report(rows.Count);
        }

        foreach (KeyValuePair<NodeId, OpcAddressSpaceNode> pair in rowsByNodeId)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!string.Equals(pair.Value.NodeClass, NodeClass.Variable.ToString(), StringComparison.Ordinal)) continue;
            try
            {
                Node node = await session
                    .ReadNodeAsync(pair.Key, NodeClass.Variable, true, cancellationToken)
                    .ConfigureAwait(false);
                VariableNode variable = node as VariableNode;
                if (variable == null) continue;
                pair.Value.DataType = variable.DataType == null ? string.Empty : variable.DataType.ToString();
                pair.Value.ValueRank = variable.ValueRank.ToString();
                pair.Value.AccessLevel = FormatAccessLevel(variable.AccessLevel);
                pair.Value.UserAccessLevel = FormatAccessLevel(variable.UserAccessLevel);
            }
            catch (Exception ex)
            {
                pair.Value.Error = AppendError(pair.Value.Error, "Read attributes: " + ex.Message);
            }
        }

        if (progress != null) progress.Report(rows.Count);
        return rows.OrderBy(x => x.BrowsePath, StringComparer.Ordinal).ToArray();
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
        _connectionGate.Dispose();
    }

    private async Task<ISession> GetConnectedSessionAsync(CancellationToken cancellationToken)
    {
        // 第一次讀寫採「延遲連線」；連線中斷後，下一次操作也會嘗試重建 Session。
        // 正式產線若要背景自動重連，應另外建立具退避時間的連線監控服務。
        if (!IsConnected) await ConnectAsync(cancellationToken).ConfigureAwait(false);
        if (_session == null)
            throw new InvalidOperationException("Unable to create an OPC UA session.");

        return _session;
    }

    private static string GetAuditLogDirectory(MitsubishiOpcUaOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.AuditLogDirectory))
            return options.AuditLogDirectory;

        return Path.Combine(AppContext.BaseDirectory, "Logs");
    }

    private void WriteAudit(
        string direction,
        NodeDescriptor node,
        object requestValue,
        object responseValue,
        string statusCode,
        bool succeeded,
        string message)
    {
        _auditLogger.Write(new OpcAuditLogEntry(
            DateTime.Now,
            direction,
            node == null ? string.Empty : node.Node.ToString(),
            node == null ? string.Empty : node.SpecTag,
            requestValue,
            responseValue,
            statusCode,
            succeeded,
            message));
    }

    private NodeId ResolveNodeId(NodeDescriptor node)
    {
        string configuredNodeId;
        if (_options.NodeIdsBySpecTag.TryGetValue(node.SpecTag, out configuredNodeId))
            return NodeId.Parse(configuredNodeId);

        return NodeId.Parse($"ns={_options.DefaultNamespaceIndex};s={node.SpecTag}");
    }

    private static string AppendError(string current, string error)
    {
        return string.IsNullOrEmpty(current) ? error : current + "; " + error;
    }

    private static string FormatAccessLevel(byte accessLevel)
    {
        var flags = new List<string>();
        if ((accessLevel & AccessLevels.CurrentRead) != 0) flags.Add("CurrentRead");
        if ((accessLevel & AccessLevels.CurrentWrite) != 0) flags.Add("CurrentWrite");
        if ((accessLevel & AccessLevels.HistoryRead) != 0) flags.Add("HistoryRead");
        if ((accessLevel & AccessLevels.HistoryWrite) != 0) flags.Add("HistoryWrite");
        if ((accessLevel & AccessLevels.SemanticChange) != 0) flags.Add("SemanticChange");
        if ((accessLevel & AccessLevels.StatusWrite) != 0) flags.Add("StatusWrite");
        if ((accessLevel & AccessLevels.TimestampWrite) != 0) flags.Add("TimestampWrite");
        return accessLevel + (flags.Count == 0 ? string.Empty : " (" + string.Join("|", flags) + ")");
    }

    private sealed class BrowseWorkItem
    {
        public BrowseWorkItem(NodeId nodeId, string browsePath)
        {
            NodeId = nodeId;
            BrowsePath = browsePath;
        }

        public NodeId NodeId { get; private set; }
        public string BrowsePath { get; private set; }
    }

    private IUserIdentity CreateIdentity()
    {
        if (string.IsNullOrWhiteSpace(_options.UserName))
            return new UserIdentity(new AnonymousIdentityToken());

        return new UserIdentity(_options.UserName, System.Text.Encoding.UTF8.GetBytes(_options.Password ?? string.Empty));
    }

    private async Task<ApplicationConfiguration> CreateConfigurationAsync()
    {
        var pkiPath = Path.Combine(AppContext.BaseDirectory, "pki");
        var configuration = new ApplicationConfiguration
        {
            ApplicationName = _options.ApplicationName,
            ApplicationUri = $"urn:{Utils.GetHostName()}:{_options.ApplicationName}",
            ApplicationType = ApplicationType.Client,
            TransportQuotas = new TransportQuotas { OperationTimeout = 15_000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = checked((int)_options.SessionTimeoutMs) },
            SecurityConfiguration = new SecurityConfiguration
            {
                AutoAcceptUntrustedCertificates = _options.AutoAcceptUntrustedCertificates,
                RejectSHA1SignedCertificates = _options.RejectSha1SignedCertificates,
                // UAExpert confirms this DeviceXPlorer endpoint uses SecurityPolicy=None.
                // Its legacy Server certificate is 1024-bit, so the Demo passes 1024 through options.
                // Secure production endpoints should retain the default 2048-bit requirement.
                MinimumCertificateKeySize = _options.MinimumCertificateKeySize,
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiPath, "own"),
                    SubjectName = $"CN={_options.ApplicationName}",
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiPath, "trusted"),
                },
                // OPC UA SDK validates all configured certificate stores, even when the selected
                // DeviceXPlorer Endpoint uses SecurityPolicy=None. Without this path,
                // ValidateAsync fails locally with "TrustedIssuerCertificates StorePath must be specified"
                // before any connection is made to the machine.
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiPath, "issuers"),
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiPath, "rejected"),
                },
            },
        };

        await configuration.ValidateAsync(ApplicationType.Client).ConfigureAwait(false);
        configuration.CertificateValidator.CertificateValidation += (sender, args) =>
        {
            if (_options.AutoAcceptUntrustedCertificates && args.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                args.Accept = true;
        };

        return configuration;
    }

    private sealed class OpcUaSubscription : IDisposable
    {
        private readonly ISession _session;
        private Subscription _subscription;

        public OpcUaSubscription(ISession session, Subscription subscription)
        {
            _session = session;
            _subscription = subscription;
        }

        public void Dispose()
        {
            var current = Interlocked.Exchange(ref _subscription, null);
            if (current == null) return;
            try
            {
                current.DeleteAsync(true).GetAwaiter().GetResult();
                _session.RemoveSubscriptionAsync(current).GetAwaiter().GetResult();
            }
            catch (ServiceResultException) { /* session was already closed */ }
        }
    }
}
}
