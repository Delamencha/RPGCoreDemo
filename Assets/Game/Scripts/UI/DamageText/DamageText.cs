using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText = null;
 
        public void DestroyText()
        {

            Destroy(gameObject);
        }

        public void SendDamage(float n)
        {
            damageText.text = string.Format("{0:0.0}", n);
        }

    }
}

