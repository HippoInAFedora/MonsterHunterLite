
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Misc._Content;
using MonsterHunterMod.Survivors.Glaive;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Components
{
    public class GlaiveTracker : MonoBehaviour
    {
        public GameObject trackingPrefab;

        public float maxTrackingDistance = 50f;

        public float maxTrackingAngle = 20f;

        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private CharacterBody characterBody;
        private CharacterBody enemyBody;
        private String enemyName;

        private TeamComponent teamComponent;

        private InputBankTest inputBank;

        private float trackerUpdateStopwatch;

        private Indicator indicator;

        private readonly BullseyeSearch search = new BullseyeSearch();

        public List<string> whiteFlagNames = GlaiveSurvivor.whiteFlagNames;

        public List<string> redFlagNames = GlaiveSurvivor.redFlagNames;

        public List<string> orangeFlagNames = GlaiveSurvivor.orangeFlagNames;

        private Color whiteColor = Color.white;
        private Color redColor = Color.red;
        private Color orangeColor = new Color(1f, .5f, 0f);
        private Color unknownColor = new Color(173 / 255, 29 / 255, 245 / 255);

        private Color whiteGlowColor = Color.white;
        private Color redGlowColor = Color.red;
        private Color orangeGlowColor = new Color(1f, .5f, 0f);

        private void Awake()
        {
            if (trackingPrefab == null)
            {
                trackingPrefab = GlaiveAssets.bugCrosshair;
            }
            indicator = new Indicator(base.gameObject, trackingPrefab);
        }

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            inputBank = GetComponent<InputBankTest>();
            teamComponent = GetComponent<TeamComponent>();
            GlaiveBugTypeController controller = GetComponent<GlaiveBugTypeController>();
            if (controller != null)
            {
                GlaiveBugTypeController.BugInfo bugInfo = controller.ReturnBugInfo();
                maxTrackingDistance = bugInfo.trackingRange;
            }
            //order was a little messed up in unity. white is last, red is first.
            indicator.visualizerInstance.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
            indicator.visualizerInstance.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            indicator.visualizerInstance.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
        }

        public HurtBox GetTrackingTarget()
        {
            return trackingTarget;
        }

        private void OnEnable()
        {
            indicator.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
        }

        private void Update()
        {
            //order was a little messed up in unity. white is last, red is first.
            indicator.visualizerInstance.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(characterBody.HasBuff(GlaiveBuffs.whiteBugBuff) ? true : false);
            indicator.visualizerInstance.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(characterBody.HasBuff(GlaiveBuffs.redBugBuff) ? true : false);
            indicator.visualizerInstance.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(characterBody.HasBuff(GlaiveBuffs.orangeBugBuff) ? true : false);

            trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
                _ = trackingTarget;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);              
            }

            
        }

        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.all;
            search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = maxTrackingDistance;
            search.maxAngleFilter = maxTrackingAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);
            trackingTarget = search.GetResults().FirstOrDefault();

            if (trackingTarget)
            {
                enemyBody = trackingTarget.healthComponent.body;
                if (enemyBody != null)
                {
                    indicator.visualizerInstance.transform.GetChild(3).transform.GetChild(1).GetComponent<SpriteRenderer>().color = ReturningColorType();
                    indicator.visualizerInstance.transform.GetChild(4).transform.GetChild(1).GetComponent<SpriteRenderer>().color = ReturningColorType();
                    indicator.visualizerInstance.transform.GetChild(5).transform.GetChild(1).GetComponent<SpriteRenderer>().color = ReturningColorType();
                }

                //color glow logic
                if (ReturningColorType() == whiteColor && !characterBody.HasBuff(GlaiveBuffs.whiteBugBuff))
                {
                    indicator.visualizerInstance.transform.GetChild(4).transform.GetChild(2).GetComponent<GlowBreathe>().color = whiteGlowColor;
                    ColorGlowSwapper(2);
                }
                else if (ReturningColorType() == redColor && !characterBody.HasBuff(GlaiveBuffs.redBugBuff))
                {
                    indicator.visualizerInstance.transform.GetChild(4).transform.GetChild(2).GetComponent<GlowBreathe>().color = redGlowColor;
                    ColorGlowSwapper(0);
                }
                else if (ReturningColorType() == orangeColor && !characterBody.HasBuff(GlaiveBuffs.orangeBugBuff))
                {
                    indicator.visualizerInstance.transform.GetChild(4).transform.GetChild(2).GetComponent<GlowBreathe>().color = orangeGlowColor;
                    ColorGlowSwapper(1);
                }
                else
                {
                    indicator.visualizerInstance.transform.GetChild(4).transform.GetChild(2).GetComponent<GlowBreathe>().color = Color.clear;
                    ColorGlowSwapper(-1);
                }

            }
        }

        private Color ChooseColor(bool[] bodyFlags)
        {
            if (bodyFlags[0] != true)
            {
                return whiteColor;
            }
            else if (bodyFlags[1] != true)
            {
                return redColor;
            }
            else if (bodyFlags[2] != true)
            {
                return orangeColor;
            }
            else return unknownColor;
        }


        public Color ReturningColorType()
        {
            enemyName = enemyBody.name;
            bool[] bodyFlags = new bool[3];
            bodyFlags[0] = false;
            bodyFlags[1] = false;
            bodyFlags[2] = false;
            if (characterBody)
            {
                if (!characterBody.HasBuff(GlaiveBuffs.whiteBugBuff))
                {
                    bodyFlags[0] = true;
                }
                if (!characterBody.HasBuff(GlaiveBuffs.redBugBuff))
                {
                    bodyFlags[1] = true;
                }
                if (!characterBody.HasBuff(GlaiveBuffs.orangeBugBuff))
                {
                    bodyFlags[2] = true;
                }
            }
            bool flag = enemyBody.HasBuff(DLC1Content.Buffs.EliteVoid) ? true : false;
            if (flag)
            {
                return unknownColor;
            }

            bool flag2 = enemyBody.isBoss || enemyBody.isChampion ? true : false;
            if (flag2)
            {
                return ChooseColor(bodyFlags);
            }

            if (whiteFlagNames.Contains(enemyName))
            {
                return whiteColor;
            }
            if (redFlagNames.Contains(enemyName))
            {
                return redColor;
            }
            if (orangeFlagNames.Contains(enemyName))
            {
                return orangeColor;
            }

            Debug.LogWarning("[MonsterHunterMod] enemy has no set buff or is modded.");
            return ChooseColor(bodyFlags);
        }

        private void ColorGlowSwapper(int childNumber)
        {
            bool[] notChildNumber = new bool[3];
            notChildNumber[0] = childNumber == 0 ? false : true;
            notChildNumber[1] = childNumber == 1 ? false : true;
            notChildNumber[2] = childNumber == 2 ? false : true;
            for (int i = 0; i < 3; i++)
            {
                if (notChildNumber[i] == true)
                {
                    indicator.visualizerInstance.transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            if (childNumber >= 0)
            {
                indicator.visualizerInstance.transform.GetChild(childNumber).transform.GetChild(2).gameObject.SetActive(true);
            }           
        }
    }
}
