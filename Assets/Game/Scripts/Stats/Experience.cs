using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {

        [SerializeField] float experiencePoints = 0;

        //delegate 1.定义返回type和参数类型;2.创建实例，指向一系列方法  Action：简化创建，return void ,无参数的delegate 
        //public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;

        

        public void GainExperience(float experience)
        {
            experiencePoints += experience;

            onExperienceGained();
        }

        public float GetXP()
        {
            return experiencePoints;
        }


        //ISaveable interface
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }

}

