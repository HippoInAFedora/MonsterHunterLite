using RoR2;
using R2API;
using UnityEngine;

namespace MonsterHunterMod.Survivors.Greatsword
{
    public static class GreatswordDots
    {
        public static DotController.DotIndex greatswordDot;
        public static void Init(AssetBundle assetBundle)
        {
            greatswordDot = DotAPI.RegisterDotDef(new DotController.DotDef
            {
                interval = .25f,
                damageCoefficient = .25f,
                associatedBuff = GreatswordBuffs.wyvernStakeDotDebuff,
                resetTimerOnAdd = false
            });
        }
    }
}
