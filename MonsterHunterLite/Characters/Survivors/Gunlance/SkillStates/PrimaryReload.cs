using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using RoR2.Skills;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class PrimaryReload : GunlanceReload, SteppedSkillDef.IStepSetter
    {
        public int step;
        public override void OnEnter()
        {
            reloadsPrimary = true;
            base.OnEnter();
        }

        public void SetStep(int i)
        {
            step = i;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            step = reader.ReadByte();
        }
    }
}

