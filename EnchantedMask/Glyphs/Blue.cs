using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
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
        /// </summary>
        /// <returns></returns>
        internal int GetHealChance()
        {
            // Per my Utils, healing an extra mask is worth 8 notches.
            float chance = 100f / NotchCosts.NotchesPerHeal();
            //SharedData.Log($"{ID} - Chance of healing 1 mask: {chance}");

            // However, a Lifeblood mask is worth 3/4 of a notch while a regular Mask is worth 2 notches
            // That makes a Lifeblood mask worth about 0.375 Masks
            float lifebloodPerMask = NotchCosts.NotchesPerBlueMask() / NotchCosts.NotchesPerMask();
            //SharedData.Log($"{ID} - 1 mask is worth {lifebloodPerMask} lifeblood masks");

            // That means the value of healing a Lifeblood masks is 0.375 * 8 = 3 notches
            // So for 1 notch we should have a 1/3 chance of getting a Lifeblood mask upon healing
            int healChance = (int)(chance / lifebloodPerMask);
            //SharedData.Log($"{ID} - Chance of healing 1 lifeblood mask: {healChance}");

            // To add some fun synergy, we'll reduce the odds slightly and increase the chance for
            // each Lifeblood charm equipped
            healChance -= 3;

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