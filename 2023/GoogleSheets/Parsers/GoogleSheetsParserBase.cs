using Project.Network;
using UnityEngine;

namespace Project.Data.Google
{
    public abstract class GoogleSheetsParserBase : MonoBehaviour
    {
        public abstract object GetParsedData(ServerResponse data);
    } 
}