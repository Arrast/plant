using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace versoft.data_model
{
    public class DataModelDatabase
    {
        private readonly int columnsBeforeDataStarts = 2;

        private Dictionary<Type, Dictionary<string, object>> storedData = new Dictionary<Type, Dictionary<string, object>>();

        public DataModelDatabase()
        {
            ReadAllFiles();
        }

        public void ReadAllFiles()
        {
            string path = Const.GetDataFilePath();
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, $"*{Const.DataFileExtension}");
                if (files.Length == 0)
                {
                    Debug.LogError($"{path} does not contain any files with the extension {Const.DataFileExtension}");
                    return;
                }

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    ParseFile(fileName);
                }
            }
            else
            {
                Debug.LogError($"{path} does not currently exist.");
            }
        }

        private void ParseFile(string className)
        {
            string path = Const.GetDataFilePath();
            if (Directory.Exists(path))
            {
                try
                {
                    // Open the file and store the data into an array of arrays.
                    // It's an array of lines, and each line is an array of cells.
                    string fileName = Path.Combine(path, className + Const.DataFileExtension);
                    List<string[]> lines = new List<string[]>();
                    using (StreamReader stream = new StreamReader(fileName))
                    {
                        while (!stream.EndOfStream)
                        {
                            string readLine = stream.ReadLine();
                            string[] splitLine = Const.SplitLineFromCSV(readLine);
                            lines.Add(splitLine);
                        }

                    }
                    string[][] data = lines.ToArray();

                    // Now start filling the dictionaries.
                    var otherType = Type.GetType(className);
                    if (otherType != null && data != null && data.Length > columnsBeforeDataStarts)
                    {
                        if (!storedData.ContainsKey(otherType))
                        {
                            storedData.Add(otherType, new Dictionary<string, object>());
                        }

                        // We don't read the first rows because those are meant to be used to specify types.
                        int numFields = data[0].Length;
                        for (int i = columnsBeforeDataStarts; i < data.Length; i++)
                        {
                            var instance = Activator.CreateInstance(otherType);
                            string key = string.Empty;
                            for (int j = 0; j < numFields; j++)
                            {
                                var rawValue = data[i][j];
                                var property = instance.GetType().GetField(data[0][j]);
                                if (property != null)
                                {
                                    Type t = Nullable.GetUnderlyingType(property.FieldType) ?? property.FieldType;
                                    object castedValue = CastValueToType(t, rawValue);
                                    property.SetValue(instance, castedValue);

                                    // Let's save the primary key, which should be the first in the datamodel.
                                    if (j == 0 && t == typeof(string))
                                    {
                                        key = rawValue;
                                    }
                                }
                            }

                            if (key != string.Empty)
                            {
                                storedData[otherType].Add(key, instance);
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message);
                }
            }
        }

        private object CastValueToType(Type t, string rawValue)
        {
            object castedValue = null;
            if (typeof(IEnumerable).IsAssignableFrom(t) && !t.IsAssignableFrom(typeof(string)))
            {
                castedValue = CastArrayOrList(t, rawValue);
            }
            else if (t.IsEnum)
            {
                castedValue = Enum.Parse(t, rawValue);
            }
            else if(!t.IsClass || t.IsSealed)
            {
                castedValue = (rawValue == null) ? null : Convert.ChangeType(rawValue, t);
            }
            else
            {
                castedValue = JsonUtility.FromJson(rawValue, t);
            }

            return castedValue;
        }

        private IList CastArrayOrList(Type t, string rawValue)
        {
            if (typeof(IEnumerable).IsAssignableFrom(t) && !string.IsNullOrEmpty(rawValue))
            {
                string[] split = rawValue.Split(Const.DelimiterForArrays);
                if (t.IsGenericType)
                {
                    Type containedType = t.GenericTypeArguments.First();
                    object instance = Activator.CreateInstance(t);
                    IList genericList = (IList)instance;
                    for (int index = 0; index < split.Length; index++)
                    {
                        object castedValue = CastValueToType(containedType, split[index]);
                        genericList.Add(castedValue);
                    }
                    return genericList;
                }
                else if (t.IsArray)
                {
                    Type containedType = t.GetElementType();
                    object instance = Array.CreateInstance(containedType, split.Length);
                    IList genericList = (IList)instance;
                    for (int index = 0; index < split.Length; index++)
                    {
                        object castedValue = CastValueToType(containedType, split[index]);
                        genericList[index] = castedValue;
                    }
                    return genericList;
                }
            }

            return null;
        }

        public List<T> GetList<T>()
        {
            if (storedData.TryGetValue(typeof(T), out var dictionary))
            {
                if (dictionary == null) { return null; }

                return dictionary.Values.Cast<T>().ToList();
            }
            return null;
        }

        public T Get<T>(string itemId)
        {
            if (storedData.TryGetValue(typeof(T), out var dictionary))
            {
                if (dictionary != null && dictionary.TryGetValue(itemId, out var item))
                {
                    return (T)item;
                }
            }
            return default(T);
        }

    }
}