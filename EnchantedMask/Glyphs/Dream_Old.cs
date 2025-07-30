using EnchantedMask.Settings;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    /// <summary>
    /// Placeholder for old Dream logic in case I ever need it
    /// </summary>
    public class Dream_Old : Glyph
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

            On.EnemyDeathEffects.RecieveDeathEvent -= GetEssence;
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
                int essenceChance = EssenceChance();
                //SharedData.Log($"{ID} - Checking Essence: {random} vs {essenceChance}");
                if (random <= essenceChance)
                {
                    GameObject dreamPrefab = SharedData.GetField<EnemyDeathEffects, GameObject>(self, "dreamEssenceCorpseGetPrefab");
                    dreamPrefab.Spawn(self.transform.position + self.effectOrigin);

                    PlayerData.instance.dreamOrbs++;
                    PlayerData.instance.dreamOrbsSpent--;
                    EventRegister.SendEvent("DREAM ORB COLLECT");
                }
            }
        }

        /// <summary>
        /// Gets the probability of getting Essence
        /// </summary>
        /// <returns></returns>
        private int EssenceChance()
        {
            // Dream is an Uncommon glyph, so it's worth 2 notches
            // Dream Wielder increases the chance by 50% for 1 notch, though this just 1 of its many effects.
            // It also significantly reduces Dream Nail charge time, which I would say is 50% of its value,
            //      and increases the SOUL gained, which I would classify as 1/3 of its value.
            // So, that leaves 1/6 of its value being extra Essence. Multiplying from 1/6 of a notch to 
            //      2 full ones, we want to increase the chance of gaining Essence by 600%

            // There are 2 cases: for default we have a 1 in 300 chance of getting Essence, if we've spent
            //      a lot of Essence we have a 1 in 60 chance.
            // So, Dream Wielder gives a 50% boost, specifically converting the cases above to 1 in 200 and
            //      1 in 40.
            // So, 1/300 * 1.5 = 1/200 and 1/60 * 1.5 = 1/40
            // In our case, we need to approximate 1/300 * 7 and 1/60 * 7

            // So, our new equations are 1/300 + X = 7/300 and 1/60 + Y = 7/60
            // X = 1/50, Y = 1/10

            // So if PlayerData.instance.dreamOrbsSpent > 0, we want a 10% chance of getting Essence.
            // Otherwise, we want a 2% chance.

            if (PlayerData.instance.dreamOrbsSpent > 0)
            {
                return 10;
            }
            else
            {
                return 2;
            }
        }
    }
}