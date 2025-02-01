using MonsterHunterMod.Survivors.Glaive;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Components
{
    public class GlaiveBugTypeController : MonoBehaviour
    {
        //public SkillDef speedBugSkillDef;

        //public SkillDef blastBugSkillDef;

        //public enum BugType
        //{
        //    speedBug,
        //    blastBug,
        //    none
        //}

        //bug type sklls
        public GenericSkill bugTypeSkillSlot;
        public SkillDef cuttingSkillDef;
        public SkillDef impactSkillDef;

        //bug style skills
        public GenericSkill bugStyleSkillSlot;
        public SkillDef bruiserSkillDef;
        public SkillDef ailmentSkillDef;
        public SkillDef powderSkillDef;
        public SkillDef gluttonSkillDef;

        public struct BugInfo
        {
            public float speed;

            public float baseDamage;

            public DamageType bugTypeDamageType;

            public BuffDef debuff;

            public bool isGlutton;

            public float trackingRange;

            public float rollChance;
        }


        [Tooltip("Bug Type controls cutting or impact damage.")]
        public enum BugType
        {
            cutting,
            impact,
            none
        }

        [Tooltip("Bug Style controls the interaction style the bug will have.")]
        public enum BugStyle
        {
            bruiser,
            ailment,
            powder,
            glutton,
            none
        }

        public BugType ReturnBugType()
        {
            if (bugTypeSkillSlot)
            {
                if (bugTypeSkillSlot.skillDef == cuttingSkillDef)
                {
                    return BugType.cutting;
                }
                if (bugTypeSkillSlot.skillDef == impactSkillDef)
                {
                    return BugType.impact;
                }
            }
            return BugType.none;
        }

        public BugStyle ReturnBugStyle()
        {
            if (bugStyleSkillSlot)
            {
                if (bugStyleSkillSlot.skillDef == bruiserSkillDef)
                {
                    return BugStyle.bruiser;
                }
                if (bugStyleSkillSlot.skillDef == ailmentSkillDef)
                {
                    return BugStyle.ailment;
                }
                if (bugStyleSkillSlot.skillDef == powderSkillDef)
                {
                    return BugStyle.powder;
                }
                if (bugStyleSkillSlot.skillDef == gluttonSkillDef)
                {
                    return BugStyle.glutton;
                }
            }
            return BugStyle.none;
        }

        public BugInfo ReturnBugInfo()
        {
            BugType bugType = ReturnBugType();
            BugStyle bugStyle = ReturnBugStyle();
            float speed = 40f;
            float baseDamage = 0f;
            DamageType damageType = DamageType.Generic;
            BuffDef debuff = null;
            bool isGlutton = false;
            float trackingRange = 50f;
            float rollChance = bugStyle == BugStyle.ailment ? 15f : 10f;

            if (bugType == BugType.cutting)
            {
                damageType = bugStyle == BugStyle.ailment ? DamageType.Generic: DamageType.BleedOnHit;
                debuff = bugStyle == BugStyle.ailment ? RoR2Content.Buffs.SuperBleed : null;
                baseDamage = GlaiveStaticValues.cuttingDamageCoefficient;
            }
            if ((bugType == BugType.impact))
            {
                damageType = bugStyle == BugStyle.ailment ? DamageType.Generic : DamageType.Stun1s;
                debuff = bugStyle == BugStyle.ailment ? GlaiveBuffs.stunBlast : null;
                baseDamage = GlaiveStaticValues.impactDamageCoefficient;
            }

            if (bugStyle == BugStyle.bruiser)
            {
                trackingRange = 75f;
                speed *= 2f;
                baseDamage *= 1.2f;
            }
            //ailment style logic is in the BugType statements
            if (bugStyle == BugStyle.powder)
            {
                debuff = GlaiveBuffs.blastOnStrike;
            }
            if (bugStyle == BugStyle.glutton)
            {
                isGlutton = true;
                baseDamage *= .8f;
            }

            return new BugInfo
            {
                speed = speed,
                baseDamage = baseDamage,
                bugTypeDamageType = damageType,
                debuff = debuff,
                trackingRange = trackingRange,
                isGlutton = isGlutton,
                rollChance = rollChance,
            };
        }
    }
}
