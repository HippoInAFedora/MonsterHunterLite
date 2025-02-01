using BepInEx.Configuration;
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs;
using MonsterHunterMod.Characters.Survivors.Glaive.SkillStates;
using MonsterHunterMod.Modules;
using MonsterHunterMod.Modules.Characters;
using MonsterHunterMod.Survivors.Glaive.Components;
using MonsterHunterMod.Survivors.Glaive.SkillStates;
using RoR2.Orbs;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R2API;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using UnityEngine.Networking;
using EntityStates;
using HG;
using UnityEngine.UIElements;
using System.ComponentModel;

namespace MonsterHunterMod.Survivors.Glaive
{
    public class GlaiveSurvivor : SurvivorBase<GlaiveSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "monsterhunterliteassetbundle"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "GlaiveBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "GlaiveMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlGlaive";
        public override string displayPrefabName => "GlaiveDisplay";

        public const string GLAIVE_PREFIX = MonsterHunterPlugin.DEVELOPER_PREFIX + "_GLAIVE_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => GLAIVE_PREFIX;

        public static List<string> whiteFlagNames = new List<string>
        {
            "VultureBody(Clone)",
            "BisonBody(Clone)",
            
            "ChildBody(Clone)",
            "ParentBody(Clone)",
            "GipBody(Clone)",
            "GreaterWispBody(Clone)",
            "JellyfishBody(Clone)",
            "AcidLarvaBody(Clone)",
            "WispBody(Clone)",
            "LunarExploderBody(Clone)",
            "RoboBallMiniBody"
        };

        public static List<string> redFlagNames = new List<string>
        {
            //"VultureBody(Clone)",
            //"BeetleBody(Clone)",
            //"BeetleGuardBody(Clone)",
            "FlyingVerminBody(Clone)",
            "VerminBody(Clone)",
            "BellBody(Clone)",
            //"ClayGrenadierBody(Clone)",
            "ClayBruiserBody(Clone)",
            "LemurianBruiserBody(Clone)",
            "GeepBody(Clone)",
            "GreaterWispBody(Clone)",
            "ImpBody(Clone)",
            "AcidLarvaBody(Clone)",
            "LemurianBody(Clone)",
            "LunarWispBody(Clone)",

        };

        public static List<string> orangeFlagNames = new List<string>
        {
            "MinorConstructBody(Clone)",
            "BeetleBody(Clone)",
            "BeetleGuardBody(Clone)",
            "ClayGrenadierBody(Clone)",
            "GupBody(Clone)",
            "HalcyoniteBody(Clone)",
            "HermitCrabBody(Clone)",
            "LunarGolemBody(Clone)",
            "MiniMushroomBody(Clone)",
            "ScorchlingBody(Clone)",
            "GolemBody(Clone)",
        };


        public static TemporaryVisualEffect blastBugPowderEffectInstance = new TemporaryVisualEffect();

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = GLAIVE_PREFIX + "NAME",
            subtitleNameToken = GLAIVE_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 90f,
            healthRegen = 2.5f,
            armor = 0f,

            jumpCount = 1,
            jumpPower = 17,
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

        public override UnlockableDef characterUnlockableDef => GlaiveUnlockables.characterUnlockableDef;
        
        //public override ItemDisplaysBase itemDisplays => new GlaiveItemDisplays();

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
            ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Glaive");

            if (!characterEnabled.Value)
            {
                return;
            }
                

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            GlaiveUnlockables.Init();

            base.InitializeCharacter();

            GlaiveConfig.Init();
            GlaiveStates.Init();
            GlaiveTokens.Init();
            

            GlaiveAssets.Init(assetBundle);
            GlaiveBuffs.Init(assetBundle);
            GlaiveDamageTypes.SetupModdedDamage();

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
            bodyPrefab.AddComponent<GlaiveWeaponComponent>();
            bodyPrefab.AddComponent<GlaiveTracker>();
            if (bodyPrefab.GetComponent<GlaiveTracker>() != null )
            {
                bodyPrefab.GetComponent<GlaiveTracker>().maxTrackingDistance = 50f;
                //bodyPrefab.GetComponent<GlaiveTracker>().enabled = false;
            }
            bodyPrefab.AddComponent<GlaiveHUD>();
            //anything else here
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();
            Transform airHitboxTransform = childLocator.FindChild("AirHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordGroup", "SwordHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordGroupAir", airHitboxTransform);
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(GlaiveCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            bodyPrefab.AddComponent<GlaiveBugTypeController>();
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
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = GLAIVE_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "PASSIVE_DESCRIPTION",
                //keywordToken = "KEYWORD_STUNNING",
                icon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            };

            GlaiveBugTypeController controller = bodyPrefab.GetComponent<GlaiveBugTypeController>();
            controller.bugTypeSkillSlot = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "BugTypeSkill");
            controller.cuttingSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugTypeCutting",
                skillNameToken = GLAIVE_PREFIX + "BUG_CUTTING_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_CUTTING_DESCRIPTION",
                //keywordTokens = new string[] { "KEYWORD_BLEED" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

            controller.impactSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugTypeImpact",
                skillNameToken = GLAIVE_PREFIX + "BUG_IMPACT_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_IMPACT_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_STUNNING" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

            });
            Skills.AddSkillsToFamily(controller.bugTypeSkillSlot.skillFamily, controller.cuttingSkillDef, controller.impactSkillDef);

            controller.bugStyleSkillSlot = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "BugStyleSkill");
            controller.bruiserSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugStyleBruiser",
                skillNameToken = GLAIVE_PREFIX + "BUG_BRUISER_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_BRUISER_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

            controller.ailmentSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugStyleAilment",
                skillNameToken = GLAIVE_PREFIX + "BUG_AILMENT_NAME",
                keywordTokens = new string[] { "KEYWORD_SUPERBLEED", "KEYWORD_SHOCKING"},
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_AILMENT_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

            controller.powderSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugStylePowder",
                skillNameToken = GLAIVE_PREFIX + "BUG_POWDER_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_POWDER_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

            controller.gluttonSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BugStyleGlutton",
                skillNameToken = GLAIVE_PREFIX + "BUG_GLUTTON_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "BUG_GLUTTON_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });
            Skills.AddSkillsToFamily(controller.bugStyleSkillSlot.skillFamily, controller.bruiserSkillDef, controller.ailmentSkillDef, controller.powderSkillDef, controller.gluttonSkillDef);
        }

        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef primarySkillDef1 = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "GlaiveStrike",
                    GLAIVE_PREFIX + "PRIMARY_SLASH_NAME",
                    GLAIVE_PREFIX + "PRIMARY_SLASH_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.SlashCombo)),
                    "Weapon",
                    true
                ));
            //custom Skilldefs can have additional fields that you can set manually
            primarySkillDef1.stepCount = 2;
            primarySkillDef1.stepGraceDuration = 0.5f;

            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef1);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            //here is a basic skill def with all fields accounted for
            SkillDef secondarySkillDef1 = Skills.CreateSkillDef<BugSkillDef>(new SkillDefInfo
            {
                skillName = "BugAim",
                skillNameToken = GLAIVE_PREFIX + "SECONDARY_GUN_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "SECONDARY_GUN_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE", Tokens.whiteBugKeyword, Tokens.redBugKeyword, Tokens.orangeBugKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(AimBugMain)),

                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Any,

                baseRechargeInterval = 5f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,

                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef1);

        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            //here's a skilldef of a typical movement skill.
            SkillDef utilitySkillDef1 = Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = "GlaiveVault",
                skillNameToken = GLAIVE_PREFIX + "UTILITY_ROLL_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "UTILITY_ROLL_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                keywordTokens = new string[] { Tokens.airBugKeyword },

                activationState = new EntityStates.SerializableEntityStateType(typeof(VaultState)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 7f,
                baseMaxStock = 2,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });
            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef1);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            //a basic skill. some fields are omitted and will just have default values
            SkillDef specialSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "GlaiveCrash",
                skillNameToken = GLAIVE_PREFIX + "SPECIAL_BOMB_NAME",
                skillDescriptionToken = GLAIVE_PREFIX + "SPECIAL_BOMB_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                keywordTokens = new string[] { Tokens.airBugKeyword },

                activationState = new EntityStates.SerializableEntityStateType(typeof(HurricaneDive)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Body", interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseMaxStock = 1,
                baseRechargeInterval = 10f,

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
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(GLAIVE_PREFIX + "MASTERY_SKIN_NAME",
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
            GlaiveAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.AddBuff_BuffDef += CharacterBody_AddBuff_BuffDef;
        }

        private void CharacterBody_AddBuff_BuffDef(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (buffDef == GlaiveBuffs.stunBlast)
            {
                self.RemoveBuff(GlaiveBuffs.stunBlast);
                BullseyeSearch search = new BullseyeSearch();
                search.Reset();
                search.searchOrigin = self.transform.position;
                search.searchDirection = Vector3.zero;
                search.teamMaskFilter = TeamMask.allButNeutral;
                search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                search.filterByLoS = false;
                search.sortMode = BullseyeSearch.SortMode.Distance;
                search.filterByDistinctEntity = true;
                search.maxDistanceFilter = 10f;
                search.RefreshCandidates();
                IEnumerable<HurtBox> enumerable = search.GetResults();
                foreach (HurtBox box in enumerable)
                {
                    box.healthComponent.body.TryGetComponent<SetStateOnHurt>(out var setStateOnHurt);
                    if (setStateOnHurt.targetStateMachine)
                    {
                        StunState stunState = new StunState();
                        stunState.stunDuration = 2f;
                        setStateOnHurt.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Stun);
                    }
                    EntityStateMachine[] idleStateMachine = setStateOnHurt.idleStateMachine;
                    for (int i = 0; i < idleStateMachine.Length; i++)
                    {
                        idleStateMachine[i].SetNextStateToMain();
                    }
                }
                EffectData effectData = new EffectData();
                effectData.origin = self.transform.position;
                effectData.scale = 10f * 2;
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/BootShockwave"), effectData, transmit: true);
            }
        }

        //private void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody self)
        //{
        //    orig(self);
        //    self.UpdateSingleTemporaryVisualEffect(ref blastBugPowderEffectInstance, GlaiveAssets.blastBugAttachEffect, self.radius, self.HasBuff(GlaiveBuffs.blastOnStrike));
        //}

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            orig(self, buffDef, duration);
            if (!NetworkServer.active)
            {
                return;
            }
            if (self.bodyIndex == MonsterHunterPlugin.glaiveIndex)
            {
                GlaiveHUD hud = self.GetComponent<GlaiveHUD>();
                if (buffDef == GlaiveBuffs.whiteBugBuff)
                {
                    hud.timers[1] = duration;
                }
                if (buffDef == GlaiveBuffs.redBugBuff)
                {
                    hud.timers[0] = duration;
                }
                if (buffDef == GlaiveBuffs.orangeBugBuff)
                {
                    hud.timers[2] = duration;
                }
            }
        }


        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            CharacterBody component = victim.GetComponent<CharacterBody>();
            if (component == null)
            {
                return;
            }
            if (damageInfo.attacker != null)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                bool flag = component.HasBuff(GlaiveBuffs.blastOnStrike);

                if (attackerBody != null)
                {
                    if (flag)
                    {
                        if (component.TryGetComponent(out SetStateOnHurt setStateOnHurt))
                        {
                            if (setStateOnHurt.targetStateMachine)
                            {
                                BlastBugDetonate blastBugDetonate = new BlastBugDetonate();
                                blastBugDetonate.attacker = damageInfo.attacker;
                                blastBugDetonate.attackerBody = attackerBody;
                                setStateOnHurt.targetStateMachine.SetInterruptState(blastBugDetonate, InterruptPriority.Frozen);
                            }
                            EntityStateMachine[] array = setStateOnHurt.idleStateMachine;
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i].SetNextStateToMain();
                            };
                        }

                    }
                    if (victim == null || !damageInfo.HasModdedDamageType(GlaiveDamageTypes.glaiveHit) || attackerBody.bodyIndex != MonsterHunterPlugin.glaiveIndex)
                    {
                        return;
                    }
                    //red bug multiplier
                    int numWhite = attackerBody.HasBuff(GlaiveBuffs.whiteBugBuff) ? 1 : 0;
                    int numRed = attackerBody.HasBuff(GlaiveBuffs.redBugBuff) ? 1 : 0;
                    int numOrange = attackerBody.HasBuff(GlaiveBuffs.orangeBugBuff) ? 1 : 0;
                    int numAir = attackerBody.GetBuffCount(GlaiveBuffs.airborneDamageBuff);
                    float num = numRed + ((numWhite + numOrange) / 2) + (numAir / 3);
                    if (attackerBody.HasBuff(GlaiveBuffs.redBugBuff))
                    {
                        if (attackerBody.TryGetComponent(out GlaiveBugTypeController controller))
                        {
                            GlaiveBugTypeController.BugInfo bugInfo = controller.ReturnBugInfo();
                            if (bugInfo.debuff == RoR2Content.Buffs.SuperBleed || bugInfo.debuff == GlaiveBuffs.stunBlast)
                            {
                                bugInfo.debuff = Util.CheckRoll(bugInfo.rollChance, attackerBody.master) ? bugInfo.debuff : null;
                            }
                            GlaiveOrb genericDamageOrb = new GlaiveOrb();
                            genericDamageOrb.damageValue = attackerBody.damage * (bugInfo.baseDamage / 2) * (1f + num / 2);
                            genericDamageOrb.isCrit = damageInfo.crit;
                            genericDamageOrb.teamIndex = TeamComponent.GetObjectTeam(attackerBody.gameObject);
                            genericDamageOrb.attacker = attackerBody.gameObject;
                            genericDamageOrb.procCoefficient = 1f;
                            genericDamageOrb.debuff = bugInfo.debuff == RoR2Content.Buffs.SuperBleed ? bugInfo.debuff : null;
                            genericDamageOrb.damageType = Util.CheckRoll(bugInfo.rollChance, attackerBody.master) ? bugInfo.bugTypeDamageType : DamageType.Generic;
                            if ((bool)component)
                            {
                                Transform transform = attackerBody.transform;
                                genericDamageOrb.origin = transform.position;
                                genericDamageOrb.target = component.mainHurtBox;
                                OrbManager.instance.AddOrb(genericDamageOrb);
                            }
                        }
                    }
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            int numWhite = sender.HasBuff(GlaiveBuffs.whiteBugBuff) ? 1 : 0;
            int numRed = sender.HasBuff(GlaiveBuffs.redBugBuff) ? 1 : 0;
            int numOrange = sender.HasBuff(GlaiveBuffs.orangeBugBuff) ? 1 : 0;
            int numAir = sender.GetBuffCount(GlaiveBuffs.airborneDamageBuff);
            float num = numWhite + numRed + numOrange + (numAir / 3);
            if (sender.HasBuff(GlaiveBuffs.whiteBugBuff))
            {
                args.baseMoveSpeedAdd += .15f + .05f * num;
                args.cooldownMultAdd -= .15f + .05f * num;
            }
            if (sender.HasBuff(GlaiveBuffs.redBugBuff))
            {
                //args.damageMultAdd *= 1.25f + .25f * num; base damage, when I wanted total damage increase.
                args.attackSpeedMultAdd += .15f + .05f * num;
            }
            if (sender.HasBuff(GlaiveBuffs.orangeBugBuff))
            {
                args.armorAdd += 20 + 5 * num;
                args.regenMultAdd += .1f + .05f * num;
            }

            //int numRevWhite = sender.HasBuff(GlaiveBuffs.reverseWhiteBugBuff) ? 1 : 0;
            //int numRevRed = sender.HasBuff(GlaiveBuffs.reverseRedBugBuff) ? 1 : 0;
            //int numRevOrange = sender.HasBuff(GlaiveBuffs.reverseOrangeBugBuff) ? 1 : 0;
            //int numRev = numRevWhite + numRevRed + numRevOrange;
            //if (sender.HasBuff(GlaiveBuffs.reverseWhiteBugBuff))
            //{
            //    args.baseMoveSpeedAdd *= .8f - .05f * numRev;
            //    args.cooldownMultAdd -= .2f + .05f * numRev;
            //}
            //if (sender.HasBuff(GlaiveBuffs.reverseRedBugBuff))
            //{
            //    //args.damageMultAdd *= 1.25f + .25f * num; base damage, when I wanted total damage increase.
            //    args.attackSpeedMultAdd -= .125f + .125f * numRev;
            //}
            //if (sender.HasBuff(GlaiveBuffs.reverseOrangeBugBuff))
            //{
            //    args.armorAdd -= 25 + 25 * numRev;
            //    args.regenMultAdd -= .875f - .125f * numRev;
            //}
        }
    }
}