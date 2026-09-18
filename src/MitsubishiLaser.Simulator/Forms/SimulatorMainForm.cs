using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MitsubishiLaserMES.Core.Simulator;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaser.Simulator.Forms
{
    public partial class SimulatorMainForm : Form
    {
        private readonly ILaserMachineSimulator _simulator;

        public SimulatorMainForm() : this(LaserMachineSimulatorEngine.SharedInstance)
        {
        }

        public SimulatorMainForm(ILaserMachineSimulator simulator)
        {
            InitializeComponent();
            _simulator = simulator ?? LaserMachineSimulatorEngine.SharedInstance;

            InitCustomUI();
            BindEvents();
        }

        private void InitCustomUI()
        {
            // 初始化預置警報清單
            cboPresetAlarms.Items.Clear();
            cboPresetAlarms.Items.Add("7001 - 氣壓不足警報 (Air Pressure Low)");
            cboPresetAlarms.Items.Add("7002 - 光學鏡片溫度過高 (Lens Overheat)");
            cboPresetAlarms.Items.Add("7005 - 安全防護門未關閉 (Safety Door Open)");
            cboPresetAlarms.Items.Add("7010 - 工作台吸附壓力異常 (Vacuum Low)");
            cboPresetAlarms.Items.Add("7020 - 伺服軸過載 (Servo Axis Overload)");
            cboPresetAlarms.Items.Add("7030 - 冷卻水流量異常 (Coolant Flow Low)");
            if (cboPresetAlarms.Items.Count > 0)
            {
                cboPresetAlarms.SelectedIndex = 0;
            }

            // 初始狀態刷新
            chkWatchDogEnable.Checked = _simulator.IsWatchDogEnabled;
            RefreshDashboard();
        }

        private void BindEvents()
        {
            // 監聽引擎事件
            _simulator.StateChanged += OnSimulatorStateChanged;
            _simulator.LogEmitted += OnSimulatorLogEmitted;

            // WatchDog 控制
            chkWatchDogEnable.CheckedChanged += (s, e) =>
            {
                _simulator.IsWatchDogEnabled = chkWatchDogEnable.Checked;
                RefreshDashboard();
            };
            btnFeedDog.Click += (s, e) =>
            {
                _simulator.FeedWatchDog();
            };

            // 批號刷卡
            btnInputLot.Click += (s, e) =>
            {
                string lot = txtLotInput.Text.Trim();
                if (string.IsNullOrEmpty(lot))
                {
                    MessageBox.Show("請輸入欲刷入之批號 (Lot ID)", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _simulator.OperatorInputLot(lot);
            };

            // 運轉控制
            btnStartRun.Click += (s, e) => _simulator.StartSchedule();
            btnStopRun.Click += (s, e) => _simulator.StopSchedule();
            btnSimFailTarget.Click += (s, e) => _simulator.SimulateTargetFailure();

            // 模式切換
            btnSetOffline.Click += (s, e) => _simulator.SetOperatingMode(MachineOperatingMode.Offline);
            btnSetLocal.Click += (s, e) => _simulator.SetOperatingMode(MachineOperatingMode.OnlineLocal);
            btnSetSemiAuto.Click += (s, e) => _simulator.SetOperatingMode(MachineOperatingMode.OnlineSemiAuto);
            btnSetAuto.Click += (s, e) => _simulator.SetOperatingMode(MachineOperatingMode.OnlineAuto);

            // 警報觸發
            btnTriggerAlarm.Click += (s, e) =>
            {
                long alarmNo = 0;
                string msg = string.Empty;

                if (!string.IsNullOrWhiteSpace(txtCustomAlarmNo.Text) &&
                    long.TryParse(txtCustomAlarmNo.Text.Trim(), out long parsedNo))
                {
                    alarmNo = parsedNo;
                    msg = string.IsNullOrWhiteSpace(txtCustomAlarmMsg.Text)
                        ? "自訂警報"
                        : txtCustomAlarmMsg.Text.Trim();
                }
                else if (cboPresetAlarms.SelectedItem != null)
                {
                    string selected = cboPresetAlarms.SelectedItem.ToString();
                    var parts = selected.Split(new[] { " - " }, StringSplitOptions.None);
                    if (parts.Length >= 2 && long.TryParse(parts[0], out long pNo))
                    {
                        alarmNo = pNo;
                        msg = parts[1];
                    }
                }

                if (alarmNo > 0)
                {
                    _simulator.TriggerAlarm(alarmNo, msg, "A");
                }
            };

            // 警報復歸
            btnAlReset.Click += (s, e) => _simulator.ResetAlarm(-1);

            // 清空日誌
            btnClearLog.Click += (s, e) => txtLog.Clear();

            // 釋放資源
            this.FormClosing += (s, e) =>
            {
                _simulator.StateChanged -= OnSimulatorStateChanged;
                _simulator.LogEmitted -= OnSimulatorLogEmitted;
            };
        }

        private void OnSimulatorStateChanged()
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Action(OnSimulatorStateChanged));
                }
                catch { }
                return;
            }

            RefreshDashboard();
        }

        private void OnSimulatorLogEmitted(string message)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Action<string>(OnSimulatorLogEmitted), message);
                }
                catch { }
                return;
            }

            if (txtLog.TextLength > 30000)
            {
                txtLog.Text = txtLog.Text.Substring(15000);
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void RefreshDashboard()
        {
            // 1. 三色燈看板
            pnlGreen.BackColor = _simulator.WarningLampGreen ? Color.LimeGreen : Color.DarkGreen;
            pnlYellow.BackColor = _simulator.WarningLampYellow ? Color.Gold : Color.DarkGoldenrod;
            pnlRed.BackColor = _simulator.WarningLampRed ? Color.Red : Color.DarkRed;

            // 2. 機台狀態徽章
            lblStatusBadge.Text = $"機台狀態: {_simulator.StatusCode}";
            switch (_simulator.StatusCode)
            {
                case MachineStatus.Running:
                    lblStatusBadge.BackColor = Color.ForestGreen;
                    lblStatusBadge.ForeColor = Color.White;
                    break;
                case MachineStatus.Ready:
                    lblStatusBadge.BackColor = Color.SeaGreen;
                    lblStatusBadge.ForeColor = Color.White;
                    break;
                case MachineStatus.MachineDown:
                    lblStatusBadge.BackColor = Color.Crimson;
                    lblStatusBadge.ForeColor = Color.White;
                    lblStatusBadge.Text = "機台狀態: 警報停機 (MachineDown)";
                    break;
                case MachineStatus.Idle:
                default:
                    lblStatusBadge.BackColor = Color.SteelBlue;
                    lblStatusBadge.ForeColor = Color.White;
                    break;
            }

            // 3. OPC 模式徽章
            lblModeBadge.Text = $"OPC 模式: {_simulator.OpcMode}";

            // 4. WatchDog 狀態
            int countdown = Math.Max(0, Math.Min(10, _simulator.WatchDogCountdownSec));
            pbWatchDog.Value = countdown;

            if (!_simulator.IsWatchDogEnabled)
            {
                lblWatchDogVal.Text = "10s [心跳監控已停用]";
                lblWatchDogVal.ForeColor = Color.DimGray;
            }
            else if (_simulator.OpcMode == MachineOperatingMode.Offline)
            {
                lblWatchDogVal.Text = "10s [離線不監控心跳]";
                lblWatchDogVal.ForeColor = Color.DimGray;
            }
            else if (!_simulator.HasReceivedFirstHeartbeat)
            {
                lblWatchDogVal.Text = "10s [待命中 (等待上位機連線...)]";
                lblWatchDogVal.ForeColor = Color.SteelBlue;
            }
            else if (_simulator.IsWatchDogTimeout)
            {
                lblWatchDogVal.Text = "0s [連線逾時異常!]";
                lblWatchDogVal.ForeColor = Color.Red;
            }
            else
            {
                lblWatchDogVal.Text = $"{countdown}s (Host: {_simulator.LastHostWatchDog})";
                lblWatchDogVal.ForeColor = Color.DarkGreen;
            }

            // 5. 配方交握看板
            lblGetRecipeReq.Text = $"GetRecipe.Req: {(_simulator.GetRecipeRequest ? "TRUE (交握請求中)" : "False (閒置)")}";
            lblGetRecipeReq.ForeColor = _simulator.GetRecipeRequest ? Color.DarkOrange : Color.ForestGreen;

            txtReqPrg.Text = _simulator.RequestedProgramFile;
            txtReqCnd.Text = _simulator.RequestedConditionFile;
            txtReqSheet.Text = _simulator.RequestedSheetNum.ToString();
            txtGetRecipeAck.Text = _simulator.GetRecipeAck.ToString();

            // 6. 連續運轉與加工進度
            lblActiveLot.Text = $"當前批號: {(string.IsNullOrEmpty(_simulator.ActiveLotId) ? "--" : _simulator.ActiveLotId)}";
            lblActivePrg.Text = $"加工程式: {(string.IsNullOrEmpty(_simulator.ActiveProgramFile) ? "--" : _simulator.ActiveProgramFile)} | 條件檔: {(string.IsNullOrEmpty(_simulator.ActiveConditionFile) ? "--" : _simulator.ActiveConditionFile)}";

            int maxCount = Math.Max(1, (int)_simulator.ScheduledCount);
            pbProcessing.Maximum = maxCount;
            pbProcessing.Value = Math.Min((int)_simulator.ProcessedCount, maxCount);

            lblCounts.Text = $"總片數: {_simulator.ScheduledCount} | 已加工: {_simulator.ProcessedCount} | 未加工: {_simulator.UnProcessedCount} | 週期: {_simulator.CurrentCycleTime:F1}s";
            lblLaserParams.Text = $"雷射功率: {_simulator.ActivePower} W | 頻率: {_simulator.ActiveFrequency} Hz | 脈寬: {_simulator.ActivePulseWidth:F1} % | 真空吸附: {_simulator.TableAdsorbPressure:F1} kPa | 鏡片溫度: {_simulator.LensTemp:F1} °C";

            // 7. 警報清單
            dgvAlarms.Rows.Clear();
            foreach (var alm in _simulator.Alarms.Where(a => a.IsActive))
            {
                dgvAlarms.Rows.Add(
                    $"Slot_{alm.SlotIndex:D3}",
                    alm.AlarmNo,
                    alm.AlarmMessage,
                    alm.AlarmType,
                    alm.TriggerTime.ToString("yyyy/MM/dd HH:mm:ss")
                );
            }
        }
    }
}
