using UnityEngine;

public class PlayAnimOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        Animator _animator =GetComponent<Animator>();

        _animator.CrossFadeInFixedTime("onEnable", 0);
        
    }
}
