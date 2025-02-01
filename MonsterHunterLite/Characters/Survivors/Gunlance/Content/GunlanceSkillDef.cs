using RoR2;
using RoR2.Skills;
using JetBrains.Annotations;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using UnityEngine;
using EntityStates.LunarGolem;
using EntityStates;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.Content
{
    public class GunlanceSkillDef : SkillDef
    {
        public SerializableEntityStateType guardState;

        public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
        {
            EntityState entityState = base.InstantiateNextState(skillSlot);
            EntityStateMachine stateMachine = EntityStateMachine.FindByCustomName(skillSlot.gameObject, "Guard");
            if (stateMachine && stateMachine.state is GuardState)
            {
                return EntityStateCatalog.InstantiateState(ref guardState);
            }
            return EntityStateCatalog.InstantiateState(ref activationState);
        }

        public override int GetMaxStock([NotNull] GenericSkill skillSlot)
        {
            return ReturnShellingStock(skillSlot);
        }

        public int ReturnShellingStock([NotNull] GenericSkill skillSlot)
        {
            GunlanceShellController shellController = skillSlot.GetComponent<GunlanceShellController>();
            if (shellController)
            {
                skillSlot.maxStock = shellController.ReturnShellingInfo().maxShells;
                return shellController.ReturnShellingInfo().maxShells;
            }
            Debug.Log("someting wen wron");
            return 0;
        }
    }
}