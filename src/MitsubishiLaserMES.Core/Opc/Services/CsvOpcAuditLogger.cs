using System;
using System.Globalization;
using System.IO;
using System.Text;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Models;

namespace MitsubishiLaserOpc.Services
{
    /// <summary>
    /// Appends real-machine communication records to one UTF-8 CSV file per day.
    /// Files are never deleted by this library.
    /// </summary>
    public sealed class CsvOpcAuditLogger : IOpcAuditLogger
    {
        private const string Header = "Time,Direction,Node,Tag,RequestValue,ResponseValue,StatusCode,Result,Message";
        private readonly object _writeLock = new object();
        private readonly string _logDirectory;

        public CsvOpcAuditLogger(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentException("Log directory is required.", "logDirectory");

            _logDirectory = logDirectory;
        }

        public string CurrentLogFilePath
        {
            get
            {
                return Path.Combine(
                    _logDirectory,
                    DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    "MitsubishiOpcUa_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".csv");
            }
        }

        public void Write(OpcAuditLogEntry entry)
        {
            // Audit logging must never interrupt equipment communication.
            try
            {
                lock (_writeLock)
                {
                    string filePath = CurrentLogFilePath;
                    string directory = Path.GetDirectoryName(filePath);
                    Directory.CreateDirectory(directory);

                    bool newFile = !File.Exists(filePath);
                    using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true)))
                    {
                        if (newFile) writer.WriteLine(Header);
                        writer.WriteLine(ToCsvLine(entry));
                    }
                }
            }
            catch
            {
                // The caller still receives the original OPC UA result even if the disk is unavailable.
            }
        }

        private static string ToCsvLine(OpcAuditLogEntry entry)
        {
            return string.Join(",", new[]
            {
                Escape(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                Escape(entry.Direction),
                Escape(entry.Node),
                Escape(entry.Tag),
                Escape(FormatValue(entry.RequestValue)),
                Escape(FormatValue(entry.ResponseValue)),
                Escape(entry.StatusCode),
                Escape(entry.Succeeded ? "Success" : "Failure"),
                Escape(entry.Message),
            });
        }

        private static string FormatValue(object value)
        {
            return value == null ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            string text = value ?? string.Empty;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }
    }
}
