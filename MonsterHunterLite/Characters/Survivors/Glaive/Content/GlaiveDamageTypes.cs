using R2API;
using UnityEngine;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content
{
    internal class GlaiveDamageTypes : MonoBehaviour
    {
        internal static DamageAPI.ModdedDamageType glaiveHit;

        internal static void SetupModdedDamage()
        {
            glaiveHit = DamageAPI.ReserveDamageType();
        }
    }
}
