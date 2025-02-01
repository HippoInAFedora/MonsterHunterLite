using RoR2;
using MonsterHunterMod.Modules.Achievements;
using MonsterHunterMod.Modules.Characters;

namespace MonsterHunterMod.Survivors.Greatsword.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class GreatswordMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = GreatswordSurvivor.GREATSWORD_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = GreatswordSurvivor.GREATSWORD_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => CharacterBase<GreatswordSurvivor>.GetBodyNameSafe();

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}