using System;
using System.Drawing;
using System.Windows.Forms;
using MitsubishiLaserMES.Core.Models.Config;

namespace MitsubishiLaserMES.WinForms.Forms
{
    public class SettingsForm : Form
    {
        private readonly AppConfig _config;

        private TextBox txtMqttServer;
        private NumericUpDown numMqttPort;
        private TextBox txtClientId;
        private TextBox txtFactory;
        private TextBox txtFloor;
        private TextBox txtArea;
        private TextBox txtEqId;
        private NumericUpDown numTimeoutT1;

        private CheckBox chkUseVirtual;
        private TextBox txtOpcEndpoint;
        private CheckBox chkOpcSecurity;

        public SettingsForm() : this(new AppConfig())
        {
        }

        public SettingsForm(AppConfig config)
        {
            _config = config ?? new AppConfig();
            InitializeComponent();
            LoadConfigToUI();
        }

        private void InitializeComponent()
        {
            Text = "MES 與機台通訊設定";
            Size = new Size(540, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("微軟正黑體", 9.5F);

            // Group 1: MQTT EAP 設定
            var grpMqtt = new GroupBox
            {
                Text = "上位 EAP 通訊設定 (MQTT)",
                Location = new Point(15, 15),
                Size = new Size(495, 250)
            };

            var lblServer = new Label { Text = "Broker 位址:", Location = new Point(15, 30), AutoSize = true };
            txtMqttServer = new TextBox { Location = new Point(110, 27), Width = 180 };

            var lblPort = new Label { Text = "Port:", Location = new Point(310, 30), AutoSize = true };
            numMqttPort = new NumericUpDown { Location = new Point(360, 27), Width = 110, Maximum = 65535, Minimum = 1 };

            var lblClient = new Label { Text = "Client ID:", Location = new Point(15, 65), AutoSize = true };
            txtClientId = new TextBox { Location = new Point(110, 62), Width = 360 };

            var lblFactory = new Label { Text = "廠區(Factory):", Location = new Point(15, 100), AutoSize = true };
            txtFactory = new TextBox { Location = new Point(110, 97), Width = 120 };

            var lblFloor = new Label { Text = "樓層(Floor):", Location = new Point(255, 100), AutoSize = true };
            txtFloor = new TextBox { Location = new Point(350, 97), Width = 120 };

            var lblArea = new Label { Text = "區域(Area):", Location = new Point(15, 135), AutoSize = true };
            txtArea = new TextBox { Location = new Point(110, 132), Width = 120 };

            var lblEqId = new Label { Text = "機台(EqID):", Location = new Point(255, 135), AutoSize = true };
            txtEqId = new TextBox { Location = new Point(350, 132), Width = 120 };

            var lblT1 = new Label { Text = "T1逾時(秒):", Location = new Point(15, 175), AutoSize = true };
            numTimeoutT1 = new NumericUpDown { Location = new Point(110, 172), Width = 120, Minimum = 5, Maximum = 120 };

            var lblHint = new Label
            {
                Text = "發送Topic: {Factory}/{Floor}/{Area}/{EqID}/Report\n接收Topic: {Factory}/{Floor}/{Area}/{EqID}/Command",
                Location = new Point(15, 205),
                Size = new Size(465, 35),
                ForeColor = Color.DarkSlateGray
            };

            grpMqtt.Controls.AddRange(new Control[]
            {
                lblServer, txtMqttServer, lblPort, numMqttPort,
                lblClient, txtClientId,
                lblFactory, txtFactory, lblFloor, txtFloor,
                lblArea, txtArea, lblEqId, txtEqId,
                lblT1, numTimeoutT1, lblHint
            });

            // Group 2: 三菱雷射機 OPC 設定
            var grpOpc = new GroupBox
            {
                Text = "下位三菱雷射機通訊設定 (OPC UA)",
                Location = new Point(15, 275),
                Size = new Size(495, 140)
            };

            chkUseVirtual = new CheckBox
            {
                Text = "啟用虛擬機台模擬器 (Virtual Simulator / 離線測試模式)",
                Location = new Point(15, 28),
                AutoSize = true,
                Font = new Font("微軟正黑體", 9.5F, FontStyle.Bold)
            };

            var lblEndpoint = new Label { Text = "Endpoint URL:", Location = new Point(15, 65), AutoSize = true };
            txtOpcEndpoint = new TextBox { Location = new Point(125, 62), Width = 345 };

            chkOpcSecurity = new CheckBox
            {
                Text = "使用安全加密連線 (Use Security)",
                Location = new Point(125, 98),
                AutoSize = true
            };

            grpOpc.Controls.AddRange(new Control[]
            {
                chkUseVirtual, lblEndpoint, txtOpcEndpoint, chkOpcSecurity
            });

            // 按鈕
            var btnSave = new Button
            {
                Text = "儲存設定 (Save)",
                Location = new Point(150, 430),
                Size = new Size(110, 36),
                DialogResult = DialogResult.OK
            };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "取消 (Cancel)",
                Location = new Point(280, 430),
                Size = new Size(100, 36),
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(grpMqtt);
            Controls.Add(grpOpc);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void LoadConfigToUI()
        {
            txtMqttServer.Text = _config.Mqtt.Server;
            numMqttPort.Value = _config.Mqtt.Port;
            txtClientId.Text = _config.Mqtt.ClientId;
            txtFactory.Text = _config.Mqtt.Factory;
            txtFloor.Text = _config.Mqtt.Floor;
            txtArea.Text = _config.Mqtt.Area;
            txtEqId.Text = _config.Mqtt.EqID;
            numTimeoutT1.Value = _config.Mqtt.TimeoutT1Ms / 1000;

            chkUseVirtual.Checked = _config.Opc.UseVirtualSimulator;
            txtOpcEndpoint.Text = _config.Opc.EndpointUrl;
            chkOpcSecurity.Checked = _config.Opc.UseSecurity;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _config.Mqtt.Server = txtMqttServer.Text.Trim();
            _config.Mqtt.Port = (int)numMqttPort.Value;
            _config.Mqtt.ClientId = txtClientId.Text.Trim();
            _config.Mqtt.Factory = txtFactory.Text.Trim();
            _config.Mqtt.Floor = txtFloor.Text.Trim();
            _config.Mqtt.Area = txtArea.Text.Trim();
            _config.Mqtt.EqID = txtEqId.Text.Trim();
            _config.Mqtt.TimeoutT1Ms = (int)numTimeoutT1.Value * 1000;

            _config.Opc.UseVirtualSimulator = chkUseVirtual.Checked;
            _config.Opc.EndpointUrl = txtOpcEndpoint.Text.Trim();
            _config.Opc.UseSecurity = chkOpcSecurity.Checked;

            ConfigHelper.SaveConfig(_config);
            Close();
        }
    }
}
