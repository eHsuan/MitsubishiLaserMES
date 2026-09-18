using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EQSimulator
{
    public class EqpDictionary
    {
        public string Model { get; set; }
        public Dictionary<string, string> Events { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ManagementItems { get; set; } = new Dictionary<string, string>();

        public string GetManagementItemValue(string inputCode)
        {
            if (string.IsNullOrWhiteSpace(inputCode) || ManagementItems == null) return null;
            string trimmed = inputCode.Trim();
            foreach (var kvp in ManagementItems)
            {
                if (kvp.Key.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }
            return null;
        }

        public static EqpDictionary Load(string eqId)
        {
            try 
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string[] candidatePaths = new[]
                {
                    Path.Combine(baseDir, "Dictionaries", $"{eqId}.json"),
                    Path.Combine(baseDir, "Dictionaries", "MQTT_Standard.json"),
                    Path.Combine(baseDir, "..", "..", "..", "..", "Dictionaries", $"{eqId}.json"),
                    Path.Combine(baseDir, "..", "..", "..", "..", "Dictionaries", "MQTT_Standard.json")
                };

                foreach (var path in candidatePaths)
                {
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        var dict = JsonConvert.DeserializeObject<EqpDictionary>(json);
                        if (dict != null) return dict;
                    }
                }
            } catch { }
            return new EqpDictionary();
        }

        public string GetCeid(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName)) return eventName ?? "";
            string trimmed = eventName.Trim();
            if (Events != null)
            {
                foreach (var kvp in Events)
                {
                    if (kvp.Value.Equals(trimmed, StringComparison.OrdinalIgnoreCase)) return kvp.Key;
                }
                if (Events.ContainsKey(trimmed)) return trimmed;
            }
            return eventName;
        }

        public string GetVid(string varName)
        {
            if (string.IsNullOrWhiteSpace(varName)) return varName ?? "";
            string trimmed = varName.Trim();
            if (Variables != null)
            {
                foreach (var kvp in Variables)
                {
                    if (kvp.Value.Equals(trimmed, StringComparison.OrdinalIgnoreCase)) return kvp.Key;
                }
                if (Variables.ContainsKey(trimmed)) return trimmed;
            }
            return varName;
        }
        
        public string GetVarName(string vid)
        {
            if (string.IsNullOrWhiteSpace(vid)) return vid ?? "";
            string trimmed = vid.Trim();
            if (Variables != null && Variables.TryGetValue(trimmed, out var name))
                return name;
            return vid;
        }
    }
}
