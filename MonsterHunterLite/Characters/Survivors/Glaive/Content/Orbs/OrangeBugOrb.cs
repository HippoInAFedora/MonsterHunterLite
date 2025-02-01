using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Survivors.Glaive;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs
{
    internal class OrangeBugOrb : NoBuffBug
    {

        protected override GameObject GetOrbEffect()
        {
            return GlaiveAssets.orbOrangeBug;
        }

        public override void OnArrival()
        {
            base.OnArrival();
            if ((bool)target)
            {
                HealthComponent healthComponent = target.healthComponent;
                if ((bool)healthComponent && (bool)healthComponent.body)
                {
                    healthComponent.body.AddTimedBuff(GlaiveBuffs.orangeBugBuff, 30f);
                }
            }
        }

    }
}
