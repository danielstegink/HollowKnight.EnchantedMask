using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;
using System;

namespace EnchantedMask.Glyphs
{
    public class False : Glyph
    {
        public override string ID => "False";
        public override string Name => "Glyph of Deceit";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a lowly creature who stole power they could not control.\n\n" +
                                                "Increases the bearer's strength, allowing them to cripple great foes.";

        public override bool Unlocked()
        {
            return PlayerData.instance.falseKnightDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.falseKnightDefeated)
            {
                return "A thief hides among ruined highways, protected by stolen armor.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            staggerHelper = new StaggerHelper(Modifier());
            staggerHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (staggerHelper != null)
            {
                staggerHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private StaggerHelper staggerHelper;

        /// <summary>
        /// The False glyph reduces the number of hits required to stagger an enemy
        /// </summary>
        /// <returns></returns>
        private int Modifier()
        {
            // False is a Common glyph worth 1 notch
            // Per my Utils, 1 notch is worth 1 point of stagger
            return Math.Max(1, 1 / NotchCosts.NotchesPerStagger());
        }
    }
}