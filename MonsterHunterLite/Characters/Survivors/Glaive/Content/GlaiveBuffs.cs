using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Glaive
{
    public static class GlaiveBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;

        public static BuffDef stunBlast;

        public static BuffDef airborneDamageBuff;

        public static BuffDef whiteBugBuff;

        public static BuffDef redBugBuff;

        public static BuffDef orangeBugBuff;

        public static BuffDef blastOnStrike;

        public static BuffDef blastOnStrikeVisualCooldown;


        public static BuffDef reverseWhiteBugBuff;

        public static BuffDef reverseRedBugBuff;

        public static BuffDef reverseOrangeBugBuff;


        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            airborneDamageBuff = Modules.Content.CreateAndAddBuff("AirborneDamageBuff",
                GlaiveAssets.airBugBuffIcon,
                Color.white,
                true,
                false);

            whiteBugBuff = Modules.Content.CreateAndAddBuff("WhiteBugBuff",
                GlaiveAssets.whiteBugBuffIcon,
                Color.white,
                false,
                false);

            redBugBuff = Modules.Content.CreateAndAddBuff("RedBugBuff",
                GlaiveAssets.redBugBuffIcon,
                Color.white,
                false,
                false);

            orangeBugBuff = Modules.Content.CreateAndAddBuff("OrangeBugBuff",
                GlaiveAssets.orangeBugBuffIcon,
                Color.white,
                false,
                false);

            blastOnStrike = Modules.Content.CreateAndAddBuff("FullBugBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.magenta,
                false,
                true);

            stunBlast = Modules.Content.CreateAndAddBuff("StunBlast",
                null,
                Color.white,
                false,
                false);

            //blastOnStrikeVisualCooldown = Modules.Content.CreateAndAddBuff("FullBugBuffVisualCooldown",
            //    LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
            //    Color.magenta,
            //    false,
            //    true);

            reverseWhiteBugBuff = Modules.Content.CreateAndAddBuff("ReverseWhiteBugBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                true);

            reverseRedBugBuff = Modules.Content.CreateAndAddBuff("ReverseRedBugBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.red,
                false,
                true);

            reverseOrangeBugBuff = Modules.Content.CreateAndAddBuff("ReverseOrangeBugBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.yellow,
                false,
                true);
        }
    }
}
