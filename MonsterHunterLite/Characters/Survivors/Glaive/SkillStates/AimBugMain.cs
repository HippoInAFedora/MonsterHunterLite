using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Characters.Survivors.Glaive.SkillStates;

namespace MonsterHunterMod.Survivors.Glaive.SkillStates
{
    public class AimBugMain : BaseSkillState
    {
        private GlaiveTracker glaiveTracker;

        public EntityState nextState;

        public GlaiveBugTypeController bugType;

        public override void OnEnter()
        {
            glaiveTracker = base.GetComponent<GlaiveTracker>();
            glaiveTracker.enabled = true;
            base.OnEnter();
            
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)glaiveTracker && base.isAuthority)
            {
                outer.SetNextState(new ShootBugMain());
            }
        }
 
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}