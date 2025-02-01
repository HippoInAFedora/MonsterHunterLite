using EntityStates;
using MonsterHunterMod.Characters.Survivors.Glaive;
using MonsterHunterMod.Survivors.Glaive;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Glaive.SkillStates
{
    internal class GlaiveCharacterMain : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {         
            if (base.isGrounded && base.characterBody.HasBuff(GlaiveBuffs.airborneDamageBuff))
            {
                base.characterBody.RemoveBuff(GlaiveBuffs.airborneDamageBuff);
            }
            if (!base.isGrounded)
            {
                base.characterBody.isSprinting = true;             
            }
            base.FixedUpdate();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
        }
    }
}
