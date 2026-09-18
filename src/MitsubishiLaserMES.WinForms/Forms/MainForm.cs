using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Models.Config;
using MitsubishiLaserMES.Core.Models.Eap;
using MitsubishiLaserMES.Core.Services.Coordination;
using Protocol.Core.Messages;
using Protocol.Core.Enums;

namespace MitsubishiLaserMES.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private readonly IMesCoordinator _coordinator;
        private readonly AppConfig _config;
        private bool _isEnglish = false;

        /// <summary>
        /// 提供 Visual Studio 設計工具 (WinForms Designer) 視覺化檢視與拖拉排版使用的無參數建構函式
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }
        }

        /// <summary>
        /// 執行時期由依賴注入與進入點呼叫之主要建構函式
        /// </summary>
        public MainForm(IMesCoordinator coordinator, AppConfig config) : this()
        {
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _config = config ?? new AppConfig();

            BindCoordinatorEvents();
            BindFormEvents();
            InitSampleData();
            UpdateEapLed();
        }

        private void BindCoordinatorEvents()
        {
            // 人員登入狀態
            _coordinator.OperatorLoggedIn += (id, name) => SafeInvoke(() =>
            {
                txtCurrentOpId.Text = id;
                txtCurrentOpName.Text = name;
                txtBarcode.Clear();
                SetResult("PASS", "200", $"人員 {name} ({id}) 登入驗證成功。");
            });

            _coordinator.OperatorLoggedOut += () => SafeInvoke(() =>
            {
                txtCurrentOpId.Clear();
                txtCurrentOpName.Clear();
                SetResult("INFO", "0", "人員已更換/登出。");
            });

            // 工單進站成功
            _coordinator.TrackInCompleted += order => SafeInvoke(() =>
            {
                txtWorkOrder.Text = order.WorkOrder;
                txtBatchNo.Text = order.WorkOrder;
                txtPartNo.Text = order.PartNo;
                txtProcessNo.Text = order.ProcessNo;
                txtProcessName.Text = order.ProcessName;
                txtTotalQty.Text = order.TotalQty.ToString();
                txtRecipeId.Text = order.RecipeId;
                txtIsTrackedIn.Text = "已進站";
                txtIsTrackedIn.ForeColor = Color.DarkGreen;

                // 更新已進站清單與按鈕狀態
                dgvTrackedIn.Rows.Add(order.WorkOrder, order.CassetteId);
                btnTrackIn.Enabled = false;
                btnTrackOut.Enabled = true;
                SetResult("PASS", "200", $"工單 {order.WorkOrder} 進站成功，配方: {order.RecipeId}");
            });

            // 工單出站成功
            _coordinator.TrackOutCompleted += wo => SafeInvoke(() =>
            {
                for (int i = dgvTrackedIn.Rows.Count - 1; i >= 0; i--)
                {
                    if (dgvTrackedIn.Rows[i].Cells[0].Value?.ToString() == wo)
                    {
                        dgvTrackedIn.Rows.RemoveAt(i);
                    }
                }
                txtIsTrackedIn.Text = "未進站";
                txtIsTrackedIn.ForeColor = Color.Black;
                btnTrackIn.Enabled = true;
                btnTrackOut.Enabled = false;
                SetResult("PASS", "200", $"工單 {wo} 出站過帳成功。");
            });

            // EAP 終端訊息
            _coordinator.TerminalMessageNotified += msg => SafeInvoke(() =>
            {
                using var form = new TerminalMessageForm(msg);
                form.ShowDialog(this);
            });

            // 系統日誌
            _coordinator.SystemLogMessage += msg => SafeInvoke(() =>
            {
                AppendLog(msg);
            });

            // EAP 連線狀態
            _coordinator.EapService.ConnectionStateChanged += connected => SafeInvoke(() =>
            {
                UpdateEapLed();
            });

            _coordinator.EapService.AliveStatusChanged += isGreen => SafeInvoke(() =>
            {
                UpdateEapLed();
            });

            // MQTT 封包日誌
            _coordinator.EapService.MessageSentLog += (topic, payload) => SafeInvoke(() =>
            {
                AddMqttLog("發送 (EQ ➔ EAP)", topic, payload);
            });

            _coordinator.EapService.MessageReceivedLog += (topic, payload) => SafeInvoke(() =>
            {
                AddMqttLog("接收 (EAP ➔ EQ)", topic, payload);
            });

            // OPC 機台監聽
            _coordinator.OpcService.ConnectionStateChanged += conn => SafeInvoke(() =>
            {
                lblOpcConn.Text = $"OPC 連線: {(conn ? (_coordinator.OpcService.IsVirtual ? "已連線 (虛擬模擬器)" : "已連線 (實機)") : "離線中")}";
                lblOpcConn.ForeColor = conn ? Color.DarkGreen : Color.DarkRed;
            });

            _coordinator.OpcService.StatusLightChanged += (oldL, newL) => SafeInvoke(() =>
            {
                UpdateStatusLamps(newL);
            });

            _coordinator.OpcService.ProcessedCountChanged += (oldP, newP) => SafeInvoke(() =>
            {
                txtCompletedQty.Text = newP.ToString();
                lblProgress.Text = $"加工計數: {newP} / {_coordinator.OpcService.ScheduledCount}";
            });

            _coordinator.OpcService.AlarmTriggered += (code, msg, isStart) => SafeInvoke(() =>
            {
                if (isStart)
                {
                    dgvAlarms.Rows.Add(code, msg, "A", DateTime.Now.ToString("HH:mm:ss"));
                }
                else
                {
                    for (int i = dgvAlarms.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dgvAlarms.Rows[i].Cells[0].Value?.ToString() == code)
                        {
                            dgvAlarms.Rows.RemoveAt(i);
                        }
                    }
                }
            });
        }

        private void BindFormEvents()
        {
            this.Load += async (s, e) =>
            {
                // 自動初始化連線
                await _coordinator.InitializeAsync();
            };

            // 掃碼槍 Enter 鍵自動認證
            txtBarcode.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await DoUserAuthAsync();
                }
            };

            btnUserAuth.Click += async (s, e) => await DoUserAuthAsync();
            btnClearAuth.Click += (s, e) =>
            {
                txtBarcode.Clear();
                txtLoginUserId.Clear();
                txtLoginPassword.Clear();
                txtBarcode.Focus();
            };

            // 右上功能
            btnSettings.Click += (s, e) =>
            {
                using var form = new SettingsForm(_config);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show(this, "設定已更新！若更動連線位址，請點擊「MES 離線」再重新點擊「MES 連線」。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnConnectMes.Click += async (s, e) =>
            {
                await MaskWaitForm.RunWithWaitAsync(this, "正在重新建立 MES 與 OPC 連線...", async () =>
                {
                    return await _coordinator.InitializeAsync();
                });
            };

            btnDisconnectMes.Click += async (s, e) =>
            {
                await _coordinator.EapService.DisconnectAsync();
                await _coordinator.OpcService.DisconnectAsync();
                SetResult("OFFLINE", "0", "已切換為離線模式。");
            };

            btnToggleLang.Click += (s, e) => ToggleLanguage();

            // 工單作業按鈕初值
            btnTrackIn.Enabled = true;
            btnTrackOut.Enabled = false;

            btnTrackIn.Click += async (s, e) => await DoTrackInAsync();
            btnTrackOut.Click += async (s, e) => await DoTrackOutAsync();
            btnChangeUser.Click += (s, e) =>
            {
                _coordinator.LogoutOperator();
                txtBarcode.Focus();
            };
            btnClearOrder.Click += (s, e) => ClearOrderInputs();

            // 不良項增減
            btnAddNg.Click += (s, e) =>
            {
                chkNoNg.Checked = false;
                dgvNgList.Rows.Add("NG01", "孔偏/鑽孔不良", 1);
            };
            btnRemoveNg.Click += (s, e) =>
            {
                if (dgvNgList.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvNgList.SelectedRows)
                    {
                        dgvNgList.Rows.Remove(row);
                    }
                }
                else if (dgvNgList.Rows.Count > 0)
                {
                    dgvNgList.Rows.RemoveAt(dgvNgList.Rows.Count - 1);
                }
            };

            // 設備功能按鈕
            btnModeLocal.Click += async (s, e) => await _coordinator.SwitchOpcModeAsync((short)MitsubishiOpcMode.OnlineLocal);
            btnModeSemiAuto.Click += async (s, e) => await _coordinator.SwitchOpcModeAsync((short)MitsubishiOpcMode.OnlineSemiAuto);
            btnModeAuto.Click += async (s, e) => await _coordinator.SwitchOpcModeAsync((short)MitsubishiOpcMode.OnlineAuto);
            btnStartSchedule.Click += async (s, e) =>
            {
                bool ok = await _coordinator.OpcService.StartScheduleAsync();
                SetResult(ok ? "PASS" : "FAIL", "0", ok ? "連續運轉啟動成功" : "連續運轉啟動失敗");
            };

            btnOpenSimulator.Click += (s, e) =>
            {
                var simForm = new MitsubishiLaser.Simulator.Forms.SimulatorMainForm(MitsubishiLaserMES.Core.Simulator.LaserMachineSimulatorEngine.SharedInstance);
                simForm.Show(this);
            };

            // IT 測試按鈕
            btnTestAlive.Click += async (s, e) =>
            {
                var ping = new AliveCheckReqPayload { Machine = _config.Mqtt.EqID };
                var reply = await _coordinator.EapService.SendRequestAsync<AliveCheckReqPayload, AliveCheckReplyPayload>(ping);
                SetResult(reply.RtnResult, "0", $"存活測試結果: {reply.RtnResult} ({reply.RtnMsg})");
            };

            btnTestProcessData.Click += async (s, e) =>
            {
                var report = new ProcessDataReportPayload
                {
                    Machine = _config.Mqtt.EqID,
                    UserID = _coordinator.CurrentOperatorId,
                    WorkOrder = txtWorkOrder.Text,
                    RecipeID = txtRecipeId.Text,
                    Result = "PASS",
                    ProcessTime = "125.0",
                    ProcessData = new List<ProcessDataItem>
                    {
                        new ProcessDataItem { Parameter = "PeakPower", Value = "5500" },
                        new ProcessDataItem { Parameter = "PulseFreq", Value = "100" }
                    }
                };
                bool ok = await _coordinator.EapService.PublishReportAsync(report);
                SetResult(ok ? "PASS" : "FAIL", "0", "模擬製程資料已發布");
            };

            btnTestAlarmStart.Click += async (s, e) =>
            {
                var alarm = new AlarmReportPayload
                {
                    Machine = _config.Mqtt.EqID,
                    AlarmStatus = "Start",
                    AlarmCode = "ERR001",
                    AlarmMsg = "雷射發振器異常",
                    AlarmType = "A"
                };
                await _coordinator.EapService.PublishReportAsync(alarm);
                SetResult("PASS", "0", "已模擬發送警報開始");
            };

            btnTestAlarmEnd.Click += async (s, e) =>
            {
                var alarm = new AlarmReportPayload
                {
                    Machine = _config.Mqtt.EqID,
                    AlarmStatus = "End",
                    AlarmCode = "ERR001",
                    AlarmMsg = "雷射發振器異常解除",
                    AlarmType = "A"
                };
                await _coordinator.EapService.PublishReportAsync(alarm);
                SetResult("PASS", "0", "已模擬發送警報解除");
            };

            btnClearLogs.Click += (s, e) => dgvMqttLogs.Rows.Clear();
        }

        private async Task DoUserAuthAsync()
        {
            string barcode = txtBarcode.Text.Trim();
            if (string.IsNullOrWhiteSpace(barcode))
            {
                barcode = txtLoginUserId.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(barcode))
            {
                MessageBox.Show(this, "請刷入工號二維條碼或手動輸入工號！", "驗證提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            var reply = await MaskWaitForm.RunWithWaitAsync(this, "正在向 EAP 驗證人員權限...", async () =>
            {
                return await _coordinator.LoginWithBarcodeAsync(barcode);
            });

            if (!reply.IsPass)
            {
                MessageBox.Show(this, $"人員驗證失敗：{reply.RtnMsg}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetResult("FAIL", "401", reply.RtnMsg);
            }
        }

        private async Task DoTrackInAsync()
        {
            if (!_coordinator.IsOperatorLoggedIn)
            {
                MessageBox.Show(this, "請先執行作業員認證登入，再進行工單進站！", "權限提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            string wo = txtWorkOrder.Text.Trim();
            if (string.IsNullOrWhiteSpace(wo))
            {
                MessageBox.Show(this, "請輸入工單號碼！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWorkOrder.Focus();
                return;
            }

            var req = new TrackInReqMessage
            {
                CMD = CommandType.TrackInReq,
                WorkOrder = new List<string> { wo },
                CassetteID = new List<string> { "C" + wo },
                MaterialID = txtPartNo.Text.Trim(),
                UserID = _coordinator.CurrentOperatorId,
                ToolingID = string.Empty,
                PanelID = string.Empty,
                Qty = string.IsNullOrWhiteSpace(txtTotalQty.Text) ? "10" : txtTotalQty.Text.Trim()
            };

            var reply = await MaskWaitForm.RunWithWaitAsync(this, "系統處理中... 正在向 EAP 申請工單進站與配方交握確認", async () =>
            {
                return await _coordinator.TrackInAsync(req);
            });

            if (reply.RtnResult != RtnResult.PASS)
            {
                MessageBox.Show(this, $"工單進站失敗：{reply.RtnMsg}", "EAP 進站核可失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetResult("FAIL", "500", reply.RtnMsg);
            }
        }

        private async Task DoTrackOutAsync()
        {
            if (!_coordinator.IsTrackedIn && string.IsNullOrWhiteSpace(txtWorkOrder.Text))
            {
                MessageBox.Show(this, "當前無進行中或進站中之工單！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string wo = txtWorkOrder.Text.Trim();
            int qty = 10;
            int.TryParse(txtTotalQty.Text, out qty);

            var req = new TrackOutReqPayload
            {
                WorkOrder = new List<string> { wo },
                CassetteID = new List<string> { "C" + wo },
                MaterialID = txtPartNo.Text.Trim(),
                UserID = _coordinator.CurrentOperatorId,
                Qty = qty.ToString(),
                Result = chkNoNg.Checked ? "PASS" : "FAIL"
            };

            // 收集不良清單
            if (!chkNoNg.Checked)
            {
                foreach (DataGridViewRow row in dgvNgList.Rows)
                {
                    string code = row.Cells["NGCode"].Value?.ToString() ?? "";
                    string name = row.Cells["NGChineseName"].Value?.ToString() ?? "";
                    int ngQty = Convert.ToInt32(row.Cells["Qty"].Value ?? 1);
                    req.NgDetails.Add(new NgItem { NGCode = code, NGChineseName = name, Qty = ngQty });
                }
                req.NGCode = req.NgDetails.FirstOrDefault()?.NGCode ?? "NG";
            }

            var reply = await MaskWaitForm.RunWithWaitAsync(this, "系統處理中... 正在向 EAP 執行工單出站過帳", async () =>
            {
                return await _coordinator.TrackOutAsync(req);
            });

            if (!reply.IsPass)
            {
                MessageBox.Show(this, $"工單出站失敗：{reply.RtnMsg}", "EAP 出站過帳失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetResult("FAIL", "500", reply.RtnMsg);
            }
        }

        private void ClearOrderInputs()
        {
            txtWorkOrder.Clear();
            txtBatchNo.Clear();
            txtPartNo.Clear();
            txtProcessNo.Clear();
            txtProcessName.Clear();
            txtTotalQty.Clear();
            txtRunQty.Clear();
            txtCompletedQty.Clear();
            txtRecipeId.Clear();
            txtIsTrackedIn.Text = "未進站";
            txtIsTrackedIn.ForeColor = Color.Black;
            txtComponentNo.Clear();
            dgvNgList.Rows.Clear();
            chkNoNg.Checked = true;
            if (!_coordinator.IsTrackedIn)
            {
                btnTrackIn.Enabled = true;
                btnTrackOut.Enabled = false;
            }
        }

        private void InitSampleData()
        {
            // 預設填入範例資料
            txtWorkOrder.Text = "WO2026091601";
            txtBatchNo.Text = "B2026091601";
            txtPartNo.Text = "MAT-ML-001";
            txtProcessNo.Text = "PRC-DRILL-01";
            txtProcessName.Text = "CO2 雷射盲孔鑽孔製程";
            txtTotalQty.Text = "20";
            txtRunQty.Text = "20";
            txtCompletedQty.Text = "0";
            txtRecipeId.Text = "RECIPE_MITSUBISHI_01";
            txtIsTrackedIn.Text = "未進站";
            // 管理項目參數：優先自 Profile.ManagementItems 與 Profile.Variables 載入
            dgvManageParams.Rows.Clear();
            if (_config.Profile?.ManagementItems != null && _config.Profile.ManagementItems.Count > 0)
            {
                foreach (var item in _config.Profile.ManagementItems)
                {
                    string name = _config.Profile.GetVariableName(item.Key, item.Key);
                    dgvManageParams.Rows.Add(name, item.Key, item.Value == "Y" ? "是" : "否", "設定");
                }
            }
            else
            {
                dgvManageParams.Rows.Add("雷射發振頻率 (Hz)", "100", "是", "數值");
                dgvManageParams.Rows.Add("雷射功率 (W)", "5500", "是", "數值");
                dgvManageParams.Rows.Add("吸著氣壓 (kPa)", "-10.5", "否", "數值");
            }
        }

        private void UpdateEapLed()
        {
            if (!_coordinator.EapService.IsConnected)
            {
                pnlMqttLed.BackColor = Color.LightGray;
            }
            else if (_coordinator.EapService.IsAliveGreen)
            {
                pnlMqttLed.BackColor = Color.LimeGreen;
            }
            else
            {
                pnlMqttLed.BackColor = Color.Gold;
            }
        }

        private void UpdateStatusLamps(MachineStatusLight light)
        {
            pnlLampGreen.BackColor = Color.DarkGreen;
            pnlLampYellow.BackColor = Color.DarkGoldenrod;
            pnlLampRed.BackColor = Color.DarkRed;

            switch (light)
            {
                case MachineStatusLight.AutoRunning:
                    pnlLampGreen.BackColor = Color.LimeGreen;
                    break;
                case MachineStatusLight.Alarm:
                case MachineStatusLight.SafetyStop:
                case MachineStatusLight.QualityStop:
                    pnlLampRed.BackColor = Color.Red;
                    break;
                default:
                    pnlLampYellow.BackColor = Color.Gold;
                    break;
            }

            lblMachineState.Text = $"機台狀態代碼: [{(int)light}] {light}";
        }

        private void AddMqttLog(string direction, string topic, string payload)
        {
            dgvMqttLogs.Rows.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff"), direction, topic, payload);
            if (dgvMqttLogs.Rows.Count > 100)
            {
                dgvMqttLogs.Rows.RemoveAt(dgvMqttLogs.Rows.Count - 1);
            }
        }

        private void SetResult(string result, string code, string message)
        {
            txtResult.Text = result;
            txtResult.ForeColor = result == "PASS" ? Color.DarkGreen : (result == "FAIL" ? Color.Red : Color.Black);
            txtResultCode.Text = code;
            AppendLog(message);
        }

        private void AppendLog(string message)
        {
            if (txtResultMessage.TextLength > 30000)
            {
                txtResultMessage.Text = txtResultMessage.Text.Substring(15000);
            }
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}\r\n";
            txtResultMessage.AppendText(line);
            txtResultMessage.SelectionStart = txtResultMessage.TextLength;
            txtResultMessage.ScrollToCaret();
        }

        private void ToggleLanguage()
        {
            _isEnglish = !_isEnglish;
            if (_isEnglish)
            {
                btnUserAuth.Text = "Operator Auth";
                btnClearAuth.Text = "Clear";
                btnSettings.Text = "MES Config";
                btnConnectMes.Text = "MES Online";
                btnDisconnectMes.Text = "MES Offline";
                btnToggleLang.Text = "中文 / EN";
                tabWorkOrder.Text = "Work Order";
                tabEquipment.Text = "Equipment";
                tabItTest.Text = "IT Testing";
                btnTrackIn.Text = "Track In";
                btnTrackOut.Text = "Track Out";
                btnChangeUser.Text = "Switch User";
                btnClearOrder.Text = "Clear";
            }
            else
            {
                btnUserAuth.Text = "作業員帳號認證";
                btnClearAuth.Text = "清除";
                btnSettings.Text = "MES 設定";
                btnConnectMes.Text = "MES 連線";
                btnDisconnectMes.Text = "MES 離線";
                btnToggleLang.Text = "中英切換";
                tabWorkOrder.Text = "工單功能";
                tabEquipment.Text = "設備功能";
                tabItTest.Text = "IT 測試";
                btnTrackIn.Text = "工單進站";
                btnTrackOut.Text = "工單出站";
                btnChangeUser.Text = "更換人員";
                btnClearOrder.Text = "清除";
            }
        }

        private void SafeInvoke(Action action)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
