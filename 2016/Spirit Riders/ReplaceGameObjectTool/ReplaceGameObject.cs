/// <summary>
/// Programmed by WMADWAND
/// </summary>

#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace ReplaceGameObjectTool
{
    public class ReplaceGameObject : MonoBehaviour
    {
        [HideInInspector]
        public List<MatchedPrefabs> MatchedPrefabsList = new List<MatchedPrefabs>(1);
        private Dictionary<GameObject, GameObject> _oldInstancesDict = new Dictionary<GameObject, GameObject>();

        #region Control
        public void ReplaceObjects()
        {
            string errorStr01 = "";
            CheckPrefsForDuplication(MatchedPrefabsList, ref errorStr01);

            if (errorStr01 != "")
            {
                if (EditorUtility.DisplayDialog("Prefabs duplicates detected!", errorStr01 + "\nProceed anyway?", "Ok", "Cancel"))
                {
                    ReplaceObjectsRun();
                }
            }

            else
            {
                ReplaceObjectsRun();
            }
        }

        void ReplaceObjectsRun()
        {
            foreach (var item in MatchedPrefabsList)
            {
                GameObject newPrefab = item.newPrefab;
                GameObject oldPrefab = item.oldPrefab;

                string errorStr02 = "";
                CheckPrefsForError(newPrefab, oldPrefab, MatchedPrefabsList.IndexOf(item) + 1, ref errorStr02);

                if (errorStr02 == "no_error")
                {
                    ReplaceGameObjectUtility.FindReferencesTo(oldPrefab, ref _oldInstancesDict);
                }
                else
                {
                    Debug.LogErrorFormat(errorStr02);
                    continue;
                }

                if (_oldInstancesDict.Count > 0)
                {
                    GameObject[] rootGOs = _oldInstancesDict.Values.Distinct().ToArray(); // Array of unique (distinct) LevelBlocks instances

                    foreach (GameObject rootGO in rootGOs)
                    {
                        GameObject[] rootGOChildren = (from _el in _oldInstancesDict
                                                       where _el.Value == rootGO
                                                       select _el.Key).ToArray();

                        PrefabUtility.DisconnectPrefabInstance(rootGO);

                        foreach (var currInstanceGO in rootGOChildren)
                        {
                            GameObject newInstanceGO = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;

                            Undo.RegisterCreatedObjectUndo(newInstanceGO, "Instantiate " + currInstanceGO.name);
                            ////Undo.RegisterFullObjectHierarchyUndo(newInstanceGO, newInstanceGO.name);

                            newInstanceGO.transform.SetParent(currInstanceGO.transform.parent, true);
                            newInstanceGO.transform.localPosition = currInstanceGO.transform.localPosition;
                            newInstanceGO.transform.rotation = Quaternion.identity;
                            newInstanceGO.transform.localScale = currInstanceGO.transform.localScale;
                            newInstanceGO.name = currInstanceGO.name;

                            Undo.DestroyObjectImmediate(currInstanceGO);
                        }

                        PrefabUtility.ReplacePrefab(rootGO, PrefabUtility.GetPrefabParent(rootGO), ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }
        }
        #endregion

        #region Checks
        void CheckPrefsForError(GameObject newPref, GameObject oldPref, int _index, ref string _errorStr)
        {
            if (newPref != null && oldPref != null && newPref != oldPref)
                _errorStr = "no_error";
            else if (newPref == null || oldPref == null)
                _errorStr = String.Format("line #{0} - NULL PREFAB DETECTED, OPERATION HAS BEEN SKIPPED!", _index);
            else if (newPref == oldPref)
                _errorStr = String.Format("line #{0} - IDENTICAL PREFABS DETECTED, OPERATION HAS BEEN SKIPPED!", _index);
            else
                _errorStr = "no_error";
        }

        void CheckPrefsForDuplication(List<MatchedPrefabs> _list, ref string errorStr)
        {
            var queryNewPrefabs = (from _item in _list
                                   group _item by _item.newPrefab
                                   into itemNewPrefab
                                   where itemNewPrefab.Count() > 1
                                   select itemNewPrefab).ToArray();

            var queryOldPrefabs = (from _item in _list
                                   group _item by _item.oldPrefab
                                   into itemOldPrefab
                                   where itemOldPrefab.Count() > 1
                                   select itemOldPrefab).ToArray();

            if (queryNewPrefabs.Count() > 0 || queryOldPrefabs.Count() > 0)
            {
                errorStr = "The next prefabs have got duplicates in the list:\n\n";

                foreach (var item in queryNewPrefabs)
                {
                    errorStr = errorStr + String.Format("- NEW {0}\n", item.Key);
                }

                foreach (var item in queryOldPrefabs)
                {
                    errorStr = errorStr + String.Format("- OLD {0}\n", item.Key);
                }

            }
        }
        #endregion
    }

    [Serializable]
    public class MatchedPrefabs
    {
        public GameObject newPrefab;
        public GameObject oldPrefab;
    }
}

#endif