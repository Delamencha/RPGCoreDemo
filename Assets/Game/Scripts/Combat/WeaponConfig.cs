using UnityEngine;
using RPG.Attribute;
using System;

namespace RPG.Combat 
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {

            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(equippedPrefab != null)
            {
 
                weapon = Instantiate(equippedPrefab, isRightHanded ? rightHand : leftHand);
                weapon.gameObject.name = weaponName;
            }

            //1.weapon有AnimatorOverrideController时覆盖原有animatorController
            //2.weapon无AnimatorOverrideController时：1.原先animatorController为默认，无需改动；2.原先animatorController有AnimatorOverrideController，则还原为默认animatorController
            //2的实现步骤：将runtimeAnimatorController(父类) cast 到AnimatorOverrideController上,当不为null时，则2.2为true

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }else if(overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            //runtimeAnimatorController为animatorOverrideController父类，也即设置为原先手动设置的默认animatorController(Character)
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);

        }

        public void LaunchProojectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, (isRightHanded ? rightHand : leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

    }

}
