using UnityEngine;
using System.Collections;

public class DestroyOnDIsable : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        transform.parent = null;
        StartCoroutine(RemoveSprite());
    }
    IEnumerator RemoveSprite()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float realLength = stateInfo.length / stateInfo.speed;

        yield return new WaitForSeconds(stateInfo.length);
        Destroy(gameObject);
    }
}
