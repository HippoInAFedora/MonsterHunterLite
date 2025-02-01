using EntityStates;
using UnityEngine;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class ChargedBlastLong : ChargedBlastBase
    {

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                FireShot();
            }
        }
    }
}
