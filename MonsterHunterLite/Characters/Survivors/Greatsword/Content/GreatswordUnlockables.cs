
using MonsterHunterMod.Survivors.Glaive.Achievements;
using MonsterHunterMod.Survivors.Greatsword.Achievements;
using RoR2;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                GreatswordMasteryAchievements.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(GreatswordMasteryAchievements.identifier),
                GreatswordSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
