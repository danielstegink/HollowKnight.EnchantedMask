using System;

namespace EnchantedMask.Glyphs
{
    public class Hive : Glyph
    {
        public override string ID => "Hive";
        public override string Name => "Glyph of Honey";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of the kingdom's busiest neighbors.\n\n" +
                                                "Increases the bearer's affinity with the Hive.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedHiveKnight;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.killedHiveKnight)
            {
                return "The Hive's greatest champion safeguards his liege.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.AddHealth += ExtraHealth;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.AddHealth -= ExtraHealth;
        }

        /// <summary>
        /// Hive has a chance to heal the player for extra health when Hiveblood is equipped
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ExtraHealth(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
        {
            // Only trigger if Hiveblood is equipped and this won't get the player to max health
            if (PlayerData.instance.equippedCharm_29 &&
                PlayerData.instance.health + amount < PlayerData.instance.maxHealth)
            {
                int random = UnityEngine.Random.Range(1, 101);
                int healChance = GetHealChance();
                if (random <= healChance)
                {
                    amount++;
                }
            }

            orig(self, amount);
        }

        /// <summary>
        /// As a Rare glyph, Hive is worth 3 notches.
        /// Deep Focus is 4 notches and heals an extra mask (I know it says double, but 1x2 is just +1)
        ///     in exchange for an increased healing time of 165%. Reducing this back to 100% would require
        ///     a 40% reduction in healing speed.
        /// Quick Focus reduces healing time by 33% for 3 notches, based on this logic, the increased 
        ///     healing time from Deep Focus is worth about 4 notches.
        /// This means healing an extra mask is worth 8 notches, so Hive should have a 3/8 chance of
        ///     healing an extra mask.
        /// </summary>
        /// <returns></returns>
        internal int GetHealChance()
        {
            return 37;
        }
    }
}