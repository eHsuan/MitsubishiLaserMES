using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Protocol.Core.Enums;
using Protocol.Core.Messages;

namespace EQSimulator
{
    public class AlarmItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override string ToString() => $"[{Code}] {Name} - {Description}";
    }

    public class AlarmTestForm : Form
    {
        private readonly Action<AlarmMessage> _sendAlarmAction;
        private readonly List<AlarmItem> _presetAlarms;
        private readonly List<AlarmItem> _activeAlarms = new List<AlarmItem>();

        private ComboBox _cmbPresetAlarms;
        private ListBox _lstActiveAlarms;
        private Button _btnTrigger;
        private Button _btnResetSelected;
        private Button _btnResetAll;
        private Label _lblStatus;

        public AlarmTestForm(Action<AlarmMessage> sendAlarmAction)
        {
            _sendAlarmAction = sendAlarmAction;
            _presetAlarms = new List<AlarmItem>
            {
                new AlarmItem { Code = "A001", Name = "Emergency Stop", Description = "設備緊急停止 (Emergency Stop Triggered)" },
                new AlarmItem { Code = "A002", Name = "Safety Door Open", Description = "安全防護門開啟 (Safety Guard Open)" },
                new AlarmItem { Code = "A003", Name = "Heater Over Temp", Description = "加熱槽溫度超上限 (Heater Over Temperature)" },
                new AlarmItem { Code = "A004", Name = "Air Low Pressure", Description = "主氣壓壓力不足 (Main Air Low Pressure)" },
                new AlarmItem { Code = "A005", Name = "Motor Overload", Description = "馬達過載保護 (Motor Driver Overload)" },
                new AlarmItem { Code = "A006", Name = "Robot Arm Error", Description = "搬運手臂定位異常 (Robot Arm Position Error)" },
                new AlarmItem { Code = "A007", Name = "Vacuum Pump Error", Description = "真空幫浦壓力異常 (Vacuum Pump Abnormal)" },
                new AlarmItem { Code = "A008", Name = "Conveyor Jammed", Description = "傳送軌道卡料 (Conveyor Belt Jammed)" },
                new AlarmItem { Code = "A009", Name = "Tank Level Low", Description = "化學液位過低 (Chemical Tank Level Low)" },
                new AlarmItem { Code = "A010", Name = "Mold Position Error", Description = "模具到位感測異常 (Mold Position Sensor Timeout)" },
            };

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "EQ Simulator - Alarm Test Manager";
            this.Size = new Size(650, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            var lblPreset = new Label
            {
                Text = "選擇預設 ALARM 清單 (10 組內建):",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Microsoft JhengHei", 9.5F, FontStyle.Bold)
            };

            _cmbPresetAlarms = new ComboBox
            {
                Location = new Point(20, 45),
                Size = new Size(420, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft JhengHei", 9F)
            };
            foreach (var item in _presetAlarms) _cmbPresetAlarms.Items.Add(item);
            if (_cmbPresetAlarms.Items.Count > 0) _cmbPresetAlarms.SelectedIndex = 0;

            _btnTrigger = new Button
            {
                Text = "▶ 發出告警 (Start)",
                Location = new Point(455, 43),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnTrigger.Click += BtnTrigger_Click;

            var lblActive = new Label
            {
                Text = "目前觸發中告警 (Active Alarms List):",
                Location = new Point(20, 95),
                AutoSize = true,
                Font = new Font("Microsoft JhengHei", 9.5F, FontStyle.Bold)
            };

            _lstActiveAlarms = new ListBox
            {
                Location = new Point(20, 120),
                Size = new Size(420, 260),
                Font = new Font("Consolas", 9.5F),
                SelectionMode = SelectionMode.One
            };

            _btnResetSelected = new Button
            {
                Text = "✓ 解除所選告警 (Reset)",
                Location = new Point(455, 120),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnResetSelected.Click += BtnResetSelected_Click;

            _btnResetAll = new Button
            {
                Text = "🧹 全部解除 (Reset All)",
                Location = new Point(455, 175),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft JhengHei", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnResetAll.Click += BtnResetAll_Click;

            _lblStatus = new Label
            {
                Text = "提示：可同時發出多個告警，選擇清單中的告警可獨立 Reset。",
                Location = new Point(20, 395),
                Size = new Size(585, 30),
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Microsoft JhengHei", 9F)
            };

            this.Controls.Add(lblPreset);
            this.Controls.Add(_cmbPresetAlarms);
            this.Controls.Add(_btnTrigger);
            this.Controls.Add(lblActive);
            this.Controls.Add(_lstActiveAlarms);
            this.Controls.Add(_btnResetSelected);
            this.Controls.Add(_btnResetAll);
            this.Controls.Add(_lblStatus);
        }

        private void BtnTrigger_Click(object sender, EventArgs e)
        {
            var selected = _cmbPresetAlarms.SelectedItem as AlarmItem;
            if (selected == null) return;

            if (_activeAlarms.Any(x => x.Code == selected.Code))
            {
                MessageBox.Show($"告警 [{selected.Code}] 目前已經在觸發中清單內！", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var msg = new AlarmMessage
            {
                CMD = CommandType.AlarmReport,
                AlarmStatus = "Start",
                AlarmType = "A",
                AlarmCode = selected.Code,
                AlarmMsg = selected.Description
            };

            _sendAlarmAction?.Invoke(msg);

            _activeAlarms.Add(selected);
            RefreshActiveList();
            _lblStatus.Text = $"已發出告警: [{selected.Code}] {selected.Description}";
        }

        private void BtnResetSelected_Click(object sender, EventArgs e)
        {
            if (_lstActiveAlarms.SelectedItem == null)
            {
                MessageBox.Show("請先從「目前觸發中告警」列表中選擇要解除的告警！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string itemText = _lstActiveAlarms.SelectedItem.ToString();
            var target = _activeAlarms.FirstOrDefault(x => itemText.Contains($"[{x.Code}]"));
            if (target == null) return;

            var msg = new AlarmMessage
            {
                CMD = CommandType.AlarmReport,
                AlarmStatus = "End",
                AlarmType = "A",
                AlarmCode = target.Code,
                AlarmMsg = $"{target.Description} Clear"
            };

            _sendAlarmAction?.Invoke(msg);

            _activeAlarms.Remove(target);
            RefreshActiveList();
            _lblStatus.Text = $"已解除告警: [{target.Code}] {target.Description}";
        }

        private void BtnResetAll_Click(object sender, EventArgs e)
        {
            if (_activeAlarms.Count == 0) return;

            var copy = _activeAlarms.ToList();
            foreach (var item in copy)
            {
                var msg = new AlarmMessage
                {
                    CMD = CommandType.AlarmReport,
                    AlarmStatus = "End",
                    AlarmType = "A",
                    AlarmCode = item.Code,
                    AlarmMsg = $"{item.Description} Clear"
                };
                _sendAlarmAction?.Invoke(msg);
            }

            _activeAlarms.Clear();
            RefreshActiveList();
            _lblStatus.Text = "已重置並解除所有觸發中告警。";
        }

        private void RefreshActiveList()
        {
            _lstActiveAlarms.Items.Clear();
            foreach (var item in _activeAlarms)
            {
                _lstActiveAlarms.Items.Add($"[{item.Code}] {item.Name} ({DateTime.Now:HH:mm:ss})");
            }
        }
    }
}
