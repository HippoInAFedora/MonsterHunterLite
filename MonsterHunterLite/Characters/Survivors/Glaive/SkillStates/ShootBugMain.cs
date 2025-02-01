using EntityStates;
using RoR2.Orbs;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using R2API;

namespace MonsterHunterMod.Survivors.Glaive.SkillStates
{
    public class ShootBugMain : BaseSkillState
    {
        public float duration = .2f;
        public float damageCoefficient;
        public float orbSpeed;
        public BuffDef debuff;
        public float delay = .2f;
        public float procCoefficient = 1f;
        public static float recoil = 3f;
        public static bool isCrit;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private HurtBox initialOrbTarget;

        private GlaiveTracker glaiveTracker;

        private GlaiveBugTypeController bugController;

        private GlaiveBugTypeController.BugInfo bugInfo;


        public override void OnEnter()
        {
            base.OnEnter();
            bugController = base.GetComponent<GlaiveBugTypeController>();
            glaiveTracker = base.GetComponent<GlaiveTracker>();
            if (base.isAuthority)
            {
                if (glaiveTracker != null)
                {
                    initialOrbTarget = glaiveTracker.GetTrackingTarget();
                }
                if (bugController != null)
                {
                    bugInfo = bugController.ReturnBugInfo();
                    if (bugInfo.debuff == RoR2Content.Buffs.SuperBleed || bugInfo.debuff == GlaiveBuffs.stunBlast)
                    {
                        bugInfo.debuff = Util.CheckRoll(bugInfo.rollChance, base.characterBody.master) ? bugInfo.debuff : null;
                    }
                }
            }
            isCrit = base.RollCrit();
            characterBody.SetAimTimer(2f);
            FireOrb();
        }

        public override void OnExit()
        {    
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        private void FireOrb()
        {
            float num = 0f;
            float numWhite = base.characterBody.HasBuff(GlaiveBuffs.whiteBugBuff) ? .5f : 0;
            float numRed = base.characterBody.HasBuff(GlaiveBuffs.redBugBuff) ? 1.5f : 0;
            float numOrange = base.characterBody.HasBuff(GlaiveBuffs.orangeBugBuff) ? .5f : 0;
            float numAir = base.characterBody.GetBuffCount(GlaiveBuffs.airborneDamageBuff) / 3f;
            if (base.characterBody.HasBuff(GlaiveBuffs.redBugBuff))
            {
                num = numWhite + numRed + numOrange + numAir;
            }            
            if (NetworkServer.active)
            {
                GlaiveOrb genericDamageOrb = new GlaiveOrb();
                genericDamageOrb.bouncesRemaining = bugInfo.isGlutton ? 1 : 0;
                genericDamageOrb.damageValue = base.characterBody.damage * bugInfo.baseDamage * (1f + num/2);
                genericDamageOrb.isCrit = isCrit;
                genericDamageOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                genericDamageOrb.attacker = base.gameObject;
                genericDamageOrb.procCoefficient = procCoefficient;
                genericDamageOrb.speed = bugInfo.speed;
                genericDamageOrb.debuff = bugInfo.debuff;
                genericDamageOrb.damageType = DamageTypeCombo.GenericSecondary;
                genericDamageOrb.damageType = Util.CheckRoll(bugInfo.rollChance, base.characterBody.master) ? bugInfo.bugTypeDamageType : DamageType.Generic;
                
                HurtBox hurtBox = initialOrbTarget;
                if ((bool)hurtBox)
                {
                    Transform transform = base.transform;
                    genericDamageOrb.origin = transform.position;
                    genericDamageOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(genericDamageOrb);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(initialOrbTarget));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }
    }
}