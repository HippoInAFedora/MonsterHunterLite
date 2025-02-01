using MonsterHunterMod.Survivors.Glaive;
using IL.RoR2.Mecanim;
using Newtonsoft.Json.Bson;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Components
{
    internal class BlastOnStrikeBuffBehavior : MonoBehaviour
    {

        TemporaryVisualEffect blastBugPowderEffectInstance = new TemporaryVisualEffect();
        public CharacterBody body;
        public float stopwatch = 0f;
        public bool flag;
        public float duration = .5f;


        public void Start()
        {
            body = GetComponent<CharacterBody>();
            UpdateEffect();       
        }


        public void OnDestroy()
        {

            if ((bool)body && NetworkServer.active)
            {
                if (body.HasBuff(GlaiveBuffs.blastOnStrike))
                {
                    body.RemoveBuff(GlaiveBuffs.blastOnStrike);
                }
                //if (body.HasBuff(GlaiveBuffs.blastOnStrikeVisualCooldown))
                //{
                //    body.RemoveBuff(GlaiveBuffs.blastOnStrikeVisualCooldown);
                //}
            }
        }

        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > duration)
            {
                UpdateEffect();
                stopwatch = 0f;
                
            }
        }

        private void UpdateEffect()
        {
            if (body != null)
            {
                body.UpdateSingleTemporaryVisualEffect(ref blastBugPowderEffectInstance, GlaiveAssets.blastBugAttachEffect, 1f, body.HasBuff(GlaiveBuffs.blastOnStrike));
            }        
        }
    }
}
