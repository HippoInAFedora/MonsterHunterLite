﻿using RoR2;
using MonsterHunterMod.Modules.Achievements;
using MonsterHunterMod.Modules.Characters;

namespace MonsterHunterMod.Survivors.Glaive.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class GlaiveMasteryAchievements : BaseMasteryAchievement
    {
        public const string identifier = GlaiveSurvivor.GLAIVE_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = GlaiveSurvivor.GLAIVE_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => CharacterBase<GlaiveSurvivor>.GetBodyNameSafe();

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}