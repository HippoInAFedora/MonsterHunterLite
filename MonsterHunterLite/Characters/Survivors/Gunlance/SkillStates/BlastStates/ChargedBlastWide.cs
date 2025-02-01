using EntityStates;
using UnityEngine;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    public class ChargedBlastWide : ChargedBlastBase
    {

        Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            Vector3 rhs1 = Vector3.Cross(Vector3.up, aimRay.direction);
            Vector3 axis = Vector3.Cross(aimRay.direction, rhs1);
            Quaternion quaternionRightInner = Quaternion.AngleAxis(15, axis);
            Quaternion quaternionRightOuter = Quaternion.AngleAxis(30, axis);
            Quaternion quaternionLeftInner = Quaternion.AngleAxis(-15, axis);
            Quaternion quaternionLeftOuter = Quaternion.AngleAxis(-30, axis);

            Vector3 rightAngleInner = quaternionRightInner * aimRay.direction.normalized;
            Vector3 rightAngleOuter = quaternionRightOuter * aimRay.direction.normalized;
            Vector3 leftAngleInner = quaternionLeftInner * aimRay.direction.normalized;
            Vector3 leftAngleOuter = quaternionLeftOuter * aimRay.direction.normalized;
            Ray rightInnerRay = new Ray(aimRay.origin, rightAngleInner);
            Ray rightOuterRay = new Ray(aimRay.origin, rightAngleOuter);
            Ray leftInnerRay = new Ray(aimRay.origin, leftAngleInner);
            Ray leftOuterRay = new Ray(aimRay.origin, leftAngleOuter);
            if (base.isAuthority)
            {
                FireShot();
                FireShot(rightInnerRay);
                FireShot(rightOuterRay);
                FireShot(leftInnerRay);
                FireShot(leftOuterRay);
            }
        }

    }
}
