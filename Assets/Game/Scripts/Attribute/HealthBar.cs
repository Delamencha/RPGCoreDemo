using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attribute
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform rt = null;
        [SerializeField] Health health = null;
        [SerializeField] Canvas canvas = null;

        void Update()
        {

            if (Mathf.Approximately(health.GetPercentage(), 0) || Mathf.Approximately(health.GetPercentage(), 100))
            {
                canvas.enabled = false;
                return;
            }
            canvas.enabled = true;
            rt.localScale = new Vector3(health.GetPercentage()/100, 1, 1);


            


        }
    }

}

