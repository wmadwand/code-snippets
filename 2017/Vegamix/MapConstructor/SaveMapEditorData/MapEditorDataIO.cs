using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MapConstructor
{
    public abstract class MapEditorDataIO<T1, T2> where T2 : IMapEditorData, new() where T1 : MapEditorDataIO<T1, T2>, new()
    {
        private string filePath;
        protected Dictionary<string, T2> dict = new Dictionary<string, T2>();

        public static void Run(string _filePath)
        {
            T1 t1 = new T1();
            t1.Process(_filePath);
        }

        protected void Process(string _filePath)
        {
            filePath = _filePath;

            if (File.Exists(filePath))
            {
                Load();
            }

            Collect();
            Save();
        }

        protected void Load()
        {
            string dataAsJson = null;

            try
            {
                dataAsJson = File.ReadAllText(filePath);
            }
            catch
            {
                Debug.LogErrorFormat("{0} not read due to error(s).", filePath);
            }

            if (!string.IsNullOrEmpty(dataAsJson))
            {
                JSONObject jsonObj = new JSONObject(dataAsJson);

                if (jsonObj.Count > 0)
                {
                    foreach (var item in jsonObj.list[0])
                    {
                        T2 t2 = new T2();
                        t2.InitByJson(item);
                        dict[t2.ID] = t2;
                    }
                }
            }
        }

        private void Collect()
        {
            if (GameObject.FindGameObjectWithTag("Map"))
            {
                GameObject[] mapLocations = GameObject.FindGameObjectsWithTag("MapLocation");

                foreach (var mapLoc in mapLocations)
                {
                    MapLocationSettings currMapLocSettings = mapLoc.GetComponent<MapLocationSettings>();
                    Get(currMapLocSettings);
                }
            }
        }

        protected abstract void Get(MapLocationSettings currMapLocSettings);

        protected void Save()
        {
            string jsonToFile = "";

            // Sort the elements by id ascending
            List<T2> _list = dict.Values.ToList();
            _list.Sort((el1, el2) => { return el1.ID.CompareTo(el2.ID); });

            // @TODO: So as to get the file size reduced (but not pretty print, though) choose FALSE instead of TRUE in this statement:
            jsonToFile = JSONExtensions.ToJson(_list.ToArray(), true);

            try
            {
                File.Delete(filePath);
                File.WriteAllText(filePath, jsonToFile);

                dict.Clear();

                Debug.LogFormat("{0} successfully saved!", filePath);
            }
            catch
            {
                Debug.LogErrorFormat("{0} not saved due to error(s).", filePath);
            }
        }
    }
}
