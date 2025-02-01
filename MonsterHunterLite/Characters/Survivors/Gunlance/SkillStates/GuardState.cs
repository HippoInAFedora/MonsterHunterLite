using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class GuardState : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow80);
                base.characterBody.AddBuff(GunlanceBuffs.gunlanceGuardBuff);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow80);
                base.characterBody.RemoveBuff(GunlanceBuffs.gunlanceGuardBuff);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!IsKeyDownAuthority())
            {
                outer.SetNextStateToMain();
            }
        }
    }
}

