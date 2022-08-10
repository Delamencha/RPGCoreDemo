using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

         public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            //嵌套foreach loop ，频繁执行，影响性能
            //foreach(ProgressionCharacterClass progressionCharacterClass in characterClasses)
            //{
            //    if(progressionCharacterClass.characterClass == characterClass)
            //    {
            //        foreach(ProgressionStat progressionStat in progressionCharacterClass.stats)
            //        {
            //            if (level > progressionStat.levels.Length) continue;
            //            if(progressionStat.stat == stat)
            //            {
            //                return progressionStat.levels[level - 1];
            //            }
            //        }
            //    }
            //}
            //return 0;

            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (level > levels.Length) return 0;

            return levels[level - 1];
        }

        public float[] GetLevels(Stat stat, CharacterClass characterClass)
        {

            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            return levels;

        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                Dictionary<Stat, float[]> dict = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                {
                    dict[progressionStat.stat] =  progressionStat.levels;
                }

                lookupTable[progressionCharacterClass.characterClass]= dict;
                
            }

        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat; 
            public float[] levels;
        }

    }
}



