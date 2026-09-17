using System;
using System.Windows.Forms;
using MitsubishiLaserMES.Core.Logging;
using MitsubishiLaserMES.Core.Services.Coordination;
using MitsubishiLaserMES.Core.Services.Eap;
using MitsubishiLaserMES.Core.Services.Opc;
using MitsubishiLaserMES.WinForms.Forms;

namespace MitsubishiLaserMES.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 全域未捕捉例外防護
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                LogService.Instance.Fatal("GlobalException", "應用程式執行緒發生非預期例外", "System", e.Exception);
                MessageBox.Show($"應用程式執行緒發生例外：\n{e.Exception.Message}", "非預期錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    LogService.Instance.Fatal("GlobalException", "應用程式網域發生未處理例外", "System", ex);
                    MessageBox.Show($"應用程式發生未處理例外：\n{ex.Message}", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // 讀取設定
            var config = ConfigHelper.LoadConfig();

            // 初始化日誌服務與背景維運服務
            LogService.Instance.UpdateSettings(config.LogSettings);
            LogService.Instance.DefaultEquipmentId = config.Mqtt?.EqID ?? "PL001";
            using var maintenanceService = new LogMaintenanceService(config.LogSettings);

            LogService.Instance.Info("System", "==== 三菱雷射鑽孔機 MES 中繼軟體啟動 ====");

            // 建立服務實例
            var bufferService = new OfflineBufferService();
            var eapService = new EapMqttService(config.Mqtt, bufferService);
            var opcService = new MitsubishiOpcService(config.Opc);
            using var coordinator = new MesCoordinator(config, eapService, opcService, LogService.Instance);

            // 啟動主視窗
            Application.Run(new MainForm(coordinator, config));

            LogService.Instance.Info("System", "==== 三菱雷射鑽孔機 MES 中繼軟體正常結束 ====");
        }
    }
}