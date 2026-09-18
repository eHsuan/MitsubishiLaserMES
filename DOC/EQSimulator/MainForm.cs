using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CyntecMqttLib.Interfaces;
using CyntecMqttLib.Services;
using Protocol.Core.Base;
using Protocol.Core.Enums;
using Protocol.Core.Factory;
using Protocol.Core.Messages;
using Newtonsoft.Json;

namespace EQSimulator
{
    public partial class MainForm : Form
    {
        private MqttBus _bus;
        private string _eqId = "TS0003";
        private string _baseTopic = "TN2/Floor2/Yellow/TS0003/";
        private EqpDictionary _dictionary = new EqpDictionary();

        // Custom Variables for Auto Test
        private bool _isAutoTesting = false;
        private TaskCompletionSource<bool> _currentEventTcs;
        private System.Threading.CancellationTokenSource _autoCts;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
        };

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeStatusCombo();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BtnConnect_Click(null, null);
        }

        private void InitializeCustomComponents()
        {
            // 自動載入設定
            LoadSettings();
            InitializeScenarioCombo();
        }

        private void InitializeScenarioCombo()
        {
            if (_cboScenario != null)
            {
                _cboScenario.Items.Clear();
                _cboScenario.Items.AddRange(new object[]
                {
                    "[情境 1] 正常出站 (投入1000, PASS 980, 一般NG自動平減)",
                    "[情境 2] 缺報總數或剩料 (未報 TotalCount/Leftovers 阻擋)",
                    "[情境 3] 良率過低異常 (<95%, PASS 800/投入1000 阻擋)",
                    "[情境 4] 良率過高/溢出 (>100%, PASS 1100/投入1000 阻擋)",
                    "[情境 5] NG 數溢出 (特殊NG總和25 > 算出的NG 20 阻擋)"
                });
                if (_cboScenario.Items.Count > 0)
                {
                    _cboScenario.SelectedIndex = 0;
                }
            }
        }

        private string GetPanelListRaw()
        {
            if (_txtPanelList == null || string.IsNullOrEmpty(_txtPanelList.Text)) return "";
            var list = _txtPanelList.Text.Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p));
            return string.Join(",", list);
        }

        private void InitializeStatusCombo()
        {
            if (_cmbStatusType != null)
            {
                if (_cmbStatusType.Items.Count == 0)
                {
                    _cmbStatusType.Items.AddRange(new object[] { "Cyntec", "Delta" });
                }
                if (_cmbStatusType.Items.Count > 0 && _cmbStatusType.SelectedIndex < 0)
                {
                    _cmbStatusType.SelectedIndex = 0;
                }
                _cmbStatusType.SelectedIndexChanged += (s, e) => UpdateStatusComboSource();
            }
            UpdateStatusComboSource();
        }

        private void UpdateStatusComboSource()
        {
            string type = _cmbStatusType?.SelectedItem?.ToString() ?? "Cyntec";
            if ("Cyntec".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                var cyntecStatuses = new[] {
                    new { Value = 1, Text = "1: RUNNING (Cyntec)" },
                    new { Value = 2, Text = "2: ALARM (Cyntec)" },
                    new { Value = 3, Text = "3: IDLE (Cyntec)" }
                };
                _cmbStatus.DataSource = cyntecStatuses;
            }
            else
            {
                var deltaStatuses = new[] {
                    new { Value = 0, Text = "0: RUNNING (Delta)" },
                    new { Value = 1, Text = "1: ALARM (Delta)" },
                    new { Value = 2, Text = "2: PAUSED (Delta)" },
                    new { Value = 3, Text = "3: IDLE (Delta)" },
                    new { Value = 4, Text = "4: SETUP (Delta)" },
                    new { Value = 5, Text = "5: WAIT (Delta)" },
                    new { Value = 6, Text = "6: RUN (Delta)" },
                    new { Value = 7, Text = "7: STOP (Delta)" },
                    new { Value = 8, Text = "8: MANUAL (Delta)" },
                    new { Value = 9, Text = "9: ERROR (Delta)" },
                    new { Value = 10, Text = "10: DOWN (Delta)" }
                };
                _cmbStatus.DataSource = deltaStatuses;
            }
            _cmbStatus.DisplayMember = "Text";
            _cmbStatus.ValueMember = "Value";
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if (_bus != null && _bus.IsConnected)
            {
                try { await _bus.DisconnectAsync(); _bus.Dispose(); } catch { }
                _bus = null;
                _btnConnect.Text = "Connect";
                _grpActions.Enabled = false;
                Log("Disconnected from Broker.");
                return;
            }

            if (_bus != null) { try { _bus.Dispose(); } catch { } _bus = null; }

            _eqId = _txtEqId.Text;
            _baseTopic = _txtTopic.Text.TrimEnd('/') + "/";
            _dictionary = EqpDictionary.Load(_eqId);
            Log($"Dictionary loaded for {_eqId}");
            
            try
            {
                string clientId = $"Sim_{_eqId}_{Guid.NewGuid().ToString().Substring(0, 4)}";
                _bus = new MqttBus(_txtBroker.Text, 1883, clientId);
                await _bus.ConnectAsync();
                
                string cmdTopic = _baseTopic + "Command";
                await _bus.SubscribeAsync(cmdTopic, (topic, msg) => HandleIncomingCommand(msg));
                
                _btnConnect.Text = "Disconnect";
                _grpActions.Enabled = true;
                Log($"Connected as {clientId}!");
            }
            catch (Exception ex) { Log($"[ERR] Connection failed: {ex.Message}"); }
        }

        private void BtnUser_Click(object sender, EventArgs e)
        {
            var evm = new EventReportMessage 
            { 
                CMD = CommandType.EventReport, 
                EventID = _dictionary.GetCeid("USER_VERIFY"),
                Data = new List<EventItem> 
                { 
                    new EventItem { Parameter = _dictionary.GetVid("UserID"), Value = _txtUserId.Text },
                    new EventItem { Parameter = _dictionary.GetVid("Barcode"), Value = _txtBarcode.Text }
                }
            };
            SendMessage(evm);
        }

        private void BtnTrackIn_Click(object sender, EventArgs e)
        {
            var wos = (_txtWorkOrder.Text ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var cids = (_txtCassetteId.Text ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            SendMessage(new TrackInReqMessage 
            { 
                CMD = CommandType.TrackInReq, 
                WorkOrder = wos, 
                MaterialID = _txtMaterialId.Text, 
                CassetteID = cids,
                ToolingID = _txtToolingId.Text,
                UserID = _txtUserId.Text,
                Qty = "1"
            });
        }

        private void BtnTrackOut_Click(object sender, EventArgs e)
        {
            int qty = 0; int.TryParse(_txtQty.Text, out qty);
            var wos = (_txtWorkOrder.Text ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var cids = (_txtCassetteId.Text ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            SendMessage(new TrackOutReqMessage 
            { 
                CMD = CommandType.TrackOutReq, 
                WorkOrder = wos, 
                MaterialID = _txtMaterialId.Text, 
                Qty = qty.ToString(), 
                UserID = _txtUserId.Text,
                CassetteID = cids,
                ToolingID = _txtToolingId.Text,
                PanelID = GetPanelListRaw(),
                Result = "PASS"
            });
        }

        private AlarmTestForm _alarmTestForm;

        private void BtnAlarm_Click(object sender, EventArgs e)
        {
            if (_alarmTestForm == null || _alarmTestForm.IsDisposed)
            {
                _alarmTestForm = new AlarmTestForm(msg => SendMessage(msg));
            }
            _alarmTestForm.ShowDialog(this);
        }

        private void BtnPanelIn_Click(object sender, EventArgs e)
        {
            SendMessage(new EventReportMessage 
            { 
                CMD = CommandType.EventReport, 
                EventID = _dictionary.GetCeid("PANEL_IN"),
                Data = new List<EventItem> { new EventItem { Parameter = _dictionary.GetVid("LotID"), Value = _txtWorkOrder.Text } }
            });
        }

        private void BtnPanelOut_Click(object sender, EventArgs e)
        {
            SendMessage(new EventReportMessage 
            { 
                CMD = CommandType.EventReport, 
                EventID = _dictionary.GetCeid("PANEL_OUT"),
                Data = new List<EventItem> { new EventItem { Parameter = _dictionary.GetVid("LotID"), Value = _txtWorkOrder.Text } }
            });
        }

        private string _currentStatusCode = "0";

        private void BtnReportStatus_Click(object sender, EventArgs e)
        {
            string newStatus = _cmbStatus.SelectedValue?.ToString() ?? "0";
            string oldStatus = _currentStatusCode;
            _currentStatusCode = newStatus;

            SendMessage(new StatusChangeReportMessage { CMD = CommandType.StatusChangeReport, NewStatus = newStatus, OldStatus = oldStatus });
            Log($"[StatusChange] EQ State updated: {oldStatus} -> {newStatus}");
        }

        private void CboNGCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string code = _cboNGCode.Text ?? "";
            switch (code.Trim().ToUpperInvariant())
            {
                case "UPNG501":
                    _txtNGName.Text = "機台卡片";
                    break;
                case "UPNG502":
                    _txtNGName.Text = "人為疏失折片";
                    break;
                case "UPNG503":
                    _txtNGName.Text = "工程驗證抽片";
                    break;
                case "QC_KEEP":
                    _txtNGName.Text = "QC Keep Sample";
                    break;
                case "FREE_SUP":
                    _txtNGName.Text = "免補料";
                    break;
            }
        }

        private void BtnSendNGCode_Click(object sender, EventArgs e)
        {
            string wo = (_txtWorkOrder.Text ?? "").Split(',').FirstOrDefault()?.Trim() ?? "";
            string ngCode = _cboNGCode.Text?.Trim() ?? "UPNG501";
            string ngName = _txtNGName.Text?.Trim() ?? "機台卡片";
            string ngQty = _txtNGQty.Text?.Trim() ?? "1";
            string ngUnit = _txtNGUnit.Text?.Trim() ?? "PCS";

            var evm = new EventReportMessage
            {
                CMD = CommandType.EventReport,
                EventID = _dictionary.GetCeid("NGCodeReport"),
                Data = new List<EventItem>
                {
                    new EventItem { Parameter = _dictionary.GetVid("WorkOrder"), Value = wo },
                    new EventItem { Parameter = _dictionary.GetVid("NGCode"), Value = ngCode },
                    new EventItem { Parameter = _dictionary.GetVid("NGName"), Value = ngName },
                    new EventItem { Parameter = _dictionary.GetVid("NGQty"), Value = ngQty },
                    new EventItem { Parameter = _dictionary.GetVid("NGUnit"), Value = ngUnit }
                }
            };
            SendMessage(evm);
            Log($"[NGCodeReport] Sent NGCode: {ngCode}, Qty: {ngQty}, Unit: {ngUnit} for WO: {wo}");
        }

        private async void BtnSendAllNG_Click(object sender, EventArgs e)
        {
            string wo = (_txtWorkOrder.Text ?? "").Split(',').FirstOrDefault()?.Trim() ?? "";
            var defaultList = new[]
            {
                ("UPNG501", "機台卡片", "20", "PCS"),
                ("UPNG502", "人為疏失折片", "10", "PCS"),
                ("UPNG503", "工程驗證抽片", "0", "PCS"),
                ("QC_KEEP", "QC Keep Sample", "5", "PCS"),
                ("FREE_SUP", "免補料", "5", "PCS")
            };

            foreach (var (code, name, qty, unit) in defaultList)
            {
                var evm = new EventReportMessage
                {
                    CMD = CommandType.EventReport,
                    EventID = _dictionary.GetCeid("NGCodeReport"),
                    Data = new List<EventItem>
                    {
                        new EventItem { Parameter = _dictionary.GetVid("WorkOrder"), Value = wo },
                        new EventItem { Parameter = _dictionary.GetVid("NGCode"), Value = code },
                        new EventItem { Parameter = _dictionary.GetVid("NGName"), Value = name },
                        new EventItem { Parameter = _dictionary.GetVid("NGQty"), Value = qty },
                        new EventItem { Parameter = _dictionary.GetVid("NGUnit"), Value = unit }
                    }
                };
                SendMessage(evm);
                Log($"[NGCodeReport] Auto sent NGCode: {code} ({name}), Qty: {qty}, Unit: {unit} for WO: {wo}");
                await Task.Delay(50);
            }
        }

        private void BtnSendLeftover_Click(object sender, EventArgs e)
        {
            string wo = (_txtWorkOrder.Text ?? "").Split(',').FirstOrDefault()?.Trim() ?? "";
            string reelId = _txtLeftoverReel.Text?.Trim() ?? "REEL001";
            string qty = _txtLeftoverQty.Text?.Trim() ?? "10";

            var evm = new EventReportMessage
            {
                CMD = CommandType.EventReport,
                EventID = _dictionary.GetCeid("LeftoversReport") ?? "502",
                Data = new List<EventItem>
                {
                    new EventItem { Parameter = _dictionary.GetVid("WorkOrder") ?? "WorkOrder", Value = wo },
                    new EventItem { Parameter = _dictionary.GetVid("Qty") ?? "Qty", Value = qty },
                    new EventItem { Parameter = _dictionary.GetVid("ReelID") ?? "ReelID", Value = reelId }
                }
            };
            SendMessage(evm);
            Log($"[LeftoversReport] Sent WorkOrder: {wo}, ReelID: {reelId}, Qty: {qty}");
        }

        private void BtnSendUseLeftover_Click(object sender, EventArgs e)
        {
            string wo = (_txtWorkOrder.Text ?? "").Split(',').FirstOrDefault()?.Trim() ?? "";
            string reelId = _txtLeftoverReel.Text?.Trim() ?? "REEL001";
            string qty = _txtLeftoverQty.Text?.Trim() ?? "10";

            var evm = new EventReportMessage
            {
                CMD = CommandType.EventReport,
                EventID = _dictionary.GetCeid("ProductLeftoverReport") ?? "ProductLeftoverReport",
                Data = new List<EventItem>
                {
                    new EventItem { Parameter = _dictionary.GetVid("WorkOrder") ?? "WorkOrder", Value = wo },
                    new EventItem { Parameter = _dictionary.GetVid("ReelID") ?? "ReelID", Value = reelId },
                    new EventItem { Parameter = _dictionary.GetVid("LeftoverWorkOrder") ?? "LeftoverWorkOrder", Value = "WO_PREV001" },
                    new EventItem { Parameter = _dictionary.GetVid("LeftoverBatchNo") ?? "LeftoverBatchNo", Value = "BATCH001" },
                    new EventItem { Parameter = _dictionary.GetVid("AddQty") ?? "AddQty", Value = qty },
                    new EventItem { Parameter = _dictionary.GetVid("GapQty") ?? "GapQty", Value = "0" }
                }
            };
            SendMessage(evm);
            Log($"[ProductLeftoverReport] Sent WorkOrder: {wo}, ReelID: {reelId}, AddQty: {qty}");
        }

        private void BtnSendTotalCount_Click(object sender, EventArgs e)
        {
            string wo = (_txtWorkOrder.Text ?? "").Split(',').FirstOrDefault()?.Trim() ?? "";
            string reelCount = _txtTotalReelCount.Text?.Trim() ?? "10";
            string qtyPerReel = _txtTotalQtyPerReel.Text?.Trim() ?? "500";

            string eventCeid = _dictionary.GetCeid("TotalCountReport") ?? "1006";
            string woVid = _dictionary.GetVid("WorkOrder") ?? "WorkOrder";
            string reelCountVid = _dictionary.GetVid("ReelCount") ?? "ReelCount";
            string qtyPerReelVid = _dictionary.GetVid("QtyPerReel") ?? "QtyPerReel";

            var evm = new EventReportMessage
            {
                CMD = CommandType.EventReport,
                EventID = eventCeid,
                Data = new List<EventItem>
                {
                    new EventItem { Parameter = woVid, Value = wo },
                    new EventItem { Parameter = reelCountVid, Value = reelCount },
                    new EventItem { Parameter = qtyPerReelVid, Value = qtyPerReel }
                }
            };
            SendMessage(evm);
            Log($"[TotalCountReport] Sent EventID: {eventCeid}, WorkOrder: {wo}, ReelCount: {reelCount}, QtyPerReel: {qtyPerReel}");
        }

        private void BtnApplyScenario_Click(object sender, EventArgs e)
        {
            int idx = _cboScenario?.SelectedIndex ?? 0;

            switch (idx)
            {
                case 0: // 情境 1: 正常出站 (PASS 980 / 投入 1000, 一般 NG 平減)
                    _txtWorkOrder.Text = "WO_TP_NORM_01";
                    _txtQty.Text = "980";
                    _txtTotalReelCount.Text = "9";
                    _txtTotalQtyPerReel.Text = "100"; // 900
                    _txtLeftoverReel.Text = "REEL001";
                    _txtLeftoverQty.Text = "80";      // 剩料 80 (補料0 => PASS 980, NG 20)
                    _cboNGCode.Text = "UPNG501";
                    _txtNGName.Text = "機台卡片";
                    _txtNGQty.Text = "30";
                    Log("[Scenario 1 載入完成] 正常出站: 投入1000, 總數9*100=900, 剩料80 => PASS=980 (良率98%), NG=20。請依序發送: TrackIn -> 剩料上報 -> 總數上報 -> 上報全部NG -> TrackOut。");
                    break;

                case 1: // 情境 2: 缺報總數或剩料
                    _txtWorkOrder.Text = "WO_TP_MISS_02";
                    Log("[Scenario 2 載入完成] 缺報總數/剩料: 請在 TrackIn 後直接點擊 TrackOut，觀察 EAP 阻擋並回覆缺少報告。");
                    break;

                case 2: // 情境 3: 良率過低 (<95%)
                    _txtWorkOrder.Text = "WO_TP_LOW_03";
                    _txtTotalReelCount.Text = "8";
                    _txtTotalQtyPerReel.Text = "100"; // 800
                    _txtLeftoverQty.Text = "0";       // PASS=800 / 投入1000 = 80.0% < 95%
                    Log("[Scenario 3 載入完成] 良率過低異常: 投入1000, PASS=800 (良率80% < 95%)。請依序發送: 剩料0 -> 總數8*100 -> TrackOut，觀察良率過低阻擋。");
                    break;

                case 3: // 情境 4: 良率過高 (>100%)
                    _txtWorkOrder.Text = "WO_TP_HIGH_04";
                    _txtTotalReelCount.Text = "11";
                    _txtTotalQtyPerReel.Text = "100"; // 1100
                    _txtLeftoverQty.Text = "0";       // PASS=1100 / 投入1000 = 110.0% > 100%
                    Log("[Scenario 4 載入完成] 良率過高異常: 投入1000, PASS=1100 (良率110% > 100%)。請依序發送: 剩料0 -> 總數11*100 -> TrackOut，觀察良率過高阻擋。");
                    break;

                case 4: // 情境 5: NG 數溢出
                    _txtWorkOrder.Text = "WO_TP_OVR_05";
                    _txtTotalReelCount.Text = "9";
                    _txtTotalQtyPerReel.Text = "100"; // 900
                    _txtLeftoverQty.Text = "80";      // PASS=980, CalcNG=20
                    _cboNGCode.Text = "QC_KEEP";
                    _txtNGName.Text = "QC Keep Sample";
                    _txtNGQty.Text = "25";            // 特殊項目 25 > 20
                    Log("[Scenario 5 載入完成] NG數溢出: 投入1000, PASS=980 => CalcNG=20。請上報 QC Keep Sample 25顆 -> 剩料80 -> 總數9*100 -> TrackOut，觀察溢出阻擋。");
                    break;
            }
        }

        private void HandleIncomingCommand(string json)
        {
            if (this.IsDisposed) return;
            this.Invoke((MethodInvoker)delegate {
                try {
                    var envelope = JsonConvert.DeserializeObject<CyntecMqttLib.Contracts.MessageEnvelope>(json);
                    if (envelope == null) return;
                    
                    string innerJson = envelope.Payload?.ToString();
                    if (string.IsNullOrEmpty(innerJson)) return;

                    var msg = MessageFactory.Create(innerJson);
                    Log($"[RECV] {msg.CMD}");
                    
                    switch (msg.CMD) {
                        case CommandType.AreYouThere:
                            SendMessage(new IamHereMessage { CMD = CommandType.IamHere }, msg);
                            break;
                        
                        case CommandType.StatusReq:
                            Log($"-> Received StatusReq from EAP. Replying current status ({_currentStatusCode})...");
                            SendMessage(new ReplyStatusReqMessage { 
                                CMD = CommandType.ReplyStatusReq, 
                                RtnResult = RtnResult.PASS, 
                                CurrentStatus = _currentStatusCode ?? "0" 
                            }, msg);
                            break;
                        
                        case CommandType.ReplyTrackInReq:
                            var rti = (ReplyTrackInReqMessage)msg;
                            Log($"-> TrackIn Result: {rti.RtnResult}, Msg: {rti.RtnMsg}");
                            _currentEventTcs?.TrySetResult(true);
                            break;

                        case CommandType.RemoteCMD:
                            var rcmd = (RemoteCMDMessage)msg;
                            if (rcmd.RemoteCMDType == RemoteCommandType.PP_SELECT) {
                                bool isPass = false;
                                try {
                                    if (this.InvokeRequired) {
                                        isPass = (bool)this.Invoke(new Func<bool>(() => chkPPSelectPass == null || chkPPSelectPass.Checked));
                                    } else {
                                        isPass = chkPPSelectPass == null || chkPPSelectPass.Checked;
                                    }
                                } catch { isPass = true; }

                                if (isPass) {
                                    Log($"-> RemoteCMD: PP_SELECT. Recipe: {rcmd.RecipeID}. Switching...");
                                    Task.Delay(2000).ContinueWith(_ => {
                                        SendMessage(new ReplyRemoteCMDMessage { 
                                            CMD = CommandType.ReplyRemoteCMD, 
                                            RemoteCMDType = RemoteCommandType.PP_SELECT,
                                            RtnResult = RtnResult.PASS 
                                        }, rcmd);
                                        Log($"-> Recipe {rcmd.RecipeID} Change SUCCESS.");
                                    });
                                } else {
                                    Log($"-> RemoteCMD: PP_SELECT. Recipe: {rcmd.RecipeID}. [Forced FAIL] Recipe Mismatch.");
                                    Task.Delay(1000).ContinueWith(_ => {
                                        SendMessage(new ReplyRemoteCMDMessage { 
                                            CMD = CommandType.ReplyRemoteCMD, 
                                            RemoteCMDType = RemoteCommandType.PP_SELECT,
                                            RtnResult = RtnResult.FAIL,
                                            RtnMsg = "Recipe ID mismatch: Target recipe parameter incompatible with station config"
                                        }, rcmd);
                                        Log($"-> Recipe {rcmd.RecipeID} Change FAILED (Simulated Mismatch).");
                                    });
                                }
                            }
                            break;

                        case CommandType.ReplyStatusReq:
                            var rsr = (ReplyStatusReqMessage)msg;
                            Log($"-> Status Req ACK received.");
                            break;

                        case CommandType.ReplyStatusChangeReport:
                            Log("-> Status Change ACK received.");
                            _currentEventTcs?.TrySetResult(true);
                            break;

                        case CommandType.ReplyALARM:
                            Log("-> Alarm Report ACK received.");
                            _currentEventTcs?.TrySetResult(true);
                            break;

                        case CommandType.ReplyTrackOutReq:
                            var rto = (ReplyTrackOutReqMessage)msg;
                            if (rto.RtnResult == RtnResult.PASS)
                            {
                                Log($"-> TrackOut Result: PASS, Msg: {rto.RtnMsg}");
                            }
                            else
                            {
                                Log($"[TRACKOUT REJECTED] EAP 要求必須先上報以下未完成之 NGCode: {rto.RtnMsg}");
                            }
                            _currentEventTcs?.TrySetResult(true);
                            break;

                        case CommandType.ReplyRtnTraceData:
                            var rrtd = (ReplyRtnTraceDataMessage)msg;
                            Log($"-> TraceData ACK: {rrtd.RtnResult}, Msg: {rrtd.RtnMsg}");
                            break;

                        case CommandType.ReplyEventReport:
                            var revm = (ReplyEventReportMessage)msg;
                            Log($"-> Event Reply Result: {revm.RtnResult}, Msg: {revm.RtnMsg}");
                            _currentEventTcs?.TrySetResult(true);
                            break;

                        case CommandType.TimeCalibrate:
                            Log($"-> TimeCalibrate received. Syncing...");
                            SendMessage(new ReplyTimeCalibrateMessage { CMD = CommandType.ReplyTimeCalibrate, RtnResult = RtnResult.PASS });
                            break;
                        }
                        } catch (Exception ex) { Log($"[ERR] Parse failed: {ex.Message}"); }
                        });
        }

        private void BtnAutoTest_Click(object sender, EventArgs e)
        {
            if (_bus == null || !_bus.IsConnected)
            {
                MessageBox.Show("Please connect to MQTT Broker first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isAutoTesting)
            {
                _isAutoTesting = false;
                _btnAutoTest.Text = "Start Auto Test";
                _autoCts?.Cancel();
                _currentEventTcs?.TrySetResult(false);
            }
            else
            {
                _isAutoTesting = true;
                _btnAutoTest.Text = "Stop Auto Test";
                _autoCts = new System.Threading.CancellationTokenSource();
                _ = Task.Run(() => RunAutoTestLoop(_autoCts.Token));
            }
        }

        private async Task RunAutoTestLoop(System.Threading.CancellationToken ct)
        {
            Log("[AutoTest] Starting Auto Test Loop...");
            int step = 0;
            while (_isAutoTesting && !ct.IsCancellationRequested)
            {
                try
                {
                    switch (step)
                    {
                        case 0:
                            Log("[AutoTest] Step 1: Status Change Report (RUNNING)");
                            this.Invoke((MethodInvoker)delegate {
                                _cmbStatus.SelectedValue = 0; // RUNNING
                                BtnReportStatus_Click(null, null);
                            });
                            await WaitForCurrentEventAsync(10000, ct);
                            break;
                        case 1:
                            Log("[AutoTest] Step 2: User Verify");
                            this.Invoke((MethodInvoker)delegate {
                                BtnUser_Click(null, null);
                            });
                            await WaitForCurrentEventAsync(10000, ct);
                            break;
                        case 2:
                            Log("[AutoTest] Step 3: Track In Request");
                            this.Invoke((MethodInvoker)delegate {
                                BtnTrackIn_Click(null, null);
                            });
                            await WaitForCurrentEventAsync(25000, ct); // 包含 PP-SELECT 比對，稍微等長一點
                            break;
                        case 3:
                            Log("[AutoTest] Step 4: Alarm Set Report");
                            this.Invoke((MethodInvoker)delegate {
                                SendMessage(new AlarmMessage { CMD = CommandType.AlarmReport, AlarmStatus = "Start", AlarmType = "A", AlarmCode = "E004", AlarmMsg = "Emergency Stop" });
                            });
                            await WaitForCurrentEventAsync(10000, ct);
                            break;
                        case 4:
                            Log("[AutoTest] Step 5: Alarm End Report");
                            this.Invoke((MethodInvoker)delegate {
                                SendMessage(new AlarmMessage { CMD = CommandType.AlarmReport, AlarmStatus = "End", AlarmType = "A", AlarmCode = "E004", AlarmMsg = "Emergency Stop Clear" });
                            });
                            await WaitForCurrentEventAsync(10000, ct);
                            break;
                        case 5:
                            Log("[AutoTest] Step 6: Track Out Request");
                            this.Invoke((MethodInvoker)delegate {
                                BtnTrackOut_Click(null, null);
                            });
                            await WaitForCurrentEventAsync(10000, ct);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log($"[AutoTest] Error in Step {step + 1}: {ex.Message}");
                }

                if (!_isAutoTesting || ct.IsCancellationRequested) break;

                step = (step + 1) % 6;
                // 每個事件之間停頓 2 秒
                await Task.Delay(2000, ct);
            }
            Log("[AutoTest] Auto Test Loop Stopped.");
        }

        private async Task WaitForCurrentEventAsync(int timeoutMs, System.Threading.CancellationToken ct)
        {
            _currentEventTcs = new TaskCompletionSource<bool>();
            using (ct.Register(() => _currentEventTcs.TrySetResult(false)))
            {
                var delayTask = Task.Delay(timeoutMs, ct);
                var completedTask = await Task.WhenAny(_currentEventTcs.Task, delayTask);
                if (completedTask == delayTask)
                {
                    Log("[AutoTest] [TIMEOUT] Wait timeout for event reply.");
                }
                else
                {
                    Log("[AutoTest] Reply received.");
                }
            }
    }

    private void BtnRtnTraceData_Click(object sender, EventArgs e)
                        {
                        var rtd = new RtnTraceDataMessage
                        {
                        CMD = CommandType.RtnTraceData,
                        Status = _cmbStatus.SelectedValue.ToString(),
                        CycleTime = "12.5",
                        Availability = "98.5",
                        Parameter = new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string> { { "Temperature", "25.4" } },
                            new Dictionary<string, string> { { "Pressure", "101.3" } }
                        },
                        RecipeDATA = new RecipeDataContent
                        {
                            RecipeID = "RECIPE_001",
                            RecipeParameter = new Dictionary<string, string>
                            {
                                { "TargetSpeed", "1500.0" },
                                { "LimitTime", "60.0" }
                            }
                        }
                        };
                        SendMessage(rtd);
                        }

                        private void SendMessage(BaseMessage msg, BaseMessage requestMsg = null)
                        {
                            if (_bus == null || !_bus.IsConnected) return;
                            msg.Machine = _txtEqId.Text;
                            msg.Date = DateTime.Now.ToString("yyyyMMddHHmmss");
                            if (requestMsg != null)
                            {
                                msg.TransactionID = requestMsg.TransactionID;
                            }
                            else if (string.IsNullOrEmpty(msg.TransactionID))
                            {
                                msg.TransactionID = Guid.NewGuid().ToString("N");
                            }
                            string reportTopic = _txtTopic.Text.TrimEnd('/') + "/Report";
                            _bus.PublishAsync(reportTopic, msg);
                            Log($"[SEND] {msg.CMD}");
                        }

        private void Log(string text)
        {
            if (_txtLog.InvokeRequired) { _txtLog.Invoke((MethodInvoker)(() => Log(text))); return; }
            _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = new EqSettings
                {
                    Broker = _txtBroker.Text,
                    EqId = _txtEqId.Text,
                    Topic = _txtTopic.Text,
                    UserId = _txtUserId.Text,
                    Barcode = _txtBarcode.Text,
                    WorkOrder = _txtWorkOrder.Text,
                    MaterialId = _txtMaterialId.Text,
                    CassetteId = _txtCassetteId.Text,
                    ToolingId = _txtToolingId.Text,
                    Qty = _txtQty.Text,
                    PanelList = _txtPanelList?.Text ?? ""
                };
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "eq_settings.json");
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
                Log("設定儲存成功！已儲存至 eq_settings.json");
            }
            catch (Exception ex)
            {
                Log($"[ERR] 儲存設定失敗: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "eq_settings.json");
                if (System.IO.File.Exists(path))
                {
                    string json = System.IO.File.ReadAllText(path);
                    var settings = JsonConvert.DeserializeObject<EqSettings>(json);
                    if (settings != null)
                    {
                        _txtBroker.Text = settings.Broker;
                        _txtEqId.Text = settings.EqId;
                        _txtTopic.Text = settings.Topic;
                        _txtUserId.Text = settings.UserId;
                        _txtBarcode.Text = settings.Barcode;
                        _txtWorkOrder.Text = settings.WorkOrder;
                        _txtMaterialId.Text = settings.MaterialId;
                        _txtCassetteId.Text = settings.CassetteId;
                        _txtToolingId.Text = settings.ToolingId;
                        _txtQty.Text = settings.Qty;
                        if (_txtPanelList != null) _txtPanelList.Text = settings.PanelList;
                        Log("成功載入記憶的設定值。");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[ERR] 載入設定失敗: {ex.Message}");
            }
        }
    }

    public class EqSettings
    {
        public string Broker { get; set; }
        public string EqId { get; set; }
        public string Topic { get; set; }
        public string UserId { get; set; }
        public string Barcode { get; set; }
        public string WorkOrder { get; set; }
        public string MaterialId { get; set; }
        public string CassetteId { get; set; }
        public string ToolingId { get; set; }
        public string Qty { get; set; }
        public string PanelList { get; set; }
    }
}
