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

namespace MonsterHunterMod.Survivors.Glaive
{
    public static class GlaiveAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;
        public static GameObject circleSlashEffect;

        public static GameObject orbWhiteBug;
        public static GameObject orbRedBug;
        public static GameObject orbOrangeBug;
        public static GameObject orbBugBug;

        public static GameObject bugCrosshair;
        public static GameObject bugStatuses;

        public static Sprite airBugBuffIcon;
        public static Sprite whiteBugBuffIcon;
        public static Sprite redBugBuffIcon;
        public static Sprite orangeBugBuffIcon;

        public static GameObject blastBugAttachEffect;
        public static GameObject blastBugEffect;

        public static GameObject bombExplosionEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("HenrySwordHit");

            CreateSprites();

            CreateEffects();

            CreateProjectiles();
        }

        private static void CreateSprites()
        {
            airBugBuffIcon = _assetBundle.LoadAsset<Sprite>("AirBugBuffIcon");
            whiteBugBuffIcon = _assetBundle.LoadAsset<Sprite>("WhiteBugBuffIcon");
            redBugBuffIcon = _assetBundle.LoadAsset<Sprite>("RedBugBuffIcon");
            orangeBugBuffIcon = _assetBundle.LoadAsset<Sprite>("OrangeBugBuffIcon");



            bugCrosshair = _assetBundle.LoadAsset<GameObject>("GlaiveCrosshair");
            bugCrosshair.transform.GetChild(0).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>().isAnOddVariation = true; ;
            bugCrosshair.transform.GetChild(1).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>().isAnOddVariation = true; ;
            bugCrosshair.transform.GetChild(2).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>().isAnOddVariation = true; ;
            bugCrosshair.transform.GetChild(4).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>().isAnOddVariation = true;
            bugStatuses = _assetBundle.LoadAsset<GameObject>("SwarmStatusHolder");
            bugStatuses.transform.GetChild(0).transform.GetChild(1).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(1).transform.GetChild(1).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(2).transform.GetChild(1).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(0).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(1).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(2).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();

            bugStatuses.transform.GetChild(3).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(4).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();
            bugStatuses.transform.GetChild(5).transform.GetChild(2).gameObject.AddComponent<GlowBreathe>();
        }

        #region effects
        private static void CreateEffects()
        {
            CreateBombExplosionEffect();
            CreateOrbVariants();
            swordSwingEffect = _assetBundle.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = _assetBundle.LoadEffect("ImpactHenrySlash");
            circleSlashEffect = _assetBundle.LoadEffect("SlashCircle", true);
            circleSlashEffect.AddComponent<NetworkIdentity>();
            circleSlashEffect.AddComponent<EffectComponent>();
            circleSlashEffect.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(circleSlashEffect);

            blastBugAttachEffect = _assetBundle.LoadAsset<GameObject>("BlastBugAttachEffect");
            blastBugAttachEffect.AddComponent<NetworkIdentity>();
            //blastBugAttachEffect.AddComponent<EffectComponent>();
            //Content.CreateAndAddEffectDef(blastBugAttachEffect);

            blastBugEffect = _assetBundle.LoadEffect("BlastBugDetonationEffect");
            blastBugEffect.AddComponent<NetworkIdentity>();
            blastBugEffect.AddComponent<EffectComponent>();
            blastBugEffect.GetComponent<EffectComponent>().applyScale = true;
            Content.CreateAndAddEffectDef(blastBugEffect);
        }

        private static void CreateOrbVariants()
        {
            Color bugColor = Color.gray;
            orbBugBug = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/ArrowOrbEffect.prefab").WaitForCompletion(), "OrbBug");
            orbBugBug.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_TintColor", bugColor);
            orbBugBug.AddComponent<NetworkIdentity>();
            Content.CreateAndAddEffectDef(orbBugBug);

            Color bugWhiteColor = Color.white;
            orbWhiteBug = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/ArrowOrbEffect.prefab").WaitForCompletion(), "OrbWhiteBug");
            orbWhiteBug.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_TintColor", bugWhiteColor);
            orbWhiteBug.AddComponent<NetworkIdentity>();
            Content.CreateAndAddEffectDef(orbWhiteBug);

            Color bugRedColor = Color.red;
            orbRedBug = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/ArrowOrbEffect.prefab").WaitForCompletion(), "OrbRedBug");
            orbRedBug.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_TintColor", bugRedColor);
            orbRedBug.AddComponent<NetworkIdentity>();
            Content.CreateAndAddEffectDef(orbRedBug);

            Color bugOrangeColor = Color.yellow;
            orbOrangeBug = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/ArrowOrbEffect.prefab").WaitForCompletion(), "OrbOrangeBug");
            orbOrangeBug.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_TintColor", bugOrangeColor);
            orbOrangeBug.AddComponent<NetworkIdentity>();
            Content.CreateAndAddEffectDef(orbOrangeBug);

        }

        private static void CreateBombExplosionEffect()
        {
            bombExplosionEffect = _assetBundle.LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (!bombExplosionEffect)
                return;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 40f,
                cycleOffset = 0f
            };

        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateBombProjectile();
            Content.AddProjectilePrefab(bombProjectilePrefab);
        }

        private static void CreateBombProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            bombProjectilePrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            //remove their ProjectileImpactExplosion component and start from default values
            UnityEngine.Object.Destroy(bombProjectilePrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileImpactExplosion bombImpactExplosion = bombProjectilePrefab.AddComponent<ProjectileImpactExplosion>();
            
            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.blastDamageCoefficient = 1f;
            bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = bombExplosionEffect;
            bombImpactExplosion.lifetimeExpiredSound = Content.CreateAndAddNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("HenryBombGhost") != null)
                bombController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("HenryBombGhost");
            
            bombController.startSound = "";
        }
        #endregion projectiles
    }
}
