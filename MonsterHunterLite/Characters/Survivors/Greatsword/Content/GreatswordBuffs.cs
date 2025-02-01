using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordBuffs
    {
        public static BuffDef wyvernStakeDotDebuff;
        public static void Init(AssetBundle assetBundle)
        {
            wyvernStakeDotDebuff = Modules.Content.CreateAndAddBuff("HenryArmorBuff",
               LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
               Color.white,
               false,
               false);
        }
    }
}
