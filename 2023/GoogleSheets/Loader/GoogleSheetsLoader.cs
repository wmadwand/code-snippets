using Cysharp.Threading.Tasks;
using Project.Network;

namespace Project.Data.Google
{
    public interface IGoogleSheetsLoader
    {
        UniTask<ServerResponse> Get(string sheetName);
    }

    public class GoogleSheetsLoader : IGoogleSheetsLoader
    {
        private const string SpreadsheetId = "";
        private const string APIKey = "";
        private const string BaseUrl = "";
        private readonly string _finalURL;
        private readonly IServer _server;

        //--------------------------------------------------------------

        public GoogleSheetsLoader(IServer server)
        {
            _server = server;
            _finalURL = BaseUrl.Replace("<SpreadsheetId>", SpreadsheetId);
            _finalURL = _finalURL.Replace("<ApiKey>", APIKey);
        }

        //--------------------------------------------------------------        

        public async UniTask<ServerResponse> Get(string sheetName)
        {
            var sheetURL = _finalURL.Replace("<SheetName>", sheetName);
            return await _server.GetRequest(sheetURL);
        }
    }
}