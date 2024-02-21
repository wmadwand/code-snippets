using Project.Network;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data.Google
{
    public class GoogleSheetsParserSettings : GoogleSheetsParserBase
    {
        private object _data = null;

        public override object GetParsedData(ServerResponse sheetData)
        {
            if (_data != null)
            {
                return _data;
            }

            if (string.IsNullOrEmpty(sheetData.text))
            {
                return null;
            }

            Dictionary<string, List<string>> resDict;

            try
            {
                var parsedJson = JSON.Parse(sheetData.text);
                resDict = new Dictionary<string, List<string>>();
                var parsedValues = parsedJson["values"];

                foreach (var settingsValue in parsedValues)
                {
                    var settings = JSON.Parse(settingsValue.ToString()).AsArray;
                    var key = settings[0][0];
                    if (key != "Param") { resDict.Add(key, new List<string> { settings[0][1], settings[0][2] }); }
                }

                foreach (var key in resDict.Keys)
                {
                    Debug.Log(key + "|key|" + resDict[key] + "|value");
                }

                _data = resDict;
            }
            catch (System.Exception)
            {
                throw;
            }

            return _data;
        }
    }
}