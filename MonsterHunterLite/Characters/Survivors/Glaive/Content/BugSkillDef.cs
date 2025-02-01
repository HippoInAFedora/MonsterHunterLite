
using RoR2;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using RoR2.Skills;
using JetBrains.Annotations;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content
{
    public class BugSkillDef : SkillDef
    {
        protected class InstanceData : BaseSkillInstanceData
        {
            public GlaiveTracker glaiveTracker;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                glaiveTracker = skillSlot.GetComponent<GlaiveTracker>()
            };
        }

        public override void OnUnassigned([NotNull] GenericSkill skillSlot)
        {
            base.OnUnassigned(skillSlot);
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            if (!(((InstanceData)skillSlot.skillInstanceData).glaiveTracker?.GetTrackingTarget()))
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            if (!HasTarget(skillSlot))
            {
                return false;
            }
            return base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (base.IsReady(skillSlot))
            {
                return HasTarget(skillSlot);
            }
            return false;
        }
    }
}