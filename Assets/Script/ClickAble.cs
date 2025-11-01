using System.Runtime.InteropServices;
using UnityEngine;

public class ClickAble : MonoBehaviour
{
    void OnMouseOver()
    {

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.material.EnableKeyword("_EMISSION");
        }
        print("work");
    }

    private void OnMouseExit()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.material.DisableKeyword("_EMISSION");
            }
    }
}
