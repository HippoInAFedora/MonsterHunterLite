using MonsterHunterMod.Modules;
using MonsterHunterMod.Modules.Characters;
using MonsterHunterMod.Survivors.Gunlance.Components;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using EntityStates;
using BepInEx.Configuration;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates;
using R2API;
using MonsterHunterMod.Characters.Survivors.Gunlance.Content;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;
using System;
using MonsterHunterMod.Characters.Survivors.Glaive.SkillStates;
using MonsterHunterMod.Survivors.Glaive;
using System.ComponentModel;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;

namespace MonsterHunterMod.Survivors.Gunlance
{
    public class GunlanceSurvivor : SurvivorBase<GunlanceSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "monsterhunterliteassetbundle"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "GunlanceBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "GunlanceMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlGunlance";
        public override string displayPrefabName => "GunlanceDisplay";

        public const string GUNLANCE_PREFIX = MonsterHunterPlugin.DEVELOPER_PREFIX + "_GUNLANCE_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => GUNLANCE_PREFIX;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = GUNLANCE_PREFIX + "NAME",
            subtitleNameToken = GUNLANCE_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 160f,
            healthRegen = 2.5f,
            armor = 0f,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = assetBundle.LoadMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => GunlanceUnlockables.characterUnlockableDef;
        
        //public override ItemDisplaysBase itemDisplays => new GunlanceItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Gunlance");

            if (!characterEnabled.Value)
            {
                return;
            }


            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            GunlanceUnlockables.Init();

            base.InitializeCharacter();

            GunlanceConfig.Init();
            GunlanceStates.Init();
            GunlanceTokens.Init();

            GunlanceAssets.Init(assetBundle);
            GunlanceBuffs.Init(assetBundle);
            GunlanceDamage.SetupModdedDamage();

            bodyPrefab.AddComponent<GunlanceShellController>();

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<GunlanceWeaponComponent>();
            //anything else here
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();
            Transform slamHitboxTransform = childLocator.FindChild("SlamHitbox");
            Transform blastDashHitboxTransform = childLocator.FindChild("BlastDashHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SlamGroup", slamHitboxTransform);
            Prefabs.SetupHitBoxGroup(characterModelObject, "BlastDashGroup", blastDashHitboxTransform);
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Guard");
        }

        #region skills

        public static SkillDef secondaryOverrideSkillDef;
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
           
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        //skip if you don't have a passive
        //also skip if this is your first look at skills
        private void AddPassiveSkill()
        {
            //option 1.fake passive icon just to describe functionality we will implement elsewhere
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = GUNLANCE_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "PASSIVE_DESCRIPTION",
                //keywordToken = "KEYWORD_STUNNING",
                icon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            };

            //option 2. a new SkillFamily for a passive, used if you want multiple selectable passives
            GunlanceShellController controller = bodyPrefab.GetComponent<GunlanceShellController>();
            controller.passiveSkillSlot = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "ShellingStyle");
            controller.normalShellSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "NormalShelling",
                skillNameToken = GUNLANCE_PREFIX + "NORMAL_SHELLING_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "NORMAL_SHELLING_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

            });

            controller.longShellSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "LongShelling",
                skillNameToken = GUNLANCE_PREFIX + "LONG_SHELLING_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "LONG_SHELLING_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

            });

            controller.wideShellSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "WideShelling",
                skillNameToken = GUNLANCE_PREFIX + "WIDE_SHELLING_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "WIDE_SHELLING_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

            Skills.AddSkillsToFamily(controller.passiveSkillSlot.skillFamily, controller.normalShellSkillDef, controller.longShellSkillDef, controller.wideShellSkillDef);
        }

        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            GunlanceSteppedSKillDef primarySkillDef1 = Skills.CreateSkillDef<GunlanceSteppedSKillDef>(new SkillDefInfo
                (
                    "Thrust",
                    GUNLANCE_PREFIX + "PRIMARY_THRUST_NAME",
                    GUNLANCE_PREFIX + "PRIMARY_THRUST_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(Thrust)),
                    "Weapon",
                    false
                ));
            primarySkillDef1.keywordTokens = new string[] { Tokens.gunlanceSlamKeyword, Tokens.wyrmStakeKeyword };
            primarySkillDef1.baseStateType = new EntityStates.SerializableEntityStateType(typeof(SteppedThrust));
            primarySkillDef1.jumpStateType = new EntityStates.SerializableEntityStateType(typeof(Slam));
            primarySkillDef1.finisherStateType = new EntityStates.SerializableEntityStateType(typeof(WyrmStake));
            primarySkillDef1.reloadStateType = new EntityStates.SerializableEntityStateType(typeof(PrimaryReload));
            primarySkillDef1.guardState = new EntityStates.SerializableEntityStateType(typeof(Thrust));
            primarySkillDef1.hideStockCount = false;
            primarySkillDef1.stepGraceDuration = .25f;
            primarySkillDef1.stepCount = 4;
            primarySkillDef1.requiredStock = 0;
            primarySkillDef1.rechargeStock = 0;
            primarySkillDef1.baseMaxStock = 2;
            primarySkillDef1.interruptPriority = InterruptPriority.Skill;
            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef1);

            //Override skilldef for jump state
            secondaryOverrideSkillDef = Skills.CreateSkillDef(new SkillDefInfo
                (
                    "GunlanceFullBurst",
                    GUNLANCE_PREFIX + "PRIMARY_THRUST_NAME",
                    GUNLANCE_PREFIX + "PRIMARY_THRUST_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(FullBurstSwitch)),
                    "Weapon",
                    false
                ));
            secondaryOverrideSkillDef.requiredStock = 0;
            secondaryOverrideSkillDef.rechargeStock = 0;
            secondaryOverrideSkillDef.interruptPriority = InterruptPriority.PrioritySkill;
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            //here is a basic skill def with all fields accounted for
            GunlanceSkillDef secondarySkillDef1 = Skills.CreateSkillDef<GunlanceSkillDef>(new SkillDefInfo
            {
                skillName = "ChargedShelling",
                skillNameToken = GUNLANCE_PREFIX + "SECONDARY_SHELLING_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "SECONDARY_SHELLING_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ChargingBlast)),

                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0f,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,

                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });
            secondarySkillDef1.guardState = new EntityStates.SerializableEntityStateType(typeof(FullReload));

            GunlanceSkillDef secondarySkillDef2 = Skills.CreateSkillDef<GunlanceSkillDef>(new SkillDefInfo
            {
                skillName = "BlastDash",
                skillNameToken = GUNLANCE_PREFIX + "SECONDARY_BLAST_DASH_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "SECONDARY_BLAST_DASH_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ChargingBlastDash)),

                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                keywordTokens = new string[] { "KEYWORD_HEAVY" },

                baseRechargeInterval = 0f,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,

                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });
            secondarySkillDef2.guardState = new EntityStates.SerializableEntityStateType(typeof(FullReload));
            Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef1, secondarySkillDef2);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            //here's a skilldef of a typical movement skill.
            SkillDef utilitySkillDef1 = Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = "GunlanceGuard",
                skillNameToken = GUNLANCE_PREFIX + "UTILITY_GUARD_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "UTILITY_GUARD_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(GuardState)),
                activationStateMachineName = "Guard",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });
            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef1);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            //a basic skill. some fields are omitted and will just have default values
            SkillDef specialSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "WurmFire",
                skillNameToken = GUNLANCE_PREFIX + "SPECIAL_DEVASTATION_NAME",
                skillDescriptionToken = GUNLANCE_PREFIX + "SPECIAL_DEVASTATION_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(WyvernFire)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Body", interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseMaxStock = 1,
                baseRechargeInterval = 70f,

                isCombatSkill = true,
                mustKeyPress = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, specialSkillDef1);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(GUNLANCE_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            GunlanceAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (victim == null)
            {
                return;
            }
            CharacterBody component = victim.GetComponent<CharacterBody>();
            if (component == null)
            {
                return;
            }
            if (damageInfo.attacker != null)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();

                if (component == null || !damageInfo.HasModdedDamageType(GunlanceDamage.wyrmStakeDamage) || attackerBody.bodyIndex != MonsterHunterPlugin.gunlanceIndex)
                {
                    return;
                }
                if (component.TryGetComponent(out SetStateOnHurt setStateOnHurt))
                {
                    if (setStateOnHurt.targetStateMachine)
                    {
                        WyrmStakeBlast wyrmStakeDetonate = new WyrmStakeBlast();
                        wyrmStakeDetonate.attacker = damageInfo.attacker;
                        wyrmStakeDetonate.attackerBody = attackerBody;
                        setStateOnHurt.targetStateMachine.SetInterruptState(wyrmStakeDetonate, InterruptPriority.Frozen);
                    }
                    EntityStateMachine[] array = setStateOnHurt.idleStateMachine;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].SetNextStateToMain();
                    };
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(GunlanceBuffs.gunlanceGuardBuff))
            {
                args.armorAdd += 500f + sender.level * 25f;
            }
        }
    }
}