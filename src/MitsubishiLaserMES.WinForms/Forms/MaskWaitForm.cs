using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MitsubishiLaserMES.WinForms.Forms
{
    public class MaskWaitForm : Form
    {
        private readonly Label _lblMessage;
        private readonly ProgressBar _progressBar;

        public MaskWaitForm(string message = "系統處理中，請稍候...")
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            ShowInTaskbar = false;
            Size = new Size(380, 150);
            Text = "作業處理中";

            _lblMessage = new Label
            {
                Text = message,
                Font = new Font("微軟正黑體", 11F, FontStyle.Bold),
                Location = new Point(20, 25),
                Size = new Size(330, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(40, 65),
                Size = new Size(290, 24)
            };

            Controls.Add(_lblMessage);
            Controls.Add(_progressBar);
        }

        public static async Task<T> RunWithWaitAsync<T>(Form parent, string message, Func<Task<T>> action)
        {
            using var form = new MaskWaitForm(message);
            Task<T> actionTask = action();

            form.Shown += async (s, e) =>
            {
                try
                {
                    await actionTask;
                }
                catch { }
                finally
                {
                    if (!form.IsDisposed)
                    {
                        form.Close();
                    }
                }
            };

            form.ShowDialog(parent);
            return await actionTask;
        }
    }
}
