using System.Collections;
using System.Collections.Generic;
 using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.U2D.Animation;

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
        public bool stunt;
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
                Destroy(gameObject);
                TurnBaseManager.turnBaseData.charQueue.Remove(_controllerHandler);
            }
            Vector3 facingDir = transform.forward;

            SpriteSkin spriteSkin = GetComponentInChildren<SpriteSkin>();

            if (spriteSkin != null)
            {
                Transform rootBone = spriteSkin.rootBone;
                if (rootBone != null)
                {
                    Vector3 localScale = rootBone.localScale;

                    if (facingDir.x < 0)
                        localScale.x = -Mathf.Abs(localScale.x); // flip left
                    else if (facingDir.x > 0)
                        localScale.x = Mathf.Abs(localScale.x); // face right

                    rootBone.localScale = localScale;
                }
            }
            else
            {
                if (facingDir.x < 0)
                {
                    foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                        sr.flipX = true;
                }
                else if (facingDir.x > 0)
                {
                    foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                        sr.flipX = false;
                }
            }
            
        }
        private void LateUpdate(){
            _currentClipName = _controllerHandler._animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        }
        private void ApplyHitbox(int hitboxNum)
        {
            Vector3 hitboxSpace = new Vector3(HitboxList[hitboxNum].hitboxSpace.x, HitboxList[hitboxNum].hitboxSpace.y, HitboxList[hitboxNum].hitboxSpace.z);
            GameObject Hitbox = Instantiate(HitboxList[hitboxNum].hitbox, transform.position + transform.forward * hitboxSpace.x + transform.right * hitboxSpace.y + transform.up * hitboxSpace.z
            , transform.rotation, transform);
            
            DamageDealing hitboxCom = Hitbox.GetComponent<DamageDealing>();
            hitboxCom._permenant = HitboxList[hitboxNum].permenant;
            hitboxCom._damage = HitboxList[hitboxNum].damage;
            hitboxCom._stunt = HitboxList[hitboxNum].stunt;
            hitboxCom._knockBack = HitboxList[hitboxNum].knockBack;

            // TurnBaseManager.turnBaseData.queue++;
        }
        public void ApplyImpluse(float _applyImpluse)
        {
            _impluse += _applyImpluse;
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
        private void OnMouseEnter() {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                    sr.material.EnableKeyword("_EMISSION");
            
        }
        private void OnMouseExit() {
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                    sr.material.DisableKeyword("_EMISSION");
        }
    }
}