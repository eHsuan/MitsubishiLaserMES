namespace MitsubishiLaserMES.Core.Logging
{
    /// <summary>
    /// 日誌維運與分卷組態設定
    /// </summary>
    public class LogSettings
    {
        /// <summary>
        /// 是否啟用日誌維運排程（壓縮與過期清除）
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 超過此天數的歷史日誌資料夾自動壓縮為 .zip (預設 3 天)
        /// </summary>
        public int CompressDays { get; set; } = 3;

        /// <summary>
        /// 超過此天數的 .zip 或資料夾自動刪除 (預設 30 天)
        /// </summary>
        public int RetentionDays { get; set; } = 30;

        /// <summary>
        /// 單一小時文字檔上限 (MB，預設 20 MB)
        /// </summary>
        public int MaxFileSizeMB { get; set; } = 20;
    }
}
