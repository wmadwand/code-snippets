using Cysharp.Threading.Tasks;
using NoName.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoName.Data.Google
{
    public interface IGoogleSheets
    {
        Dictionary<GoogleSheet, object> SheetsData { get; }
        UniTask<string> Load();
    }

    [CreateAssetMenu(fileName = "GoogleSheets", menuName = "GoogleSheets")]
    public class GoogleSheets : ScriptableObject, IGoogleSheets
    {
        public Dictionary<GoogleSheet, object> SheetsData { get; private set; } = new Dictionary<GoogleSheet, object>();
        public List<SheetParserPair> Sheets => _sheets;
        [SerializeField] private List<SheetParserPair> _sheets;

        //--------------------------------------------------------------

        public async UniTask<string> Load()
        {
            var errors = await LoadAsync();

            string errorStr = null;
            if (errors == null) { errorStr = "In Editor add sheets for GoogleSheets class"; }
            else if (errors.Count > 0) { errorStr = errors[0]; }

            return errorStr;
        }

        //--------------------------------------------------------------

        private async UniTask<List<string>> LoadAsync()
        {
            if (_sheets.Count < 1) { return null; }

            List<string> errors = new List<string>();
            using (IGoogleSheet gameData = new GoogleSheetsGameDataProxy())
            {
                var tasks = new UniTask<ServerResponse>[_sheets.Count];
                for (int i = 0; i < _sheets.Count; i++)
                {
                    tasks[i] = gameData.GetSheetData(_sheets[i].name);
                }

                var responses = await UniTask.WhenAll(tasks);

                foreach (var resp in responses)
                {
                    if (!string.IsNullOrEmpty(resp.error))
                    {
                        errors.Add(resp.error);
                    }
                }

                if (errors.Count < 1)
                {
                    for (int i = 0; i < _sheets.Count; i++)
                    {
                        if (!_sheets[i].Parser)
                        {
                            _sheets[i].SetParser(Instantiate(_sheets[i].parserPrefab));
                        }

                        var parsedData = _sheets[i].Parser.GetParsedData(responses[i]);
                        SheetsData[_sheets[i].type] = parsedData;
                    }
                }
            }

            return errors;
        }
    }
}