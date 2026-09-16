using System;
using System.Drawing;
using System.Windows.Forms;

namespace MitsubishiLaserMES.WinForms.Forms
{
    public class TerminalMessageForm : Form
    {
        public TerminalMessageForm(string message)
        {
            Text = "EAP 系統下發訊息 (Terminal Display)";
            Size = new Size(460, 240);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true;

            var lblTitle = new Label
            {
                Text = "上位 EAP 廣播訊息通知",
                Font = new Font("微軟正黑體", 12F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 15),
                Size = new Size(400, 25)
            };

            var txtContent = new TextBox
            {
                Text = message,
                Font = new Font("微軟正黑體", 11F),
                Location = new Point(20, 50),
                Size = new Size(405, 90),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            var btnOk = new Button
            {
                Text = "確認 (OK)",
                Font = new Font("微軟正黑體", 10F),
                Location = new Point(170, 155),
                Size = new Size(100, 35),
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblTitle);
            Controls.Add(txtContent);
            Controls.Add(btnOk);
            AcceptButton = btnOk;
        }
    }
}
