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
        private readonly int _slot;

        public int Slot => _slot;

        public SaveStorage(int slot)
        {
            _slot = Mathf.Max(1, slot);
        }

        private string PathToFile =>
            Path.Combine(Application.persistentDataPath, $"save_slot_{_slot}.json");

        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string dir = Path.GetDirectoryName(PathToFile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string tempPath = PathToFile + ".tmp";
            string backupPath = PathToFile + ".old";

            File.WriteAllText(tempPath, json);

            if (File.Exists(PathToFile))
            {
                try
                {
                    File.Replace(tempPath, PathToFile, backupPath, true);
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                }
                catch
                {
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Move(PathToFile, backupPath);
                    File.Move(tempPath, PathToFile);
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                }
            }
            else
            {
                File.Move(tempPath, PathToFile);
            }
        }

        public SaveData Load()
        {
            if (!File.Exists(PathToFile))
                return new SaveData(); // defaults

            string json = File.ReadAllText(PathToFile);
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        public bool HasSave() => File.Exists(PathToFile);

        public void DeleteSave()
        {
            if (File.Exists(PathToFile)) File.Delete(PathToFile);
            if (File.Exists(PathToFile + ".old")) File.Delete(PathToFile + ".old");
            if (File.Exists(PathToFile + ".tmp")) File.Delete(PathToFile + ".tmp");
        }
    }
}
