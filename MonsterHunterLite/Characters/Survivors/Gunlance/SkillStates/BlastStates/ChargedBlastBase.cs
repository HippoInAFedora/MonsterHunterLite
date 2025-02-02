using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;
using MonsterHunterMod.Survivors.Glaive;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    public class ChargedBlastBase : BaseSkillState
    {
        public GunlanceShellController shell;
        public GunlanceShellController.ShellingInfo shellInfo;

        public GameObject blastEffectPrefab = GlaiveAssets.blastBugEffect;

        public Transform shellTransform;

        public float[] timer;
        public bool[] flag;
        public Vector3 idealPos;
        public Vector3[] idealPosExtended;

        public float baseDuration = .5f;
        public float duration;
        public int shotCount = 0;
        public int chargeLevel = 1;

        public bool isFullBurst = false;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            shell = GetComponent<GunlanceShellController>();
            if (shell != null)
            {
                shellInfo = shell.ReturnShellingInfo();
            }
            float hopStrength = 15 + (shellInfo.shotsPerShell * 2 * (shotCount + chargeLevel));
            if (base.characterMotor != null && !base.isGrounded)
            {
                base.SmallHop(characterMotor, hopStrength);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
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

        public void FireBlast(Vector3 position)
        {
            float damage = isFullBurst ? shellInfo.burstDamage : shellInfo.baseDamage;
            BlastAttack blast = new BlastAttack();
            blast.attacker = gameObject;
            blast.inflictor = gameObject;
            blast.position = position + UnityEngine.Random.insideUnitSphere * shellInfo.shotSpread;
            blast.attackerFiltering = AttackerFiltering.NeverHitSelf;
            blast.crit = RollCrit();
            blast.teamIndex = teamComponent.teamIndex;
            blast.falloffModel = BlastAttack.FalloffModel.Linear;
            blast.radius = chargeLevel > 1 || isFullBurst ? shellInfo.chargedRadius : shellInfo.baseRadius;
            blast.baseDamage = chargeLevel > 1 ? characterBody.damage * damage * ((chargeLevel - 1) * shellInfo.chargeMult) : characterBody.damage * damage;
            blast.Fire();
        }

        public void FireShot()
        {
            FireShot(GetAimRay());
        }

        public void FireShot(Ray aimRay)
        {
            float damage = isFullBurst ? shellInfo.burstDamage : shellInfo.baseDamage * (1 + ((chargeLevel - 1) * shellInfo.chargeMult));
            BulletAttack bullet = new BulletAttack();
            bullet.owner = gameObject;
            bullet.weapon = gameObject;
            bullet.origin = aimRay.origin;
            bullet.aimVector = aimRay.direction;
            bullet.isCrit = RollCrit();
            bullet.falloffModel = BulletAttack.FalloffModel.None;
            bullet.stopperMask = LayerIndex.noCollision.mask;
            bullet.radius = chargeLevel > 1 || isFullBurst ? shellInfo.chargedRadius : shellInfo.baseRadius;
            bullet.maxDistance = chargeLevel > 1 ? shellInfo.chargedDistance : shellInfo.baseDistance;
            bullet.damage = characterBody.damage * damage;
            bullet.Fire();
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                origin = aimRay.GetPoint(bullet.maxDistance - 5f),
                scale = bullet.radius * 1.5f
            }, transmit: true);
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                origin = aimRay.GetPoint(bullet.maxDistance / 3),
                scale = bullet.radius * 1.5f
            }, transmit: true);
        }

    }
}
