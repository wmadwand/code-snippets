#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;


using UnityEditor;


namespace ReplaceGameObjectTool
{
    public static class ReplaceGameObjectUtility
    {
        public static void FindReferencesTo(Object to, ref Dictionary<GameObject, GameObject> _dictGOs)
        {
            Dictionary<GameObject, GameObject> referencedByDict = new Dictionary<GameObject, GameObject>();
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

            for (int j = 0; j < allObjects.Length; j++)
            {
                var go = allObjects[j];

                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
                    if (go.name.Contains(to.name))
                    {
                        referencedByDict[go] = go.transform.root.gameObject; // = root/parent obj of go
                    }
                }
            }

            if (referencedByDict.Count > 0)
            {
                _dictGOs = referencedByDict;
            }
            else
            {
                _dictGOs.Clear();
                Debug.Log("No references in the scene.");
            }
        }
    }
}

#endif