
using MonsterHunterMod.Survivors.Glaive.Achievements;
using MonsterHunterMod.Survivors.Gunlance.Achievements;
using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            //masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
            //    GunlanceMasteryAchievements.unlockableIdentifier,
            //    Modules.Tokens.GetAchievementNameToken(GunlanceMasteryAchievements.identifier),
            //    GunlanceSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
