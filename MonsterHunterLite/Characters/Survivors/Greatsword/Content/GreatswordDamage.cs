using R2API;
using RoR2;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordDamage
    {
        public static DamageAPI.ModdedDamageType wyvernDamage;
        internal static void SetupModdedDamage()
        {
            wyvernDamage = DamageAPI.ReserveDamageType();
        }
    }
}
