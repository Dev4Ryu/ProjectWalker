using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class FreezeDealing : DamageDealing
    {
        public int freezeTime = 0;
        private CombatHandler Charfreeze;
        public override void OnTriggerEnter(Collider other)
        {
            CombatHandler _targetData = other.GetComponent<CombatHandler>();
            if (_targetData != null && _targetData.transform.tag != _ownerTag)
            {
                if (_haveOwner)
                {
                    Vector3 enemy = new Vector3(other.transform.position.x, 0f, other.transform.position.z);
                    Quaternion rotation = Quaternion.LookRotation(-transform.parent.forward);
                    other.transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);
                }
                _targetData.ChangeAnimation("Stunt");
                _targetData._health = _targetData._health - _damage;
                freezeTime++;
                Charfreeze = _targetData;
                transform.parent = null;
            }
        }
        public void FreezeTurn()
        {
            if (TurnBaseManager.turnBaseData.charQueue[TurnBaseManager.turnBaseData.queue] == Charfreeze && freezeTime > 0)
            {
                TurnBaseManager.turnBaseData.queue++;
                TurnBaseManager.turnBaseData.savedOriginal = true;
                TurnBaseManager.turnBaseData.charSelect = null;
                freezeTime--;
            }
            else if(freezeTime <= 0 || !TurnBaseManager.turnBaseData._turnBaseMode)
            {
                Charfreeze.ChangeAnimation("Idle");
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            FreezeTurn();   
        }
    }
}