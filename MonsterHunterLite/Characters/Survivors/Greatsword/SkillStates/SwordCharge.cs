using EntityStates;
using RoR2;
using UnityEngine;
using MonsterHunterMod.Characters.Survivors.Greatsword.Components;

namespace MonsterHunterMod.Characters.Survivors.Greatsword.SkillStates
{
    public class SwordCharge : BaseSkillState
    {
        public GreatswordChargeController controller;
        public float baseDuration = 1f;
        public float earlyChargeDuration;
        public float fullChargeDuration;
        public float stopwatch = 0f;

        public override void OnEnter()
        {
            earlyChargeDuration = baseDuration / attackSpeedStat;
            fullChargeDuration = baseDuration;
            controller = base.GetComponent<GreatswordChargeController>();
            if (controller != null )
            {
                controller.inState = true;
            }
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RunCharge();
            if (base.isAuthority && inputBank.skill1.justReleased)
            {
                outer.SetNextState(new Slam());
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public void RunCharge() 
        { 
            if (controller != null)
            {
                stopwatch += Time.fixedDeltaTime;
                if (controller.charge < GreatswordChargeController.ChargeLevel.Tier3)
                {
                    if (stopwatch > earlyChargeDuration)
                    {
                        controller.charge++;
                        stopwatch = 0f;
                    }
                }
                if (controller.charge == GreatswordChargeController.ChargeLevel.Tier3)
                {
                    if (stopwatch > fullChargeDuration)
                    {
                        controller.charge++;
                        stopwatch = 0f;
                    }
                }
            }
        }
    }
}

