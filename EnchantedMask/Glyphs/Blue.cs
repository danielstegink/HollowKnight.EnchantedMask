using System;

namespace EnchantedMask.Glyphs
{
    public class Blue : Glyph
    {
        public override string ID => "Blue";
        public override string Name => "Glyph of Life";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of lifeblood.\n\n" +
                                                "Contains the essence of lifeblood, which may create new lifeblood when the bearer channels SOUL.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotCharm_9;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasKingsBrand)
            {
                return "The King's legacy waits beyond the kingdom's reach.";
            }
            else if (!PlayerData.instance.gotCharm_9)
            {
                return "The living essence of lifeblood behind a forgotten seal.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.AddHealth += CreateLifeblood;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.AddHealth -= CreateLifeblood;
        }

        /// <summary>
        /// Blue gives a chance to create blue masks when the player heals
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateLifeblood(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
        {
            int random = UnityEngine.Random.Range(1, 101);
            int healChance = GetHealChance();
            //SharedData.Log($"{ID} - Attempting to heal: {random} vs {healChance}");
            if (random <= healChance)
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
            }

            orig(self, amount);
        }

        /// <summary>
        /// As a Common glyph, Blue is worth 1 notch.
        /// Deep Focus is 4 notches and heals an extra mask (I know it says double, but 1X2 is just +1)
        ///     in exchange for an increased healing time of 165%. Reducing this back to 100% would require
        ///     a 40% reduction in healing speed.
        /// Quick Focus reduces healing time by 33% for 3 notches, based on this logic,
        ///     the increased healing time from Deep Focus is worth about 4 notches.
        /// This means healing an extra mask is worth 8 notches, so Blue should have a 1/8 chance of
        ///     healing an extra mask.
        /// Lifeblood masks are less valuable than regular masks because you can't heal them back, so 
        ///     the odds can be improved. 
        /// Lifeblood Heart gives 2 masks for 2 notches and Lifeblood Core gives 4 notches for 3, so 
        ///     1 notch is worth about 1.5 lifeblood masks.
        /// In comparison, Fragile Heart gives 2 regular masks for 2 notches, but its fragile status
        ///     means its worth about 2 extra notches, making 1 notch worth 1/2 of a regular mask.
        /// If 1 mask is worth about 3 lifeblood masks, then we can increase the healing chance from
        ///     1/8 to 3/8.
        /// To add some synergy, lets round down a little in exchange for lifeblood charms increasing
        ///     the probability.
        /// </summary>
        /// <returns></returns>
        internal int GetHealChance()
        {
            int healChance = 34;

            if (PlayerData.instance.equippedCharm_8)
            {
                healChance += 2;
            }

            if (PlayerData.instance.equippedCharm_9)
            {
                healChance += 3;
            }

            if (PlayerData.instance.equippedCharm_27)
            {
                healChance += 4;
            }

            return healChance;
        }
    }
}