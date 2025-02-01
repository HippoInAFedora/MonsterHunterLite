using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Skills;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class SteppedThrust : Thrust, SteppedSkillDef.IStepSetter
    {
        //baseDuration here controls percentages, not a full duration aspect.
        //public float baseDuration = 1.2f;
        //public float thrustDelayPercentTime = .5f;
        //public float thrustCompletePercentTime = .7f;
        //public float duration;

        public int step;

        //public float stopwatch = 0f;
        //public GunlanceShellController controller;

        public void SetStep(int i)
        {
            step = i;
        }

        //public override void OnEnter()
        //{
        //    duration = baseDuration / attackSpeedStat;
        //    controller = base.GetComponent<GunlanceShellController>();
        //    base.OnEnter();
        //    if (base.isAuthority)
        //    {
        //        Fire();
        //    }
        //}
        //public override void OnExit()
        //{
        //    base.OnExit();
        //}

        //public override InterruptPriority GetMinimumInterruptPriority()
        //{
        //    if (base.fixedAge <  thrustDelayPercentTime)
        //    {
        //        return InterruptPriority.PrioritySkill;
        //    }
        //    return InterruptPriority.Any;
        //}

        //public override void FixedUpdate()
        //{
        //    base.FixedUpdate();
        //    if (base.isAuthority)
        //    {
        //        //if (base.fixedAge > duration * thrustDelayPercentTime && IsKeyDownAuthority())
        //        //{
        //        //    if (skillLocator.primary.stock > 0)
        //        //    {
        //        //        outer.SetNextState(new WyrmStake());
        //        //    }
        //        //    else
        //        //    {
        //        //        outer.SetNextState(new GunlanceReload{
        //        //            reloadsPrimary = true
        //        //        });
        //        //    }
        //        //}
        //        if (base.fixedAge > duration * thrustCompletePercentTime)
        //        {
        //            outer.SetNextStateToMain();
        //        }}
        //}

        //public void Fire()
        //{
        //    BulletAttack bullet = new BulletAttack();
        //    bullet.owner = base.gameObject;
        //    bullet.weapon = base.gameObject;
        //    bullet.origin = base.GetAimRay().origin;
        //    bullet.aimVector = base.GetAimRay().direction;
        //    bullet.force = 500f;
        //    bullet.radius = 3f;
        //    bullet.bulletCount = 1;
        //    bullet.stopperMask = LayerIndex.defaultLayer.mask;
        //    bullet.isCrit = base.RollCrit();
        //    bullet.procCoefficient = 1f;
        //    bullet.maxSpread = 0f;
        //    bullet.minSpread = 0f;
        //    bullet.maxDistance = 12f;
        //    bullet.damage = base.characterBody.damage * GunlanceStaticValues.thrustDamageCoefficient;
        //    bullet.Fire();
        //}
        

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

