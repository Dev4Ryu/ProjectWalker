using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class DamageDealing : MonoBehaviour
    {
        public bool _noComponent;
        
        private CombatHandler _combat;
        private string _ownerTag;

        public float _breakImpluse;
        public int _damage;
        public int _knockBack;
        public bool _permenant;
        public bool _stunt;

        private float _timer = 0.3f;
        public float _projectileSpeed;
        private void Start()
        {
            if (!_noComponent) {
                _ownerTag = transform.parent.tag; 
                _combat = transform.parent.GetComponent<CombatHandler>();
            }
        }
        private void Update()
        {
            _timer -= 1 * Time.deltaTime;
            if (_timer <= 0 && !_permenant)
            {
                Destroy(gameObject);
            }
            if (_projectileSpeed != 0)
            {
                transform.position += (transform.forward * _projectileSpeed) * Time.deltaTime;
            }
        }
        private void OnTriggerEnter (Collider other){
            CombatHandler _targetData = other.GetComponent<CombatHandler>();
            if (_targetData != null && _targetData.transform.tag != _ownerTag)
            {
                if (!_noComponent) {
                    _combat.ApplyImpluse(_combat._impluse * 0);
                }
                if (_stunt)
                {
                    if (!_noComponent) {
                        Vector3 enemy = new Vector3(other.transform.position.x, 0f, other.transform.position.z);
                        Quaternion rotation = Quaternion.LookRotation(-transform.parent.forward);
                        other.transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);   
                    }
                    _targetData.ChangeAnimation("Stunt");
                    _targetData.ApplyImpluse(_knockBack);
                }
                _targetData._health = _targetData._health - _damage;
                Destroy(gameObject);
                TurnBaseManager.turnBaseData.queue++;
            }
            
        }
    }
}