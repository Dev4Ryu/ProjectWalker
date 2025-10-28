using UnityEngine.UI;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    private RawImage background_Scene;
    public Texture2D newScene;
    public void changeScene()
    {
        background_Scene = GetComponentInParent<RawImage>();
        background_Scene.texture = newScene;
    }
}
