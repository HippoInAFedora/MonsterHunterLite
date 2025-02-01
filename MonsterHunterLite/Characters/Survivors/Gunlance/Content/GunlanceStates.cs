using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Thrust));
            Modules.Content.AddEntityState(typeof(ChargingBlast));
            Modules.Content.AddEntityState(typeof(ChargedBlastBase));
            Modules.Content.AddEntityState(typeof(GunlanceReload));

            Modules.Content.AddEntityState(typeof(Sweep));
            Modules.Content.AddEntityState(typeof(Slam));
            Modules.Content.AddEntityState(typeof(BlastDash));
            Modules.Content.AddEntityState(typeof(ChargingBlastDash));
            Modules.Content.AddEntityState(typeof(WyrmStake));
        }
    }
}
