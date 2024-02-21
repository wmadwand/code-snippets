using Cysharp.Threading.Tasks;
using Project.Network;

namespace Project.Data.Google
{
    interface IGoogleSheetsGameData
    {
        void Dispose();
        UniTask<ServerResponse> GetSheetData(string sheetName);
    }

    class GoogleSheetsGameData : IGoogleSheet, IGoogleSheetsGameData
    {
        private IGoogleSheetsLoader _loader;

        public GoogleSheetsGameData()
        {
            var server = new Server();
            _loader = new GoogleSheetsLoader(server);
        }

        public async UniTask<ServerResponse> GetSheetData(string sheetName)
        {
            return await _loader.Get(sheetName);
        }

        public void Dispose()
        {
            _loader = null;
        }
    }
}