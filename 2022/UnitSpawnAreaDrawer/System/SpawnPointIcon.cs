using UnityEngine;

public class SpawnPointIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Sprite[] Sprites;

    public void SetSprite(int index)
    {
        Mathf.Clamp(index, 0, Sprites.Length);
        SpriteRenderer.sprite = Sprites[index];
    }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            SpriteRenderer.enabled = false;
        }
    }
}