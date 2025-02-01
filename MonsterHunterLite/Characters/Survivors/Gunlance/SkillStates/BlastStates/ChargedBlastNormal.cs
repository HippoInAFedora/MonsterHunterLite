using EntityStates;
using UnityEngine;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class ChargedBlastNormal : ChargedBlastBase
    {

        Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            timer = new float[chargeLevel];
            flag = new bool[chargeLevel];

            for (int i = 0; i < timer.Length; i++)
            {
                timer[i] = i == 0 ? 0f : UnityEngine.Random.Range(0f, duration);
                flag[i] = false;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            idealPos = aimRay.GetPoint(10f);
            if (base.isAuthority)
            {
                for (int i = 0; i < timer.Length; i++)
                {
                    if (base.fixedAge > timer[i] && !flag[i])
                    {
                        FireShot();
                        flag[i] = true;
                    }
                }
            }
        }
    }
}
