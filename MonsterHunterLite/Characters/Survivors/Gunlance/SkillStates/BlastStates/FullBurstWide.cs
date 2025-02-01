using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    public class FullBurstWide : ChargedBlastBase
    {
        public override void OnEnter()
        {
            baseDuration = .5f;
            base.OnEnter();

            
            isFullBurst = true;
            shotCount = base.skillLocator.secondary.stock;

            //change is made for the fact that it inherits from wide, which already shoots the first shot OnEnter
            if (base.skillLocator.secondary.stock > 0)
            {
                timer = new float[shotCount];
                flag = new bool[shotCount];
            }
           

            for (int i = 0; i < timer.Length; i++)
            {
                timer[i] = UnityEngine.Random.Range(0f, duration);
                flag[i] = false;
            }

            base.skillLocator.secondary.RemoveAllStocks();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Ray aimRay = base.GetAimRay();
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
                for (int i = 0; i < timer.Length; i++)
                {
                    if (base.fixedAge > timer[i] && !flag[i])
                    {
                        FireShot();
                        FireShot(rightInnerRay);
                        FireShot(rightOuterRay);
                        FireShot(leftInnerRay);
                        FireShot(leftOuterRay);
                        flag[i] = true;
                    }
                }
            }
        }
    }
}
