using UnityEngine;
using UnityEngine.UI;

public class CutSceneHeroView : MonoBehaviour
{	
    [SerializeField] private Image Image;


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Activate(Transform place, Sprite icon)
    {
        if (!icon)
        {
            gameObject.SetActive(false);
            Image.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            Image.gameObject.SetActive(true);
            Image.sprite = icon;
            Image.color = Color.white;
        }
        transform.SetParent(place, false);
        transform.localPosition = Vector2.zero;
    }

    public void Deactivate()
    {
        Image.color = new Color(0.5f, 0.5f, 0.5f);
    }
}

