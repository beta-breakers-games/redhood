using System;
using System.IO;
using UnityEngine;

namespace Runtime.Core
{
    [Serializable]
    public class GlobalConfig
    {
        public int masterVolume = 1;
        public int musicVolume = 1;
        public int activeSlot = 1;
    }

    public static class GlobalConfigStorage
    {
        private static string PathToFile =>
            Path.Combine(Application.persistentDataPath, "config.json");

        public static GlobalConfig Load()
        {
            if (!File.Exists(PathToFile)) return new GlobalConfig();
            var json = File.ReadAllText(PathToFile);
            return JsonUtility.FromJson<GlobalConfig>(json) ?? new GlobalConfig();
        }

        public static void Save(GlobalConfig cfg)
        {
            var json = JsonUtility.ToJson(cfg, true);
            File.WriteAllText(PathToFile, json);
        }
    }
}