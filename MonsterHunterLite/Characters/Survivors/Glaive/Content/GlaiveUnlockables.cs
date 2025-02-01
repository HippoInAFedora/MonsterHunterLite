using MonsterHunterMod.Survivors.Glaive.Achievements;
using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Glaive
{
    public static class GlaiveUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                GlaiveMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(GlaiveMasteryAchievements.identifier),
                GlaiveSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
