namespace MitsubishiLaser.Simulator.Forms
{
    partial class SimulatorMainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // 頂部儀表看板
            this.grpDashboard = new System.Windows.Forms.GroupBox();
            this.pnlGreen = new System.Windows.Forms.Panel();
            this.lblGreen = new System.Windows.Forms.Label();
            this.pnlYellow = new System.Windows.Forms.Panel();
            this.lblYellow = new System.Windows.Forms.Label();
            this.pnlRed = new System.Windows.Forms.Panel();
            this.lblRed = new System.Windows.Forms.Label();
            this.lblStatusBadge = new System.Windows.Forms.Label();
            this.lblModeBadge = new System.Windows.Forms.Label();
            this.lblWatchDogVal = new System.Windows.Forms.Label();
            this.pbWatchDog = new System.Windows.Forms.ProgressBar();
            this.lblWatchDogTitle = new System.Windows.Forms.Label();
            this.chkWatchDogEnable = new System.Windows.Forms.CheckBox();
            this.btnFeedDog = new System.Windows.Forms.Button();

            // 配方交握區
            this.grpRecipe = new System.Windows.Forms.GroupBox();
            this.lblLotInput = new System.Windows.Forms.Label();
            this.txtLotInput = new System.Windows.Forms.TextBox();
            this.btnInputLot = new System.Windows.Forms.Button();
            this.lblGetRecipeReq = new System.Windows.Forms.Label();
            this.lblReqPrg = new System.Windows.Forms.Label();
            this.txtReqPrg = new System.Windows.Forms.TextBox();
            this.lblReqCnd = new System.Windows.Forms.Label();
            this.txtReqCnd = new System.Windows.Forms.TextBox();
            this.lblReqSheet = new System.Windows.Forms.Label();
            this.txtReqSheet = new System.Windows.Forms.TextBox();
            this.lblGetRecipeAck = new System.Windows.Forms.Label();
            this.txtGetRecipeAck = new System.Windows.Forms.TextBox();

            // 連續運轉與加工進度
            this.grpSchedule = new System.Windows.Forms.GroupBox();
            this.lblActivePrg = new System.Windows.Forms.Label();
            this.lblActiveLot = new System.Windows.Forms.Label();
            this.pbProcessing = new System.Windows.Forms.ProgressBar();
            this.lblCounts = new System.Windows.Forms.Label();
            this.lblLaserParams = new System.Windows.Forms.Label();
            this.btnStartRun = new System.Windows.Forms.Button();
            this.btnStopRun = new System.Windows.Forms.Button();
            this.btnSimFailTarget = new System.Windows.Forms.Button();

            // 模式控制
            this.grpMode = new System.Windows.Forms.GroupBox();
            this.btnSetOffline = new System.Windows.Forms.Button();
            this.btnSetLocal = new System.Windows.Forms.Button();
            this.btnSetSemiAuto = new System.Windows.Forms.Button();
            this.btnSetAuto = new System.Windows.Forms.Button();

            // 警報控制與列表
            this.grpAlarm = new System.Windows.Forms.GroupBox();
            this.cboPresetAlarms = new System.Windows.Forms.ComboBox();
            this.lblPreset = new System.Windows.Forms.Label();
            this.txtCustomAlarmNo = new System.Windows.Forms.TextBox();
            this.txtCustomAlarmMsg = new System.Windows.Forms.TextBox();
            this.btnTriggerAlarm = new System.Windows.Forms.Button();
            this.btnAlReset = new System.Windows.Forms.Button();
            this.dgvAlarms = new System.Windows.Forms.DataGridView();
            this.colSlot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTriggerTime = new System.Windows.Forms.DataGridViewTextBoxColumn();

            // 日誌
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnClearLog = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarms)).BeginInit();
            this.grpDashboard.SuspendLayout();
            this.grpRecipe.SuspendLayout();
            this.grpSchedule.SuspendLayout();
            this.grpMode.SuspendLayout();
            this.grpAlarm.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();

            // ================= grpDashboard =================
            this.grpDashboard.Controls.Add(this.pnlGreen);
            this.grpDashboard.Controls.Add(this.lblGreen);
            this.grpDashboard.Controls.Add(this.pnlYellow);
            this.grpDashboard.Controls.Add(this.lblYellow);
            this.grpDashboard.Controls.Add(this.pnlRed);
            this.grpDashboard.Controls.Add(this.lblRed);
            this.grpDashboard.Controls.Add(this.lblStatusBadge);
            this.grpDashboard.Controls.Add(this.lblModeBadge);
            this.grpDashboard.Controls.Add(this.lblWatchDogTitle);
            this.grpDashboard.Controls.Add(this.chkWatchDogEnable);
            this.grpDashboard.Controls.Add(this.btnFeedDog);
            this.grpDashboard.Controls.Add(this.pbWatchDog);
            this.grpDashboard.Controls.Add(this.lblWatchDogVal);
            this.grpDashboard.Location = new System.Drawing.Point(12, 10);
            this.grpDashboard.Name = "grpDashboard";
            this.grpDashboard.Size = new System.Drawing.Size(1140, 95);
            this.grpDashboard.TabIndex = 0;
            this.grpDashboard.TabStop = false;
            this.grpDashboard.Text = "機台即時狀態看板 (Dashboard)";

            // 綠燈
            this.pnlGreen.BackColor = System.Drawing.Color.DarkGreen;
            this.pnlGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGreen.Location = new System.Drawing.Point(20, 30);
            this.pnlGreen.Name = "pnlGreen";
            this.pnlGreen.Size = new System.Drawing.Size(26, 26);
            this.pnlGreen.TabIndex = 0;

            this.lblGreen.AutoSize = true;
            this.lblGreen.Location = new System.Drawing.Point(52, 35);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(56, 17);
            this.lblGreen.TabIndex = 1;
            this.lblGreen.Text = "綠燈 (Run)";

            // 黃燈
            this.pnlYellow.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.pnlYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlYellow.Location = new System.Drawing.Point(135, 30);
            this.pnlYellow.Name = "pnlYellow";
            this.pnlYellow.Size = new System.Drawing.Size(26, 26);
            this.pnlYellow.TabIndex = 2;

            this.lblYellow.AutoSize = true;
            this.lblYellow.Location = new System.Drawing.Point(167, 35);
            this.lblYellow.Name = "lblYellow";
            this.lblYellow.Size = new System.Drawing.Size(68, 17);
            this.lblYellow.TabIndex = 3;
            this.lblYellow.Text = "黃燈 (Beam)";

            // 紅燈
            this.pnlRed.BackColor = System.Drawing.Color.DarkRed;
            this.pnlRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRed.Location = new System.Drawing.Point(255, 30);
            this.pnlRed.Name = "pnlRed";
            this.pnlRed.Size = new System.Drawing.Size(26, 26);
            this.pnlRed.TabIndex = 4;

            this.lblRed.AutoSize = true;
            this.lblRed.Location = new System.Drawing.Point(287, 35);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(69, 17);
            this.lblRed.TabIndex = 5;
            this.lblRed.Text = "紅燈 (Alarm)";

            // 機台狀態 Badge
            this.lblStatusBadge.BackColor = System.Drawing.Color.Gold;
            this.lblStatusBadge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatusBadge.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatusBadge.Location = new System.Drawing.Point(380, 24);
            this.lblStatusBadge.Name = "lblStatusBadge";
            this.lblStatusBadge.Size = new System.Drawing.Size(180, 42);
            this.lblStatusBadge.TabIndex = 6;
            this.lblStatusBadge.Text = "[3] Idle (待機)";
            this.lblStatusBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 模式 Badge
            this.lblModeBadge.BackColor = System.Drawing.Color.LightSkyBlue;
            this.lblModeBadge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblModeBadge.Font = new System.Drawing.Font("微軟正黑體", 11F, System.Drawing.FontStyle.Bold);
            this.lblModeBadge.Location = new System.Drawing.Point(575, 24);
            this.lblModeBadge.Name = "lblModeBadge";
            this.lblModeBadge.Size = new System.Drawing.Size(160, 42);
            this.lblModeBadge.TabIndex = 7;
            this.lblModeBadge.Text = "Mode: Online-Auto";
            this.lblModeBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // WatchDog
            this.lblWatchDogTitle.AutoSize = true;
            this.lblWatchDogTitle.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            this.lblWatchDogTitle.Location = new System.Drawing.Point(680, 22);
            this.lblWatchDogTitle.Name = "lblWatchDogTitle";
            this.lblWatchDogTitle.Size = new System.Drawing.Size(145, 16);
            this.lblWatchDogTitle.TabIndex = 8;
            this.lblWatchDogTitle.Text = "Host WatchDog 心跳監控:";

            this.chkWatchDogEnable.AutoSize = true;
            this.chkWatchDogEnable.Checked = true;
            this.chkWatchDogEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWatchDogEnable.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.chkWatchDogEnable.Location = new System.Drawing.Point(830, 20);
            this.chkWatchDogEnable.Name = "chkWatchDogEnable";
            this.chkWatchDogEnable.Size = new System.Drawing.Size(75, 20);
            this.chkWatchDogEnable.TabIndex = 9;
            this.chkWatchDogEnable.Text = "啟用監控";
            this.chkWatchDogEnable.UseVisualStyleBackColor = true;

            this.btnFeedDog.Font = new System.Drawing.Font("微軟正黑體", 8.5F);
            this.btnFeedDog.Location = new System.Drawing.Point(910, 17);
            this.btnFeedDog.Name = "btnFeedDog";
            this.btnFeedDog.Size = new System.Drawing.Size(95, 25);
            this.btnFeedDog.TabIndex = 10;
            this.btnFeedDog.Text = "手動餵狗 (+1)";
            this.btnFeedDog.UseVisualStyleBackColor = true;

            this.pbWatchDog.Location = new System.Drawing.Point(683, 45);
            this.pbWatchDog.Maximum = 10;
            this.pbWatchDog.Name = "pbWatchDog";
            this.pbWatchDog.Size = new System.Drawing.Size(220, 20);
            this.pbWatchDog.TabIndex = 11;
            this.pbWatchDog.Value = 10;

            this.lblWatchDogVal.AutoSize = true;
            this.lblWatchDogVal.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblWatchDogVal.Location = new System.Drawing.Point(910, 47);
            this.lblWatchDogVal.Name = "lblWatchDogVal";
            this.lblWatchDogVal.Size = new System.Drawing.Size(220, 16);
            this.lblWatchDogVal.TabIndex = 12;
            this.lblWatchDogVal.Text = "10s [待命中 (等待上位機連線...)]";

            // ================= grpRecipe =================
            this.grpRecipe.Controls.Add(this.lblLotInput);
            this.grpRecipe.Controls.Add(this.txtLotInput);
            this.grpRecipe.Controls.Add(this.btnInputLot);
            this.grpRecipe.Controls.Add(this.lblGetRecipeReq);
            this.grpRecipe.Controls.Add(this.lblReqPrg);
            this.grpRecipe.Controls.Add(this.txtReqPrg);
            this.grpRecipe.Controls.Add(this.lblReqCnd);
            this.grpRecipe.Controls.Add(this.txtReqCnd);
            this.grpRecipe.Controls.Add(this.lblReqSheet);
            this.grpRecipe.Controls.Add(this.txtReqSheet);
            this.grpRecipe.Controls.Add(this.lblGetRecipeAck);
            this.grpRecipe.Controls.Add(this.txtGetRecipeAck);
            this.grpRecipe.Location = new System.Drawing.Point(12, 115);
            this.grpRecipe.Name = "grpRecipe";
            this.grpRecipe.Size = new System.Drawing.Size(560, 230);
            this.grpRecipe.TabIndex = 1;
            this.grpRecipe.TabStop = false;
            this.grpRecipe.Text = "操作員條碼輸入與配方交握 (Recipe Handshake SPEC 4-4)";

            this.lblLotInput.AutoSize = true;
            this.lblLotInput.Location = new System.Drawing.Point(15, 30);
            this.lblLotInput.Name = "lblLotInput";
            this.lblLotInput.Size = new System.Drawing.Size(71, 17);
            this.lblLotInput.TabIndex = 0;
            this.lblLotInput.Text = "批號 (LotID):";

            this.txtLotInput.Location = new System.Drawing.Point(95, 27);
            this.txtLotInput.Name = "txtLotInput";
            this.txtLotInput.Size = new System.Drawing.Size(200, 23);
            this.txtLotInput.TabIndex = 1;
            this.txtLotInput.Text = "LOT20260916-01";

            this.btnInputLot.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnInputLot.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            this.btnInputLot.Location = new System.Drawing.Point(310, 24);
            this.btnInputLot.Name = "btnInputLot";
            this.btnInputLot.Size = new System.Drawing.Size(230, 30);
            this.btnInputLot.TabIndex = 2;
            this.btnInputLot.Text = "模擬操作員刷入 (發出 GetRecipe.Req)";
            this.btnInputLot.UseVisualStyleBackColor = false;

            this.lblGetRecipeReq.AutoSize = true;
            this.lblGetRecipeReq.Font = new System.Drawing.Font("微軟正黑體", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblGetRecipeReq.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblGetRecipeReq.Location = new System.Drawing.Point(15, 68);
            this.lblGetRecipeReq.Name = "lblGetRecipeReq";
            this.lblGetRecipeReq.Size = new System.Drawing.Size(211, 17);
            this.lblGetRecipeReq.TabIndex = 3;
            this.lblGetRecipeReq.Text = "GetRecipe.Req: False (等待刷卡中)";

            this.lblReqPrg.AutoSize = true;
            this.lblReqPrg.Location = new System.Drawing.Point(15, 105);
            this.lblReqPrg.Name = "lblReqPrg";
            this.lblReqPrg.Size = new System.Drawing.Size(89, 17);
            this.lblReqPrg.TabIndex = 4;
            this.lblReqPrg.Text = "ProgramFile :";

            this.txtReqPrg.Location = new System.Drawing.Point(115, 102);
            this.txtReqPrg.Name = "txtReqPrg";
            this.txtReqPrg.ReadOnly = true;
            this.txtReqPrg.Size = new System.Drawing.Size(425, 23);
            this.txtReqPrg.TabIndex = 5;

            this.lblReqCnd.AutoSize = true;
            this.lblReqCnd.Location = new System.Drawing.Point(15, 140);
            this.lblReqCnd.Name = "lblReqCnd";
            this.lblReqCnd.Size = new System.Drawing.Size(91, 17);
            this.lblReqCnd.TabIndex = 6;
            this.lblReqCnd.Text = "ConditionFile :";

            this.txtReqCnd.Location = new System.Drawing.Point(115, 137);
            this.txtReqCnd.Name = "txtReqCnd";
            this.txtReqCnd.ReadOnly = true;
            this.txtReqCnd.Size = new System.Drawing.Size(425, 23);
            this.txtReqCnd.TabIndex = 7;

            this.lblReqSheet.AutoSize = true;
            this.lblReqSheet.Location = new System.Drawing.Point(15, 175);
            this.lblReqSheet.Name = "lblReqSheet";
            this.lblReqSheet.Size = new System.Drawing.Size(71, 17);
            this.lblReqSheet.TabIndex = 8;
            this.lblReqSheet.Text = "SheetNum :";

            this.txtReqSheet.Location = new System.Drawing.Point(115, 172);
            this.txtReqSheet.Name = "txtReqSheet";
            this.txtReqSheet.ReadOnly = true;
            this.txtReqSheet.Size = new System.Drawing.Size(120, 23);
            this.txtReqSheet.TabIndex = 9;

            this.lblGetRecipeAck.AutoSize = true;
            this.lblGetRecipeAck.Location = new System.Drawing.Point(280, 175);
            this.lblGetRecipeAck.Name = "lblGetRecipeAck";
            this.lblGetRecipeAck.Size = new System.Drawing.Size(97, 17);
            this.lblGetRecipeAck.TabIndex = 10;
            this.lblGetRecipeAck.Text = "GetRecipe.Ack :";

            this.txtGetRecipeAck.Location = new System.Drawing.Point(385, 172);
            this.txtGetRecipeAck.Name = "txtGetRecipeAck";
            this.txtGetRecipeAck.ReadOnly = true;
            this.txtGetRecipeAck.Size = new System.Drawing.Size(155, 23);
            this.txtGetRecipeAck.TabIndex = 11;

            // ================= grpSchedule =================
            this.grpSchedule.Controls.Add(this.lblActivePrg);
            this.grpSchedule.Controls.Add(this.lblActiveLot);
            this.grpSchedule.Controls.Add(this.pbProcessing);
            this.grpSchedule.Controls.Add(this.lblCounts);
            this.grpSchedule.Controls.Add(this.lblLaserParams);
            this.grpSchedule.Controls.Add(this.btnStartRun);
            this.grpSchedule.Controls.Add(this.btnStopRun);
            this.grpSchedule.Controls.Add(this.btnSimFailTarget);
            this.grpSchedule.Location = new System.Drawing.Point(590, 115);
            this.grpSchedule.Name = "grpSchedule";
            this.grpSchedule.Size = new System.Drawing.Size(562, 230);
            this.grpSchedule.TabIndex = 2;
            this.grpSchedule.TabStop = false;
            this.grpSchedule.Text = "連續運轉與加工監控 (Schedule & Progress SPEC 4-5)";

            this.lblActivePrg.AutoSize = true;
            this.lblActivePrg.Font = new System.Drawing.Font("微軟正黑體", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblActivePrg.Location = new System.Drawing.Point(15, 28);
            this.lblActivePrg.Name = "lblActivePrg";
            this.lblActivePrg.Size = new System.Drawing.Size(161, 17);
            this.lblActivePrg.TabIndex = 0;
            this.lblActivePrg.Text = "當前加工程式: (尚未載入)";

            this.lblActiveLot.AutoSize = true;
            this.lblActiveLot.Location = new System.Drawing.Point(15, 55);
            this.lblActiveLot.Name = "lblActiveLot";
            this.lblActiveLot.Size = new System.Drawing.Size(95, 17);
            this.lblActiveLot.TabIndex = 1;
            this.lblActiveLot.Text = "作用批號: (None)";

            this.pbProcessing.Location = new System.Drawing.Point(18, 85);
            this.pbProcessing.Name = "pbProcessing";
            this.pbProcessing.Size = new System.Drawing.Size(525, 22);
            this.pbProcessing.TabIndex = 2;

            this.lblCounts.AutoSize = true;
            this.lblCounts.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.lblCounts.Location = new System.Drawing.Point(15, 120);
            this.lblCounts.Name = "lblCounts";
            this.lblCounts.Size = new System.Drawing.Size(325, 18);
            this.lblCounts.TabIndex = 3;
            this.lblCounts.Text = "加工進度: Processed=0 / Scheduled=0 (失敗=0)";

            this.lblLaserParams.AutoSize = true;
            this.lblLaserParams.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblLaserParams.Location = new System.Drawing.Point(15, 150);
            this.lblLaserParams.Name = "lblLaserParams";
            this.lblLaserParams.Size = new System.Drawing.Size(434, 17);
            this.lblLaserParams.TabIndex = 4;
            this.lblLaserParams.Text = "雷射功率: 5500W | 頻率: 100Hz | 氣壓: -10.5kPa | 聚焦鏡溫度: 24.2℃";

            this.btnStartRun.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartRun.Font = new System.Drawing.Font("微軟正黑體", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnStartRun.Location = new System.Drawing.Point(18, 180);
            this.btnStartRun.Name = "btnStartRun";
            this.btnStartRun.Size = new System.Drawing.Size(160, 36);
            this.btnStartRun.TabIndex = 5;
            this.btnStartRun.Text = "手動啟動連續運轉";
            this.btnStartRun.UseVisualStyleBackColor = false;

            this.btnStopRun.BackColor = System.Drawing.Color.MistyRose;
            this.btnStopRun.Font = new System.Drawing.Font("微軟正黑體", 9.5F);
            this.btnStopRun.Location = new System.Drawing.Point(190, 180);
            this.btnStopRun.Name = "btnStopRun";
            this.btnStopRun.Size = new System.Drawing.Size(120, 36);
            this.btnStopRun.TabIndex = 6;
            this.btnStopRun.Text = "暫停/停止運轉";
            this.btnStopRun.UseVisualStyleBackColor = false;

            this.btnSimFailTarget.Location = new System.Drawing.Point(325, 180);
            this.btnSimFailTarget.Name = "btnSimFailTarget";
            this.btnSimFailTarget.Size = new System.Drawing.Size(150, 36);
            this.btnSimFailTarget.TabIndex = 7;
            this.btnSimFailTarget.Text = "模擬靶位抓取失敗";
            this.btnSimFailTarget.UseVisualStyleBackColor = true;

            // ================= grpMode =================
            this.grpMode.Controls.Add(this.btnSetOffline);
            this.grpMode.Controls.Add(this.btnSetLocal);
            this.grpMode.Controls.Add(this.btnSetSemiAuto);
            this.grpMode.Controls.Add(this.btnSetAuto);
            this.grpMode.Location = new System.Drawing.Point(12, 355);
            this.grpMode.Name = "grpMode";
            this.grpMode.Size = new System.Drawing.Size(560, 75);
            this.grpMode.TabIndex = 3;
            this.grpMode.TabStop = false;
            this.grpMode.Text = "機台操作盤模式切換 (SPEC 4-1，機台端手動切換)";

            this.btnSetOffline.Location = new System.Drawing.Point(20, 25);
            this.btnSetOffline.Name = "btnSetOffline";
            this.btnSetOffline.Size = new System.Drawing.Size(115, 34);
            this.btnSetOffline.TabIndex = 0;
            this.btnSetOffline.Text = "0: OFFLINE";
            this.btnSetOffline.UseVisualStyleBackColor = true;

            this.btnSetLocal.Location = new System.Drawing.Point(150, 25);
            this.btnSetLocal.Name = "btnSetLocal";
            this.btnSetLocal.Size = new System.Drawing.Size(115, 34);
            this.btnSetLocal.TabIndex = 1;
            this.btnSetLocal.Text = "1: LOCAL";
            this.btnSetLocal.UseVisualStyleBackColor = true;

            this.btnSetSemiAuto.Location = new System.Drawing.Point(280, 25);
            this.btnSetSemiAuto.Name = "btnSetSemiAuto";
            this.btnSetSemiAuto.Size = new System.Drawing.Size(125, 34);
            this.btnSetSemiAuto.TabIndex = 2;
            this.btnSetSemiAuto.Text = "2: SEMI-AUTO";
            this.btnSetSemiAuto.UseVisualStyleBackColor = true;

            this.btnSetAuto.BackColor = System.Drawing.Color.LightCyan;
            this.btnSetAuto.Location = new System.Drawing.Point(420, 25);
            this.btnSetAuto.Name = "btnSetAuto";
            this.btnSetAuto.Size = new System.Drawing.Size(120, 34);
            this.btnSetAuto.TabIndex = 3;
            this.btnSetAuto.Text = "3: AUTO";
            this.btnSetAuto.UseVisualStyleBackColor = false;

            // ================= grpAlarm =================
            this.grpAlarm.Controls.Add(this.lblPreset);
            this.grpAlarm.Controls.Add(this.cboPresetAlarms);
            this.grpAlarm.Controls.Add(this.txtCustomAlarmNo);
            this.grpAlarm.Controls.Add(this.txtCustomAlarmMsg);
            this.grpAlarm.Controls.Add(this.btnTriggerAlarm);
            this.grpAlarm.Controls.Add(this.btnAlReset);
            this.grpAlarm.Controls.Add(this.dgvAlarms);
            this.grpAlarm.Location = new System.Drawing.Point(12, 440);
            this.grpAlarm.Name = "grpAlarm";
            this.grpAlarm.Size = new System.Drawing.Size(560, 320);
            this.grpAlarm.TabIndex = 4;
            this.grpAlarm.TabStop = false;
            this.grpAlarm.Text = "警報注入與 AL-RESET 復歸 (SPEC 4-7，10 組 Alarm Slot)";

            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(15, 28);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(59, 17);
            this.lblPreset.TabIndex = 0;
            this.lblPreset.Text = "預置警報:";

            this.cboPresetAlarms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPresetAlarms.FormattingEnabled = true;
            this.cboPresetAlarms.Location = new System.Drawing.Point(80, 25);
            this.cboPresetAlarms.Name = "cboPresetAlarms";
            this.cboPresetAlarms.Size = new System.Drawing.Size(250, 25);
            this.cboPresetAlarms.TabIndex = 1;

            this.txtCustomAlarmNo.Location = new System.Drawing.Point(340, 25);
            this.txtCustomAlarmNo.Name = "txtCustomAlarmNo";
            this.txtCustomAlarmNo.Size = new System.Drawing.Size(60, 23);
            this.txtCustomAlarmNo.TabIndex = 2;
            this.txtCustomAlarmNo.Text = "0216";

            this.txtCustomAlarmMsg.Location = new System.Drawing.Point(405, 25);
            this.txtCustomAlarmMsg.Name = "txtCustomAlarmMsg";
            this.txtCustomAlarmMsg.Size = new System.Drawing.Size(140, 23);
            this.txtCustomAlarmMsg.TabIndex = 3;
            this.txtCustomAlarmMsg.Text = "工件偏移異常";

            this.btnTriggerAlarm.BackColor = System.Drawing.Color.LightCoral;
            this.btnTriggerAlarm.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            this.btnTriggerAlarm.Location = new System.Drawing.Point(15, 60);
            this.btnTriggerAlarm.Name = "btnTriggerAlarm";
            this.btnTriggerAlarm.Size = new System.Drawing.Size(160, 32);
            this.btnTriggerAlarm.TabIndex = 4;
            this.btnTriggerAlarm.Text = "注入警報 (Trigger)";
            this.btnTriggerAlarm.UseVisualStyleBackColor = false;

            this.btnAlReset.BackColor = System.Drawing.Color.LightYellow;
            this.btnAlReset.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold);
            this.btnAlReset.Location = new System.Drawing.Point(190, 60);
            this.btnAlReset.Name = "btnAlReset";
            this.btnAlReset.Size = new System.Drawing.Size(190, 32);
            this.btnAlReset.TabIndex = 5;
            this.btnAlReset.Text = "機台 AL-RESET (警報復歸)";
            this.btnAlReset.UseVisualStyleBackColor = false;

            this.dgvAlarms.AllowUserToAddRows = false;
            this.dgvAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlarms.BackgroundColor = System.Drawing.Color.White;
            this.dgvAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlarms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colSlot,
                this.colAlarmNo,
                this.colAlarmMsg,
                this.colAlarmType,
                this.colTriggerTime});
            this.dgvAlarms.Location = new System.Drawing.Point(15, 100);
            this.dgvAlarms.Name = "dgvAlarms";
            this.dgvAlarms.ReadOnly = true;
            this.dgvAlarms.RowTemplate.Height = 25;
            this.dgvAlarms.Size = new System.Drawing.Size(530, 205);
            this.dgvAlarms.TabIndex = 6;

            this.colSlot.HeaderText = "Slot";
            this.colSlot.Name = "colSlot";
            this.colSlot.ReadOnly = true;

            this.colAlarmNo.HeaderText = "代碼";
            this.colAlarmNo.Name = "colAlarmNo";
            this.colAlarmNo.ReadOnly = true;

            this.colAlarmMsg.HeaderText = "警報訊息";
            this.colAlarmMsg.Name = "colAlarmMsg";
            this.colAlarmMsg.ReadOnly = true;

            this.colAlarmType.HeaderText = "等級";
            this.colAlarmType.Name = "colAlarmType";
            this.colAlarmType.ReadOnly = true;

            this.colTriggerTime.HeaderText = "發生時間";
            this.colTriggerTime.Name = "colTriggerTime";
            this.colTriggerTime.ReadOnly = true;

            // ================= grpLog =================
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Controls.Add(this.btnClearLog);
            this.grpLog.Location = new System.Drawing.Point(590, 355);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(562, 405);
            this.grpLog.TabIndex = 5;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "機台事件與通訊日誌 (Event Log)";

            this.txtLog.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(3, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(556, 345);
            this.txtLog.TabIndex = 0;

            this.btnClearLog.Location = new System.Drawing.Point(440, 368);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 30);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "清空日誌";
            this.btnClearLog.UseVisualStyleBackColor = true;

            // ================= 根表單配置 =================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 771);
            this.Controls.Add(this.grpDashboard);
            this.Controls.Add(this.grpRecipe);
            this.Controls.Add(this.grpSchedule);
            this.Controls.Add(this.grpMode);
            this.Controls.Add(this.grpAlarm);
            this.Controls.Add(this.grpLog);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SimulatorMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "三菱 CO2 雷射加工機 完整功能測試模擬器 (SPEC: S26888-C)";

            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarms)).EndInit();
            this.grpDashboard.ResumeLayout(false);
            this.grpDashboard.PerformLayout();
            this.grpRecipe.ResumeLayout(false);
            this.grpRecipe.PerformLayout();
            this.grpSchedule.ResumeLayout(false);
            this.grpSchedule.PerformLayout();
            this.grpMode.ResumeLayout(false);
            this.grpAlarm.ResumeLayout(false);
            this.grpAlarm.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpDashboard;
        private System.Windows.Forms.Panel pnlGreen;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Panel pnlYellow;
        private System.Windows.Forms.Label lblYellow;
        private System.Windows.Forms.Panel pnlRed;
        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblStatusBadge;
        private System.Windows.Forms.Label lblModeBadge;
        private System.Windows.Forms.Label lblWatchDogTitle;
        private System.Windows.Forms.CheckBox chkWatchDogEnable;
        private System.Windows.Forms.Button btnFeedDog;
        private System.Windows.Forms.ProgressBar pbWatchDog;
        private System.Windows.Forms.Label lblWatchDogVal;

        private System.Windows.Forms.GroupBox grpRecipe;
        private System.Windows.Forms.Label lblLotInput;
        private System.Windows.Forms.TextBox txtLotInput;
        private System.Windows.Forms.Button btnInputLot;
        private System.Windows.Forms.Label lblGetRecipeReq;
        private System.Windows.Forms.Label lblReqPrg;
        private System.Windows.Forms.TextBox txtReqPrg;
        private System.Windows.Forms.Label lblReqCnd;
        private System.Windows.Forms.TextBox txtReqCnd;
        private System.Windows.Forms.Label lblReqSheet;
        private System.Windows.Forms.TextBox txtReqSheet;
        private System.Windows.Forms.Label lblGetRecipeAck;
        private System.Windows.Forms.TextBox txtGetRecipeAck;

        private System.Windows.Forms.GroupBox grpSchedule;
        private System.Windows.Forms.Label lblActivePrg;
        private System.Windows.Forms.Label lblActiveLot;
        private System.Windows.Forms.ProgressBar pbProcessing;
        private System.Windows.Forms.Label lblCounts;
        private System.Windows.Forms.Label lblLaserParams;
        private System.Windows.Forms.Button btnStartRun;
        private System.Windows.Forms.Button btnStopRun;
        private System.Windows.Forms.Button btnSimFailTarget;

        private System.Windows.Forms.GroupBox grpMode;
        private System.Windows.Forms.Button btnSetOffline;
        private System.Windows.Forms.Button btnSetLocal;
        private System.Windows.Forms.Button btnSetSemiAuto;
        private System.Windows.Forms.Button btnSetAuto;

        private System.Windows.Forms.GroupBox grpAlarm;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cboPresetAlarms;
        private System.Windows.Forms.TextBox txtCustomAlarmNo;
        private System.Windows.Forms.TextBox txtCustomAlarmMsg;
        private System.Windows.Forms.Button btnTriggerAlarm;
        private System.Windows.Forms.Button btnAlReset;
        private System.Windows.Forms.DataGridView dgvAlarms;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSlot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTriggerTime;

        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnClearLog;
    }
}
