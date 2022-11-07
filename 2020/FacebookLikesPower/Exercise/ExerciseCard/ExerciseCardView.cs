using TMPro;
using UnityEngine;

public class ExerciseCardView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _frameDefault;
    [SerializeField] private Sprite _frameBorderless;
    [SerializeField] private ParticleSystem _sunRays;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private TextMeshPro _exerciseText;
    [SerializeField] private SpriteRenderer _avatarRenderer;
    [SerializeField] private SpriteRenderer _photoRenderer;
    [SerializeField] private GameObject _backlight;

    //---------------------------------------------------

    public void SetExerciseText(params int[] numbers)
    {
        _exerciseText.text = $"<size=100%>{numbers[0]}<voffset=0.15em><size=50%>\u2022</voffset><size=100%>{numbers[1]}";
    }

    public void SetProfile(ProfileProxy profile)
    {
        _photoRenderer.sprite = profile.GetPhotoFromQueue();
        _nameText.text = profile.Data.name;
        _avatarRenderer.sprite = profile.Data.avatar;
    }

    public void ResetView(Vector3 startPos)
    {
        transform.localScale = Vector3.zero;
        transform.position = startPos;
        SetFrame(false);
        BacklightSetActive(false);
    }

    public void BacklightSetActive(bool value)
    {
        _backlight.SetActive(value);

        if (value)
        {
            _sunRays.Play();
        }
        else
        {
            _sunRays.Stop();
        }
    }

    public void SunRaysSetActive(bool value)
    {
        

        if (value)
        {
            _sunRays.Play();
        }
        else
        {
            _sunRays.Stop();            
            _sunRays.gameObject.SetActive(value);
        }
    }

    public void SetFrame(bool isWin)
    {
        _spriteRenderer.sprite = isWin ? _frameBorderless : _frameDefault;
    }
}