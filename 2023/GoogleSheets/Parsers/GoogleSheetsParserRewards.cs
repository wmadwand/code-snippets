using Cysharp.Threading.Tasks;
using Project.Network;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Project.Data.Google
{
    public class GoogleSheetsParserRewards : GoogleSheetsParserBase
    {
        private object _data = null;
        private string _currentMerchant = "";

        public override object GetParsedData(ServerResponse sheetData)
        {
            if (_data != null) { return _data; }
            if (string.IsNullOrEmpty(sheetData.text)) { return null; }

            try
            {
                var jsonParsed = JSON.Parse(sheetData.text);
                var rewardsList = new List<RewardData>();

                foreach (var row in jsonParsed["values"])
                {
                    var items = JSON.Parse(row.ToString()).AsArray;

                    var name = items[0][0];
                    if (name == "merchant_name" || string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    var listItem = new RewardData(items[0][0], items[0][1], items[0][2], items[0][3], items[0][4], items[0][5], items[0][6]
                        , items[0][7], items[0][8], items[0][9], items[0][10], items[0][11], items[0][12], items[0][13], items[0][14], items[0][15]
                        , items[0][16], items[0][17], items[0][18], items[0][19], items[0][20], items[0][21], items[0][22], items[0][23], items[0][24]
                        , items[0][25], items[0][26], items[0][27], items[0][28]);

                    rewardsList.Add(listItem);

                    if (string.IsNullOrEmpty(listItem.reward_creative_image_url))
                    {
                        continue;
                    }

                    if (listItem.merchant_name != _currentMerchant)
                    {
                        _ = DownLoadImage(listItem.reward_creative_image_url, listItem.merchant_name + "RewardCreative");
                        _ = DownLoadImage(listItem.token_creative_image_url, listItem.merchant_name + "TokenCreative");
                        _ = DownLoadImage(listItem.token_bg_creative_image_url, listItem.merchant_name + "TokenCreativeBackground");
                        _ = DownLoadImage(listItem.campaign_creative_image_url, listItem.merchant_name + "CampaignCreative");

                        _currentMerchant = listItem.merchant_name;
                    }
                }

                var query = from reward in rewardsList
                            group reward by new GroupStruct(reward.campaign_id, reward.merchant_name)
                            into group01
                            group group01 by group01.Key.merchant_name
                            into group02
                            select group02 //select new KeyValuePair<String, String>(p.NickName, p.Name);
                            ;


                //var queryAlt = rewardsList.GroupBy(p => p.campaign_id, p => p.merchant_name).GroupBy(p => p.Key);
                //var queryAlt1 = rewardsList.GroupBy(p => new GroupStruct(p.campaign_id, p.merchant_name)).GroupBy(p => p.Key.merchant_name);

                //var dict = TableObj.Select(t => new { t.Key, t.TimeStamp })
                //       .ToDictionary(t => t.Key, t => t.TimeStamp);

                _data = query.ToDictionary(x => x.Key);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return _data;
        }

        private async UniTask DownLoadImage(string imageUrl, string imgName)
        {
            string path = UnityEngine.Application.persistentDataPath;
            var fullPathImg = path + "/" + imgName + ".png";
            Debug.Log(fullPathImg);

            if (!File.Exists(fullPathImg))
            {
                var data = await DownloadHelper.DownloadAssetData(imageUrl);
                File.WriteAllBytes(fullPathImg, data);
            }
        }
    }

    public struct GroupStruct
    {
        public string campaign_id;
        public string merchant_name;

        public GroupStruct(string campaign_id, string merchant_name)
        {
            this.campaign_id = campaign_id;
            this.merchant_name = merchant_name;
        }
    }
}
