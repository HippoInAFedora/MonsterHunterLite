using System;
using MonsterHunterMod.Modules;
using MonsterHunterMod.Survivors.Gunlance.Achievements;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceTokens
    {
        public static void Init()
        {
            AddGunlanceTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddGunlanceTokens()
        {
            string prefix = GunlanceSurvivor.GUNLANCE_PREFIX;

            string desc = "Rally Point Guard is a heavy-hitting hybrid survivor which manages a unique ammo system<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Thrust." + Environment.NewLine + Environment.NewLine
             + "< ! > Sweep." + Environment.NewLine + Environment.NewLine
             + "< ! > Reload." + Environment.NewLine + Environment.NewLine
             + "< ! > Contextual." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            Language.Add(prefix + "NAME", "Rally Point Guard");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Guard of the Survivors");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Ammo Stocks");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Ammo is stored in Cooldowns. It does not recharge autmatically; rather, ammo must be reloaded when out of stocks or by manually reloading. Quick reloading reloads up to base stocks, while manual reloading reloads ALL stocks.");

            Language.Add(prefix + "NORMAL_SHELLING_NAME", "Normal Shelling");
            Language.Add(prefix + "NORMAL_SHELLING_DESCRIPTION", $"Hold 6 shells. Shells deal <style=cIsDamage>{100 * GunlanceStaticValues.normalDamageCoefficient}%</style>. Charging increases shells unloaded. Full burst shells deal <style=cIsDamage>{100 * GunlanceStaticValues.normalDamageCoefficientBurst}% damage</style>");

            Language.Add(prefix + "LONG_SHELLING_NAME", "Long Shelling");
            Language.Add(prefix + "LONG_SHELLING_DESCRIPTION", $"Hold 3 shells. Shells deal <style=cIsDamage>{100 * GunlanceStaticValues.longDamageCoefficient}%</style> at extended range.");

            Language.Add(prefix + "WIDE_SHELLING_NAME", "Wide Shelling");
            Language.Add(prefix + "WIDE_SHELLING_DESCRIPTION", $"Hold 2 shells. Each shell deals <style=cIsDamage>5 * {100 * GunlanceStaticValues.wideDamageCoefficient}%</style>. Can charge up to two times.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_THRUST_NAME", "Thrust & WurmStake");
            Language.Add(prefix + "PRIMARY_THRUST_DESCRIPTION", $"Thrust forward for <style=cIsDamage>{100 * GunlanceStaticValues.thrustDamageCoefficient}%</style>. After 3 strikes, shoot a WurmStake for <style=cIsDamage>10 * 25% + {100 * GunlanceStaticValues.wyrmStake}% damage</style> or Quick Reload. Primary stocks dictate current WurmStake stock.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_SHELLING_NAME", "Charged Shelling");
            Language.Add(prefix + "SECONDARY_SHELLING_DESCRIPTION", $"Charge and shoot a shell, or Quick Reload if empty. Bounce upwards on shot if airborne. Full burst if used immediately after landing a slam.");

            Language.Add(prefix + "SECONDARY_BLAST_DASH_NAME", "Blast Dash");
            Language.Add(prefix + "SECONDARY_BLAST_DASH_DESCRIPTION", $"<style=cIsUtility>Heavy</style>. Charging converts a shell into a blast dash!");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_GUARD_NAME", "Guard");
            Language.Add(prefix + "UTILITY_GUARD_DESCRIPTION", "Hold to guard, gaining <style=cIsUtility>500 armor</style>. Primary skill can only thrust, and Secondary skill reloads all ammo.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_DEVASTATION_NAME", "WurmFire");
            Language.Add(prefix + "SPECIAL_DEVASTATION_DESCRIPTION", $"Gain <style=cIsUtility>500 armor</style>, bracing yourself for 4s, then shoot a devastating beam for <style=cIsDamage>{100 * GunlanceStaticValues.wyvernFireCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(GunlanceMasteryAchievements.identifier), "Gunlance: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(GunlanceMasteryAchievements.identifier), "As Gunlance, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
