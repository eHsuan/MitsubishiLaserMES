using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserOpc.Clients
{
    public sealed class VirtualOpcUaClient : IOpcUaClient
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private readonly List<SubscriptionItem> _subscriptions = new List<SubscriptionItem>();

        public VirtualOpcUaClient()
        {
            Set(LaserOpcNode.MachineStatusCode, (int)MachineStatus.Idle);
            Set(LaserOpcNode.MachineOpcMode, (short)MachineOperatingMode.OnlineLocal);
            Set(LaserOpcNode.ChangeOpcModeAck, (short)0);
        }

        public Task<OpcReadResult> ReadAsync(NodeDescriptor node, CancellationToken cancellationToken = default(CancellationToken))
        {
            object value;
            if (_values.TryGetValue(node.SpecTag, out value)) return Task.FromResult(new OpcReadResult(true, value, "Good"));
            return Task.FromResult(new OpcReadResult(true, null, "Good"));
        }

        public Task<OpcWriteResult> WriteAsync(NodeDescriptor node, object value, CancellationToken cancellationToken = default(CancellationToken))
        {
            Set(node.Node, value);
            bool request = value is bool && (bool)value;
            if (node.Node == LaserOpcNode.ChangeOpcModeRequest && request)
            {
                object target;
                short targetMode = (short)MachineOperatingMode.OnlineLocal;
                if (_values.TryGetValue(LaserOpcNodeCatalog.Get(LaserOpcNode.RequestedOpcMode).SpecTag, out target)) targetMode = Convert.ToInt16(target);
                Set(LaserOpcNode.MachineOpcMode, targetMode);
                Set(LaserOpcNode.ChangeOpcModeAck, (short)1);
            }
            if (node.Node == LaserOpcNode.ChangeOpcModeRequest && !request) Set(LaserOpcNode.ChangeOpcModeAck, (short)0);
            if (node.Node == LaserOpcNode.GetRecipeAck && Convert.ToInt16(value) == 1)
            {
                Set(LaserOpcNode.MachineStatusCode, (int)MachineStatus.Ready);
                Copy(LaserOpcNode.RemoteLotId, LaserOpcNode.ActiveLotId);
                Copy(LaserOpcNode.RecipeProgramFile, LaserOpcNode.ActiveProgramFile);
                Copy(LaserOpcNode.RecipeConditionFile, LaserOpcNode.ActiveConditionFile);
                Set(LaserOpcNode.GetRecipeRequest, false);
            }
            if (node.Node == LaserOpcNode.StartScheduleRequest && request) Set(LaserOpcNode.MachineStatusCode, (int)MachineStatus.Running);
            return Task.FromResult(new OpcWriteResult(true, "Good"));
        }

        public void SimulateRecipeRequest(string lotId) { Set(LaserOpcNode.RemoteLotId, lotId); Set(LaserOpcNode.GetRecipeRequest, true); Set(LaserOpcNode.MachineStatusCode, (int)MachineStatus.Idle); }
        public void SimulateOperatorStart() { Set(LaserOpcNode.MachineStatusCode, (int)MachineStatus.Running); }

        public IDisposable Subscribe(IReadOnlyCollection<NodeDescriptor> nodes, Action<OpcSubscriptionUpdate> onUpdate)
        {
            HashSet<string> tags = new HashSet<string>(nodes.Select(x => x.SpecTag), StringComparer.Ordinal);
            SubscriptionItem item = new SubscriptionItem(tags, onUpdate);
            _subscriptions.Add(item);
            return new Subscription(delegate { _subscriptions.Remove(item); });
        }

        private void Set(LaserOpcNode node, object value)
        {
            NodeDescriptor descriptor = LaserOpcNodeCatalog.Get(node);
            _values[descriptor.SpecTag] = value;
            foreach (SubscriptionItem subscription in _subscriptions.Where(x => x.Tags.Contains(descriptor.SpecTag)).ToArray()) subscription.Callback(new OpcSubscriptionUpdate(descriptor, value, "Good", DateTimeOffset.Now));
        }
        private void Copy(LaserOpcNode source, LaserOpcNode target) { object value; if (_values.TryGetValue(LaserOpcNodeCatalog.Get(source).SpecTag, out value)) Set(target, value); }

        private sealed class SubscriptionItem { public SubscriptionItem(HashSet<string> tags, Action<OpcSubscriptionUpdate> callback) { Tags = tags; Callback = callback; } public HashSet<string> Tags { get; private set; } public Action<OpcSubscriptionUpdate> Callback { get; private set; } }
        private sealed class Subscription : IDisposable { private Action _dispose; public Subscription(Action dispose) { _dispose = dispose; } public void Dispose() { Action dispose = Interlocked.Exchange(ref _dispose, null); if (dispose != null) dispose(); } }
    }
}
