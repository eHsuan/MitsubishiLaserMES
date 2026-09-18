namespace EQSimulator
{
    partial class MainForm
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
            panelTop = new System.Windows.Forms.Panel();
            lblMqttStatus = new System.Windows.Forms.Label();
            lblEqpId = new System.Windows.Forms.Label();
            _txtEqId = new System.Windows.Forms.TextBox();
            lblBrokerIp = new System.Windows.Forms.Label();
            _txtBroker = new System.Windows.Forms.TextBox();
            lblPort = new System.Windows.Forms.Label();
            _txtPort = new System.Windows.Forms.TextBox();
            lblTopic = new System.Windows.Forms.Label();
            _txtTopic = new System.Windows.Forms.TextBox();
            _btnConnect = new System.Windows.Forms.Button();
            grpParams = new System.Windows.Forms.GroupBox();
            lblUserId = new System.Windows.Forms.Label();
            _txtUserId = new System.Windows.Forms.TextBox();
            lblBarcode = new System.Windows.Forms.Label();
            _txtBarcode = new System.Windows.Forms.TextBox();
            lblWorkOrder = new System.Windows.Forms.Label();
            _txtWorkOrder = new System.Windows.Forms.TextBox();
            lblMaterialId = new System.Windows.Forms.Label();
            _txtMaterialId = new System.Windows.Forms.TextBox();
            lblCassetteId = new System.Windows.Forms.Label();
            _txtCassetteId = new System.Windows.Forms.TextBox();
            lblToolingId = new System.Windows.Forms.Label();
            _txtToolingId = new System.Windows.Forms.TextBox();
            lblQty = new System.Windows.Forms.Label();
            _txtQty = new System.Windows.Forms.TextBox();
            lblPanelList = new System.Windows.Forms.Label();
            _txtPanelList = new System.Windows.Forms.TextBox();
            _grpActions = new System.Windows.Forms.GroupBox();
            flowActions = new System.Windows.Forms.FlowLayoutPanel();
            btnUser = new System.Windows.Forms.Button();
            btnTrackIn = new System.Windows.Forms.Button();
            btnTrackOut = new System.Windows.Forms.Button();
            btnPanelIn = new System.Windows.Forms.Button();
            btnPanelOut = new System.Windows.Forms.Button();
            btnRtnTraceData = new System.Windows.Forms.Button();
            btnAlarm = new System.Windows.Forms.Button();
            _btnAutoTest = new System.Windows.Forms.Button();
            chkPPSelectPass = new System.Windows.Forms.CheckBox();
            pnlStatusReport = new System.Windows.Forms.Panel();
            _cmbStatusType = new System.Windows.Forms.ComboBox();
            _cmbStatus = new System.Windows.Forms.ComboBox();
            btnReportStatus = new System.Windows.Forms.Button();
            pnlNGGroup = new System.Windows.Forms.Panel();
            lblNGCode = new System.Windows.Forms.Label();
            _cboNGCode = new System.Windows.Forms.ComboBox();
            _txtNGName = new System.Windows.Forms.TextBox();
            _txtNGQty = new System.Windows.Forms.TextBox();
            _txtNGUnit = new System.Windows.Forms.TextBox();
            btnSendNGCode = new System.Windows.Forms.Button();
            btnSendAllNG = new System.Windows.Forms.Button();
            pnlLeftoverGroup = new System.Windows.Forms.Panel();
            lblLeftoverReel = new System.Windows.Forms.Label();
            _txtLeftoverReel = new System.Windows.Forms.TextBox();
            lblLeftoverQty = new System.Windows.Forms.Label();
            _txtLeftoverQty = new System.Windows.Forms.TextBox();
            btnSendLeftover = new System.Windows.Forms.Button();
            btnSendUseLeftover = new System.Windows.Forms.Button();
            pnlTotalCountGroup = new System.Windows.Forms.Panel();
            lblTotalReelCount = new System.Windows.Forms.Label();
            _txtTotalReelCount = new System.Windows.Forms.TextBox();
            lblTotalQtyPerReel = new System.Windows.Forms.Label();
            _txtTotalQtyPerReel = new System.Windows.Forms.TextBox();
            btnSendTotalCount = new System.Windows.Forms.Button();
            pnlScenarioGroup = new System.Windows.Forms.Panel();
            lblScenario = new System.Windows.Forms.Label();
            _cboScenario = new System.Windows.Forms.ComboBox();
            btnApplyScenario = new System.Windows.Forms.Button();
            _txtLog = new System.Windows.Forms.TextBox();
            panelTop.SuspendLayout();
            grpParams.SuspendLayout();
            _grpActions.SuspendLayout();
            flowActions.SuspendLayout();
            pnlStatusReport.SuspendLayout();
            pnlNGGroup.SuspendLayout();
            pnlLeftoverGroup.SuspendLayout();
            pnlTotalCountGroup.SuspendLayout();
            pnlScenarioGroup.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(lblMqttStatus);
            panelTop.Controls.Add(lblEqpId);
            panelTop.Controls.Add(_txtEqId);
            panelTop.Controls.Add(lblBrokerIp);
            panelTop.Controls.Add(_txtBroker);
            panelTop.Controls.Add(lblPort);
            panelTop.Controls.Add(_txtPort);
            panelTop.Controls.Add(lblTopic);
            panelTop.Controls.Add(_txtTopic);
            panelTop.Controls.Add(_btnConnect);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Location = new System.Drawing.Point(0, 0);
            panelTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            panelTop.Name = "panelTop";
            panelTop.Size = new System.Drawing.Size(915, 100);
            panelTop.TabIndex = 0;
            // 
            // lblMqttStatus
            // 
            lblMqttStatus.BackColor = System.Drawing.Color.LightPink;
            lblMqttStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblMqttStatus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            lblMqttStatus.Location = new System.Drawing.Point(12, 10);
            lblMqttStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMqttStatus.Name = "lblMqttStatus";
            lblMqttStatus.Size = new System.Drawing.Size(128, 38);
            lblMqttStatus.TabIndex = 0;
            lblMqttStatus.Text = "MQTT: OFF";
            lblMqttStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEqpId
            // 
            lblEqpId.Location = new System.Drawing.Point(152, 18);
            lblEqpId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEqpId.Name = "lblEqpId";
            lblEqpId.Size = new System.Drawing.Size(58, 24);
            lblEqpId.TabIndex = 1;
            lblEqpId.Text = "EQ ID:";
            // 
            // _txtEqId
            // 
            _txtEqId.Location = new System.Drawing.Point(218, 14);
            _txtEqId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtEqId.Name = "_txtEqId";
            _txtEqId.Size = new System.Drawing.Size(93, 23);
            _txtEqId.TabIndex = 2;
            _txtEqId.Text = "EQ01";
            // 
            // lblBrokerIp
            // 
            lblBrokerIp.Location = new System.Drawing.Point(320, 18);
            lblBrokerIp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblBrokerIp.Name = "lblBrokerIp";
            lblBrokerIp.Size = new System.Drawing.Size(70, 24);
            lblBrokerIp.TabIndex = 3;
            lblBrokerIp.Text = "Broker IP:";
            // 
            // _txtBroker
            // 
            _txtBroker.Location = new System.Drawing.Point(398, 14);
            _txtBroker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtBroker.Name = "_txtBroker";
            _txtBroker.Size = new System.Drawing.Size(116, 23);
            _txtBroker.TabIndex = 4;
            _txtBroker.Text = "127.0.0.1";
            // 
            // lblPort
            // 
            lblPort.Location = new System.Drawing.Point(523, 18);
            lblPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPort.Name = "lblPort";
            lblPort.Size = new System.Drawing.Size(47, 24);
            lblPort.TabIndex = 5;
            lblPort.Text = "Port:";
            // 
            // _txtPort
            // 
            _txtPort.Location = new System.Drawing.Point(578, 14);
            _txtPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtPort.Name = "_txtPort";
            _txtPort.Size = new System.Drawing.Size(58, 23);
            _txtPort.TabIndex = 6;
            _txtPort.Text = "1883";
            // 
            // lblTopic
            // 
            lblTopic.Location = new System.Drawing.Point(12, 62);
            lblTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTopic.Name = "lblTopic";
            lblTopic.Size = new System.Drawing.Size(93, 24);
            lblTopic.TabIndex = 7;
            lblTopic.Text = "Topic:";
            // 
            // _txtTopic
            // 
            _txtTopic.Location = new System.Drawing.Point(117, 59);
            _txtTopic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtTopic.Name = "_txtTopic";
            _txtTopic.Size = new System.Drawing.Size(519, 23);
            _txtTopic.TabIndex = 8;
            _txtTopic.Text = "EQ/EQ01";
            // 
            // _btnConnect
            // 
            _btnConnect.Location = new System.Drawing.Point(660, 11);
            _btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _btnConnect.Name = "_btnConnect";
            _btnConnect.Size = new System.Drawing.Size(240, 74);
            _btnConnect.TabIndex = 11;
            _btnConnect.Text = "Connect";
            _btnConnect.UseVisualStyleBackColor = true;
            _btnConnect.Click += BtnConnect_Click;
            // 
            // grpParams
            // 
            grpParams.Controls.Add(lblUserId);
            grpParams.Controls.Add(_txtUserId);
            grpParams.Controls.Add(lblBarcode);
            grpParams.Controls.Add(_txtBarcode);
            grpParams.Controls.Add(lblWorkOrder);
            grpParams.Controls.Add(_txtWorkOrder);
            grpParams.Controls.Add(lblMaterialId);
            grpParams.Controls.Add(_txtMaterialId);
            grpParams.Controls.Add(lblCassetteId);
            grpParams.Controls.Add(_txtCassetteId);
            grpParams.Controls.Add(lblToolingId);
            grpParams.Controls.Add(_txtToolingId);
            grpParams.Controls.Add(lblQty);
            grpParams.Controls.Add(_txtQty);
            grpParams.Controls.Add(lblPanelList);
            grpParams.Controls.Add(_txtPanelList);
            grpParams.Dock = System.Windows.Forms.DockStyle.Top;
            grpParams.Location = new System.Drawing.Point(0, 100);
            grpParams.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            grpParams.Name = "grpParams";
            grpParams.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            grpParams.Size = new System.Drawing.Size(915, 185);
            grpParams.TabIndex = 1;
            grpParams.TabStop = false;
            grpParams.Text = "Parameters Setup";
            // 
            // lblUserId
            // 
            lblUserId.Location = new System.Drawing.Point(12, 26);
            lblUserId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new System.Drawing.Size(93, 29);
            lblUserId.TabIndex = 0;
            lblUserId.Text = "User ID:";
            // 
            // _txtUserId
            // 
            _txtUserId.Location = new System.Drawing.Point(105, 22);
            _txtUserId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtUserId.Name = "_txtUserId";
            _txtUserId.Size = new System.Drawing.Size(116, 23);
            _txtUserId.TabIndex = 1;
            _txtUserId.Text = "OP01";
            // 
            // lblBarcode
            // 
            lblBarcode.Location = new System.Drawing.Point(233, 26);
            lblBarcode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblBarcode.Name = "lblBarcode";
            lblBarcode.Size = new System.Drawing.Size(93, 29);
            lblBarcode.TabIndex = 2;
            lblBarcode.Text = "Barcode/Lot:";
            // 
            // _txtBarcode
            // 
            _txtBarcode.Location = new System.Drawing.Point(327, 22);
            _txtBarcode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtBarcode.Name = "_txtBarcode";
            _txtBarcode.Size = new System.Drawing.Size(116, 23);
            _txtBarcode.TabIndex = 3;
            _txtBarcode.Text = "LOT999";
            // 
            // lblWorkOrder
            // 
            lblWorkOrder.Location = new System.Drawing.Point(12, 62);
            lblWorkOrder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblWorkOrder.Name = "lblWorkOrder";
            lblWorkOrder.Size = new System.Drawing.Size(93, 29);
            lblWorkOrder.TabIndex = 4;
            lblWorkOrder.Text = "Work Order:";
            // 
            // _txtWorkOrder
            // 
            _txtWorkOrder.Location = new System.Drawing.Point(105, 59);
            _txtWorkOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtWorkOrder.Name = "_txtWorkOrder";
            _txtWorkOrder.Size = new System.Drawing.Size(116, 23);
            _txtWorkOrder.TabIndex = 5;
            _txtWorkOrder.Text = "WO123456";
            // 
            // lblMaterialId
            // 
            lblMaterialId.Location = new System.Drawing.Point(233, 62);
            lblMaterialId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMaterialId.Name = "lblMaterialId";
            lblMaterialId.Size = new System.Drawing.Size(93, 29);
            lblMaterialId.TabIndex = 6;
            lblMaterialId.Text = "Material ID:";
            // 
            // _txtMaterialId
            // 
            _txtMaterialId.Location = new System.Drawing.Point(327, 59);
            _txtMaterialId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtMaterialId.Name = "_txtMaterialId";
            _txtMaterialId.Size = new System.Drawing.Size(116, 23);
            _txtMaterialId.TabIndex = 7;
            _txtMaterialId.Text = "MAT001";
            // 
            // lblCassetteId
            // 
            lblCassetteId.Location = new System.Drawing.Point(12, 100);
            lblCassetteId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblCassetteId.Name = "lblCassetteId";
            lblCassetteId.Size = new System.Drawing.Size(93, 29);
            lblCassetteId.TabIndex = 8;
            lblCassetteId.Text = "Cassette ID:";
            // 
            // _txtCassetteId
            // 
            _txtCassetteId.Location = new System.Drawing.Point(105, 96);
            _txtCassetteId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtCassetteId.Name = "_txtCassetteId";
            _txtCassetteId.Size = new System.Drawing.Size(116, 23);
            _txtCassetteId.TabIndex = 9;
            _txtCassetteId.Text = "C0000000000002";
            // 
            // lblToolingId
            // 
            lblToolingId.Location = new System.Drawing.Point(233, 100);
            lblToolingId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblToolingId.Name = "lblToolingId";
            lblToolingId.Size = new System.Drawing.Size(93, 29);
            lblToolingId.TabIndex = 10;
            lblToolingId.Text = "Tooling ID:";
            // 
            // _txtToolingId
            // 
            _txtToolingId.Location = new System.Drawing.Point(327, 96);
            _txtToolingId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtToolingId.Name = "_txtToolingId";
            _txtToolingId.Size = new System.Drawing.Size(116, 23);
            _txtToolingId.TabIndex = 11;
            _txtToolingId.Text = "TOOL01";
            // 
            // lblQty
            // 
            lblQty.Location = new System.Drawing.Point(467, 100);
            lblQty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblQty.Name = "lblQty";
            lblQty.Size = new System.Drawing.Size(40, 29);
            lblQty.TabIndex = 12;
            lblQty.Text = "Qty:";
            // 
            // _txtQty
            // 
            _txtQty.Location = new System.Drawing.Point(510, 96);
            _txtQty.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtQty.Name = "_txtQty";
            _txtQty.Size = new System.Drawing.Size(116, 23);
            _txtQty.TabIndex = 13;
            _txtQty.Text = "100";
            // 
            // lblPanelList
            // 
            lblPanelList.Location = new System.Drawing.Point(12, 140);
            lblPanelList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPanelList.Name = "lblPanelList";
            lblPanelList.Size = new System.Drawing.Size(93, 29);
            lblPanelList.TabIndex = 14;
            lblPanelList.Text = "Panel List:";
            // 
            // _txtPanelList
            // 
            _txtPanelList.Location = new System.Drawing.Point(105, 136);
            _txtPanelList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtPanelList.Name = "_txtPanelList";
            _txtPanelList.Size = new System.Drawing.Size(571, 23);
            _txtPanelList.TabIndex = 15;
            _txtPanelList.Text = "P0000000000011,P0000000000012,P0000000000013,P0000000000014,P0000000000015,P0000000000016,P0000000000017,P0000000000018,P0000000000019,P0000000000020";
            // 
            // _grpActions
            // 
            _grpActions.Controls.Add(flowActions);
            _grpActions.Dock = System.Windows.Forms.DockStyle.Top;
            _grpActions.Enabled = false;
            _grpActions.Location = new System.Drawing.Point(0, 285);
            _grpActions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _grpActions.Name = "_grpActions";
            _grpActions.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _grpActions.Size = new System.Drawing.Size(960, 270);
            _grpActions.TabIndex = 2;
            _grpActions.TabStop = false;
            _grpActions.Text = "Simulation Actions";
            // 
            // flowActions
            // 
            flowActions.AutoScroll = true;
            flowActions.Controls.Add(btnUser);
            flowActions.Controls.Add(btnTrackIn);
            flowActions.Controls.Add(btnTrackOut);
            flowActions.Controls.Add(btnPanelIn);
            flowActions.Controls.Add(btnPanelOut);
            flowActions.Controls.Add(btnRtnTraceData);
            flowActions.Controls.Add(btnAlarm);
            flowActions.Controls.Add(_btnAutoTest);
            flowActions.Controls.Add(chkPPSelectPass);
            flowActions.Controls.Add(pnlStatusReport);
            flowActions.Controls.Add(pnlNGGroup);
            flowActions.Controls.Add(pnlLeftoverGroup);
            flowActions.Controls.Add(pnlTotalCountGroup);
            flowActions.Controls.Add(pnlScenarioGroup);
            flowActions.Dock = System.Windows.Forms.DockStyle.Fill;
            flowActions.Location = new System.Drawing.Point(4, 20);
            flowActions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            flowActions.Name = "flowActions";
            flowActions.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            flowActions.Size = new System.Drawing.Size(952, 246);
            flowActions.TabIndex = 0;
            // 
            // btnUser
            // 
            btnUser.Location = new System.Drawing.Point(10, 10);
            btnUser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnUser.Name = "btnUser";
            btnUser.Size = new System.Drawing.Size(140, 29);
            btnUser.TabIndex = 0;
            btnUser.Text = "Login (UserVerify)";
            btnUser.UseVisualStyleBackColor = true;
            btnUser.Click += BtnUser_Click;
            // 
            // btnTrackIn
            // 
            btnTrackIn.Location = new System.Drawing.Point(158, 10);
            btnTrackIn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnTrackIn.Name = "btnTrackIn";
            btnTrackIn.Size = new System.Drawing.Size(140, 29);
            btnTrackIn.TabIndex = 1;
            btnTrackIn.Text = "Track In Request";
            btnTrackIn.UseVisualStyleBackColor = true;
            btnTrackIn.Click += BtnTrackIn_Click;
            // 
            // btnTrackOut
            // 
            btnTrackOut.Location = new System.Drawing.Point(306, 10);
            btnTrackOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnTrackOut.Name = "btnTrackOut";
            btnTrackOut.Size = new System.Drawing.Size(140, 29);
            btnTrackOut.TabIndex = 2;
            btnTrackOut.Text = "Track Out Request";
            btnTrackOut.UseVisualStyleBackColor = true;
            btnTrackOut.Click += BtnTrackOut_Click;
            // 
            // btnPanelIn
            // 
            btnPanelIn.Location = new System.Drawing.Point(454, 10);
            btnPanelIn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnPanelIn.Name = "btnPanelIn";
            btnPanelIn.Size = new System.Drawing.Size(105, 29);
            btnPanelIn.TabIndex = 3;
            btnPanelIn.Text = "Panel In";
            btnPanelIn.UseVisualStyleBackColor = true;
            btnPanelIn.Click += BtnPanelIn_Click;
            // 
            // btnPanelOut
            // 
            btnPanelOut.Location = new System.Drawing.Point(567, 10);
            btnPanelOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnPanelOut.Name = "btnPanelOut";
            btnPanelOut.Size = new System.Drawing.Size(105, 29);
            btnPanelOut.TabIndex = 4;
            btnPanelOut.Text = "Panel Out";
            btnPanelOut.UseVisualStyleBackColor = true;
            btnPanelOut.Click += BtnPanelOut_Click;
            // 
            // btnRtnTraceData
            // 
            btnRtnTraceData.Location = new System.Drawing.Point(680, 10);
            btnRtnTraceData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnRtnTraceData.Name = "btnRtnTraceData";
            btnRtnTraceData.Size = new System.Drawing.Size(117, 29);
            btnRtnTraceData.TabIndex = 5;
            btnRtnTraceData.Text = "Trace Data";
            btnRtnTraceData.UseVisualStyleBackColor = true;
            btnRtnTraceData.Click += BtnRtnTraceData_Click;
            // 
            // btnAlarm
            // 
            btnAlarm.Location = new System.Drawing.Point(10, 47);
            btnAlarm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnAlarm.Name = "btnAlarm";
            btnAlarm.Size = new System.Drawing.Size(140, 29);
            btnAlarm.TabIndex = 6;
            btnAlarm.Text = "Trigger ALARM";
            btnAlarm.UseVisualStyleBackColor = true;
            btnAlarm.Click += BtnAlarm_Click;
            // 
            // _btnAutoTest
            // 
            _btnAutoTest.Location = new System.Drawing.Point(158, 47);
            _btnAutoTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _btnAutoTest.Name = "_btnAutoTest";
            _btnAutoTest.Size = new System.Drawing.Size(120, 29);
            _btnAutoTest.TabIndex = 7;
            _btnAutoTest.Text = "Start Auto Test";
            _btnAutoTest.UseVisualStyleBackColor = true;
            _btnAutoTest.Click += BtnAutoTest_Click;
            // 
            // chkPPSelectPass
            // 
            chkPPSelectPass.AutoSize = true;
            chkPPSelectPass.Checked = true;
            chkPPSelectPass.CheckState = System.Windows.Forms.CheckState.Checked;
            chkPPSelectPass.Location = new System.Drawing.Point(286, 52);
            chkPPSelectPass.Margin = new System.Windows.Forms.Padding(4, 5, 4, 4);
            chkPPSelectPass.Name = "chkPPSelectPass";
            chkPPSelectPass.Size = new System.Drawing.Size(115, 19);
            chkPPSelectPass.TabIndex = 8;
            chkPPSelectPass.Text = "PP-SELECT PASS";
            chkPPSelectPass.UseVisualStyleBackColor = true;
            // 
            // pnlStatusReport
            // 
            pnlStatusReport.Controls.Add(_cmbStatusType);
            pnlStatusReport.Controls.Add(_cmbStatus);
            pnlStatusReport.Controls.Add(btnReportStatus);
            pnlStatusReport.Location = new System.Drawing.Point(409, 47);
            pnlStatusReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlStatusReport.Name = "pnlStatusReport";
            pnlStatusReport.Size = new System.Drawing.Size(370, 34);
            pnlStatusReport.TabIndex = 9;
            // 
            // _cmbStatusType
            // 
            _cmbStatusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cmbStatusType.FormattingEnabled = true;
            _cmbStatusType.Items.AddRange(new object[] { "Cyntec", "Delta" });
            _cmbStatusType.Location = new System.Drawing.Point(4, 4);
            _cmbStatusType.Name = "_cmbStatusType";
            _cmbStatusType.Size = new System.Drawing.Size(110, 23);
            _cmbStatusType.TabIndex = 0;
            // 
            // _cmbStatus
            // 
            _cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cmbStatus.FormattingEnabled = true;
            _cmbStatus.Location = new System.Drawing.Point(120, 4);
            _cmbStatus.Name = "_cmbStatus";
            _cmbStatus.Size = new System.Drawing.Size(140, 23);
            _cmbStatus.TabIndex = 1;
            // 
            // btnReportStatus
            // 
            btnReportStatus.Location = new System.Drawing.Point(266, 1);
            btnReportStatus.Name = "btnReportStatus";
            btnReportStatus.Size = new System.Drawing.Size(95, 29);
            btnReportStatus.TabIndex = 2;
            btnReportStatus.Text = "Report Status";
            btnReportStatus.UseVisualStyleBackColor = true;
            btnReportStatus.Click += BtnReportStatus_Click;
            // 
            // pnlNGGroup
            // 
            pnlNGGroup.Controls.Add(lblNGCode);
            pnlNGGroup.Controls.Add(_cboNGCode);
            pnlNGGroup.Controls.Add(_txtNGName);
            pnlNGGroup.Controls.Add(_txtNGQty);
            pnlNGGroup.Controls.Add(_txtNGUnit);
            pnlNGGroup.Controls.Add(btnSendNGCode);
            pnlNGGroup.Controls.Add(btnSendAllNG);
            pnlNGGroup.Location = new System.Drawing.Point(10, 89);
            pnlNGGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlNGGroup.Name = "pnlNGGroup";
            pnlNGGroup.Size = new System.Drawing.Size(530, 38);
            pnlNGGroup.TabIndex = 10;
            // 
            // lblNGCode
            // 
            lblNGCode.Location = new System.Drawing.Point(4, 8);
            lblNGCode.Name = "lblNGCode";
            lblNGCode.Size = new System.Drawing.Size(32, 23);
            lblNGCode.TabIndex = 0;
            lblNGCode.Text = "NG:";
            // 
            // _cboNGCode
            // 
            _cboNGCode.FormattingEnabled = true;
            _cboNGCode.Items.AddRange(new object[] { "UPNG501", "UPNG502", "UPNG503", "QC_KEEP", "FREE_SUP" });
            _cboNGCode.Location = new System.Drawing.Point(38, 4);
            _cboNGCode.Name = "_cboNGCode";
            _cboNGCode.Size = new System.Drawing.Size(95, 23);
            _cboNGCode.TabIndex = 1;
            _cboNGCode.Text = "UPNG501";
            _cboNGCode.SelectedIndexChanged += CboNGCode_SelectedIndexChanged;
            // 
            // _txtNGName
            // 
            _txtNGName.Location = new System.Drawing.Point(139, 4);
            _txtNGName.Name = "_txtNGName";
            _txtNGName.Size = new System.Drawing.Size(100, 23);
            _txtNGName.TabIndex = 2;
            _txtNGName.Text = "機台卡片";
            // 
            // _txtNGQty
            // 
            _txtNGQty.Location = new System.Drawing.Point(245, 4);
            _txtNGQty.Name = "_txtNGQty";
            _txtNGQty.Size = new System.Drawing.Size(38, 23);
            _txtNGQty.TabIndex = 3;
            _txtNGQty.Text = "1";
            // 
            // _txtNGUnit
            // 
            _txtNGUnit.Location = new System.Drawing.Point(289, 4);
            _txtNGUnit.Name = "_txtNGUnit";
            _txtNGUnit.Size = new System.Drawing.Size(45, 23);
            _txtNGUnit.TabIndex = 4;
            _txtNGUnit.Text = "PCS";
            // 
            // btnSendNGCode
            // 
            btnSendNGCode.Location = new System.Drawing.Point(340, 1);
            btnSendNGCode.Name = "btnSendNGCode";
            btnSendNGCode.Size = new System.Drawing.Size(80, 29);
            btnSendNGCode.TabIndex = 5;
            btnSendNGCode.Text = "上報 NG";
            btnSendNGCode.UseVisualStyleBackColor = true;
            btnSendNGCode.Click += BtnSendNGCode_Click;
            // 
            // btnSendAllNG
            // 
            btnSendAllNG.Location = new System.Drawing.Point(426, 1);
            btnSendAllNG.Name = "btnSendAllNG";
            btnSendAllNG.Size = new System.Drawing.Size(95, 29);
            btnSendAllNG.TabIndex = 6;
            btnSendAllNG.Text = "一鍵上報全部";
            btnSendAllNG.UseVisualStyleBackColor = true;
            btnSendAllNG.Click += BtnSendAllNG_Click;
            // 
            // pnlLeftoverGroup
            // 
            pnlLeftoverGroup.Controls.Add(lblLeftoverReel);
            pnlLeftoverGroup.Controls.Add(_txtLeftoverReel);
            pnlLeftoverGroup.Controls.Add(lblLeftoverQty);
            pnlLeftoverGroup.Controls.Add(_txtLeftoverQty);
            pnlLeftoverGroup.Controls.Add(btnSendLeftover);
            pnlLeftoverGroup.Controls.Add(btnSendUseLeftover);
            pnlLeftoverGroup.Location = new System.Drawing.Point(548, 89);
            pnlLeftoverGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlLeftoverGroup.Name = "pnlLeftoverGroup";
            pnlLeftoverGroup.Size = new System.Drawing.Size(355, 38);
            pnlLeftoverGroup.TabIndex = 11;
            // 
            // lblLeftoverReel
            // 
            lblLeftoverReel.Location = new System.Drawing.Point(4, 8);
            lblLeftoverReel.Name = "lblLeftoverReel";
            lblLeftoverReel.Size = new System.Drawing.Size(46, 23);
            lblLeftoverReel.TabIndex = 0;
            lblLeftoverReel.Text = "Reel:";
            // 
            // _txtLeftoverReel
            // 
            _txtLeftoverReel.Location = new System.Drawing.Point(50, 4);
            _txtLeftoverReel.Name = "_txtLeftoverReel";
            _txtLeftoverReel.Size = new System.Drawing.Size(75, 23);
            _txtLeftoverReel.TabIndex = 1;
            _txtLeftoverReel.Text = "REEL001";
            // 
            // lblLeftoverQty
            // 
            lblLeftoverQty.Location = new System.Drawing.Point(127, 8);
            lblLeftoverQty.Name = "lblLeftoverQty";
            lblLeftoverQty.Size = new System.Drawing.Size(30, 23);
            lblLeftoverQty.TabIndex = 2;
            lblLeftoverQty.Text = "Qty:";
            // 
            // _txtLeftoverQty
            // 
            _txtLeftoverQty.Location = new System.Drawing.Point(157, 4);
            _txtLeftoverQty.Name = "_txtLeftoverQty";
            _txtLeftoverQty.Size = new System.Drawing.Size(35, 23);
            _txtLeftoverQty.TabIndex = 3;
            _txtLeftoverQty.Text = "10";
            // 
            // btnSendLeftover
            // 
            btnSendLeftover.Location = new System.Drawing.Point(196, 1);
            btnSendLeftover.Name = "btnSendLeftover";
            btnSendLeftover.Size = new System.Drawing.Size(75, 29);
            btnSendLeftover.TabIndex = 4;
            btnSendLeftover.Text = "上報剩料";
            btnSendLeftover.UseVisualStyleBackColor = true;
            btnSendLeftover.Click += BtnSendLeftover_Click;
            // 
            // btnSendUseLeftover
            // 
            btnSendUseLeftover.Location = new System.Drawing.Point(275, 1);
            btnSendUseLeftover.Name = "btnSendUseLeftover";
            btnSendUseLeftover.Size = new System.Drawing.Size(75, 29);
            btnSendUseLeftover.TabIndex = 5;
            btnSendUseLeftover.Text = "上報補料";
            btnSendUseLeftover.UseVisualStyleBackColor = true;
            btnSendUseLeftover.Click += BtnSendUseLeftover_Click;
            // 
            // pnlTotalCountGroup
            // 
            pnlTotalCountGroup.Controls.Add(lblTotalReelCount);
            pnlTotalCountGroup.Controls.Add(_txtTotalReelCount);
            pnlTotalCountGroup.Controls.Add(lblTotalQtyPerReel);
            pnlTotalCountGroup.Controls.Add(_txtTotalQtyPerReel);
            pnlTotalCountGroup.Controls.Add(btnSendTotalCount);
            pnlTotalCountGroup.Location = new System.Drawing.Point(10, 131);
            pnlTotalCountGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlTotalCountGroup.Name = "pnlTotalCountGroup";
            pnlTotalCountGroup.Size = new System.Drawing.Size(370, 38);
            pnlTotalCountGroup.TabIndex = 12;
            // 
            // lblTotalReelCount
            // 
            lblTotalReelCount.Location = new System.Drawing.Point(4, 8);
            lblTotalReelCount.Name = "lblTotalReelCount";
            lblTotalReelCount.Size = new System.Drawing.Size(65, 23);
            lblTotalReelCount.TabIndex = 0;
            lblTotalReelCount.Text = "ReelCount:";
            // 
            // _txtTotalReelCount
            // 
            _txtTotalReelCount.Location = new System.Drawing.Point(72, 4);
            _txtTotalReelCount.Name = "_txtTotalReelCount";
            _txtTotalReelCount.Size = new System.Drawing.Size(45, 23);
            _txtTotalReelCount.TabIndex = 1;
            _txtTotalReelCount.Text = "10";
            // 
            // lblTotalQtyPerReel
            // 
            lblTotalQtyPerReel.Location = new System.Drawing.Point(122, 8);
            lblTotalQtyPerReel.Name = "lblTotalQtyPerReel";
            lblTotalQtyPerReel.Size = new System.Drawing.Size(75, 23);
            lblTotalQtyPerReel.TabIndex = 2;
            lblTotalQtyPerReel.Text = "QtyPerReel:";
            // 
            // _txtTotalQtyPerReel
            // 
            _txtTotalQtyPerReel.Location = new System.Drawing.Point(200, 4);
            _txtTotalQtyPerReel.Name = "_txtTotalQtyPerReel";
            _txtTotalQtyPerReel.Size = new System.Drawing.Size(55, 23);
            _txtTotalQtyPerReel.TabIndex = 3;
            _txtTotalQtyPerReel.Text = "500";
            // 
            // btnSendTotalCount
            // 
            btnSendTotalCount.Location = new System.Drawing.Point(262, 1);
            btnSendTotalCount.Name = "btnSendTotalCount";
            btnSendTotalCount.Size = new System.Drawing.Size(85, 29);
            btnSendTotalCount.TabIndex = 4;
            btnSendTotalCount.Text = "上報總數";
            btnSendTotalCount.UseVisualStyleBackColor = true;
            btnSendTotalCount.Click += BtnSendTotalCount_Click;
            // 
            // pnlScenarioGroup
            // 
            pnlScenarioGroup.Controls.Add(lblScenario);
            pnlScenarioGroup.Controls.Add(_cboScenario);
            pnlScenarioGroup.Controls.Add(btnApplyScenario);
            pnlScenarioGroup.Location = new System.Drawing.Point(10, 175);
            pnlScenarioGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlScenarioGroup.Name = "pnlScenarioGroup";
            pnlScenarioGroup.Size = new System.Drawing.Size(550, 38);
            pnlScenarioGroup.TabIndex = 13;
            // 
            // lblScenario
            // 
            lblScenario.Location = new System.Drawing.Point(4, 8);
            lblScenario.Name = "lblScenario";
            lblScenario.Size = new System.Drawing.Size(85, 23);
            lblScenario.TabIndex = 0;
            lblScenario.Text = "TP 測試情境:";
            // 
            // _cboScenario
            // 
            _cboScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cboScenario.FormattingEnabled = true;
            _cboScenario.Location = new System.Drawing.Point(92, 4);
            _cboScenario.Name = "_cboScenario";
            _cboScenario.Size = new System.Drawing.Size(320, 23);
            _cboScenario.TabIndex = 1;
            // 
            // btnApplyScenario
            // 
            btnApplyScenario.Location = new System.Drawing.Point(420, 1);
            btnApplyScenario.Name = "btnApplyScenario";
            btnApplyScenario.Size = new System.Drawing.Size(120, 29);
            btnApplyScenario.TabIndex = 2;
            btnApplyScenario.Text = "載入情境參數";
            btnApplyScenario.UseVisualStyleBackColor = true;
            btnApplyScenario.Click += BtnApplyScenario_Click;
            // 
            // _txtLog
            // 
            _txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            _txtLog.Location = new System.Drawing.Point(0, 490);
            _txtLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            _txtLog.Multiline = true;
            _txtLog.Name = "_txtLog";
            _txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            _txtLog.Size = new System.Drawing.Size(915, 230);
            _txtLog.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(960, 780);
            Controls.Add(_txtLog);
            Controls.Add(_grpActions);
            Controls.Add(grpParams);
            Controls.Add(panelTop);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "MainForm";
            Text = "Cyntec MQTT EQ Simulator (Equipment Side)";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            grpParams.ResumeLayout(false);
            grpParams.PerformLayout();
            _grpActions.ResumeLayout(false);
            flowActions.ResumeLayout(false);
            flowActions.PerformLayout();
            pnlStatusReport.ResumeLayout(false);
            pnlNGGroup.ResumeLayout(false);
            pnlNGGroup.PerformLayout();
            pnlLeftoverGroup.ResumeLayout(false);
            pnlLeftoverGroup.PerformLayout();
            pnlTotalCountGroup.ResumeLayout(false);
            pnlTotalCountGroup.PerformLayout();
            pnlScenarioGroup.ResumeLayout(false);
            pnlScenarioGroup.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblMqttStatus;
        private System.Windows.Forms.Label lblEqpId;
        private System.Windows.Forms.TextBox _txtEqId;
        private System.Windows.Forms.Label lblBrokerIp;
        private System.Windows.Forms.TextBox _txtBroker;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox _txtPort;
        private System.Windows.Forms.Label lblTopic;
        private System.Windows.Forms.TextBox _txtTopic;
        private System.Windows.Forms.Button _btnConnect;
        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.TextBox _txtUserId;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox _txtBarcode;
        private System.Windows.Forms.Label lblWorkOrder;
        private System.Windows.Forms.TextBox _txtWorkOrder;
        private System.Windows.Forms.Label lblMaterialId;
        private System.Windows.Forms.TextBox _txtMaterialId;
        private System.Windows.Forms.Label lblCassetteId;
        private System.Windows.Forms.TextBox _txtCassetteId;
        private System.Windows.Forms.Label lblToolingId;
        private System.Windows.Forms.TextBox _txtToolingId;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.TextBox _txtQty;
        private System.Windows.Forms.Label lblPanelList;
        private System.Windows.Forms.TextBox _txtPanelList;
        private System.Windows.Forms.GroupBox _grpActions;
        private System.Windows.Forms.FlowLayoutPanel flowActions;
        private System.Windows.Forms.Button btnUser;
        private System.Windows.Forms.Button btnTrackIn;
        private System.Windows.Forms.Button btnTrackOut;
        private System.Windows.Forms.Button btnPanelIn;
        private System.Windows.Forms.Button btnPanelOut;
        private System.Windows.Forms.Button btnRtnTraceData;
        private System.Windows.Forms.Button btnAlarm;
        private System.Windows.Forms.Button _btnAutoTest;
        private System.Windows.Forms.CheckBox chkPPSelectPass;
        private System.Windows.Forms.Panel pnlStatusReport;
        private System.Windows.Forms.ComboBox _cmbStatusType;
        private System.Windows.Forms.ComboBox _cmbStatus;
        private System.Windows.Forms.Button btnReportStatus;
        private System.Windows.Forms.Panel pnlNGGroup;
        private System.Windows.Forms.Label lblNGCode;
        private System.Windows.Forms.ComboBox _cboNGCode;
        private System.Windows.Forms.TextBox _txtNGName;
        private System.Windows.Forms.TextBox _txtNGQty;
        private System.Windows.Forms.TextBox _txtNGUnit;
        private System.Windows.Forms.Button btnSendNGCode;
        private System.Windows.Forms.Button btnSendAllNG;
        private System.Windows.Forms.Panel pnlLeftoverGroup;
        private System.Windows.Forms.Label lblLeftoverReel;
        private System.Windows.Forms.TextBox _txtLeftoverReel;
        private System.Windows.Forms.Label lblLeftoverQty;
        private System.Windows.Forms.TextBox _txtLeftoverQty;
        private System.Windows.Forms.Button btnSendLeftover;
        private System.Windows.Forms.Button btnSendUseLeftover;
        private System.Windows.Forms.Panel pnlTotalCountGroup;
        private System.Windows.Forms.Label lblTotalReelCount;
        private System.Windows.Forms.TextBox _txtTotalReelCount;
        private System.Windows.Forms.Label lblTotalQtyPerReel;
        private System.Windows.Forms.TextBox _txtTotalQtyPerReel;
        private System.Windows.Forms.Button btnSendTotalCount;
        private System.Windows.Forms.Panel pnlScenarioGroup;
        private System.Windows.Forms.Label lblScenario;
        private System.Windows.Forms.ComboBox _cboScenario;
        private System.Windows.Forms.Button btnApplyScenario;
        private System.Windows.Forms.TextBox _txtLog;
    }
}
