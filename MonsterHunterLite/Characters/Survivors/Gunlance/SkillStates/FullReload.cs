using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using RoR2.Skills;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class FullReload : GunlanceReload
    {
        public override void OnEnter()
        {
            reloadsPrimary = true;
            reloadsSecondary = true;
            isFullReload = true;
            base.OnEnter();
        }

    }
}

