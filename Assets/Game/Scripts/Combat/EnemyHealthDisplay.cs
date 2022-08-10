using UnityEngine;
using RPG.Attribute;
using System;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health;
        Fighter player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            health = player.GetTarget();

            if (health != null)
            {
                //GetComponent<Text>().text = String.Format("{0:0.0}%", health.GetPercentage());
                GetComponent<Text>().text = String.Format("{0:0.0}/{1:0.0}", health.GetHealthPoint(), health.GetMaxHealthPoint());
            }
            else
            {
                GetComponent<Text>().text = " ";
            }
            

        }
    }
}

