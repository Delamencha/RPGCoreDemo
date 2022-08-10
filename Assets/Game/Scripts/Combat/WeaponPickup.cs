using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Attribute;
using RPG.Movement;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {

        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 3f;
        [SerializeField] float pickupRange = 3f;
        

        private void OnTriggerEnter(Collider other)
        {
            
            if(other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);

            }

        }

        private void Pickup(GameObject subject)
        {
            if(weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);

            }
            
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {

            ShowPickup(false);

            yield return new WaitForSeconds(respawnTime);

            ShowPickup(true);
        }

        private void ShowPickup(bool flag)
        {

            GetComponent<Collider>().enabled = flag;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(flag);
            }
            

        }

        public bool HandleRaycast(PlayerController callingController)
        {

            if (Input.GetMouseButtonDown(0))
            {
                if(Vector3.Distance(callingController.transform.position, transform.position) > pickupRange)
                {
                    callingController.GetComponent<Mover>().MoveTo(transform.position, 1f);
                }
                else
                {
                    Pickup(callingController.gameObject);
                }
               
                
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

