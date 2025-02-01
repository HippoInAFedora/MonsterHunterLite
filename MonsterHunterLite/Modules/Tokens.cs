using MonsterHunterMod.Survivors.Gunlance;

namespace MonsterHunterMod.Modules
{
    internal static class Tokens
    {
        public const string agilePrefix = "<style=cIsUtility>Agile.</style>";

        public const string whiteBugPrefix = "<style=cIsUtility>White Extraction</style>";

        public const string redBugPrefix = "<style=cIsHealth>Red Extraction</style>";

        public const string orangeBugPrefix = "<style=cIsDamage>Orange Extraction</style>";

        public const string airBugPrefix = "<style=cIsDamage>Blue Extraction</style>";

        public static string whiteBugKeyword = KeywordText("White Extraction", "Found mainly in <style=cIsUtility>fast and flying enemies</style>, along with <style=cIsHealth>bosses</style>. <style=cIsUtility>Increase movement speed and lower cooldowns</style>. Swarm Status improved with other types.");

        public static string redBugKeyword = KeywordText("Red Extraction", "Found mainly in <style=cIsHealth>ground based, high-damage enemies</style>, along with <style=cIsHealth>bosses</style>. All glaive hits strike an additional <style=cIsDamage>50% of bug damage</style>, and <style=cIsHealth>attack speed is increased</style>. Swarm Status improved with other types.");

        public static string orangeBugKeyword = KeywordText("Orange Extraction", "Found mainly in <style=cIsDamage>large or slow enemies</style>, along with <style=cIsHealth>bosses</style>. <style=cIsHealing>Gain armor and increase health regen</style>. Swarm Status improved with other types.");

        public static string airBugKeyword = KeywordText("Blue Extraction", "Inert extraction activated upon vaulting. <style=cIsDamage>Increases damage by 120%</style> and boosts all other Swarm Statuses. Extraction removed on touching the ground. Can stack 3 times.");

        public static string gunlanceSlamKeyword = KeywordText("Slam", $"While airborne, instead slam for <style=cIsDamage>{100 * GunlanceStaticValues.slamDamageCoefficient}</style>. After slamming, secondary can unload all shells in a Full Burst!");

        public static string DamageText(string text)
        {
            return $"<style=cIsDamage>{text}</style>";
        }
        public static string DamageValueText(float value)
        {
            return $"<style=cIsDamage>{value * 100}% damage</style>";
        }
        public static string UtilityText(string text)
        {
            return $"<style=cIsUtility>{text}</style>";
        }
        public static string RedText(string text) => HealthText(text);
        public static string HealthText(string text)
        {
            return $"<style=cIsHealth>{text}</style>";
        }
        public static string KeywordText(string keyword, string sub)
        {
            return $"<style=cKeywordName>{keyword}</style><style=cSub>{sub}</style>";
        }
        public static string ScepterDescription(string desc)
        {
            return $"\n<color=#d299ff>SCEPTER: {desc}</color>";
        }

        /// <summary>
        /// gets langauge token from achievement's registered identifier
        /// </summary>
        public static string GetAchievementNameToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_NAME";
        }
        /// <summary>
        /// gets langauge token from achievement's registered identifier
        /// </summary>
        public static string GetAchievementDescriptionToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_DESCRIPTION";
        }
    }
}