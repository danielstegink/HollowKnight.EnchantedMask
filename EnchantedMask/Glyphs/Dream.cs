using EnchantedMask.Settings;
using System;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Dream : Glyph
    {
        public override string ID => "Dream";
        public override string Name => "Glyph of Dreams";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a forgotten master of dreams.\n\n" +
                                                "Increases the bearer's affinity with dreams, making it easier to gather Essence.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mothDeparted;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.mothDeparted)
            {
                return "The master of dreams waits for you to complete your training.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.EnemyDeathEffects.RecieveDeathEvent += GetEssence;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.EnemyDeathEffects.RecieveDeathEvent += GetEssence;
        }

        /// <summary>
        /// Dream increases the chance of getting Essence when an enemy dies
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="attackDirection"></param>
        /// <param name="resetDeathEvent"></param>
        /// <param name="spellBurn"></param>
        /// <param name="isWatery"></param>
        private void GetEssence(On.EnemyDeathEffects.orig_RecieveDeathEvent orig, EnemyDeathEffects self, float? attackDirection, bool resetDeathEvent, 
                                bool spellBurn, bool isWatery)
        {
            orig(self, attackDirection, resetDeathEvent, spellBurn, isWatery);

            EnemyDeathTypes deathType = SharedData.GetField<EnemyDeathEffects, EnemyDeathTypes>(self, "enemyDeathType");
            EnemyDeathTypes[] validTypes = new EnemyDeathTypes[] 
            { 
                EnemyDeathTypes.Infected,
                EnemyDeathTypes.LargeInfected,
                EnemyDeathTypes.SmallInfected,
                EnemyDeathTypes.Uninfected
            };

            // We only get Essence from enemies with certain conditions
            if (validTypes.Contains(deathType) && 
                !BossSceneController.IsBossScene)
            {
                int random = UnityEngine.Random.Range(1, 101);
                if (random <= EssenceChance())
                {
                    GameObject dreamPrefab = SharedData.GetField<EnemyDeathEffects, GameObject>(self, "dreamEssenceCorpseGetPrefab");
                    dreamPrefab.Spawn(self.transform.position + self.effectOrigin);

                    int essenceAmount = 10;
                    PlayerData.instance.dreamOrbs += essenceAmount;
                    PlayerData.instance.dreamOrbsSpent -= essenceAmount;
                    EventRegister.SendEvent("DREAM ORB COLLECT");
                }
            }
        }

        /// <summary>
        /// Dream is an Uncommon glyph, so it's worth 2 notches.
        /// Dream Wielder increases the chance by 50% for 1 notch,
        ///     though this just 1 of its many effects.
        /// Balancing for relative value, I'd say 2 notches is worth
        ///     a 600% increase in the chance of gaining Essence.
        /// </summary>
        /// <returns></returns>
        private int EssenceChance()
        {
            // By default, the chance of getting new essence is 1 in 300.
            // So a 600% increase would be 200%, a statistical guarantee
            //      of getting 2 essence per kill.
            // But this kind of probability is boring, so let's spice things up.
            // We have (1/300 + x) = 2, so x is 1.9967 which means there is a 
            //      99% chance of getting 2 essence. But if we divide this by
            //      5, it becomes a 20% chance of getting 10 Essence.
            return 20;
        }
    }
}