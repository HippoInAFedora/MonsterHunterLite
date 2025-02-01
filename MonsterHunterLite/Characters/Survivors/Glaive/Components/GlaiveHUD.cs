
using MonsterHunterMod.Characters.Survivors.Glaive.Content.Misc._Content;
using MonsterHunterMod.Survivors.Glaive;
using Newtonsoft.Json.Utilities;
using RoR2;
using RoR2.HudOverlay;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Components
{
    public class GlaiveHUD : MonoBehaviour
    {
        private CharacterBody characterBody;

        private GameObject statusPrefab = GlaiveAssets.bugStatuses;

        private GameObject statusInstance;

        private OverlayController overlayController;

        public float[] timers = new float[3];


        public void Start()
        {
            characterBody = base.GetComponent<CharacterBody>();
            timers[0] = 0;
            timers[1] = 0;
            timers[2] = 0;
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0)
                {
                    timers[i] = 0;
                }
            }          
        }
    } 
}
