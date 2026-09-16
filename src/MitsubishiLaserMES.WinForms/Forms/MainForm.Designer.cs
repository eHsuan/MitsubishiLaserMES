using System;
using System.Drawing;
using System.Windows.Forms;

namespace MitsubishiLaserMES.WinForms.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // 頂部區塊
        private GroupBox grpCurrentOperator;
        private Label lblOpId;
        private Label lblOpName;
        private TextBox txtCurrentOpId;
        private TextBox txtCurrentOpName;

        private GroupBox grpAuth;
        private Label lblBarcode;
        private Label lblUserId;
        private Label lblPassword;
        private TextBox txtBarcode;
        private TextBox txtLoginUserId;
        private TextBox txtLoginPassword;
        private Button btnUserAuth;
        private Button btnClearAuth;

        private Button btnSettings;
        private Button btnConnectMes;
        private Button btnToggleLang;
        private Button btnDisconnectMes;
        private Panel pnlMqttLed;
        private Label lblMqttLed;

        // 主 TabControl
        private TabControl tabMain;
        private TabPage tabWorkOrder;
        private TabPage tabEquipment;
        private TabPage tabItTest;

        // Tab 1: 工單功能
        private GroupBox grpOrderInfo;
        private Label lblWo;
        private Label lblBatchNo;
        private Label lblPartNo;
        private Label lblProcessNo;
        private Label lblProcessName;
        private Label lblTotalQty;
        private Label lblRunQty;
        private Label lblCompletedQty;
        private Label lblRecipeId;
        private Label lblIsTrackedIn;
        private Label lblComponentNo;

        private TextBox txtWorkOrder;
        private TextBox txtBatchNo;
        private TextBox txtPartNo;
        private TextBox txtProcessNo;
        private TextBox txtProcessName;
        private TextBox txtTotalQty;
        private TextBox txtRunQty;
        private TextBox txtCompletedQty;
        private TextBox txtRecipeId;
        private TextBox txtIsTrackedIn;
        private TextBox txtComponentNo;

        private GroupBox grpNgInput;
        private CheckBox chkNoNg;
        private DataGridView dgvNgList;
        private Button btnAddNg;
        private Button btnRemoveNg;

        private GroupBox grpTrackedInList;
        private DataGridView dgvTrackedIn;

        private GroupBox grpManageParams;
        private DataGridView dgvManageParams;

        private GroupBox grpOrderActions;
        private Button btnQueryOrder;
        private Button btnTrackIn;
        private Button btnTrackOut;
        private Button btnChangeUser;
        private Button btnClearOrder;

        // Tab 2: 設備功能
        private GroupBox grpOpcStatus;
        private Label lblOpcConn;
        private Label lblMachineState;
        private Label lblActivePrg;
        private Label lblProgress;
        private Panel pnlLampGreen;
        private Panel pnlLampYellow;
        private Panel pnlLampRed;
        private Label lblLampGreen;
        private Label lblLampYellow;
        private Label lblLampRed;
        private Button btnModeLocal;
        private Button btnModeSemiAuto;
        private Button btnModeAuto;
        private Button btnStartSchedule;

        private GroupBox grpAlarmList;
        private DataGridView dgvAlarms;

        // Tab 3: IT 測試
        private GroupBox grpTestActions;
        private Button btnTestAlive;
        private Button btnTestProcessData;
        private Button btnTestAlarmStart;
        private Button btnTestAlarmEnd;
        private Button btnClearLogs;
        private DataGridView dgvMqttLogs;

        // 底部執行結果
        private GroupBox grpResult;
        private Label lblResult;
        private Label lblResultCode;
        private Label lblResultMessage;
        private TextBox txtResult;
        private TextBox txtResultCode;
        private TextBox txtResultMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "MES - " + DateTime.Now.ToString("yyyy/MM/dd");
            this.Size = new Size(1180, 800);
            this.MinimumSize = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("微軟正黑體", 9F, FontStyle.Regular);

            // ================= 頂部區塊 =================
            // 1. 當前作業人員
            grpCurrentOperator = new GroupBox
            {
                Text = "當前作業人員 :",
                Location = new Point(12, 8),
                Size = new Size(260, 100)
            };
            lblOpId = new Label { Text = "工號:", Location = new Point(15, 28), AutoSize = true };
            txtCurrentOpId = new TextBox { Location = new Point(65, 25), Size = new Size(180, 23), ReadOnly = true, BackColor = Color.WhiteSmoke };
            lblOpName = new Label { Text = "姓名:", Location = new Point(15, 62), AutoSize = true };
            txtCurrentOpName = new TextBox { Location = new Point(65, 59), Size = new Size(180, 23), ReadOnly = true, BackColor = Color.WhiteSmoke };
            grpCurrentOperator.Controls.AddRange(new Control[] { lblOpId, txtCurrentOpId, lblOpName, txtCurrentOpName });

            // 2. 帳密驗證
            grpAuth = new GroupBox
            {
                Text = "帳密驗證",
                Location = new Point(280, 8),
                Size = new Size(540, 100)
            };
            lblBarcode = new Label { Text = "工號二維碼:", Location = new Point(12, 28), AutoSize = true };
            txtBarcode = new TextBox { Location = new Point(95, 25), Size = new Size(300, 23) };
            btnUserAuth = new Button { Text = "作業員帳號認證", Location = new Point(405, 23), Size = new Size(120, 28) };

            lblUserId = new Label { Text = "工號:", Location = new Point(12, 62), AutoSize = true };
            txtLoginUserId = new TextBox { Location = new Point(95, 59), Size = new Size(120, 23) };
            lblPassword = new Label { Text = "密碼:", Location = new Point(225, 62), AutoSize = true };
            txtLoginPassword = new TextBox { Location = new Point(265, 59), Size = new Size(130, 23), PasswordChar = '*' };
            btnClearAuth = new Button { Text = "清除", Location = new Point(405, 57), Size = new Size(120, 28) };
            grpAuth.Controls.AddRange(new Control[] { lblBarcode, txtBarcode, btnUserAuth, lblUserId, txtLoginUserId, lblPassword, txtLoginPassword, btnClearAuth });

            // 3. 右上功能按鈕
            btnSettings = new Button { Text = "MES 設定", Location = new Point(840, 18), Size = new Size(110, 32) };
            btnConnectMes = new Button { Text = "MES 連線", Location = new Point(965, 18), Size = new Size(110, 32) };
            btnToggleLang = new Button { Text = "中英切換", Location = new Point(840, 60), Size = new Size(110, 32) };
            btnDisconnectMes = new Button { Text = "MES 離線", Location = new Point(965, 60), Size = new Size(110, 32) };

            pnlMqttLed = new Panel { Location = new Point(1090, 26), Size = new Size(22, 22), BackColor = Color.LightGray, BorderStyle = BorderStyle.FixedSingle };
            lblMqttLed = new Label { Text = "EAP", Location = new Point(1085, 53), AutoSize = true, Font = new Font("Arial", 8F, FontStyle.Bold) };

            // ================= 主 TabControl =================
            tabMain = new TabControl
            {
                Location = new Point(12, 115),
                Size = new Size(1140, 500),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            tabWorkOrder = new TabPage { Text = "工單功能" };
            tabEquipment = new TabPage { Text = "設備功能" };
            tabItTest = new TabPage { Text = "IT 測試" };
            tabMain.TabPages.AddRange(new TabPage[] { tabWorkOrder, tabEquipment, tabItTest });

            // ----------------- Tab 1: 工單功能 -----------------
            // 工單訊息
            grpOrderInfo = new GroupBox
            {
                Text = "工單訊息",
                Location = new Point(10, 10),
                Size = new Size(540, 230)
            };

            lblWo = new Label { Text = "工單號碼 :", Location = new Point(15, 25), AutoSize = true };
            txtWorkOrder = new TextBox { Location = new Point(95, 22), Size = new Size(160, 23) };
            lblBatchNo = new Label { Text = "BatchNo :", Location = new Point(275, 25), AutoSize = true };
            txtBatchNo = new TextBox { Location = new Point(365, 22), Size = new Size(160, 23) };

            lblPartNo = new Label { Text = "料號 :", Location = new Point(15, 55), AutoSize = true };
            txtPartNo = new TextBox { Location = new Point(95, 52), Size = new Size(160, 23) };
            lblProcessNo = new Label { Text = "製程編號 :", Location = new Point(275, 55), AutoSize = true };
            txtProcessNo = new TextBox { Location = new Point(365, 52), Size = new Size(160, 23) };

            lblProcessName = new Label { Text = "製程名稱 :", Location = new Point(15, 85), AutoSize = true };
            txtProcessName = new TextBox { Location = new Point(95, 82), Size = new Size(430, 23) };

            lblTotalQty = new Label { Text = "工單數量 :", Location = new Point(15, 115), AutoSize = true };
            txtTotalQty = new TextBox { Location = new Point(95, 112), Size = new Size(430, 23) };

            lblRunQty = new Label { Text = "工單跑貨片數 :", Location = new Point(15, 145), AutoSize = true };
            txtRunQty = new TextBox { Location = new Point(105, 142), Size = new Size(150, 23) };
            lblCompletedQty = new Label { Text = "已完成片數 :", Location = new Point(275, 145), AutoSize = true };
            txtCompletedQty = new TextBox { Location = new Point(365, 142), Size = new Size(160, 23), ReadOnly = true, BackColor = Color.WhiteSmoke };

            lblRecipeId = new Label { Text = "RecipeID :", Location = new Point(15, 175), AutoSize = true };
            txtRecipeId = new TextBox { Location = new Point(95, 172), Size = new Size(430, 23) };

            lblIsTrackedIn = new Label { Text = "是否已進站 :", Location = new Point(15, 203), AutoSize = true };
            txtIsTrackedIn = new TextBox { Location = new Point(95, 200), Size = new Size(160, 23), ReadOnly = true, BackColor = Color.WhiteSmoke };
            lblComponentNo = new Label { Text = "ComponentNo :", Location = new Point(265, 203), AutoSize = true };
            txtComponentNo = new TextBox { Location = new Point(365, 200), Size = new Size(160, 23) };

            grpOrderInfo.Controls.AddRange(new Control[]
            {
                lblWo, txtWorkOrder, lblBatchNo, txtBatchNo,
                lblPartNo, txtPartNo, lblProcessNo, txtProcessNo,
                lblProcessName, txtProcessName,
                lblTotalQty, txtTotalQty,
                lblRunQty, txtRunQty, lblCompletedQty, txtCompletedQty,
                lblRecipeId, txtRecipeId,
                lblIsTrackedIn, txtIsTrackedIn, lblComponentNo, txtComponentNo
            });

            // 不良輸入
            grpNgInput = new GroupBox
            {
                Text = "不良輸入",
                Location = new Point(560, 10),
                Size = new Size(380, 230)
            };
            chkNoNg = new CheckBox { Text = "本站無不良", Location = new Point(15, 20), AutoSize = true, Checked = true };
            btnAddNg = new Button { Text = "新增不良", Location = new Point(200, 16), Size = new Size(80, 25) };
            btnRemoveNg = new Button { Text = "刪除不良", Location = new Point(290, 16), Size = new Size(80, 25) };

            dgvNgList = new DataGridView
            {
                Location = new Point(15, 48),
                Size = new Size(355, 170),
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvNgList.Columns.Add("NGCode", "NGCode");
            dgvNgList.Columns.Add("NGChineseName", "NG Chinese Name");
            dgvNgList.Columns.Add("Qty", "Qty");
            grpNgInput.Controls.AddRange(new Control[] { chkNoNg, btnAddNg, btnRemoveNg, dgvNgList });

            // 工單作業按鈕區
            grpOrderActions = new GroupBox
            {
                Text = "工單作業",
                Location = new Point(955, 10),
                Size = new Size(165, 455),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnQueryOrder = new Button { Text = "工單查詢", Location = new Point(15, 25), Size = new Size(135, 38), Font = new Font("微軟正黑體", 10F, FontStyle.Bold) };
            btnTrackIn = new Button { Text = "工單進站", Location = new Point(15, 75), Size = new Size(135, 38), Font = new Font("微軟正黑體", 10F, FontStyle.Bold), BackColor = Color.LightSteelBlue };
            btnTrackOut = new Button { Text = "工單出站", Location = new Point(15, 125), Size = new Size(135, 38), Font = new Font("微軟正黑體", 10F, FontStyle.Bold), BackColor = Color.LightSkyBlue };
            btnChangeUser = new Button { Text = "更換人員", Location = new Point(15, 210), Size = new Size(135, 34) };
            btnClearOrder = new Button { Text = "清除", Location = new Point(15, 255), Size = new Size(135, 34) };
            grpOrderActions.Controls.AddRange(new Control[] { btnQueryOrder, btnTrackIn, btnTrackOut, btnChangeUser, btnClearOrder });

            // 已進站工單清單
            grpTrackedInList = new GroupBox
            {
                Text = "已進站工單清單",
                Location = new Point(10, 245),
                Size = new Size(260, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            dgvTrackedIn = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvTrackedIn.Columns.Add("WoNo", "工單號碼(WoNo)");
            dgvTrackedIn.Columns.Add("Other1", "其他1");
            grpTrackedInList.Controls.Add(dgvTrackedIn);

            // 管理項目 參數
            grpManageParams = new GroupBox
            {
                Text = "管理項目 參 數",
                Location = new Point(280, 245),
                Size = new Size(660, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvManageParams = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvManageParams.Columns.Add("Name", "管理項目名稱(Manage item name)");
            dgvManageParams.Columns.Add("Value", "管理項目值(Manage item values)");
            dgvManageParams.Columns.Add("MustInput", "需輸入?(Must Input?)");
            dgvManageParams.Columns.Add("InputType", "輸入類型(Input type)");
            grpManageParams.Controls.Add(dgvManageParams);

            tabWorkOrder.Controls.AddRange(new Control[]
            {
                grpOrderInfo, grpNgInput, grpOrderActions, grpTrackedInList, grpManageParams
            });

            // ----------------- Tab 2: 設備功能 -----------------
            grpOpcStatus = new GroupBox
            {
                Text = "三菱雷射機狀態與控制 (OPC UA)",
                Location = new Point(15, 15),
                Size = new Size(1095, 180)
            };
            lblOpcConn = new Label { Text = "OPC 連線: 離線中", Location = new Point(20, 30), AutoSize = true, Font = new Font("微軟正黑體", 10F, FontStyle.Bold) };
            lblMachineState = new Label { Text = "機台狀態代碼: [3] Idle", Location = new Point(220, 30), AutoSize = true, Font = new Font("微軟正黑體", 10F, FontStyle.Bold) };
            lblProgress = new Label { Text = "加工計數: 0 / 0", Location = new Point(480, 30), AutoSize = true, Font = new Font("微軟正黑體", 10F) };
            lblActivePrg = new Label { Text = "目前加工程式: 無", Location = new Point(20, 65), AutoSize = true };

            // 三色燈視覺
            pnlLampGreen = new Panel { Location = new Point(20, 100), Size = new Size(24, 24), BackColor = Color.DarkGreen, BorderStyle = BorderStyle.FixedSingle };
            lblLampGreen = new Label { Text = "綠燈 (伺服ON/運行)", Location = new Point(50, 104), AutoSize = true };

            pnlLampYellow = new Panel { Location = new Point(200, 100), Size = new Size(24, 24), BackColor = Color.DarkGoldenrod, BorderStyle = BorderStyle.FixedSingle };
            lblLampYellow = new Label { Text = "黃燈 (待機/BeamON)", Location = new Point(230, 104), AutoSize = true };

            pnlLampRed = new Panel { Location = new Point(380, 100), Size = new Size(24, 24), BackColor = Color.DarkRed, BorderStyle = BorderStyle.FixedSingle };
            lblLampRed = new Label { Text = "紅燈 (警報/異常)", Location = new Point(410, 104), AutoSize = true };

            // 控制按鈕
            btnModeLocal = new Button { Text = "Local 模式", Location = new Point(600, 95), Size = new Size(95, 32) };
            btnModeSemiAuto = new Button { Text = "Semi-Auto 模式", Location = new Point(705, 95), Size = new Size(115, 32) };
            btnModeAuto = new Button { Text = "Auto 模式", Location = new Point(830, 95), Size = new Size(95, 32) };
            btnStartSchedule = new Button { Text = "啟動連續運轉", Location = new Point(940, 95), Size = new Size(125, 32), BackColor = Color.PaleGreen };

            grpOpcStatus.Controls.AddRange(new Control[]
            {
                lblOpcConn, lblMachineState, lblProgress, lblActivePrg,
                pnlLampGreen, lblLampGreen, pnlLampYellow, lblLampYellow, pnlLampRed, lblLampRed,
                btnModeLocal, btnModeSemiAuto, btnModeAuto, btnStartSchedule
            });

            grpAlarmList = new GroupBox
            {
                Text = "當前活躍警報清單 (Active Alarms 000~009)",
                Location = new Point(15, 205),
                Size = new Size(1095, 260),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvAlarms = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvAlarms.Columns.Add("AlarmCode", "警報代碼");
            dgvAlarms.Columns.Add("AlarmMsg", "警報訊息內容");
            dgvAlarms.Columns.Add("AlarmType", "等級");
            dgvAlarms.Columns.Add("Time", "偵測時間");
            grpAlarmList.Controls.Add(dgvAlarms);

            tabEquipment.Controls.AddRange(new Control[] { grpOpcStatus, grpAlarmList });

            // ----------------- Tab 3: IT 測試 -----------------
            grpTestActions = new GroupBox
            {
                Text = "MQTT 通訊測試與模擬工具",
                Location = new Point(15, 15),
                Size = new Size(1095, 80)
            };
            btnTestAlive = new Button { Text = "測試存活檢測 (AreYouThere)", Location = new Point(15, 28), Size = new Size(190, 35) };
            btnTestProcessData = new Button { Text = "模擬製程資料上報", Location = new Point(220, 28), Size = new Size(160, 35) };
            btnTestAlarmStart = new Button { Text = "模擬警報發生 (Start)", Location = new Point(395, 28), Size = new Size(160, 35) };
            btnTestAlarmEnd = new Button { Text = "模擬警報解除 (End)", Location = new Point(570, 28), Size = new Size(160, 35) };
            btnClearLogs = new Button { Text = "清空通訊日誌", Location = new Point(745, 28), Size = new Size(120, 35) };
            grpTestActions.Controls.AddRange(new Control[] { btnTestAlive, btnTestProcessData, btnTestAlarmStart, btnTestAlarmEnd, btnClearLogs });

            var grpLog = new GroupBox
            {
                Text = "MQTT 即時封包監聽紀錄",
                Location = new Point(15, 105),
                Size = new Size(1095, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvMqttLogs = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvMqttLogs.Columns.Add("Time", "時間");
            dgvMqttLogs.Columns.Add("Direction", "方向");
            dgvMqttLogs.Columns.Add("Topic", "主題 (Topic)");
            dgvMqttLogs.Columns.Add("Payload", "訊息內文 (JSON Payload)");
            grpLog.Controls.Add(dgvMqttLogs);

            tabItTest.Controls.AddRange(new Control[] { grpTestActions, grpLog });

            // ================= 底部執行結果區 =================
            grpResult = new GroupBox
            {
                Text = "執行結果",
                Location = new Point(12, 620),
                Size = new Size(1140, 130),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            lblResult = new Label { Text = "Result :", Location = new Point(15, 25), AutoSize = true };
            txtResult = new TextBox { Location = new Point(75, 22), Size = new Size(120, 23), ReadOnly = true, Font = new Font("微軟正黑體", 9.5F, FontStyle.Bold) };

            lblResultCode = new Label { Text = "ResultCode :", Location = new Point(15, 58), AutoSize = true };
            txtResultCode = new TextBox { Location = new Point(95, 55), Size = new Size(100, 23), ReadOnly = true };

            lblResultMessage = new Label { Text = "ResultMessage :", Location = new Point(220, 25), AutoSize = true };
            txtResultMessage = new TextBox
            {
                Location = new Point(325, 22),
                Size = new Size(800, 95),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.WhiteSmoke
            };

            grpResult.Controls.AddRange(new Control[]
            {
                lblResult, txtResult, lblResultCode, txtResultCode, lblResultMessage, txtResultMessage
            });

            // 根表單配置
            this.Controls.AddRange(new Control[]
            {
                grpCurrentOperator, grpAuth,
                btnSettings, btnConnectMes, btnToggleLang, btnDisconnectMes,
                pnlMqttLed, lblMqttLed,
                tabMain,
                grpResult
            });
        }
    }
}
