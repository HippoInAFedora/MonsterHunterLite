using MonsterHunterMod.Characters.Survivors.Glaive.SkillStates;
using MonsterHunterMod.Characters.Survivors.Glaive.SkillStates.old;
using MonsterHunterMod.Survivors.Glaive.SkillStates;

namespace MonsterHunterMod.Survivors.Glaive
{
    public static class GlaiveStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(GlaiveCharacterMain));

            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(AimBugMain));
            Modules.Content.AddEntityState(typeof(ShootBugMain));

            Modules.Content.AddEntityState(typeof(VaultState));
            //Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(HurricaneDive));
            Modules.Content.AddEntityState(typeof(CrashEnd));
            //Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
