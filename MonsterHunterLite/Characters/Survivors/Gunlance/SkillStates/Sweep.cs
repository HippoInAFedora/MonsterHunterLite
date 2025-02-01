using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using MonsterHunterMod.Modules.BaseStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class Sweep : BaseMeleeAttack
    {
        public GunlanceShellController controller;

        public float wyvernStakeEnterPercentTime = .6f;

        public override void OnEnter()
        {           
            controller = base.GetComponent<GunlanceShellController>();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
            }
            hitboxGroupName = "SweepGroup";

            damageCoefficient = base.characterBody.damage * GunlanceStaticValues.sweepDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.25f;
            attackEndPercentTime = 0.6f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = .9f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 6f;

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && inputBank)
            {
                if (base.inputBank.skill2.down && base.fixedAge > duration * wyvernStakeEnterPercentTime)
                {
                    outer.SetNextState(new WyrmStake());
                }
            }
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
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}

