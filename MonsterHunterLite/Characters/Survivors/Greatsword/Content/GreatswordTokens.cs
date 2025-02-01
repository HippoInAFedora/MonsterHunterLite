using System;
using MonsterHunterMod.Modules;
using MonsterHunterMod.Survivors.Greatsword.Achievements;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordTokens
    {
        public static void Init()
        {
            AddGreatswordTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddGreatswordTokens()
        {
            string prefix = GreatswordSurvivor.GREATSWORD_PREFIX;

            string desc = "Rally Point Guard is a heavy-hitting hybrid survivor which manages a unique ammo system<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Thrust." + Environment.NewLine + Environment.NewLine
             + "< ! > Sweep." + Environment.NewLine + Environment.NewLine
             + "< ! > Reload." + Environment.NewLine + Environment.NewLine
             + "< ! > Contextual." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Champion");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Guard of the Survivors");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Evade");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Jump is converted to a sidestep during moves. Temporary invincibility.");

            Language.Add(prefix + "NORMAL_SHELLING_NAME", "Normal Shelling");
            Language.Add(prefix + "NORMAL_SHELLING_DESCRIPTION", "Normal Shelling");

            Language.Add(prefix + "LONG_SHELLING_NAME", "Long Shelling");
            Language.Add(prefix + "LONG_SHELLING_DESCRIPTION", "Long Shelling.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_CHARGED_SLASH", "Charged Slash");
            Language.Add(prefix + "PRIMARY_CHARGED_SLASH_DESCRIPTION", "Charge up to 3 tiers of charge.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Extraction");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Tokens.agilePrefix + $"Send a bug and retrieve a one of three Swarm Statuses from an enemy: {Tokens.whiteBugPrefix}, {Tokens.redBugPrefix}, or {Tokens.orangeBugPrefix}. Bug deals <style=cIsDamage>100% bug damage</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Vault");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Become temporarily <style=cIsUtility>invincible</style>, vaulting upwards into the air (or horizontally if airborne). Deal <style=cIsDamage>200% damage</style>, gaining " + Tokens.airBugPrefix + " and <style=cIsUtility>resetting jump count</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Swarm Crash");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Deal a <style=cIsDamage>200% damage</style> multi-hit downward, and deal an additional <style=cIsDamage>1000% damage</style> that is affected by " + Tokens.airBugPrefix + ".");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(GreatswordMasteryAchievements.identifier), "Greatsword: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(GreatswordMasteryAchievements.identifier), "As Greatsword, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
