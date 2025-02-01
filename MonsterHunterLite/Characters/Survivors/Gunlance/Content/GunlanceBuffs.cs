using MonsterHunterMod.Modules;
using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceBuffs
    {
        public static BuffDef wyrmStakeDotDebuff;

        public static BuffDef gunlanceGuardBuff;

        public static void Init(AssetBundle assetBundle)
        {
            wyrmStakeDotDebuff = Modules.Content.CreateAndAddBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                true);

            gunlanceGuardBuff = Modules.Content.CreateAndAddBuff("GunlanceGuardBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
        }
    }
}
