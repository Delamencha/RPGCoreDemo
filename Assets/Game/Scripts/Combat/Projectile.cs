using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attribute;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float moveSpeed = 1 ;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health target;
        GameObject instigator = null;
        float damage;

        private void Start()
        {
            if (target == null) return;
            transform.LookAt(GetAimLocation());
        }
        private void Update()
        {
            if (target == null) return;

            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
                
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        }

        public void SetTarget(Health target,GameObject instigator, float damage)
        {

            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);

        }

        Vector3 GetAimLocation()
        {

            CapsuleCollider tc = target.GetComponent<CapsuleCollider>();
            if (tc == null) return target.transform.position;
            return target.transform.position + Vector3.up * tc.height / 2;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;

            target.TakeDamage(instigator, damage);

            moveSpeed = 0;

            onHit.Invoke();

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
                
            }
            
            foreach(GameObject  gb in destroyOnHit)
            {
                Destroy(gb);
            }

            Destroy(gameObject, lifeAfterImpact);

        }

    }
}


