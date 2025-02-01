﻿using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    internal class FullBurstNormal : ChargedBlastBase
    {
        Ray aimRay;

        public override void OnEnter()
        {
            baseDuration = .5f;
            base.OnEnter();
            
            isFullBurst = true;
            shotCount = base.skillLocator.secondary.stock;
            aimRay = base.GetAimRay();
            timer = new float[shotCount];
            flag = new bool[shotCount];

            for (int i = 0; i < timer.Length; i++)
            {
                timer[i] = i == 0 ? 0f : UnityEngine.Random.Range(0f, duration);
                flag[i] = false;
            }
            base.skillLocator.secondary.RemoveAllStocks();
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
