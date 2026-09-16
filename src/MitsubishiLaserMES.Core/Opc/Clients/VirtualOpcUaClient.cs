using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Simulator;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserOpc.Clients
{
    /// <summary>
    /// 虛擬 OPC UA 客戶端，直接與 MitsubishiLaserSimulatorEngine 引擎雙向聯動
    /// </summary>
    public sealed class VirtualOpcUaClient : IOpcUaClient
    {
        private readonly ILaserMachineSimulator _simulator;
        private readonly Dictionary<string, object> _fallbackStore = new Dictionary<string, object>();
        private readonly List<SubscriptionItem> _subscriptions = new List<SubscriptionItem>();
        private readonly object _subLock = new object();

        public VirtualOpcUaClient(ILaserMachineSimulator simulator = null)
        {
            _simulator = simulator ?? LaserMachineSimulatorEngine.SharedInstance;
            _simulator.StateChanged += OnSimulatorStateChanged;
        }

        public Task<OpcReadResult> ReadAsync(NodeDescriptor node, CancellationToken cancellationToken = default)
        {
            if (node == null)
            {
                return Task.FromResult(new OpcReadResult(false, null, "BadNodeIdInvalid"));
            }

            // 優先從模擬器核心讀取
            object simValue = _simulator.HandleHostRead(node);
            if (simValue != null)
            {
                return Task.FromResult(new OpcReadResult(true, simValue, "Good"));
            }

            // 次之從備用字典讀取
            lock (_fallbackStore)
            {
                if (_fallbackStore.TryGetValue(node.SpecTag, out var val))
                {
                    return Task.FromResult(new OpcReadResult(true, val, "Good"));
                }
            }

            return Task.FromResult(new OpcReadResult(true, null, "Good"));
        }

        public Task<OpcWriteResult> WriteAsync(NodeDescriptor node, object value, CancellationToken cancellationToken = default)
        {
            if (node == null)
            {
                return Task.FromResult(new OpcWriteResult(false, "BadNodeIdInvalid"));
            }

            // 寫入模擬器
            bool handled = _simulator.HandleHostWrite(node, value);
            if (!handled)
            {
                lock (_fallbackStore)
                {
                    _fallbackStore[node.SpecTag] = value;
                }
            }

            // 通知訂閱者
            NotifySubscriptions(node, value);

            return Task.FromResult(new OpcWriteResult(true, "Good"));
        }

        public void SimulateRecipeRequest(string lotId)
        {
            _simulator.OperatorInputLot(lotId);
        }

        public void SimulateOperatorStart()
        {
            _simulator.StartSchedule();
        }

        public IDisposable Subscribe(IReadOnlyCollection<NodeDescriptor> nodes, Action<OpcSubscriptionUpdate> onUpdate)
        {
            if (nodes == null || onUpdate == null)
            {
                return new EmptySubscription();
            }

            var nodeDict = nodes.ToDictionary(x => x.SpecTag, x => x, StringComparer.Ordinal);
            var item = new SubscriptionItem(nodeDict, onUpdate);

            lock (_subLock)
            {
                _subscriptions.Add(item);
            }

            // 訂閱建立時立即發布一次當前值
            Task.Run(() =>
            {
                foreach (var kvp in nodeDict)
                {
                    object val = _simulator.HandleHostRead(kvp.Value);
                    if (val != null)
                    {
                        onUpdate(new OpcSubscriptionUpdate(kvp.Value, val, "Good", DateTimeOffset.Now));
                    }
                }
            });

            return new Subscription(() =>
            {
                lock (_subLock)
                {
                    _subscriptions.Remove(item);
                }
            });
        }

        private void OnSimulatorStateChanged()
        {
            SubscriptionItem[] activeSubs;
            lock (_subLock)
            {
                activeSubs = _subscriptions.ToArray();
            }

            foreach (var sub in activeSubs)
            {
                foreach (var descriptor in sub.NodeMap.Values)
                {
                    object currentVal = _simulator.HandleHostRead(descriptor);
                    if (currentVal != null)
                    {
                        sub.Callback(new OpcSubscriptionUpdate(descriptor, currentVal, "Good", DateTimeOffset.Now));
                    }
                }
            }
        }

        private void NotifySubscriptions(NodeDescriptor descriptor, object value)
        {
            SubscriptionItem[] activeSubs;
            lock (_subLock)
            {
                activeSubs = _subscriptions.ToArray();
            }

            foreach (var sub in activeSubs)
            {
                if (sub.NodeMap.ContainsKey(descriptor.SpecTag))
                {
                    sub.Callback(new OpcSubscriptionUpdate(descriptor, value, "Good", DateTimeOffset.Now));
                }
            }
        }

        private sealed class SubscriptionItem
        {
            public SubscriptionItem(Dictionary<string, NodeDescriptor> nodeMap, Action<OpcSubscriptionUpdate> callback)
            {
                NodeMap = nodeMap;
                Callback = callback;
            }

            public Dictionary<string, NodeDescriptor> NodeMap { get; }
            public Action<OpcSubscriptionUpdate> Callback { get; }
        }

        private sealed class Subscription : IDisposable
        {
            private Action _dispose;
            public Subscription(Action dispose) { _dispose = dispose; }
            public void Dispose()
            {
                var dispose = Interlocked.Exchange(ref _dispose, null);
                dispose?.Invoke();
            }
        }

        private sealed class EmptySubscription : IDisposable
        {
            public void Dispose() { }
        }
    }
}
