using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    public class FullBurstSwitch : BaseSkillState
    {
        public GunlanceShellController shell;
        public GunlanceShellController.ShellingInfo shellInfo;

        public override void OnEnter()
        {
            base.OnEnter();
            shell = GetComponent<GunlanceShellController>();
            if (shell != null)
            {
                shellInfo = shell.ReturnShellingInfo();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (base.skillLocator.secondary.stock > 0)
                {
                    outer.SetNextState(GetNextStateAuthority());
                }
                else
                {
                    outer.SetNextState(new GunlanceReload { reloadsSecondary = true });
                }    
            }
        }

        protected virtual EntityState GetNextStateAuthority()
        {
            switch (shellInfo.type)
            {
                case GunlanceShellController.ShellType.normalShell:
                    return new FullBurstNormal();
                case GunlanceShellController.ShellType.longShell:
                    return new FullBurstLong();
                case GunlanceShellController.ShellType.wideShell:
                    return new FullBurstWide();
            }
            return new FullBurstNormal();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

    }
}
