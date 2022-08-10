using GameDevTV.Utils;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 100)]
        [SerializeField] int startLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression =  null;
        [SerializeField] GameObject levelUpEffect = null;
        [SerializeField] bool shouldUseModifier = false;
 
        LazyValue<int> currentLevel ;

        public event Action onLevelUp;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }


        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }

        public float GetStat(Stat stat)
        {

            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }


        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }



        public int GetLevel()
        {
            //与不同类的start执行先后相关，当在start中调用Getlevel的类(Health)先执行，currentLevel未初始化，值为-1，出现outOfBounds error
            //引入lazyValue后解决race condition问题
            //if (currentLevel.value < 1) currentLevel.value = CalculateLevel();

            return currentLevel.value;
        }

        private int CalculateLevel()
        {
            if (GetComponent<Experience>() == null) return startLevel;

            float currentXP = GetComponent<Experience>().GetXP();

            float[] levels = progression.GetLevels(Stat.ExperienceToLevelUP, characterClass);
            for (int i = 1; i <= levels.Length; i++)
            {
                //i 为等级, i-1 = index;
                if(currentXP < levels[i-1])
                {
                    return i;
                }

            }
            return levels.Length + 1;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifier) return 0;
            float sum = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifier(stat))
                {
                    sum += modifier;
                }
            }
            return sum;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifier) return 0;
            float sum = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    sum += modifier;
                }
            }
            return sum;

        }



    }
}


