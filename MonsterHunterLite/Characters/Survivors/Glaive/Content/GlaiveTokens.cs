using System;
using MonsterHunterMod.Modules;
using MonsterHunterMod.Survivors.Glaive.Achievements;

namespace MonsterHunterMod.Survivors.Glaive
{
    public static class GlaiveTokens
    {
        public static void Init()
        {
            AddGlaiveTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddGlaiveTokens()
        {
            string prefix = GlaiveSurvivor.GLAIVE_PREFIX;

            string desc = "The Swarm Shepherd is an aerial melee fighter that uses swarm statuses to improve his abilities.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Glaive Strike is a fast all-rounder melee tool." + Environment.NewLine + Environment.NewLine
             + "< ! > Extraction let's you gain status buffs, improving speed, damage, and defense." + Environment.NewLine + Environment.NewLine
             + "< ! > While already able to jump higher than other survivors, Vault increases your aerial damage output and provides excellent mobility." + Environment.NewLine + Environment.NewLine
             + "< ! > Swarm Crash is the perfect tool to finish off a large group of enemies." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Swarm Shepherd");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Lord of the Flies");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Swarm Statuses");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Bugs extract a temporary swarm status, which buffs <style=cIsUtility>mobility</style>, <style=cIsDamage>damage</style>, or <style=cIsHealing>defense</style>. Having multiple types improves each status.");

            Language.Add(prefix + "BUG_CUTTING_NAME", "Bug Type: Cutting");
            Language.Add(prefix + "BUG_CUTTING_DESCRIPTION", "Swarm has a 10% chance to inflict <style=cIsHealth>bleed</style>. deals " + Tokens.DamageValueText(GlaiveStaticValues.cuttingDamageCoefficient));

            Language.Add(prefix + "BUG_IMPACT_NAME", "Bug Type: Impact");
            Language.Add(prefix + "BUG_IMPACT_DESCRIPTION", "Swarm has a 10% chance to stun. deals " + Tokens.DamageValueText(GlaiveStaticValues.impactDamageCoefficient));


            Language.Add(prefix + "BUG_BRUISER_NAME", "Bug Style: Bruiser");
            Language.Add(prefix + "BUG_BRUISER_DESCRIPTION", "Swarm twice as fast and 20% stronger. Increased extraction range.");
     
            Language.Add(prefix + "BUG_AILMENT_NAME", "Bug Style: Ailment");
            Language.Add(prefix + "BUG_AILMENT_DESCRIPTION", "If swarm is Cutting type, it now has a 15% chance to cause <style=cIsHealth>hemorrhaging</style>. If swarm is Impact type, it now has a 15% chance to deal a stunning blast.");
  
            Language.Add(prefix + "BUG_POWDER_NAME", "Bug Style: Powder");
            Language.Add(prefix + "BUG_POWDER_DESCRIPTION", "Upon extraction, a bug will attach a powder that explodes on hit for " + Tokens.DamageText("6 x 150% damage."));

            Language.Add(prefix + "BUG_GLUTTON_NAME", "Bug Style: Dual Bug");
            Language.Add(prefix + "BUG_GLUTTON_DESCRIPTION", "Extractions will bounce to one more enemy, but is 20% weaker.");

            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_SLASH_NAME", "Glaive Strike");
            Language.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Tokens.agilePrefix + $" Swing forward for <style=cIsDamage>{100f * GlaiveStaticValues.glaiveDamageCoefficient}% damage</style>. If airborne, swing slightly faster.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Extraction");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Tokens.agilePrefix + $" Send a bug and retrieve a one of three Swarm Statuses from an enemy: {Tokens.whiteBugPrefix}, {Tokens.redBugPrefix}, or {Tokens.orangeBugPrefix}. Bug deals <style=cIsDamage>100% bug damage</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Vault");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Become temporarily <style=cIsUtility>invincible</style>, vaulting upwards into the air (or horizontally if airborne). Deal <style=cIsDamage>200% damage</style>, gaining " + Tokens.airBugPrefix + " and <style=cIsUtility>resetting jump count</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Hurricane Dive");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Deal a <style=cIsDamage>200% damage</style> multi-hit downward, and deal an additional <style=cIsDamage>1200% damage</style> that is affected by " + Tokens.airBugPrefix + ". <style=cIsUtility>Jump cancel for half the cooldown.</style>");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(GlaiveMasteryAchievements.identifier), "Glaive: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(GlaiveMasteryAchievements.identifier), "As Glaive, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
