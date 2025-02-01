using MonsterHunterMod.Characters.Survivors.Greatsword.SkillStates;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Slam));
            Modules.Content.AddEntityState(typeof(SwordCharge));

        }
    }
}
