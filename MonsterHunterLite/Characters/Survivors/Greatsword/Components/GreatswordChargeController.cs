using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace MonsterHunterMod.Characters.Survivors.Greatsword.Components
{
    public class GreatswordChargeController : MonoBehaviour
    {
        public enum ChargeLevel
        {
            None,
            Tier1,
            Tier2,
            Tier3,
            Overcharged
        }

        public ChargeLevel charge;
        public bool inState = false;

        private float duration = 1f;
        private float stopwatch = 0f;

        public void Awake()
        {

        }

        public void Start()
        {
            charge = ChargeLevel.None;
        }

        public void FixedUpdate()
        {
            if (inState) 
            {
                stopwatch = 0f;
            }
            if (charge != ChargeLevel.None && !inState)
            {
                if (charge == ChargeLevel.Overcharged)
                {
                    charge = ChargeLevel.None;
                }
                RunCharge();
            }
        }

        public void RunCharge()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > duration)
            {
                charge = ChargeLevel.None;
                stopwatch = 0f;
            }
        }
    }
}
