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
    public class GunlanceSteppedSKillDef : SteppedSkillDef
    {

        public SerializableEntityStateType baseStateType;

        public SerializableEntityStateType finisherStateType;

        public SerializableEntityStateType reloadStateType;

        public SerializableEntityStateType jumpStateType;

        public SerializableEntityStateType guardState;

        public override EntityState InstantiateNextState(GenericSkill skillSlot)
        {
            EntityState entityState = base.InstantiateNextState(skillSlot);
            InstanceData instanceData = (InstanceData)skillSlot.skillInstanceData;
            EntityStateMachine stateMachine = EntityStateMachine.FindByCustomName(skillSlot.gameObject, "Guard");
            if (stateMachine && stateMachine.state is GuardState)
            {
                return EntityStateCatalog.InstantiateState(ref guardState);
            }
            if (entityState is IStepSetter stepSetter)
            {
                stepSetter.SetStep(instanceData.step);
            }
            bool num = IsGrounded(skillSlot);
            if (!num)
            {
                return EntityStateCatalog.InstantiateState(ref jumpStateType);
            }
            if (instanceData.step == stepCount - 1)
            {
                if (skillSlot.stock > 0)
                {
                    return EntityStateCatalog.InstantiateState(ref finisherStateType);
                }
                else
                {
                    return EntityStateCatalog.InstantiateState(ref reloadStateType);
                }
            }
            return EntityStateCatalog.InstantiateState(ref baseStateType);
        }

        public bool IsGrounded([NotNull] GenericSkill skillSlot)
        {
            CharacterBody characterBody = skillSlot.GetComponent<CharacterBody>();
            if (skillSlot?.stateMachine.state.GetType() != jumpStateType.stateType)
            {
                if ((bool)characterBody.characterMotor && characterBody.characterMotor.isGrounded)
                {
                    return true;
                }
            }           
            return false;
        }

    }
}