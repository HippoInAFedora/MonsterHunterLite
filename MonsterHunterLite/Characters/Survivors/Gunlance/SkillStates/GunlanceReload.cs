using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class GunlanceReload : BaseSkillState
    {
        //baseDuration here controls percentages, not a full duration aspect.
        public float baseDuration = 1f;
        public float duration;
        public GunlanceShellController.ShellingInfo shellInfo;
        public bool reloadsPrimary = false;
        public bool reloadsSecondary = false;
        public bool isFullReload = false;
        public override void OnEnter()
        {
            duration = baseDuration / attackSpeedStat;
            base.OnEnter();
            GunlanceShellController shellController = base.GetComponent<GunlanceShellController>();
            if (shellController != null )
            {
                shellInfo = shellController.ReturnShellingInfo();
            }
            if (base.characterMotor)
            {
                base.characterMotor.velocity.x = 0f;
                base.characterMotor.velocity.z = 0f;
            }
            if (base.isAuthority)
            {
                if (!isFullReload)
                {
                    //if (reloadsPrimary)
                    //{ 
                    //    int reloadCount = base.skillLocator.primary.maxStock + base.skillLocator.secondary.bonusStockFromBody - base.skillLocator.primary.stock < 2 ? base.skillLocator.primary.maxStock - base.skillLocator.primary.stock : 2;
                    //    ReloadPrimaryStock(reloadCount);
                    //}
                    //if (reloadsSecondary)
                    //{
                    //    int reloadCount = base.skillLocator.secondary.maxStock - base.skillLocator.secondary.stock < shellInfo.maxShells ? base.skillLocator.secondary.maxStock - base.skillLocator.secondary.stock : shellInfo.maxShells;
                    //    ReloadSecondaryStock(reloadCount);
                    //}
                    if (reloadsPrimary)
                    {
                        int reloadCount = base.skillLocator.primary.maxStock + base.skillLocator.secondary.bonusStockFromBody - base.skillLocator.primary.stock;
                        ReloadPrimaryStock(reloadCount);
                    }
                    if (reloadsSecondary)
                    {
                        int reloadCount = base.skillLocator.secondary.maxStock - base.skillLocator.secondary.stock;
                        ReloadSecondaryStock(reloadCount);
                    }
                }
                if (isFullReload)
                {
                    if (reloadsPrimary)
                    {
                        int reloadCount = base.skillLocator.primary.maxStock + base.skillLocator.secondary.bonusStockFromBody - base.skillLocator.primary.stock;
                        ReloadPrimaryStock(reloadCount);
                    }
                    if (reloadsSecondary)
                    {
                        int reloadCount = base.skillLocator.secondary.maxStock - base.skillLocator.secondary.stock;
                        ReloadSecondaryStock(reloadCount);
                    }
                }
                
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor)
            {
                base.characterMotor.velocity.x = 0f;
                base.characterMotor.velocity.z = 0f;
            }
            if (base.isAuthority && base.fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void ReloadPrimaryStock(int i)
        {
            for (int j = 0; j < i; j++)
            {
                base.skillLocator.primary.AddOneStock();
            }
        }

        private void ReloadSecondaryStock(int i)
        {
            for (int j = 0; j < i; j++)
            {
                base.skillLocator.secondary.AddOneStock();
            }
        }

    }
}

