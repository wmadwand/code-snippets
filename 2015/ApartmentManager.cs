using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using MiniJSON;

public class ApartmentManager : MonoBehaviour
{
    public LinksManager linksScript;

    string mainURL;
    public string urlAptItems = "";
    public List<AptItem> aptItemsList = new List<AptItem>();
    Dictionary<string, int> AptItemsKeys = new Dictionary<string, int>();

    public Image[] aptPartsImages;

    void Start()
    {
        mainURL = linksScript.questionDataManager.urlMain;
        GetAptItems();
        AddAptItemsKeys();
    }

    void AddAptItemsKeys()
    {
        AptItemsKeys.Add("floor", 0);
        AptItemsKeys.Add("bed", 1);
        AptItemsKeys.Add("table", 2);
        AptItemsKeys.Add("computer", 3);
        AptItemsKeys.Add("flower", 4);
        AptItemsKeys.Add("wallpaper", 5);
        AptItemsKeys.Add("window", 6);
        AptItemsKeys.Add("picture", 7);
        AptItemsKeys.Add("wall", 8);
    }

    public WWW GET(string url, Action onComplete)
    {
        WWWForm form = new WWWForm();

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("X-Auth", linksScript.socketIOManager.token);
        form.AddField("test_id", 1);

        WWW www = new WWW(url, null, headers);

        StartCoroutine(RequestAptItems(www, onComplete));
        return www;
    }

    private IEnumerator RequestAptItems(WWW www, Action onComplete)
    {
        yield return www;
        if (www.error == null)
        {
            aptItemsList = ProcessAptItems(www.text);

            www.Dispose();
            www = null;

            onComplete();
        }
        else
        {
            Debug.Log("ERROR RequestAptItems: " + www.error);
        }
    }

    public void GetAptItems()
    {
        GET(mainURL + urlAptItems, () =>
        {
            DisplayAptItems();
        });
    }

    List<AptItem> ProcessAptItems(string url)
    {
        JSONObject jsonObject = new JSONObject(url);
        Debug.Log(jsonObject);

        foreach (var currJS in jsonObject.list)
        {
            int currId = (int)Convert.ToUInt64(currJS[0].ToString());
            string currKey = linksScript.socketIOManager.JsonToString(currJS[1].ToString(), "\"");
            string currName = linksScript.socketIOManager.JsonToString(currJS[2].ToString(), "\"");
            string currURLImage = linksScript.socketIOManager.JsonToString(currJS[3].ToString(), "\"");
            aptItemsList.Add(new AptItem(currId, currKey, currName, currURLImage));
        }

        aptItemsList.Sort(delegate (AptItem ainf1, AptItem ainf2)
        {
            return ainf1.id.CompareTo(ainf2.id);
        });

        return aptItemsList;
    }

    public void DisplayAptItems()
    {
        for (int i = 0; i < aptItemsList.Count; i++)
        {
            int currImgIndex = AptItemsKeys[aptItemsList[i].key];
            string currUrl = mainURL + aptItemsList[i].URLImage;
            linksScript.downloadImageManager.GetWebImageInSprite(currUrl, aptPartsImages[currImgIndex]);
        }
    }
}
