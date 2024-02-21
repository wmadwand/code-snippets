using System;
using UnityEngine;

namespace Project.Utils
{
    public class PrefsStorage 
    {
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public T Deserialize<T>(string key) where T : class
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return null;
            }

            var json = PlayerPrefs.GetString(key);
            T res = null;
            try
            {
                res = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return res;
        }

        public void Serialize<T>(string key, T obj)
        {
            var json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public void DeleteKey(string value)
        {
            PlayerPrefs.DeleteKey(value);
        }
    }
}