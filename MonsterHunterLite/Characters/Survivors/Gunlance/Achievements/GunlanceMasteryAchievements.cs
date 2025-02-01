using RoR2;
using MonsterHunterMod.Modules.Achievements;
using MonsterHunterMod.Modules.Characters;

namespace MonsterHunterMod.Survivors.Gunlance.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class GunlanceMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = GunlanceSurvivor.GUNLANCE_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = GunlanceSurvivor.GUNLANCE_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => CharacterBase<GunlanceSurvivor>.GetBodyNameSafe();

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}