using Cysharp.Threading.Tasks;
using Project.Network;
using System.Collections.Generic;

namespace Project.Data.Google
{
    interface IGoogleSheetsGameDataProxy
    {
        void Dispose();
        UniTask<ServerResponse> GetSheetData(string sheetName);
    }

    class GoogleSheetsGameDataProxy : IGoogleSheet, IGoogleSheetsGameDataProxy
    {
        private readonly Dictionary<string, ServerResponse> _sheets;
        private readonly IGoogleSheetsGameData _gameData;

        public GoogleSheetsGameDataProxy()
        {
            _gameData = new GoogleSheetsGameData();
            _sheets = new Dictionary<string, ServerResponse>();
        }

        public async UniTask<ServerResponse> GetSheetData(string sheetName)
        {
            _sheets.TryGetValue(sheetName, out ServerResponse sheet);

            if (sheet == null)
            {
                sheet = await _gameData.GetSheetData(sheetName);
                _sheets[sheetName] = sheet;
            }

            return sheet;
        }

        public void Dispose()
        {
            if (_gameData != null) { _gameData.Dispose(); }
        }
    }
}