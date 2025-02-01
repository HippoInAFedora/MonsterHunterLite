using EntityStates;
using UnityEngine;
using System.Collections.Generic;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;
using HarmonyLib;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    internal class ChargingBlastDash : BaseSkillState
    {
        public GunlanceShellController shell;
        public GunlanceShellController.ShellingInfo shellInfo;

        public float baseDuration = .25f;
        public float duration;
        public int chargeLevel = 1;
        public float stopwatch = 0f;

        public override void OnEnter()
        {
            base.OnEnter();
            shell = GetComponent<GunlanceShellController>();
            if (shell != null)
            {
                shellInfo = shell.ReturnShellingInfo();
            }
            duration = .25f + baseDuration / attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (base.isAuthority)
            {
                if (skillLocator.secondary.stock <= 0)
                {
                    outer.SetNextState(new GunlanceReload{
                    reloadsSecondary = true});
                }
                if (base.fixedAge > duration && base.inputBank.skill2.down)
                {
                    outer.SetNextState(new BlastDash());
                }
                if (!base.inputBank.skill2.down)
                {
                    outer.SetNextState(NextState());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public EntityState NextState()
        {
            EntityState nextState = null;
            switch (shellInfo.type)
            {
                case GunlanceShellController.ShellType.normalShell:
                    base.skillLocator.secondary.DeductStock(chargeLevel);
                    nextState =  new ChargedBlastNormal { chargeLevel = chargeLevel };
                    break;
                case GunlanceShellController.ShellType.longShell:
                    base.skillLocator.secondary.DeductStock(1);
                    nextState = new ChargedBlastLong { chargeLevel = chargeLevel };
                    break;
                case GunlanceShellController.ShellType.wideShell:
                    base.skillLocator.secondary.DeductStock(1);
                    nextState = new ChargedBlastWide { chargeLevel= chargeLevel };
                    break;
                case GunlanceShellController.ShellType.none:
                    base.skillLocator.secondary.DeductStock(chargeLevel);
                    nextState = new ChargedBlastNormal { chargeLevel = chargeLevel };
                    break;
            }
            return nextState;
        }
    }
}

