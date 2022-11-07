using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DownloadImageManager : MonoBehaviour
{
    public LinksManager linksScript;

    /// <summary>
    /// RETURN SPRITE
    /// </summary>
    /// begin
    private IEnumerator DownloadSprite(string imgURL, Action<Sprite> callback)
    {
        WWW www = new WWW(imgURL);
        yield return www;

        print(www.error);

        if (www.error == null)
        {
            Texture2D currTexture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(currTexture as Texture2D);
            www.Dispose();
            www = null;

            Sprite sprite = Sprite.Create(currTexture, new Rect(0, 0, currTexture.width, currTexture.height), Vector2.zero);
            callback(sprite);
        }
        else
        {
            Debug.Log("ERROR DownloadImage: " + www.error);
        }
    }

    public void GetWebImageInSprite(string imgURL, Image ImgDestination)
    {
        StartCoroutine(DownloadSprite(imgURL, (Sprite resultSprite) =>
        {
            ImgDestination.sprite = resultSprite;
        }));
    }
    /// end

    /// <summary>
    /// RETURN TEXTURE
    /// </summary>
    /// begin
    private IEnumerator DownloadTexture(string imgURL, Action<Texture2D> callback)
    {
        WWW www = new WWW(imgURL);
        yield return www;

        print(www.error);

        if (www.error == null)
        {
            Texture2D currTexture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(currTexture as Texture2D);
            www.Dispose();
            www = null;

            callback(currTexture);
        }
        else
        {
            Debug.Log("ERROR DownloadImage: " + www.error);
        }
    }

    public void GetWebImageInTexture(string imgURL, Texture2D resultTexture)
    {
        StartCoroutine(DownloadTexture(imgURL, (Texture2D _resultTexture) =>
        {
            resultTexture = _resultTexture;
        }));
    }
    /// end
}
