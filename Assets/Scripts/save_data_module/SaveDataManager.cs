using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace versoft.save_data
{
    public class SaveDataManager
    {
        public bool SaveData<T>(T objectToSave)
        {
            if (objectToSave == null)
            { return false; }

            var jsonString = JsonUtility.ToJson(objectToSave, true);

            if (string.IsNullOrEmpty(jsonString))
            { return false; }

#if !DEBUG || ENCRYPT_SAVE
        // Here is where we will encrypt the save.
#endif
            var path = Path.Combine(Application.persistentDataPath, "save.bin");
            using (var streamWriter = new StreamWriter(path, false))
            {
                streamWriter.Write(jsonString);
                streamWriter.Close();

                UnityEngine.Debug.LogError($"path: {path}");
            }

            return true;
        }

        public T LoadData<T>()
        {
            var path = Path.Combine(Application.persistentDataPath, "save.bin");
            if (!File.Exists(path))
            {
                return default;
            }

            using (var streamReader = new StreamReader(path))
            {
                var json = streamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        T saveData = JsonUtility.FromJson<T>(json);
                        return saveData;
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError($"Found an error: {e.Message}");
                        return default;
                    }
                }
            }

            return default;
        }
    }
}
