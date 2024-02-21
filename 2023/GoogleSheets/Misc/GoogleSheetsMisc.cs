using Cysharp.Threading.Tasks;
using Project.Network;
using System;

namespace Project.Data.Google
{
    [Serializable]
    public class SheetParserPair
    {
        public GoogleSheet type;
        public string name;
        public GoogleSheetsParserBase parserPrefab;
        public GoogleSheetsParserBase Parser { get; private set; }

        public void SetParser(GoogleSheetsParserBase parser)
        {
            Parser = parser;
        }
    }

    public enum GoogleSheet
    {
        Settings,
        Rewards
    }

    interface IGoogleSheet : IDisposable
    {
        UniTask<ServerResponse> GetSheetData(string sheetName);
    }
}