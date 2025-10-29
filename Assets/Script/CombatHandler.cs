using System.Collections;
using System.Collections.Generic;
 using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Runtime.InteropServices;

namespace StarterAssets
{
    [System.Serializable]
    public class HitboxList
    {
        public GameObject hitbox;
        public Vector3 hitboxSpace;
        public bool permenant;
        public int damage;
        
        public int knockBack;
        public bool skipTurn;
        public bool lockTarget;
    }
    [System.Serializable]
    public class AbilityMove
    {
        public string moveName;
        public float distance;
        public string moveDescription;
        public int actionCost;
    }
    public class CombatHandler : MonoBehaviour
    {
        private ControllerHandler _controllerHandler;
        
        public int _currentClipName;
        public float _impluse;//need to be public due protection

        public HitboxList[] HitboxList;
        public AbilityMove[] AbilityMove;
        
        public int _maxHealth;
        public int _health;

        public int _maxAction;
        public int _action;

        public int _baseDamage;
        public int _speedMove;
        
        public bool _canBeDestroy = true;
        public bool _resetIfDie = false;

        public bool _flip = true;

        private void Start()
        {
            _health = _maxHealth;

            _controllerHandler = GetComponent<ControllerHandler>();
        }
        private void Update()
        {
            Impluse();
            if (_health <= 0 && _resetIfDie)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else if (_health <= 0 && _canBeDestroy)
            {
                int queue = TurnBaseManager.turnBaseData.queue;
                if (_controllerHandler._animator.GetCurrentAnimatorStateInfo(0).IsName("Stunt"))
                {
                    ChangeAnimation("Died");
                }
            }
            FlipRigSprite();
        }
        private void LateUpdate() {
            _currentClipName = _controllerHandler._animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        }
        private void FlipRigSprite()
        {
            Vector3 facingDir = transform.forward;

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                SpriteSkin sk = sr.GetComponent<SpriteSkin>();
                if (sk != null)
                {
                    Transform rootBone = sk.rootBone;

                    if (rootBone != null)
                    {
                        Vector3 localScale = rootBone.localScale;

                        if (facingDir.x < 0)
                            if (!_flip)
                            {
                                localScale.x = -Mathf.Abs(localScale.x);
                            }
                            else
                            {
                                localScale.x = Mathf.Abs(localScale.x);
                            }
                        else if (facingDir.x > 0)
                            if (!_flip)
                            {
                                localScale.x = Mathf.Abs(localScale.x);
                            }
                            else
                            {
                                localScale.x = -Mathf.Abs(localScale.x);
                            }
                        rootBone.localScale = localScale;
                    }
                }
                else if (sk == null)
                {
                    if (facingDir.x < 0)
                    {
                        sr.flipX = _flip ? false : true;
                    }
                    else if (facingDir.x > 0)
                    {
                        sr.flipX = _flip ? true : false;
                    }
                }
            }
        }
        private void ApplyHitbox(int hitboxNum)
        {
            Vector3 hitboxSpace = new Vector3(HitboxList[hitboxNum].hitboxSpace.x, HitboxList[hitboxNum].hitboxSpace.y, HitboxList[hitboxNum].hitboxSpace.z);
            GameObject Hitbox = Instantiate(HitboxList[hitboxNum].hitbox, transform.position + transform.forward * hitboxSpace.x + transform.right * hitboxSpace.y + transform.up * hitboxSpace.z
            , transform.rotation, transform);
            if (HitboxList[hitboxNum].lockTarget)
            {
                Hitbox.transform.position = TurnBaseManager.turnBaseData.charSelect.transform.position;
            }
            
            DamageDealing hitboxCom = Hitbox.GetComponent<DamageDealing>();
            hitboxCom._permenant = HitboxList[hitboxNum].permenant;
            hitboxCom._damage = HitboxList[hitboxNum].damage;
            hitboxCom._skipTurn = HitboxList[hitboxNum].skipTurn;
            hitboxCom._knockBack = HitboxList[hitboxNum].knockBack;
        }
        public void ApplyImpluse(float _applyImpluse)
        {
            _impluse = _applyImpluse;
        }
        private void Impluse()
        {
            if (_impluse >= 1 || _impluse <= -1)
            {
                _controllerHandler._controller.Move(transform.forward * _impluse * Time.deltaTime);
                _impluse -= Time.deltaTime * _impluse * 10;
            }
            else
            {
                _impluse = 0f;
            }
        }
        public void ChangeAnimation(string animation)
        {
            _controllerHandler._animator.CrossFadeInFixedTime(animation, 0);
        }
        public void ChangeEffect(string animation)
        {
            _controllerHandler._animator.CrossFadeInFixedTime(animation, 1);
        }
        public void Died()
        {
            Destroy(gameObject);
        }
        public void CameraShake()
        {
            TurnBaseManager.turnBaseData.StartCoroutine(TurnBaseManager.turnBaseData.Shake(1f, 0.6f));
        }
        void OnMouseOver()
        {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.material.EnableKeyword("_EMISSION");
        }
        void OnMouseExit()
            {
                foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.material.DisableKeyword("_EMISSION");
            }
    }
}