using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Logging;
using MitsubishiLaserMES.Core.Models.Config;
using MitsubishiLaserMES.Core.Models.Eap;
using MitsubishiLaserMES.Core.Services.Eap;
using MitsubishiLaserMES.Core.Services.Opc;

namespace MitsubishiLaserMES.Core.Services.Coordination
{
    public class MesCoordinator : IMesCoordinator
    {
        private readonly AppConfig _config;
        private readonly ILogService _logger;
        private readonly List<TrackedInOrderInfo> _trackedInOrders = new List<TrackedInOrderInfo>();

        public IEapMqttService EapService { get; }
        public IOpcService OpcService { get; }

        public string CurrentOperatorId { get; private set; } = string.Empty;
        public string CurrentOperatorName { get; private set; } = string.Empty;
        public bool IsOperatorLoggedIn => !string.IsNullOrWhiteSpace(CurrentOperatorId);
        public bool IsTrackedIn => CurrentOrder != null;
        public TrackedInOrderInfo CurrentOrder { get; private set; }
        public IReadOnlyList<TrackedInOrderInfo> TrackedInOrders => _trackedInOrders.AsReadOnly();

        public event Action<string, string> OperatorLoggedIn;
        public event Action OperatorLoggedOut;
        public event Action<TrackedInOrderInfo> TrackInCompleted;
        public event Action<string> TrackOutCompleted;
        public event Action<string> TerminalMessageNotified;
        public event Action<string> SystemLogMessage;

        public MesCoordinator(AppConfig config, IEapMqttService eapService, IOpcService opcService, ILogService logger = null)
        {
            _config = config ?? new AppConfig();
            _logger = logger ?? LogService.Instance;
            if (!string.IsNullOrWhiteSpace(_config.Mqtt?.EqID))
            {
                _logger.DefaultEquipmentId = _config.Mqtt.EqID;
            }

            EapService = eapService;
            OpcService = opcService;

            // 綁定日誌
            EapService.LogMessage += msg =>
            {
                _logger.Info("MQTT", msg, _config.Mqtt?.EqID);
                SystemLogMessage?.Invoke(msg);
            };
            OpcService.LogMessage += msg =>
            {
                _logger.Info("OPC", msg, _config.Mqtt?.EqID);
                SystemLogMessage?.Invoke(msg);
            };

            // 綁定 OPC 狀態與事件監聽
            OpcService.StatusLightChanged += OnOpcStatusLightChanged;
            OpcService.ProcessedCountChanged += OnOpcProcessedCountChanged;
            OpcService.AlarmTriggered += OnOpcAlarmTriggered;

            // 綁定 EAP 下行指令監聽
            EapService.RemoteCommandReceived += OnRemoteCommandReceived;
            EapService.TerminalDisplayReceived += OnTerminalDisplayReceived;
            EapService.TimeCalibrationReceived += OnTimeCalibrationReceived;
        }

        public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
        {
            SystemLogMessage?.Invoke("[系統初始化] 開始連線上位 EAP 與機台 OPC...");
            var eapTask = EapService.ConnectAsync(cancellationToken);
            var opcTask = OpcService.ConnectAsync(cancellationToken);
            await Task.WhenAll(eapTask, opcTask).ConfigureAwait(false);

            SystemLogMessage?.Invoke($"[系統初始化完成] EAP連線: {EapService.IsConnected}, OPC連線: {OpcService.IsConnected}");
            return EapService.IsConnected || OpcService.IsConnected;
        }

        public async Task<UserVerifyReplyPayload> LoginWithBarcodeAsync(string userBarcode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userBarcode))
            {
                return new UserVerifyReplyPayload { RtnResult = "FAIL", RtnMsg = "請輸入或掃描人員二維條碼" };
            }

            _logger.Info("UserVerify", $"正在向 EAP 驗證工號/條碼: {userBarcode}");
            SystemLogMessage?.Invoke($"[人員驗證] 正在向 EAP 驗證工號/條碼: {userBarcode}");
            var req = new UserVerifyReqPayload
            {
                Machine = _config.Mqtt.EqID,
                Data = new List<EventReportItem>
                {
                    new EventReportItem { Parameter = "1023", Value = userBarcode }
                }
            };

            var reply = await EapService.SendRequestAsync<UserVerifyReqPayload, UserVerifyReplyPayload>(req, cancellationToken).ConfigureAwait(false);
            if (reply.IsPass)
            {
                CurrentOperatorId = userBarcode;
                CurrentOperatorName = string.IsNullOrWhiteSpace(reply.RtnMsg) ? userBarcode : reply.RtnMsg;
                OperatorLoggedIn?.Invoke(CurrentOperatorId, CurrentOperatorName);
                _logger.Info("UserVerify", $"人員登入成功，工號: {CurrentOperatorId}, 姓名: {CurrentOperatorName}");
                SystemLogMessage?.Invoke($"[人員登入成功] 工號: {CurrentOperatorId}, 姓名: {CurrentOperatorName}");
            }
            else
            {
                _logger.Warn("UserVerify", $"人員登入失敗: {reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[人員登入失敗] {reply.RtnMsg}");
            }
            return reply;
        }

        public void LogoutOperator()
        {
            _logger.Info("UserLogout", $"人員登出: 工號 {CurrentOperatorId}");
            CurrentOperatorId = string.Empty;
            CurrentOperatorName = string.Empty;
            OperatorLoggedOut?.Invoke();
            SystemLogMessage?.Invoke("[人員登出] 目前已切換為未登入狀態。");
        }

        public async Task<ReplyTrackInReqPayload> TrackInAsync(TrackInReqPayload req, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(req.UserID))
            {
                req.UserID = CurrentOperatorId;
            }
            req.Machine = _config.Mqtt.EqID;

            _logger.Info("TrackIn", $"發送進站請求: 工單={req.WorkOrder}, 批號={req.BatchNo}, 卡匣={req.CassetteID}, 數量={req.Qty}");
            SystemLogMessage?.Invoke($"[工單進站] 發送 TrackInReq: 工單={req.WorkOrder}, 批號={req.BatchNo}, 卡匣={req.CassetteID}, 數量={req.Qty}");
            var reply = await EapService.SendRequestAsync<TrackInReqPayload, ReplyTrackInReqPayload>(req, cancellationToken).ConfigureAwait(false);

            if (reply.IsPass)
            {
                string targetRecipe = !string.IsNullOrWhiteSpace(reply.RecipeID) ? reply.RecipeID : req.RecipeID;
                short qty = 0;
                short.TryParse(req.Qty, out qty);

                var orderInfo = new TrackedInOrderInfo
                {
                    WorkOrder = req.WorkOrder,
                    CassetteId = req.CassetteID,
                    RecipeId = targetRecipe,
                    PartNo = reply.PartNo,
                    ProcessNo = reply.ProcessNo,
                    ProcessName = reply.ProcessName,
                    TotalQty = qty,
                    TrackInTime = DateTime.Now
                };

                CurrentOrder = orderInfo;
                _trackedInOrders.Add(orderInfo);
                TrackInCompleted?.Invoke(orderInfo);

                // 若有 Recipe 且 OPC 已連線，自動執行配方交握
                if (!string.IsNullOrWhiteSpace(targetRecipe) && OpcService.IsConnected)
                {
                    _logger.Info("RecipeHandshake", $"自動向雷射機下發 Recipe: {targetRecipe}, 片數: {qty}");
                    SystemLogMessage?.Invoke($"[自動配方切換] 進站核可，自動向雷射機下發 Recipe: {targetRecipe}");
                    _ = OpcService.DeliverRecipeAsync(targetRecipe, qty > 0 ? qty : (short)-1, cancellationToken);
                }

                _logger.Info("TrackIn", $"工單進站成功: 工單={req.WorkOrder}, 合法Panel數={reply.PanelList?.Count ?? 0}");
                SystemLogMessage?.Invoke($"[工單進站成功] 工單={req.WorkOrder}, 合法Panel數={reply.PanelList?.Count ?? 0}");
            }
            else
            {
                _logger.Warn("TrackIn", $"工單進站失敗: {reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[工單進站失敗] 原因: {reply.RtnMsg}");
            }

            return reply;
        }

        public async Task<ReplyTrackOutReqPayload> TrackOutAsync(TrackOutReqPayload req, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(req.UserID))
            {
                req.UserID = CurrentOperatorId;
            }
            req.Machine = _config.Mqtt.EqID;

            _logger.Info("TrackOut", $"發送出站請求: 出站數量={req.Qty}, 結果={req.Result}, NGCode={req.NGCode}");
            SystemLogMessage?.Invoke($"[工單出站] 發送 TrackOutReq: 出站數量={req.Qty}, 結果={req.Result}, NGCode={req.NGCode}");
            var reply = await EapService.SendRequestAsync<TrackOutReqPayload, ReplyTrackOutReqPayload>(req, cancellationToken).ConfigureAwait(false);

            if (reply.IsPass)
            {
                string wo = req.WorkOrder?.FirstOrDefault() ?? (CurrentOrder?.WorkOrder ?? "");
                _trackedInOrders.RemoveAll(o => o.WorkOrder == wo);
                if (CurrentOrder?.WorkOrder == wo)
                {
                    CurrentOrder = null;
                }
                TrackOutCompleted?.Invoke(wo);
                _logger.Info("TrackOut", $"工單出站成功: 工單 {wo} 帳務過帳完成。");
                SystemLogMessage?.Invoke($"[工單出站成功] 工單 {wo} 帳務過帳完成。");
            }
            else
            {
                _logger.Warn("TrackOut", $"工單出站失敗: 原因={reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[工單出站失敗] 原因: {reply.RtnMsg}");
            }

            return reply;
        }

        public async Task<bool> SwitchOpcModeAsync(short mode, CancellationToken cancellationToken = default)
        {
            _logger.Info("SwitchMode", $"請求切換 OPC 模式為: {mode}");
            return await OpcService.ChangeOperatingModeAsync(mode, cancellationToken).ConfigureAwait(false);
        }

        #region OPC 事件監聽 ➔ EAP 上報

        private void OnOpcStatusLightChanged(MachineStatusLight oldLight, MachineStatusLight newLight)
        {
            _logger.Info("MachineStatus", $"OPC 燈號變更: {(int)oldLight} ({oldLight}) ➔ {(int)newLight} ({newLight})");
            SystemLogMessage?.Invoke($"[OPC 燈號變更] {(int)oldLight} ({oldLight}) ➔ {(int)newLight} ({newLight})");
            var report = new StatusChangeReportPayload
            {
                Machine = _config.Mqtt.EqID,
                OldStatus = ((int)oldLight).ToString(),
                NewStatus = ((int)newLight).ToString()
            };
            _ = EapService.PublishReportAsync(report);
        }

        private void OnOpcAlarmTriggered(string code, string msg, bool isStart)
        {
            string stateStr = isStart ? "Start(發生)" : "End(解除)";
            if (isStart)
            {
                _logger.Warn("Alarm", $"代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            }
            else
            {
                _logger.Info("Alarm", $"代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            }
            SystemLogMessage?.Invoke($"[OPC 警報事件] 代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            var report = new AlarmReportPayload
            {
                Machine = _config.Mqtt.EqID,
                AlarmStatus = isStart ? "Start" : "End",
                AlarmCode = code,
                AlarmMsg = msg,
                AlarmType = "A"
            };
            _ = EapService.PublishReportAsync(report);
        }

        private void OnOpcProcessedCountChanged(int oldVal, int newVal)
        {
            _logger.Info("ProcessData", $"OPC 加工計數遞增: {oldVal} ➔ {newVal} (預定: {OpcService.ScheduledCount})");
            SystemLogMessage?.Invoke($"[OPC 加工計數遞增] 完成片數: {oldVal} ➔ {newVal}");

            // 自動組裝製程資料上報
            var report = new ProcessDataReportPayload
            {
                Machine = _config.Mqtt.EqID,
                UserID = CurrentOperatorId,
                WorkOrder = CurrentOrder?.WorkOrder ?? string.Empty,
                RecipeID = CurrentOrder?.RecipeId ?? string.Empty,
                CassetteID = CurrentOrder?.CassetteId ?? string.Empty,
                Result = "PASS",
                ProcessTime = "0.0",
                ProcessData = new List<ProcessDataItem>
                {
                    new ProcessDataItem { Parameter = "ProcessedCount", Value = newVal.ToString() },
                    new ProcessDataItem { Parameter = "ScheduledCount", Value = OpcService.ScheduledCount.ToString() }
                }
            };
            _ = EapService.PublishReportAsync(report);
        }

        #endregion

        #region EAP 下行指令處理

        private void OnRemoteCommandReceived(RemoteCommandReqPayload cmd)
        {
            SystemLogMessage?.Invoke($"[EAP 遠端指令] Type: {cmd.RemoteCMDType}, Recipe: {cmd.RecipeID}");
            if (string.Equals(cmd.RemoteCMDType, "PP_SELECT", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(cmd.RecipeID))
                {
                    _ = OpcService.DeliverRecipeAsync(cmd.RecipeID, -1);
                }
            }
            else if (string.Equals(cmd.RemoteCMDType, "START", StringComparison.OrdinalIgnoreCase))
            {
                _ = OpcService.StartScheduleAsync();
            }
            else if (string.Equals(cmd.RemoteCMDType, "STOP", StringComparison.OrdinalIgnoreCase))
            {
                _ = OpcService.StopScheduleAsync();
            }
        }

        private void OnTerminalDisplayReceived(TerminalDisplayReqPayload msg)
        {
            SystemLogMessage?.Invoke($"[EAP 終端訊息] {msg.Message}");
            TerminalMessageNotified?.Invoke(msg.Message);
        }

        private void OnTimeCalibrationReceived(string serverDate)
        {
            SystemLogMessage?.Invoke($"[EAP 時間校正] 收到標準時間: {serverDate}");
        }

        #endregion

        public void Dispose()
        {
            EapService?.Dispose();
            OpcService?.Dispose();
        }
    }
}
