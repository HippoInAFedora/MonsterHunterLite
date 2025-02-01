using R2API;
using RoR2;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceDamage
    {
        public static DamageAPI.ModdedDamageType wyrmStakeDamage;
        internal static void SetupModdedDamage()
        {
            wyrmStakeDamage = DamageAPI.ReserveDamageType();
        }
    }
}
