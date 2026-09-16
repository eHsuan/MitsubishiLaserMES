using System;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Nodes;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaserMES.Core.Services.Opc
{
    /// <summary>
    /// 三菱雷射加工機 WatchDog 心跳服務 (原廠 SPEC 4-2 要求每 10 秒內必須遞增數值)
    /// </summary>
    public class OpcWatchdogService : IDisposable
    {
        private readonly IOpcUaClient _client;
        private CancellationTokenSource _cts;
        private Task _workerTask;
        private ushort _counter = 0;

        public bool IsRunning { get; private set; }

        public OpcWatchdogService(IOpcUaClient client)
        {
            _client = client;
        }

        public void Start(int intervalSec = 1)
        {
            if (IsRunning) return;
            _cts = new CancellationTokenSource();
            IsRunning = true;
            _workerTask = Task.Run(() => LoopAsync(intervalSec, _cts.Token));
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            _cts?.Cancel();
            try
            {
                _workerTask?.Wait(2000);
            }
            catch { }
        }

        private async Task LoopAsync(int intervalSec, CancellationToken token)
        {
            var nodeDesc = LaserOpcNodeCatalog.Get(LaserOpcNode.HostWatchDog);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    unchecked
                    {
                        _counter++;
                    }
                    await _client.WriteAsync(nodeDesc, _counter, token).ConfigureAwait(false);
                }
                catch
                {
                    // 寫入失敗可能是連線中斷，稍候重試
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(intervalSec), token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}
