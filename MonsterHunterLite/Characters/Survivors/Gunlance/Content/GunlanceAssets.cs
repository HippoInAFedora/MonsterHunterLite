using RoR2;
using UnityEngine;
using MonsterHunterMod.Modules;
using System;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using R2API;
using UnityEngine.Networking;
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Misc._Content;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public static class GunlanceAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;
        public static GameObject circleSlashEffect;

        public static GameObject wyrmStake;
        public static GameObject wyrmStakeImpact;

        public static GameObject wyvernFire;

        public static GameObject tempThrustTracer;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;
            CreateEffects();
            //CreateProjectiles();
        }

        private static void CreateEffects()
        {
            wyrmStake = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/TracerToolbotRebar.prefab").WaitForCompletion(), "WyrmStakeTracer");
            wyrmStake.AddComponent<NetworkIdentity>();
            wyrmStake.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(wyrmStake);
            wyrmStakeImpact = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ImpactSpear.prefab").WaitForCompletion(), "WyrmStakeImpact");
            wyrmStakeImpact.AddComponent<NetworkIdentity>();
            wyrmStakeImpact.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(wyrmStakeImpact);

            wyvernFire = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgun.prefab").WaitForCompletion(), "WyvernFireTracer");
            wyvernFire.AddComponent<NetworkIdentity>();
            wyvernFire.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(wyvernFire);


            tempThrustTracer = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/TracerHuntressSnipe.prefab").WaitForCompletion(), "TempThrustTracer");
            tempThrustTracer.AddComponent<NetworkIdentity>();
            tempThrustTracer.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(tempThrustTracer);
        }

        #region projectiles
        private static void CreateProjectiles()
        {
            //CreateBombProjectile();
            //Content.AddProjectilePrefab(wyrmStake);
        }

        private static void CreateBombProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            //wyrmStake = _assetBundle.LoadAndAddProjectilePrefab("WyrmStake");
            //wyrmStake.layer = LayerIndex.projectile.intVal;
            //ProjectileImpactDot dot = wyrmStake.AddComponent<ProjectileImpactDot>();
            //if (dot != null)
            //{
            //    dot.dotIndex = GunlanceDots.gunlanceDot;
            //    dot.dotDuration = 2f;
            //    dot.dotDamageMultiplier = 1;
            //    dot.calculateTotalDamage = false;
            //}
            //if (wyrmStake.TryGetComponent(out ProjectileImpactExplosion component))
            //{
            //    component.impactEffect = LegacyResourcesAPI.Load<GameObject>("RoR2/Base/Toolbot/ImpactNailgun.prefab");
            //    component.lifetimeAfterImpact = 2.25f;
            //    component.blastRadius = 5f;
            //}
            //PrefabAPI.RegisterNetworkPrefab(wyrmStake);
        }
        #endregion projectiles
    }
}
