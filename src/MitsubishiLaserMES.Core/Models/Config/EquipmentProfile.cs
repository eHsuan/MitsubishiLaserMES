using System;
using System.Collections.Generic;
using System.Linq;

namespace MitsubishiLaserMES.Core.Models.Config
{
    /// <summary>
    /// 機台通訊字典模型 (對應 Dictionaries/DR0026.json)
    /// 定義事件 CEID、變數 VID、狀態碼與警報碼之動態映射關係
    /// </summary>
    public class EquipmentProfile
    {
        public string Model { get; set; } = "DR0026";

        public Dictionary<string, string> Events { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> StatusMapping { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> AlarmMapping { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> ManagementItems { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 依據事件名稱 (如 USER_VERIFY) 反查事件代碼 CEID (如 104)
        /// </summary>
        public string GetEventId(string eventName, string defaultVal = "5000")
        {
            if (string.IsNullOrWhiteSpace(eventName)) return defaultVal;

            // 支援不區分大小寫與底線的彈性比對
            string normalized = eventName.Replace("_", "").Replace(" ", "");
            var match = Events.FirstOrDefault(kv =>
                kv.Value.Equals(eventName, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Replace("_", "").Replace(" ", "").Equals(normalized, StringComparison.OrdinalIgnoreCase));

            return match.Key ?? defaultVal;
        }

        /// <summary>
        /// 依據事件代碼 (如 104) 查詢事件名稱 (如 USER_VERIFY)
        /// </summary>
        public string GetEventName(string eventId, string defaultVal = null)
        {
            if (!string.IsNullOrWhiteSpace(eventId) && Events.TryGetValue(eventId, out var name))
            {
                return name;
            }
            return defaultVal ?? eventId;
        }

        /// <summary>
        /// 依據變數名稱 (如 User_Barcode) 反查變數代碼 VID (如 1028)
        /// </summary>
        public string GetVariableId(string variableName, string defaultVal = "1023")
        {
            if (string.IsNullOrWhiteSpace(variableName)) return defaultVal;

            string normalized = variableName.Replace("_", "").Replace(" ", "");
            var match = Variables.FirstOrDefault(kv =>
                kv.Value.Equals(variableName, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Replace("_", "").Replace(" ", "").Equals(normalized, StringComparison.OrdinalIgnoreCase));

            return match.Key ?? defaultVal;
        }

        /// <summary>
        /// 依據變數代碼 (如 1028) 查詢變數名稱 (如 User_Barcode)
        /// </summary>
        public string GetVariableName(string variableId, string defaultVal = null)
        {
            if (!string.IsNullOrWhiteSpace(variableId) && Variables.TryGetValue(variableId, out var name))
            {
                return name;
            }
            return defaultVal ?? variableId;
        }

        /// <summary>
        /// 依據機台狀態代碼查詢文字說明 (如 0 -> Down, 3 -> Running)
        /// </summary>
        public string GetStatusText(string statusCode, string defaultVal = null)
        {
            if (!string.IsNullOrWhiteSpace(statusCode) && StatusMapping.TryGetValue(statusCode, out var text))
            {
                return text;
            }
            return defaultVal ?? statusCode;
        }

        /// <summary>
        /// 依據警報代碼查詢警報說明 (如 E001 -> ALM_SAFETY_DOOR_OPEN)
        /// </summary>
        public string GetAlarmMessage(string alarmCode, string defaultVal = null)
        {
            if (!string.IsNullOrWhiteSpace(alarmCode) && AlarmMapping.TryGetValue(alarmCode, out var msg))
            {
                return msg;
            }
            return defaultVal ?? alarmCode;
        }
    }
}
