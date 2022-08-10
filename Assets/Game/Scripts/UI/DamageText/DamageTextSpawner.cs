using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{

    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText prefab = null;

        public void Spwan(float amount)
        {
            DamageText instance = Instantiate<DamageText>(prefab, transform);

            instance.SendDamage(amount);
        }

        

    }

}

