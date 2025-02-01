﻿using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using MonsterHunterMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Survivors.Glaive.SkillStates
{
    public class SlashCombo : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            hitboxGroupName = isGrounded ? "SwordGroup" : "SwordGroupAir" ;

            moddedDamageTypeHolder.Add(GlaiveDamageTypes.glaiveHit);
            
            int num = 0;
            if (NetworkServer.active)
            {
                num = base.characterBody.GetBuffCount(GlaiveBuffs.airborneDamageBuff);
            }           
            float airborneDamage = 1f + .2f * num;
            damageCoefficient = isGrounded? GlaiveStaticValues.glaiveDamageCoefficient: GlaiveStaticValues.glaiveAirDamageCoefficient * airborneDamage;
            damageType = DamageTypeCombo.GenericPrimary;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = isGrounded ? .72f : .62f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.4f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.65f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 5f;

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = GlaiveAssets.swordSwingEffect;
            hitEffectPrefab = GlaiveAssets.swordHitImpactEffect;

            impactSound = GlaiveAssets.swordHitSoundEvent.index;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void OnExit()
        {
            moddedDamageTypeHolder.Clear();
            base.OnExit();
        }
    }
}