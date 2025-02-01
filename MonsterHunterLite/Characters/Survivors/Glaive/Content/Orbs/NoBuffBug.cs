using MonsterHunterMod.Survivors.Glaive;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs
{
    internal class NoBuffBug : Orb
    {

        public float speed;

        public float scale = 1f;

        public override void Begin()
        {
            base.duration = base.distanceToTarget / speed;
            if ((bool)GetOrbEffect())
            {
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = base.duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(GetOrbEffect(), effectData, transmit: true);
            }
        }

        protected virtual GameObject GetOrbEffect()
        {
            return GlaiveAssets.orbWhiteBug;
        }
    }
}
