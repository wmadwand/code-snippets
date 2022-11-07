using UnityEngine;

public class TutorialCanvasController : MonoBehaviour
{
    [SerializeField]
    Canvas _canvas;

    void Update()
    {
        if (!_canvas.worldCamera)
        {
            if (LoadingScreenController.Singleton.IsLevelLoaded || LoadingScreenController.Singleton.IsCurrentMapLoaded())
            {
                UpdateCanvasCamera();
            }
        }
    }

    void UpdateCanvasCamera()
    {
        _canvas.worldCamera = Camera.main;
    }
}