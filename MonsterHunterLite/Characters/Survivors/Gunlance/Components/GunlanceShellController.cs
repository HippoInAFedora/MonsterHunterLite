using MonsterHunterMod.Survivors.Gunlance;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.Components
{
    public class GunlanceShellController : MonoBehaviour
    {
        public SkillDef normalShellSkillDef;

        public SkillDef longShellSkillDef;

        public SkillDef wideShellSkillDef;

        public GenericSkill passiveSkillSlot;

        public enum ShellType
        {
            normalShell,
            longShell,
            wideShell,
            none
        }

        public struct ShellingInfo
        {
            public ShellType type;

            public float baseDamage;

            public float chargeMult;

            public float baseRadius;

            public float chargedRadius;

            public float baseDistance;

            public float chargedDistance;

            public float burstDamage;

            public int maxShells;

            public int shotsPerShell;

            public int maxCharges;

            public bool isBlast;

            public float shotSpread;
        }

        public ShellType ReturnShellType()
        {
            if (passiveSkillSlot)
            {
                if (passiveSkillSlot.skillDef == normalShellSkillDef)
                {
                    return ShellType.normalShell;
                }
                if (passiveSkillSlot.skillDef == longShellSkillDef)
                {
                    return ShellType.longShell;
                }
                if (passiveSkillSlot.skillDef == wideShellSkillDef)
                {
                    return ShellType.wideShell;
                }
            }
            return ShellType.none;
        }

        public ShellingInfo ReturnShellingInfo()
        {
            ShellingInfo shellingInfo = new ShellingInfo();
            ShellType shellType = ReturnShellType();

            switch (shellType)
            {
                case ShellType.normalShell:
                    shellingInfo.type = ShellType.normalShell;
                    shellingInfo.baseDamage = GunlanceStaticValues.normalDamageCoefficient;                   
                    shellingInfo.chargeMult = GunlanceStaticValues.normalDamageCoefficientChargeMult;
                    shellingInfo.baseRadius = GunlanceStaticValues.normalRadius;
                    shellingInfo.chargedRadius = GunlanceStaticValues.normalRadiusCharged;
                    shellingInfo.baseDistance = GunlanceStaticValues.normalDistance;
                    shellingInfo.chargedDistance = GunlanceStaticValues.normalDistanceCharged;
                    shellingInfo.burstDamage = GunlanceStaticValues.normalDamageCoefficientBurst;
                    shellingInfo.shotsPerShell = 1;
                    shellingInfo.maxCharges = 6;
                    shellingInfo.isBlast = true;
                    shellingInfo.maxShells = 6;
                    shellingInfo.shotSpread = .5f;
                    return shellingInfo;
                case ShellType.longShell:
                    shellingInfo.type = ShellType.longShell;
                    shellingInfo.baseDamage = GunlanceStaticValues.longDamageCoefficient;
                    shellingInfo.baseRadius = GunlanceStaticValues.longRadius;
                    shellingInfo.chargeMult = GunlanceStaticValues.longDamageCoefficientChargeMult;
                    shellingInfo.chargedRadius = GunlanceStaticValues.longRadiusCharged;
                    shellingInfo.baseDistance = GunlanceStaticValues.longDistance;
                    shellingInfo.chargedDistance = GunlanceStaticValues.longDistanceCharged;
                    shellingInfo.burstDamage = GunlanceStaticValues.longDamageCoefficient;
                    shellingInfo.shotsPerShell = 1;
                    shellingInfo.maxCharges = 1;
                    shellingInfo.isBlast = false;
                    shellingInfo.maxShells = 3;
                    shellingInfo.isBlast = false;
                    shellingInfo.shotSpread = 2f;
                    return shellingInfo;
                case ShellType.wideShell:
                    shellingInfo.type = ShellType.wideShell;
                    shellingInfo.baseDamage = GunlanceStaticValues.wideDamageCoefficient;
                    shellingInfo.baseRadius = GunlanceStaticValues.wideRadius;
                    shellingInfo.chargeMult = GunlanceStaticValues.wideDamageCoefficientChargeMult;
                    shellingInfo.chargedRadius = GunlanceStaticValues.wideRadiusCharged;
                    shellingInfo.baseDistance = GunlanceStaticValues.wideDistance;
                    shellingInfo.chargedDistance = GunlanceStaticValues.wideDistanceCharged;
                    shellingInfo.burstDamage = GunlanceStaticValues.wideDamageCoefficient;
                    shellingInfo.maxCharges = 2;
                    shellingInfo.shotsPerShell = 5;
                    shellingInfo.maxShells = 2;
                    shellingInfo.isBlast = true;
                    shellingInfo.shotSpread = .8f;
                    return shellingInfo;
                case ShellType.none:
                default:
                    Debug.LogError("No ShellType Detected!");
                    return shellingInfo;                  
            }
            

        }
    }
}
