using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attribute
{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> healthPoint ;
        bool isDead = false;

        [Range(0.3f, 1f)]
        [SerializeField] float healing = 0.7f;
        [SerializeField] UnityEvent onDie;

        //unity 的serializefiled不支持含参数的event,故使用subclass实现参数传递
        //[SerializeField] UnityEvent takeDamage;
        [SerializeField] TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> 
        { 
        }


        private void Awake()
        {
            healthPoint = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            healthPoint.ForceInit();

        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += SelfHeal;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= SelfHeal;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {

            print(gameObject.name + " took damage: " + damage);

            healthPoint.value = Mathf.Max(healthPoint.value - damage, 0);

            if (healthPoint.value <= 0 && !isDead)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }


        public float GetHealthPoint()
        {
            return healthPoint.value;
        }

        public float GetMaxHealthPoint()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * healthPoint.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Collider>().enabled = false;


        }

        void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            float ex = GetComponent<BaseStats>().GetStat(Stat.ExperienceReward);
            experience.GainExperience(ex);
            
        }

        public object CaptureState()
        {
            return healthPoint.value;
        }

        public void RestoreState(object state)
        {
            healthPoint.value = (float)state;
            if (healthPoint.value <= 0 && !isDead)
            {
                Die();
            }
        }

        private void SelfHeal()
        {
            healthPoint.value =Mathf.Max(healthPoint.value, healing * GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public void Heal(float n)
        {
            healthPoint.value = Mathf.Min(GetMaxHealthPoint(), healthPoint.value + n);
        }

    }
}


