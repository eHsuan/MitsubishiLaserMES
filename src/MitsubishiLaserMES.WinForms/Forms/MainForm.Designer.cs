namespace MitsubishiLaserMES.WinForms.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // 實例化頂部控制項
            this.grpCurrentOperator = new System.Windows.Forms.GroupBox();
            this.lblOpId = new System.Windows.Forms.Label();
            this.txtCurrentOpId = new System.Windows.Forms.TextBox();
            this.lblOpName = new System.Windows.Forms.Label();
            this.txtCurrentOpName = new System.Windows.Forms.TextBox();

            this.grpAuth = new System.Windows.Forms.GroupBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.btnUserAuth = new System.Windows.Forms.Button();
            this.lblUserId = new System.Windows.Forms.Label();
            this.txtLoginUserId = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtLoginPassword = new System.Windows.Forms.TextBox();
            this.btnClearAuth = new System.Windows.Forms.Button();

            this.btnSettings = new System.Windows.Forms.Button();
            this.btnConnectMes = new System.Windows.Forms.Button();
            this.btnToggleLang = new System.Windows.Forms.Button();
            this.btnDisconnectMes = new System.Windows.Forms.Button();
            this.pnlMqttLed = new System.Windows.Forms.Panel();
            this.lblMqttLed = new System.Windows.Forms.Label();

            // 實例化主 Tab
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabWorkOrder = new System.Windows.Forms.TabPage();
            this.tabEquipment = new System.Windows.Forms.TabPage();
            this.tabItTest = new System.Windows.Forms.TabPage();

            // 實例化 Tab 1 控制項
            this.grpOrderInfo = new System.Windows.Forms.GroupBox();
            this.lblWo = new System.Windows.Forms.Label();
            this.txtWorkOrder = new System.Windows.Forms.TextBox();
            this.lblBatchNo = new System.Windows.Forms.Label();
            this.txtBatchNo = new System.Windows.Forms.TextBox();
            this.lblPartNo = new System.Windows.Forms.Label();
            this.txtPartNo = new System.Windows.Forms.TextBox();
            this.lblProcessNo = new System.Windows.Forms.Label();
            this.txtProcessNo = new System.Windows.Forms.TextBox();
            this.lblProcessName = new System.Windows.Forms.Label();
            this.txtProcessName = new System.Windows.Forms.TextBox();
            this.lblTotalQty = new System.Windows.Forms.Label();
            this.txtTotalQty = new System.Windows.Forms.TextBox();
            this.lblRunQty = new System.Windows.Forms.Label();
            this.txtRunQty = new System.Windows.Forms.TextBox();
            this.lblCompletedQty = new System.Windows.Forms.Label();
            this.txtCompletedQty = new System.Windows.Forms.TextBox();
            this.lblRecipeId = new System.Windows.Forms.Label();
            this.txtRecipeId = new System.Windows.Forms.TextBox();
            this.lblIsTrackedIn = new System.Windows.Forms.Label();
            this.txtIsTrackedIn = new System.Windows.Forms.TextBox();
            this.lblComponentNo = new System.Windows.Forms.Label();
            this.txtComponentNo = new System.Windows.Forms.TextBox();

            this.grpNgInput = new System.Windows.Forms.GroupBox();
            this.chkNoNg = new System.Windows.Forms.CheckBox();
            this.btnAddNg = new System.Windows.Forms.Button();
            this.btnRemoveNg = new System.Windows.Forms.Button();
            this.dgvNgList = new System.Windows.Forms.DataGridView();

            this.grpTrackedInList = new System.Windows.Forms.GroupBox();
            this.dgvTrackedIn = new System.Windows.Forms.DataGridView();

            this.grpManageParams = new System.Windows.Forms.GroupBox();
            this.dgvManageParams = new System.Windows.Forms.DataGridView();

            this.grpOrderActions = new System.Windows.Forms.GroupBox();
            this.btnTrackIn = new System.Windows.Forms.Button();
            this.btnTrackOut = new System.Windows.Forms.Button();
            this.btnChangeUser = new System.Windows.Forms.Button();
            this.btnClearOrder = new System.Windows.Forms.Button();

            // 實例化 Tab 2 控制項
            this.grpOpcStatus = new System.Windows.Forms.GroupBox();
            this.lblOpcConn = new System.Windows.Forms.Label();
            this.lblMachineState = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblActivePrg = new System.Windows.Forms.Label();
            this.pnlLampGreen = new System.Windows.Forms.Panel();
            this.lblLampGreen = new System.Windows.Forms.Label();
            this.pnlLampYellow = new System.Windows.Forms.Panel();
            this.lblLampYellow = new System.Windows.Forms.Label();
            this.pnlLampRed = new System.Windows.Forms.Panel();
            this.lblLampRed = new System.Windows.Forms.Label();
            this.btnModeLocal = new System.Windows.Forms.Button();
            this.btnModeSemiAuto = new System.Windows.Forms.Button();
            this.btnModeAuto = new System.Windows.Forms.Button();
            this.btnStartSchedule = new System.Windows.Forms.Button();
            this.btnOpenSimulator = new System.Windows.Forms.Button();

            this.grpAlarmList = new System.Windows.Forms.GroupBox();
            this.dgvAlarms = new System.Windows.Forms.DataGridView();

            // 實例化 Tab 3 控制項
            this.grpTestActions = new System.Windows.Forms.GroupBox();
            this.btnTestAlive = new System.Windows.Forms.Button();
            this.btnTestProcessData = new System.Windows.Forms.Button();
            this.btnTestAlarmStart = new System.Windows.Forms.Button();
            this.btnTestAlarmEnd = new System.Windows.Forms.Button();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.dgvMqttLogs = new System.Windows.Forms.DataGridView();

            // 實例化 DataGridView 欄位
            this.colNgCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgChineseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgQty = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colWoNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOther1 = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colParamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colParamValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMustInput = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInputType = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colAlarmCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmTime = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colLogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogDir = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogTopic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogPayload = new System.Windows.Forms.DataGridViewTextBoxColumn();

            // 實例化底部控制項
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblResultCode = new System.Windows.Forms.Label();
            this.txtResultCode = new System.Windows.Forms.TextBox();
            this.lblResultMessage = new System.Windows.Forms.Label();
            this.txtResultMessage = new System.Windows.Forms.TextBox();

            // BeginInit DataGridViews
            ((System.ComponentModel.ISupportInitialize)(this.dgvNgList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrackedIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvManageParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMqttLogs)).BeginInit();

            this.grpCurrentOperator.SuspendLayout();
            this.grpAuth.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabWorkOrder.SuspendLayout();
            this.tabEquipment.SuspendLayout();
            this.tabItTest.SuspendLayout();
            this.grpOrderInfo.SuspendLayout();
            this.grpNgInput.SuspendLayout();
            this.grpTrackedInList.SuspendLayout();
            this.grpManageParams.SuspendLayout();
            this.grpOrderActions.SuspendLayout();
            this.grpOpcStatus.SuspendLayout();
            this.grpAlarmList.SuspendLayout();
            this.grpTestActions.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.SuspendLayout();

            // ================= grpCurrentOperator =================
            this.grpCurrentOperator.Controls.Add(this.lblOpId);
            this.grpCurrentOperator.Controls.Add(this.txtCurrentOpId);
            this.grpCurrentOperator.Controls.Add(this.lblOpName);
            this.grpCurrentOperator.Controls.Add(this.txtCurrentOpName);
            this.grpCurrentOperator.Location = new System.Drawing.Point(12, 8);
            this.grpCurrentOperator.Name = "grpCurrentOperator";
            this.grpCurrentOperator.Size = new System.Drawing.Size(260, 100);
            this.grpCurrentOperator.TabIndex = 0;
            this.grpCurrentOperator.TabStop = false;
            this.grpCurrentOperator.Text = "當前作業人員 :";

            this.lblOpId.AutoSize = true;
            this.lblOpId.Location = new System.Drawing.Point(15, 28);
            this.lblOpId.Name = "lblOpId";
            this.lblOpId.Size = new System.Drawing.Size(35, 17);
            this.lblOpId.TabIndex = 0;
            this.lblOpId.Text = "工號:";

            this.txtCurrentOpId.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCurrentOpId.Location = new System.Drawing.Point(65, 25);
            this.txtCurrentOpId.Name = "txtCurrentOpId";
            this.txtCurrentOpId.ReadOnly = true;
            this.txtCurrentOpId.Size = new System.Drawing.Size(180, 23);
            this.txtCurrentOpId.TabIndex = 1;

            this.lblOpName.AutoSize = true;
            this.lblOpName.Location = new System.Drawing.Point(15, 62);
            this.lblOpName.Name = "lblOpName";
            this.lblOpName.Size = new System.Drawing.Size(35, 17);
            this.lblOpName.TabIndex = 2;
            this.lblOpName.Text = "姓名:";

            this.txtCurrentOpName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCurrentOpName.Location = new System.Drawing.Point(65, 59);
            this.txtCurrentOpName.Name = "txtCurrentOpName";
            this.txtCurrentOpName.ReadOnly = true;
            this.txtCurrentOpName.Size = new System.Drawing.Size(180, 23);
            this.txtCurrentOpName.TabIndex = 3;

            // ================= grpAuth =================
            this.grpAuth.Controls.Add(this.lblBarcode);
            this.grpAuth.Controls.Add(this.txtBarcode);
            this.grpAuth.Controls.Add(this.btnUserAuth);
            this.grpAuth.Controls.Add(this.lblUserId);
            this.grpAuth.Controls.Add(this.txtLoginUserId);
            this.grpAuth.Controls.Add(this.lblPassword);
            this.grpAuth.Controls.Add(this.txtLoginPassword);
            this.grpAuth.Controls.Add(this.btnClearAuth);
            this.grpAuth.Location = new System.Drawing.Point(280, 8);
            this.grpAuth.Name = "grpAuth";
            this.grpAuth.Size = new System.Drawing.Size(540, 100);
            this.grpAuth.TabIndex = 1;
            this.grpAuth.TabStop = false;
            this.grpAuth.Text = "帳密驗證";

            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Location = new System.Drawing.Point(12, 28);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(71, 17);
            this.lblBarcode.TabIndex = 0;
            this.lblBarcode.Text = "工號二維碼:";

            this.txtBarcode.Location = new System.Drawing.Point(95, 25);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(300, 23);
            this.txtBarcode.TabIndex = 1;

            this.btnUserAuth.Location = new System.Drawing.Point(405, 23);
            this.btnUserAuth.Name = "btnUserAuth";
            this.btnUserAuth.Size = new System.Drawing.Size(120, 28);
            this.btnUserAuth.TabIndex = 2;
            this.btnUserAuth.Text = "作業員帳號認證";
            this.btnUserAuth.UseVisualStyleBackColor = true;

            this.lblUserId.AutoSize = true;
            this.lblUserId.Location = new System.Drawing.Point(12, 62);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(35, 17);
            this.lblUserId.TabIndex = 3;
            this.lblUserId.Text = "工號:";

            this.txtLoginUserId.Location = new System.Drawing.Point(95, 59);
            this.txtLoginUserId.Name = "txtLoginUserId";
            this.txtLoginUserId.Size = new System.Drawing.Size(120, 23);
            this.txtLoginUserId.TabIndex = 4;

            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(225, 62);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(35, 17);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "密碼:";

            this.txtLoginPassword.Location = new System.Drawing.Point(265, 59);
            this.txtLoginPassword.Name = "txtLoginPassword";
            this.txtLoginPassword.PasswordChar = '*';
            this.txtLoginPassword.Size = new System.Drawing.Size(130, 23);
            this.txtLoginPassword.TabIndex = 6;

            this.btnClearAuth.Location = new System.Drawing.Point(405, 57);
            this.btnClearAuth.Name = "btnClearAuth";
            this.btnClearAuth.Size = new System.Drawing.Size(120, 28);
            this.btnClearAuth.TabIndex = 7;
            this.btnClearAuth.Text = "清除";
            this.btnClearAuth.UseVisualStyleBackColor = true;

            // ================= 頂部右側按鈕 =================
            this.btnSettings.Location = new System.Drawing.Point(840, 18);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(110, 32);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "MES 設定";
            this.btnSettings.UseVisualStyleBackColor = true;

            this.btnConnectMes.Location = new System.Drawing.Point(965, 18);
            this.btnConnectMes.Name = "btnConnectMes";
            this.btnConnectMes.Size = new System.Drawing.Size(110, 32);
            this.btnConnectMes.TabIndex = 3;
            this.btnConnectMes.Text = "MES 連線";
            this.btnConnectMes.UseVisualStyleBackColor = true;

            this.btnToggleLang.Location = new System.Drawing.Point(840, 60);
            this.btnToggleLang.Name = "btnToggleLang";
            this.btnToggleLang.Size = new System.Drawing.Size(110, 32);
            this.btnToggleLang.TabIndex = 4;
            this.btnToggleLang.Text = "中英切換";
            this.btnToggleLang.UseVisualStyleBackColor = true;

            this.btnDisconnectMes.Location = new System.Drawing.Point(965, 60);
            this.btnDisconnectMes.Name = "btnDisconnectMes";
            this.btnDisconnectMes.Size = new System.Drawing.Size(110, 32);
            this.btnDisconnectMes.TabIndex = 5;
            this.btnDisconnectMes.Text = "MES 離線";
            this.btnDisconnectMes.UseVisualStyleBackColor = true;

            this.pnlMqttLed.BackColor = System.Drawing.Color.LightGray;
            this.pnlMqttLed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMqttLed.Location = new System.Drawing.Point(1090, 26);
            this.pnlMqttLed.Name = "pnlMqttLed";
            this.pnlMqttLed.Size = new System.Drawing.Size(22, 22);
            this.pnlMqttLed.TabIndex = 6;

            this.lblMqttLed.AutoSize = true;
            this.lblMqttLed.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.lblMqttLed.Location = new System.Drawing.Point(1085, 53);
            this.lblMqttLed.Name = "lblMqttLed";
            this.lblMqttLed.Size = new System.Drawing.Size(29, 14);
            this.lblMqttLed.TabIndex = 7;
            this.lblMqttLed.Text = "EAP";

            // ================= tabMain =================
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabWorkOrder);
            this.tabMain.Controls.Add(this.tabEquipment);
            this.tabMain.Controls.Add(this.tabItTest);
            this.tabMain.Location = new System.Drawing.Point(12, 115);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1140, 500);
            this.tabMain.TabIndex = 8;

            // ================= tabWorkOrder =================
            this.tabWorkOrder.Controls.Add(this.grpOrderInfo);
            this.tabWorkOrder.Controls.Add(this.grpNgInput);
            this.tabWorkOrder.Controls.Add(this.grpOrderActions);
            this.tabWorkOrder.Controls.Add(this.grpTrackedInList);
            this.tabWorkOrder.Controls.Add(this.grpManageParams);
            this.tabWorkOrder.Location = new System.Drawing.Point(4, 26);
            this.tabWorkOrder.Name = "tabWorkOrder";
            this.tabWorkOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabWorkOrder.Size = new System.Drawing.Size(1132, 470);
            this.tabWorkOrder.TabIndex = 0;
            this.tabWorkOrder.Text = "工單功能";
            this.tabWorkOrder.UseVisualStyleBackColor = true;

            // --- grpOrderInfo ---
            this.grpOrderInfo.Controls.Add(this.lblWo);
            this.grpOrderInfo.Controls.Add(this.txtWorkOrder);
            this.grpOrderInfo.Controls.Add(this.lblBatchNo);
            this.grpOrderInfo.Controls.Add(this.txtBatchNo);
            this.grpOrderInfo.Controls.Add(this.lblPartNo);
            this.grpOrderInfo.Controls.Add(this.txtPartNo);
            this.grpOrderInfo.Controls.Add(this.lblProcessNo);
            this.grpOrderInfo.Controls.Add(this.txtProcessNo);
            this.grpOrderInfo.Controls.Add(this.lblProcessName);
            this.grpOrderInfo.Controls.Add(this.txtProcessName);
            this.grpOrderInfo.Controls.Add(this.lblTotalQty);
            this.grpOrderInfo.Controls.Add(this.txtTotalQty);
            this.grpOrderInfo.Controls.Add(this.lblRunQty);
            this.grpOrderInfo.Controls.Add(this.txtRunQty);
            this.grpOrderInfo.Controls.Add(this.lblCompletedQty);
            this.grpOrderInfo.Controls.Add(this.txtCompletedQty);
            this.grpOrderInfo.Controls.Add(this.lblRecipeId);
            this.grpOrderInfo.Controls.Add(this.txtRecipeId);
            this.grpOrderInfo.Controls.Add(this.lblIsTrackedIn);
            this.grpOrderInfo.Controls.Add(this.txtIsTrackedIn);
            this.grpOrderInfo.Controls.Add(this.lblComponentNo);
            this.grpOrderInfo.Controls.Add(this.txtComponentNo);
            this.grpOrderInfo.Location = new System.Drawing.Point(10, 10);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(540, 230);
            this.grpOrderInfo.TabIndex = 0;
            this.grpOrderInfo.TabStop = false;
            this.grpOrderInfo.Text = "工單訊息";

            this.lblWo.AutoSize = true;
            this.lblWo.Location = new System.Drawing.Point(15, 25);
            this.lblWo.Name = "lblWo";
            this.lblWo.Size = new System.Drawing.Size(65, 17);
            this.lblWo.TabIndex = 0;
            this.lblWo.Text = "工單號碼 :";

            this.txtWorkOrder.Location = new System.Drawing.Point(95, 22);
            this.txtWorkOrder.Name = "txtWorkOrder";
            this.txtWorkOrder.Size = new System.Drawing.Size(160, 23);
            this.txtWorkOrder.TabIndex = 1;

            this.lblBatchNo.AutoSize = true;
            this.lblBatchNo.Location = new System.Drawing.Point(275, 25);
            this.lblBatchNo.Name = "lblBatchNo";
            this.lblBatchNo.Size = new System.Drawing.Size(62, 17);
            this.lblBatchNo.TabIndex = 2;
            this.lblBatchNo.Text = "BatchNo :";

            this.txtBatchNo.Location = new System.Drawing.Point(365, 22);
            this.txtBatchNo.Name = "txtBatchNo";
            this.txtBatchNo.Size = new System.Drawing.Size(160, 23);
            this.txtBatchNo.TabIndex = 3;

            this.lblPartNo.AutoSize = true;
            this.lblPartNo.Location = new System.Drawing.Point(15, 55);
            this.lblPartNo.Name = "lblPartNo";
            this.lblPartNo.Size = new System.Drawing.Size(41, 17);
            this.lblPartNo.TabIndex = 4;
            this.lblPartNo.Text = "料號 :";

            this.txtPartNo.Location = new System.Drawing.Point(95, 52);
            this.txtPartNo.Name = "txtPartNo";
            this.txtPartNo.Size = new System.Drawing.Size(160, 23);
            this.txtPartNo.TabIndex = 5;

            this.lblProcessNo.AutoSize = true;
            this.lblProcessNo.Location = new System.Drawing.Point(275, 55);
            this.lblProcessNo.Name = "lblProcessNo";
            this.lblProcessNo.Size = new System.Drawing.Size(65, 17);
            this.lblProcessNo.TabIndex = 6;
            this.lblProcessNo.Text = "製程編號 :";

            this.txtProcessNo.Location = new System.Drawing.Point(365, 52);
            this.txtProcessNo.Name = "txtProcessNo";
            this.txtProcessNo.Size = new System.Drawing.Size(160, 23);
            this.txtProcessNo.TabIndex = 7;

            this.lblProcessName.AutoSize = true;
            this.lblProcessName.Location = new System.Drawing.Point(15, 85);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(65, 17);
            this.lblProcessName.TabIndex = 8;
            this.lblProcessName.Text = "製程名稱 :";

            this.txtProcessName.Location = new System.Drawing.Point(95, 82);
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.Size = new System.Drawing.Size(430, 23);
            this.txtProcessName.TabIndex = 9;

            this.lblTotalQty.AutoSize = true;
            this.lblTotalQty.Location = new System.Drawing.Point(15, 115);
            this.lblTotalQty.Name = "lblTotalQty";
            this.lblTotalQty.Size = new System.Drawing.Size(65, 17);
            this.lblTotalQty.TabIndex = 10;
            this.lblTotalQty.Text = "工單數量 :";

            this.txtTotalQty.Location = new System.Drawing.Point(95, 112);
            this.txtTotalQty.Name = "txtTotalQty";
            this.txtTotalQty.Size = new System.Drawing.Size(430, 23);
            this.txtTotalQty.TabIndex = 11;

            this.lblRunQty.AutoSize = true;
            this.lblRunQty.Location = new System.Drawing.Point(15, 145);
            this.lblRunQty.Name = "lblRunQty";
            this.lblRunQty.Size = new System.Drawing.Size(89, 17);
            this.lblRunQty.TabIndex = 12;
            this.lblRunQty.Text = "工單跑貨片數 :";

            this.txtRunQty.Location = new System.Drawing.Point(105, 142);
            this.txtRunQty.Name = "txtRunQty";
            this.txtRunQty.Size = new System.Drawing.Size(150, 23);
            this.txtRunQty.TabIndex = 13;

            this.lblCompletedQty.AutoSize = true;
            this.lblCompletedQty.Location = new System.Drawing.Point(275, 145);
            this.lblCompletedQty.Name = "lblCompletedQty";
            this.lblCompletedQty.Size = new System.Drawing.Size(77, 17);
            this.lblCompletedQty.TabIndex = 14;
            this.lblCompletedQty.Text = "已完成片數 :";

            this.txtCompletedQty.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCompletedQty.Location = new System.Drawing.Point(365, 142);
            this.txtCompletedQty.Name = "txtCompletedQty";
            this.txtCompletedQty.ReadOnly = true;
            this.txtCompletedQty.Size = new System.Drawing.Size(160, 23);
            this.txtCompletedQty.TabIndex = 15;

            this.lblRecipeId.AutoSize = true;
            this.lblRecipeId.Location = new System.Drawing.Point(15, 175);
            this.lblRecipeId.Name = "lblRecipeId";
            this.lblRecipeId.Size = new System.Drawing.Size(64, 17);
            this.lblRecipeId.TabIndex = 16;
            this.lblRecipeId.Text = "RecipeID :";

            this.txtRecipeId.Location = new System.Drawing.Point(95, 172);
            this.txtRecipeId.Name = "txtRecipeId";
            this.txtRecipeId.Size = new System.Drawing.Size(430, 23);
            this.txtRecipeId.TabIndex = 17;

            this.lblIsTrackedIn.AutoSize = true;
            this.lblIsTrackedIn.Location = new System.Drawing.Point(15, 203);
            this.lblIsTrackedIn.Name = "lblIsTrackedIn";
            this.lblIsTrackedIn.Size = new System.Drawing.Size(77, 17);
            this.lblIsTrackedIn.TabIndex = 18;
            this.lblIsTrackedIn.Text = "是否已進站 :";

            this.txtIsTrackedIn.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtIsTrackedIn.Location = new System.Drawing.Point(95, 200);
            this.txtIsTrackedIn.Name = "txtIsTrackedIn";
            this.txtIsTrackedIn.ReadOnly = true;
            this.txtIsTrackedIn.Size = new System.Drawing.Size(160, 23);
            this.txtIsTrackedIn.TabIndex = 19;

            this.lblComponentNo.AutoSize = true;
            this.lblComponentNo.Location = new System.Drawing.Point(265, 203);
            this.lblComponentNo.Name = "lblComponentNo";
            this.lblComponentNo.Size = new System.Drawing.Size(97, 17);
            this.lblComponentNo.TabIndex = 20;
            this.lblComponentNo.Text = "ComponentNo :";

            this.txtComponentNo.Location = new System.Drawing.Point(365, 200);
            this.txtComponentNo.Name = "txtComponentNo";
            this.txtComponentNo.Size = new System.Drawing.Size(160, 23);
            this.txtComponentNo.TabIndex = 21;

            // --- grpNgInput ---
            this.grpNgInput.Controls.Add(this.chkNoNg);
            this.grpNgInput.Controls.Add(this.btnAddNg);
            this.grpNgInput.Controls.Add(this.btnRemoveNg);
            this.grpNgInput.Controls.Add(this.dgvNgList);
            this.grpNgInput.Location = new System.Drawing.Point(560, 10);
            this.grpNgInput.Name = "grpNgInput";
            this.grpNgInput.Size = new System.Drawing.Size(380, 230);
            this.grpNgInput.TabIndex = 1;
            this.grpNgInput.TabStop = false;
            this.grpNgInput.Text = "不良輸入";

            this.chkNoNg.AutoSize = true;
            this.chkNoNg.Checked = true;
            this.chkNoNg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoNg.Location = new System.Drawing.Point(15, 20);
            this.chkNoNg.Name = "chkNoNg";
            this.chkNoNg.Size = new System.Drawing.Size(87, 21);
            this.chkNoNg.TabIndex = 0;
            this.chkNoNg.Text = "本站無不良";
            this.chkNoNg.UseVisualStyleBackColor = true;

            this.btnAddNg.Location = new System.Drawing.Point(200, 16);
            this.btnAddNg.Name = "btnAddNg";
            this.btnAddNg.Size = new System.Drawing.Size(80, 25);
            this.btnAddNg.TabIndex = 1;
            this.btnAddNg.Text = "新增不良";
            this.btnAddNg.UseVisualStyleBackColor = true;

            this.btnRemoveNg.Location = new System.Drawing.Point(290, 16);
            this.btnRemoveNg.Name = "btnRemoveNg";
            this.btnRemoveNg.Size = new System.Drawing.Size(80, 25);
            this.btnRemoveNg.TabIndex = 2;
            this.btnRemoveNg.Text = "刪除不良";
            this.btnRemoveNg.UseVisualStyleBackColor = true;

            this.dgvNgList.AllowUserToAddRows = false;
            this.dgvNgList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNgList.BackgroundColor = System.Drawing.Color.White;
            this.dgvNgList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNgList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colNgCode,
                this.colNgChineseName,
                this.colNgQty});
            this.dgvNgList.Location = new System.Drawing.Point(15, 48);
            this.dgvNgList.Name = "dgvNgList";
            this.dgvNgList.RowTemplate.Height = 25;
            this.dgvNgList.Size = new System.Drawing.Size(355, 170);
            this.dgvNgList.TabIndex = 3;

            this.colNgCode.HeaderText = "NGCode";
            this.colNgCode.Name = "NGCode";
            this.colNgChineseName.HeaderText = "NG Chinese Name";
            this.colNgChineseName.Name = "NGChineseName";
            this.colNgQty.HeaderText = "Qty";
            this.colNgQty.Name = "Qty";

            // --- grpTrackedInList ---
            this.grpTrackedInList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpTrackedInList.Controls.Add(this.dgvTrackedIn);
            this.grpTrackedInList.Location = new System.Drawing.Point(10, 245);
            this.grpTrackedInList.Name = "grpTrackedInList";
            this.grpTrackedInList.Size = new System.Drawing.Size(260, 220);
            this.grpTrackedInList.TabIndex = 2;
            this.grpTrackedInList.TabStop = false;
            this.grpTrackedInList.Text = "已進站工單清單";

            this.dgvTrackedIn.AllowUserToAddRows = false;
            this.dgvTrackedIn.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTrackedIn.BackgroundColor = System.Drawing.Color.White;
            this.dgvTrackedIn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTrackedIn.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colWoNo,
                this.colOther1});
            this.dgvTrackedIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTrackedIn.Location = new System.Drawing.Point(3, 19);
            this.dgvTrackedIn.Name = "dgvTrackedIn";
            this.dgvTrackedIn.ReadOnly = true;
            this.dgvTrackedIn.RowTemplate.Height = 25;
            this.dgvTrackedIn.Size = new System.Drawing.Size(254, 198);
            this.dgvTrackedIn.TabIndex = 0;

            this.colWoNo.HeaderText = "工單號碼(WoNo)";
            this.colWoNo.Name = "WoNo";
            this.colOther1.HeaderText = "其他1";
            this.colOther1.Name = "Other1";

            // --- grpManageParams ---
            this.grpManageParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpManageParams.Controls.Add(this.dgvManageParams);
            this.grpManageParams.Location = new System.Drawing.Point(280, 245);
            this.grpManageParams.Name = "grpManageParams";
            this.grpManageParams.Size = new System.Drawing.Size(660, 220);
            this.grpManageParams.TabIndex = 3;
            this.grpManageParams.TabStop = false;
            this.grpManageParams.Text = "管理項目 參 數";

            this.dgvManageParams.AllowUserToAddRows = false;
            this.dgvManageParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvManageParams.BackgroundColor = System.Drawing.Color.White;
            this.dgvManageParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvManageParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colParamName,
                this.colParamValue,
                this.colMustInput,
                this.colInputType});
            this.dgvManageParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvManageParams.Location = new System.Drawing.Point(3, 19);
            this.dgvManageParams.Name = "dgvManageParams";
            this.dgvManageParams.RowTemplate.Height = 25;
            this.dgvManageParams.Size = new System.Drawing.Size(654, 198);
            this.dgvManageParams.TabIndex = 0;

            this.colParamName.HeaderText = "管理項目名稱(Manage item name)";
            this.colParamName.Name = "Name";
            this.colParamValue.HeaderText = "管理項目值(Manage item values)";
            this.colParamValue.Name = "Value";
            this.colMustInput.HeaderText = "需輸入?(Must Input?)";
            this.colMustInput.Name = "MustInput";
            this.colInputType.HeaderText = "輸入類型(Input type)";
            this.colInputType.Name = "InputType";

            // --- grpOrderActions ---
            this.grpOrderActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpOrderActions.Controls.Add(this.btnTrackIn);
            this.grpOrderActions.Controls.Add(this.btnTrackOut);
            this.grpOrderActions.Controls.Add(this.btnChangeUser);
            this.grpOrderActions.Controls.Add(this.btnClearOrder);
            this.grpOrderActions.Location = new System.Drawing.Point(955, 10);
            this.grpOrderActions.Name = "grpOrderActions";
            this.grpOrderActions.Size = new System.Drawing.Size(165, 455);
            this.grpOrderActions.TabIndex = 4;
            this.grpOrderActions.TabStop = false;
            this.grpOrderActions.Text = "工單作業";

            this.btnTrackIn.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnTrackIn.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnTrackIn.Location = new System.Drawing.Point(15, 30);
            this.btnTrackIn.Name = "btnTrackIn";
            this.btnTrackIn.Size = new System.Drawing.Size(135, 42);
            this.btnTrackIn.TabIndex = 0;
            this.btnTrackIn.Text = "工單進站";
            this.btnTrackIn.UseVisualStyleBackColor = false;

            this.btnTrackOut.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnTrackOut.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnTrackOut.Location = new System.Drawing.Point(15, 85);
            this.btnTrackOut.Name = "btnTrackOut";
            this.btnTrackOut.Size = new System.Drawing.Size(135, 42);
            this.btnTrackOut.TabIndex = 1;
            this.btnTrackOut.Text = "工單出站";
            this.btnTrackOut.UseVisualStyleBackColor = false;

            this.btnChangeUser.Location = new System.Drawing.Point(15, 160);
            this.btnChangeUser.Name = "btnChangeUser";
            this.btnChangeUser.Size = new System.Drawing.Size(135, 36);
            this.btnChangeUser.TabIndex = 2;
            this.btnChangeUser.Text = "更換人員";
            this.btnChangeUser.UseVisualStyleBackColor = true;

            this.btnClearOrder.Location = new System.Drawing.Point(15, 205);
            this.btnClearOrder.Name = "btnClearOrder";
            this.btnClearOrder.Size = new System.Drawing.Size(135, 36);
            this.btnClearOrder.TabIndex = 3;
            this.btnClearOrder.Text = "清除";
            this.btnClearOrder.UseVisualStyleBackColor = true;

            // ================= tabEquipment =================
            this.tabEquipment.Controls.Add(this.grpOpcStatus);
            this.tabEquipment.Controls.Add(this.grpAlarmList);
            this.tabEquipment.Location = new System.Drawing.Point(4, 26);
            this.tabEquipment.Name = "tabEquipment";
            this.tabEquipment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEquipment.Size = new System.Drawing.Size(1132, 470);
            this.tabEquipment.TabIndex = 1;
            this.tabEquipment.Text = "設備功能";
            this.tabEquipment.UseVisualStyleBackColor = true;

            // --- grpOpcStatus ---
            this.grpOpcStatus.Controls.Add(this.lblOpcConn);
            this.grpOpcStatus.Controls.Add(this.lblMachineState);
            this.grpOpcStatus.Controls.Add(this.lblProgress);
            this.grpOpcStatus.Controls.Add(this.lblActivePrg);
            this.grpOpcStatus.Controls.Add(this.pnlLampGreen);
            this.grpOpcStatus.Controls.Add(this.lblLampGreen);
            this.grpOpcStatus.Controls.Add(this.pnlLampYellow);
            this.grpOpcStatus.Controls.Add(this.lblLampYellow);
            this.grpOpcStatus.Controls.Add(this.pnlLampRed);
            this.grpOpcStatus.Controls.Add(this.lblLampRed);
            this.grpOpcStatus.Controls.Add(this.btnModeLocal);
            this.grpOpcStatus.Controls.Add(this.btnModeSemiAuto);
            this.grpOpcStatus.Controls.Add(this.btnModeAuto);
            this.grpOpcStatus.Controls.Add(this.btnStartSchedule);
            this.grpOpcStatus.Controls.Add(this.btnOpenSimulator);
            this.grpOpcStatus.Location = new System.Drawing.Point(15, 15);
            this.grpOpcStatus.Name = "grpOpcStatus";
            this.grpOpcStatus.Size = new System.Drawing.Size(1095, 180);
            this.grpOpcStatus.TabIndex = 0;
            this.grpOpcStatus.TabStop = false;
            this.grpOpcStatus.Text = "三菱雷射機狀態與控制 (OPC UA)";

            this.lblOpcConn.AutoSize = true;
            this.lblOpcConn.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.lblOpcConn.Location = new System.Drawing.Point(20, 30);
            this.lblOpcConn.Name = "lblOpcConn";
            this.lblOpcConn.Size = new System.Drawing.Size(105, 18);
            this.lblOpcConn.TabIndex = 0;
            this.lblOpcConn.Text = "OPC 連線: 離線中";

            this.lblMachineState.AutoSize = true;
            this.lblMachineState.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.lblMachineState.Location = new System.Drawing.Point(220, 30);
            this.lblMachineState.Name = "lblMachineState";
            this.lblMachineState.Size = new System.Drawing.Size(139, 18);
            this.lblMachineState.TabIndex = 1;
            this.lblMachineState.Text = "機台狀態代碼: [3] Idle";

            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.lblProgress.Location = new System.Drawing.Point(480, 30);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(95, 18);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "加工計數: 0 / 0";

            this.lblActivePrg.AutoSize = true;
            this.lblActivePrg.Location = new System.Drawing.Point(20, 65);
            this.lblActivePrg.Name = "lblActivePrg";
            this.lblActivePrg.Size = new System.Drawing.Size(104, 17);
            this.lblActivePrg.TabIndex = 3;
            this.lblActivePrg.Text = "目前加工程式: 無";

            this.pnlLampGreen.BackColor = System.Drawing.Color.DarkGreen;
            this.pnlLampGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLampGreen.Location = new System.Drawing.Point(20, 100);
            this.pnlLampGreen.Name = "pnlLampGreen";
            this.pnlLampGreen.Size = new System.Drawing.Size(24, 24);
            this.pnlLampGreen.TabIndex = 4;

            this.lblLampGreen.AutoSize = true;
            this.lblLampGreen.Location = new System.Drawing.Point(50, 104);
            this.lblLampGreen.Name = "lblLampGreen";
            this.lblLampGreen.Size = new System.Drawing.Size(117, 17);
            this.lblLampGreen.TabIndex = 5;
            this.lblLampGreen.Text = "綠燈 (伺服ON/運行)";

            this.pnlLampYellow.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.pnlLampYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLampYellow.Location = new System.Drawing.Point(200, 100);
            this.pnlLampYellow.Name = "pnlLampYellow";
            this.pnlLampYellow.Size = new System.Drawing.Size(24, 24);
            this.pnlLampYellow.TabIndex = 6;

            this.lblLampYellow.AutoSize = true;
            this.lblLampYellow.Location = new System.Drawing.Point(230, 104);
            this.lblLampYellow.Name = "lblLampYellow";
            this.lblLampYellow.Size = new System.Drawing.Size(126, 17);
            this.lblLampYellow.TabIndex = 7;
            this.lblLampYellow.Text = "黃燈 (待機/BeamON)";

            this.pnlLampRed.BackColor = System.Drawing.Color.DarkRed;
            this.pnlLampRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLampRed.Location = new System.Drawing.Point(380, 100);
            this.pnlLampRed.Name = "pnlLampRed";
            this.pnlLampRed.Size = new System.Drawing.Size(24, 24);
            this.pnlLampRed.TabIndex = 8;

            this.lblLampRed.AutoSize = true;
            this.lblLampRed.Location = new System.Drawing.Point(410, 104);
            this.lblLampRed.Name = "lblLampRed";
            this.lblLampRed.Size = new System.Drawing.Size(96, 17);
            this.lblLampRed.TabIndex = 9;
            this.lblLampRed.Text = "紅燈 (警報/異常)";

            this.btnModeLocal.Location = new System.Drawing.Point(600, 95);
            this.btnModeLocal.Name = "btnModeLocal";
            this.btnModeLocal.Size = new System.Drawing.Size(95, 32);
            this.btnModeLocal.TabIndex = 10;
            this.btnModeLocal.Text = "Local 模式";
            this.btnModeLocal.UseVisualStyleBackColor = true;

            this.btnModeSemiAuto.Location = new System.Drawing.Point(705, 95);
            this.btnModeSemiAuto.Name = "btnModeSemiAuto";
            this.btnModeSemiAuto.Size = new System.Drawing.Size(115, 32);
            this.btnModeSemiAuto.TabIndex = 11;
            this.btnModeSemiAuto.Text = "Semi-Auto 模式";
            this.btnModeSemiAuto.UseVisualStyleBackColor = true;

            this.btnModeAuto.Location = new System.Drawing.Point(830, 95);
            this.btnModeAuto.Name = "btnModeAuto";
            this.btnModeAuto.Size = new System.Drawing.Size(95, 32);
            this.btnModeAuto.TabIndex = 12;
            this.btnModeAuto.Text = "Auto 模式";
            this.btnModeAuto.UseVisualStyleBackColor = true;

            this.btnStartSchedule.BackColor = System.Drawing.Color.PaleGreen;
            this.btnStartSchedule.Location = new System.Drawing.Point(940, 95);
            this.btnStartSchedule.Name = "btnStartSchedule";
            this.btnStartSchedule.Size = new System.Drawing.Size(125, 32);
            this.btnStartSchedule.TabIndex = 13;
            this.btnStartSchedule.Text = "啟動連續運轉";
            this.btnStartSchedule.UseVisualStyleBackColor = false;

            this.btnOpenSimulator.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnOpenSimulator.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpenSimulator.Location = new System.Drawing.Point(915, 22);
            this.btnOpenSimulator.Name = "btnOpenSimulator";
            this.btnOpenSimulator.Size = new System.Drawing.Size(155, 34);
            this.btnOpenSimulator.TabIndex = 14;
            this.btnOpenSimulator.Text = "開啟機台模擬器 (OPC)";
            this.btnOpenSimulator.UseVisualStyleBackColor = false;

            // --- grpAlarmList ---
            this.grpAlarmList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAlarmList.Controls.Add(this.dgvAlarms);
            this.grpAlarmList.Location = new System.Drawing.Point(15, 205);
            this.grpAlarmList.Name = "grpAlarmList";
            this.grpAlarmList.Size = new System.Drawing.Size(1095, 260);
            this.grpAlarmList.TabIndex = 1;
            this.grpAlarmList.TabStop = false;
            this.grpAlarmList.Text = "當前活躍警報清單 (Active Alarms 000~009)";

            this.dgvAlarms.AllowUserToAddRows = false;
            this.dgvAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlarms.BackgroundColor = System.Drawing.Color.White;
            this.dgvAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlarms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colAlarmCode,
                this.colAlarmMsg,
                this.colAlarmType,
                this.colAlarmTime});
            this.dgvAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlarms.Location = new System.Drawing.Point(3, 19);
            this.dgvAlarms.Name = "dgvAlarms";
            this.dgvAlarms.ReadOnly = true;
            this.dgvAlarms.RowTemplate.Height = 25;
            this.dgvAlarms.Size = new System.Drawing.Size(1089, 238);
            this.dgvAlarms.TabIndex = 0;

            this.colAlarmCode.HeaderText = "警報代碼";
            this.colAlarmCode.Name = "AlarmCode";
            this.colAlarmMsg.HeaderText = "警報訊息內容";
            this.colAlarmMsg.Name = "AlarmMsg";
            this.colAlarmType.HeaderText = "等級";
            this.colAlarmType.Name = "AlarmType";
            this.colAlarmTime.HeaderText = "偵測時間";
            this.colAlarmTime.Name = "Time";

            // ================= tabItTest =================
            this.tabItTest.Controls.Add(this.grpTestActions);
            this.tabItTest.Controls.Add(this.grpLog);
            this.tabItTest.Location = new System.Drawing.Point(4, 26);
            this.tabItTest.Name = "tabItTest";
            this.tabItTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabItTest.Size = new System.Drawing.Size(1132, 470);
            this.tabItTest.TabIndex = 2;
            this.tabItTest.Text = "IT 測試";
            this.tabItTest.UseVisualStyleBackColor = true;

            // --- grpTestActions ---
            this.grpTestActions.Controls.Add(this.btnTestAlive);
            this.grpTestActions.Controls.Add(this.btnTestProcessData);
            this.grpTestActions.Controls.Add(this.btnTestAlarmStart);
            this.grpTestActions.Controls.Add(this.btnTestAlarmEnd);
            this.grpTestActions.Controls.Add(this.btnClearLogs);
            this.grpTestActions.Location = new System.Drawing.Point(15, 15);
            this.grpTestActions.Name = "grpTestActions";
            this.grpTestActions.Size = new System.Drawing.Size(1095, 80);
            this.grpTestActions.TabIndex = 0;
            this.grpTestActions.TabStop = false;
            this.grpTestActions.Text = "MQTT 通訊測試與模擬工具";

            this.btnTestAlive.Location = new System.Drawing.Point(15, 28);
            this.btnTestAlive.Name = "btnTestAlive";
            this.btnTestAlive.Size = new System.Drawing.Size(190, 35);
            this.btnTestAlive.TabIndex = 0;
            this.btnTestAlive.Text = "測試存活檢測 (AreYouThere)";
            this.btnTestAlive.UseVisualStyleBackColor = true;

            this.btnTestProcessData.Location = new System.Drawing.Point(220, 28);
            this.btnTestProcessData.Name = "btnTestProcessData";
            this.btnTestProcessData.Size = new System.Drawing.Size(160, 35);
            this.btnTestProcessData.TabIndex = 1;
            this.btnTestProcessData.Text = "模擬製程資料上報";
            this.btnTestProcessData.UseVisualStyleBackColor = true;

            this.btnTestAlarmStart.Location = new System.Drawing.Point(395, 28);
            this.btnTestAlarmStart.Name = "btnTestAlarmStart";
            this.btnTestAlarmStart.Size = new System.Drawing.Size(160, 35);
            this.btnTestAlarmStart.TabIndex = 2;
            this.btnTestAlarmStart.Text = "模擬警報發生 (Start)";
            this.btnTestAlarmStart.UseVisualStyleBackColor = true;

            this.btnTestAlarmEnd.Location = new System.Drawing.Point(570, 28);
            this.btnTestAlarmEnd.Name = "btnTestAlarmEnd";
            this.btnTestAlarmEnd.Size = new System.Drawing.Size(160, 35);
            this.btnTestAlarmEnd.TabIndex = 3;
            this.btnTestAlarmEnd.Text = "模擬警報解除 (End)";
            this.btnTestAlarmEnd.UseVisualStyleBackColor = true;

            this.btnClearLogs.Location = new System.Drawing.Point(745, 28);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(120, 35);
            this.btnClearLogs.TabIndex = 4;
            this.btnClearLogs.Text = "清空通訊日誌";
            this.btnClearLogs.UseVisualStyleBackColor = true;

            // --- grpLog ---
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.dgvMqttLogs);
            this.grpLog.Location = new System.Drawing.Point(15, 105);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(1095, 360);
            this.grpLog.TabIndex = 1;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "MQTT 即時封包監聽紀錄";

            this.dgvMqttLogs.AllowUserToAddRows = false;
            this.dgvMqttLogs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMqttLogs.BackgroundColor = System.Drawing.Color.White;
            this.dgvMqttLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMqttLogs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colLogTime,
                this.colLogDir,
                this.colLogTopic,
                this.colLogPayload});
            this.dgvMqttLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMqttLogs.Location = new System.Drawing.Point(3, 19);
            this.dgvMqttLogs.Name = "dgvMqttLogs";
            this.dgvMqttLogs.ReadOnly = true;
            this.dgvMqttLogs.RowTemplate.Height = 25;
            this.dgvMqttLogs.Size = new System.Drawing.Size(1089, 338);
            this.dgvMqttLogs.TabIndex = 0;

            this.colLogTime.HeaderText = "時間";
            this.colLogTime.Name = "Time";
            this.colLogDir.HeaderText = "方向";
            this.colLogDir.Name = "Direction";
            this.colLogTopic.HeaderText = "主題 (Topic)";
            this.colLogTopic.Name = "Topic";
            this.colLogPayload.HeaderText = "訊息內文 (JSON Payload)";
            this.colLogPayload.Name = "Payload";

            // ================= grpResult =================
            this.grpResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResult.Controls.Add(this.lblResult);
            this.grpResult.Controls.Add(this.txtResult);
            this.grpResult.Controls.Add(this.lblResultCode);
            this.grpResult.Controls.Add(this.txtResultCode);
            this.grpResult.Controls.Add(this.lblResultMessage);
            this.grpResult.Controls.Add(this.txtResultMessage);
            this.grpResult.Location = new System.Drawing.Point(12, 620);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(1140, 130);
            this.grpResult.TabIndex = 9;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "執行結果";

            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(15, 25);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(51, 17);
            this.lblResult.TabIndex = 0;
            this.lblResult.Text = "Result :";

            this.txtResult.Font = new System.Drawing.Font("微軟正黑體", 9.5F, System.Drawing.FontStyle.Bold);
            this.txtResult.Location = new System.Drawing.Point(75, 22);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(120, 24);
            this.txtResult.TabIndex = 1;

            this.lblResultCode.AutoSize = true;
            this.lblResultCode.Location = new System.Drawing.Point(15, 58);
            this.lblResultCode.Name = "lblResultCode";
            this.lblResultCode.Size = new System.Drawing.Size(78, 17);
            this.lblResultCode.TabIndex = 2;
            this.lblResultCode.Text = "ResultCode :";

            this.txtResultCode.Location = new System.Drawing.Point(95, 55);
            this.txtResultCode.Name = "txtResultCode";
            this.txtResultCode.ReadOnly = true;
            this.txtResultCode.Size = new System.Drawing.Size(100, 23);
            this.txtResultCode.TabIndex = 3;

            this.lblResultMessage.AutoSize = true;
            this.lblResultMessage.Location = new System.Drawing.Point(220, 25);
            this.lblResultMessage.Name = "lblResultMessage";
            this.lblResultMessage.Size = new System.Drawing.Size(99, 17);
            this.lblResultMessage.TabIndex = 4;
            this.lblResultMessage.Text = "ResultMessage :";

            this.txtResultMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResultMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtResultMessage.Location = new System.Drawing.Point(325, 22);
            this.txtResultMessage.Multiline = true;
            this.txtResultMessage.Name = "txtResultMessage";
            this.txtResultMessage.ReadOnly = true;
            this.txtResultMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResultMessage.Size = new System.Drawing.Size(800, 95);
            this.txtResultMessage.TabIndex = 5;

            // ================= 根表單配置 =================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 761);
            this.Controls.Add(this.grpCurrentOperator);
            this.Controls.Add(this.grpAuth);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnConnectMes);
            this.Controls.Add(this.btnToggleLang);
            this.Controls.Add(this.btnDisconnectMes);
            this.Controls.Add(this.pnlMqttLed);
            this.Controls.Add(this.lblMqttLed);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.grpResult);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular);
            this.MinimumSize = new System.Drawing.Size(1100, 750);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MES - 2026/09/16";

            // EndInit DataGridViews
            ((System.ComponentModel.ISupportInitialize)(this.dgvNgList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrackedIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvManageParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMqttLogs)).EndInit();

            this.grpCurrentOperator.ResumeLayout(false);
            this.grpCurrentOperator.PerformLayout();
            this.grpAuth.ResumeLayout(false);
            this.grpAuth.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabWorkOrder.ResumeLayout(false);
            this.tabEquipment.ResumeLayout(false);
            this.tabItTest.ResumeLayout(false);
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            this.grpNgInput.ResumeLayout(false);
            this.grpNgInput.PerformLayout();
            this.grpTrackedInList.ResumeLayout(false);
            this.grpManageParams.ResumeLayout(false);
            this.grpOrderActions.ResumeLayout(false);
            this.grpOpcStatus.ResumeLayout(false);
            this.grpOpcStatus.PerformLayout();
            this.grpAlarmList.ResumeLayout(false);
            this.grpTestActions.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // 宣告成員欄位
        private System.Windows.Forms.GroupBox grpCurrentOperator;
        private System.Windows.Forms.Label lblOpId;
        private System.Windows.Forms.TextBox txtCurrentOpId;
        private System.Windows.Forms.Label lblOpName;
        private System.Windows.Forms.TextBox txtCurrentOpName;

        private System.Windows.Forms.GroupBox grpAuth;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Button btnUserAuth;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.TextBox txtLoginUserId;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtLoginPassword;
        private System.Windows.Forms.Button btnClearAuth;

        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnConnectMes;
        private System.Windows.Forms.Button btnToggleLang;
        private System.Windows.Forms.Button btnDisconnectMes;
        private System.Windows.Forms.Panel pnlMqttLed;
        private System.Windows.Forms.Label lblMqttLed;

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabWorkOrder;
        private System.Windows.Forms.TabPage tabEquipment;
        private System.Windows.Forms.TabPage tabItTest;

        private System.Windows.Forms.GroupBox grpOrderInfo;
        private System.Windows.Forms.Label lblWo;
        private System.Windows.Forms.TextBox txtWorkOrder;
        private System.Windows.Forms.Label lblBatchNo;
        private System.Windows.Forms.TextBox txtBatchNo;
        private System.Windows.Forms.Label lblPartNo;
        private System.Windows.Forms.TextBox txtPartNo;
        private System.Windows.Forms.Label lblProcessNo;
        private System.Windows.Forms.TextBox txtProcessNo;
        private System.Windows.Forms.Label lblProcessName;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.Label lblTotalQty;
        private System.Windows.Forms.TextBox txtTotalQty;
        private System.Windows.Forms.Label lblRunQty;
        private System.Windows.Forms.TextBox txtRunQty;
        private System.Windows.Forms.Label lblCompletedQty;
        private System.Windows.Forms.TextBox txtCompletedQty;
        private System.Windows.Forms.Label lblRecipeId;
        private System.Windows.Forms.TextBox txtRecipeId;
        private System.Windows.Forms.Label lblIsTrackedIn;
        private System.Windows.Forms.TextBox txtIsTrackedIn;
        private System.Windows.Forms.Label lblComponentNo;
        private System.Windows.Forms.TextBox txtComponentNo;

        private System.Windows.Forms.GroupBox grpNgInput;
        private System.Windows.Forms.CheckBox chkNoNg;
        private System.Windows.Forms.Button btnAddNg;
        private System.Windows.Forms.Button btnRemoveNg;
        private System.Windows.Forms.DataGridView dgvNgList;

        private System.Windows.Forms.GroupBox grpTrackedInList;
        private System.Windows.Forms.DataGridView dgvTrackedIn;

        private System.Windows.Forms.GroupBox grpManageParams;
        private System.Windows.Forms.DataGridView dgvManageParams;

        private System.Windows.Forms.GroupBox grpOrderActions;
        private System.Windows.Forms.Button btnTrackIn;
        private System.Windows.Forms.Button btnTrackOut;
        private System.Windows.Forms.Button btnChangeUser;
        private System.Windows.Forms.Button btnClearOrder;

        private System.Windows.Forms.GroupBox grpOpcStatus;
        private System.Windows.Forms.Label lblOpcConn;
        private System.Windows.Forms.Label lblMachineState;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblActivePrg;
        private System.Windows.Forms.Panel pnlLampGreen;
        private System.Windows.Forms.Label lblLampGreen;
        private System.Windows.Forms.Panel pnlLampYellow;
        private System.Windows.Forms.Label lblLampYellow;
        private System.Windows.Forms.Panel pnlLampRed;
        private System.Windows.Forms.Label lblLampRed;
        private System.Windows.Forms.Button btnModeLocal;
        private System.Windows.Forms.Button btnModeSemiAuto;
        private System.Windows.Forms.Button btnModeAuto;
        private System.Windows.Forms.Button btnStartSchedule;
        private System.Windows.Forms.Button btnOpenSimulator;

        private System.Windows.Forms.GroupBox grpAlarmList;
        private System.Windows.Forms.DataGridView dgvAlarms;

        private System.Windows.Forms.GroupBox grpTestActions;
        private System.Windows.Forms.Button btnTestAlive;
        private System.Windows.Forms.Button btnTestProcessData;
        private System.Windows.Forms.Button btnTestAlarmStart;
        private System.Windows.Forms.Button btnTestAlarmEnd;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.DataGridView dgvMqttLogs;

        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblResultCode;
        private System.Windows.Forms.TextBox txtResultCode;
        private System.Windows.Forms.Label lblResultMessage;
        private System.Windows.Forms.TextBox txtResultMessage;

        private System.Windows.Forms.DataGridViewTextBoxColumn colNgCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgChineseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgQty;

        private System.Windows.Forms.DataGridViewTextBoxColumn colWoNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOther1;

        private System.Windows.Forms.DataGridViewTextBoxColumn colParamName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colParamValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMustInput;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInputType;

        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmTime;

        private System.Windows.Forms.DataGridViewTextBoxColumn colLogTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogDir;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogTopic;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogPayload;
    }
}
