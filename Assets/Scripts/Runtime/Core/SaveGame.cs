using System;
using System.IO;
using UnityEngine;

namespace Runtime.Core
{
    [Serializable]
    public class SaveData
    {
        public float masterVolume = 1f;
        public Vector2 darknessPosition = new Vector2(0, 0);
        public int lightBugs = 0;
        public Vector2 playerPosition = new Vector2(0, 0);
        public bool fallTreeIsDown = true;
    }

    public class SaveStorage
    {
        private const string FileName = "save.json";

        private static string PathToFile => Path.Combine(Application.persistentDataPath, FileName);

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string dir = Path.GetDirectoryName(PathToFile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(PathToFile, json);
        }

        public static SaveData Load()
        {
            if (!File.Exists(PathToFile))
                return new SaveData(); // defaults

            string json = File.ReadAllText(PathToFile);
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        public static bool HasSave() => File.Exists(PathToFile);

        public static void DeleteSave()
        {
            if (File.Exists(PathToFile)) File.Delete(PathToFile);
        }
    }
}