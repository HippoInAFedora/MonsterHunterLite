using BepInEx;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Misc._Content;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Greatsword;
using MonsterHunterMod.Survivors.Gunlance;
using R2API.Utils;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace MonsterHunterMod
{
    //[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class MonsterHunterPlugin : BaseUnityPlugin
    {
        // if you do not change this, you are giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.hippo.MonsterHunterMod";
        public const string MODNAME = "MonsterHunterMod";
        public const string MODVERSION = "1.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "HIPPO";

        public static MonsterHunterPlugin instance;

        public static BodyIndex glaiveIndex;
        public static BodyIndex gunlanceIndex;

        public static GameObject hudInstance;

        void Awake()
        {
            instance = this;

            //easy to use logger
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            // character initialization
            new GlaiveSurvivor().Initialize();
            new GunlanceSurvivor().Initialize();
            new GreatswordSurvivor().Initialize();

            // make a content pack and add it. this has to be last
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private static void Hook()
        {
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            On.RoR2.UI.HUD.Awake += HUD_Awake;
            On.RoR2.UI.HUD.Update += HUD_Update;
        }

        private static IEnumerator BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            yield return orig();
            glaiveIndex = BodyCatalog.FindBodyIndex("GlaiveBody(Clone)");
            gunlanceIndex = BodyCatalog.FindBodyIndex("GunlanceBody(Clone)");
        }

        private static void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, RoR2.UI.HUD self)
        {
            orig(self);
            CharacterBody body = self.targetBodyObject ? self.targetBodyObject.GetComponent<CharacterBody>() : null;
           
            if (body && body.bodyIndex == glaiveIndex && hudInstance != null)
            {
                hudInstance.SetActive(true);
                GlaiveHUD glaiveHUD = body.GetComponent<GlaiveHUD>();

                bool[] buffFlag = new bool[3];
                buffFlag[1] = body.HasBuff(GlaiveBuffs.whiteBugBuff);
                buffFlag[0] = body.HasBuff(GlaiveBuffs.redBugBuff);
                buffFlag[2] = body.HasBuff(GlaiveBuffs.orangeBugBuff);

                bool[] airFlag = new bool[3];
                airFlag[0] = body.GetBuffCount(GlaiveBuffs.airborneDamageBuff) > 0;
                airFlag[1] = body.GetBuffCount(GlaiveBuffs.airborneDamageBuff) > 1;
                airFlag[2] = body.GetBuffCount(GlaiveBuffs.airborneDamageBuff) > 2;

                //main indicators
                for (int i = 0; i < glaiveHUD.timers.Length; i++)
                {
                    bool flag = hudInstance.transform.GetChild(i).GetChild(1).gameObject.activeSelf ? true : false;
                    if (glaiveHUD.timers[i] <= 0f)
                    {
                        if (!buffFlag[i] && flag)
                        {
                            hudInstance.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        }
                    }
                    if (buffFlag[i])
                    {
                        if (glaiveHUD.timers[i] >= 20f)
                        {
                            if (!flag)
                            {
                                hudInstance.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                                hudInstance.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                            }
                            hudInstance.transform.GetChild(i).GetChild(1).gameObject.GetComponent<GlowBreathe>().state = GlowBreathe.State.stopped;
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.GetComponent<GlowBreathe>().state = GlowBreathe.State.stopped;
                        }
                        if (5 <= glaiveHUD.timers[i] && glaiveHUD.timers[i] < 15f)
                        {
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.GetComponent<GlowBreathe>().state = GlowBreathe.State.breathing;
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.GetComponent<GlowBreathe>().speed = 1f;
                        }
                        if (0 < glaiveHUD.timers[i] && glaiveHUD.timers[i] < 5f)
                        {
                            hudInstance.transform.GetChild(i).GetChild(1).gameObject.GetComponent<GlowBreathe>().state = GlowBreathe.State.breathing;
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.GetComponent<GlowBreathe>().state = GlowBreathe.State.breathing;
                            hudInstance.transform.GetChild(i).GetChild(1).gameObject.GetComponent<GlowBreathe>().speed = .4f;
                            hudInstance.transform.GetChild(i).GetChild(2).gameObject.GetComponent<GlowBreathe>().speed = .4f;
                        }
                    }
                }

                //air indicators
                for (int j = 0; j < airFlag.Length; j++)
                {
                    if (!airFlag[j])
                    {
                        hudInstance.transform.GetChild(j + 3).GetChild(1).gameObject.SetActive(false);
                        hudInstance.transform.GetChild(j + 3).GetChild(2).gameObject.SetActive(false);
                    }
                    if (airFlag[j])
                    {
                        hudInstance.transform.GetChild(j + 3).GetChild(1).gameObject.SetActive(true);
                        hudInstance.transform.GetChild(j + 3).GetChild(2).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                hudInstance.SetActive(false);
            }
        }

        private static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);
            CharacterBody body = self.targetBodyObject ? self.targetBodyObject.GetComponent<CharacterBody>() : null;
            hudInstance = UnityEngine.Object.Instantiate(GlaiveAssets.bugStatuses);
            hudInstance.transform.SetParent(self.mainContainer.transform);
            RectTransform component = hudInstance.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(.8f, 0f);
            component.anchorMax = new Vector2(1, .3f);
            component.pivot = new Vector2(1, 0);
            component.anchoredPosition = new Vector2(1f, 0);
        }
    }
}
