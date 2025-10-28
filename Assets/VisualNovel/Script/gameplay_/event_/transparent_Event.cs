using UnityEngine;
using UnityEngine.UI;

public class transparentEvent : MonoBehaviour
{
    private RawImage character;
    public string characterName;
    [Range(0f, 1f)]
    public float alpha = 1f;

    void Start()
    {
        GameObject obj = GameObject.Find(characterName);

        if (obj != null)
        {
            character = obj.GetComponent<RawImage>();
        }
    }
    void Update()
    {
        Color color = character.color;
        color.a = Mathf.Lerp(color.a,alpha, 0.25f);
        character.color = color;
        if(color.a >= 0.999f){
            color.a = 1;
            character.color = color;
            Destroy(this.gameObject);
        }else if(color.a <= 0.001f){
            color.a = 0;
            character.color = color;
            Destroy(this.gameObject);
        }
    }
}
