using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Glyphs
{
    public class Explorer : Glyph
    {
        public override string ID => "Explorer";
        public override string Name => "Glyph of Exploration";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a clever mapmaker.\n\n" +
                                                "Increases the running speed of the bearer, allowing them to avoid danger.";

        public override bool Unlocked()
        {
            return PlayerValues.BoughtAllMaps();
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.corniferAtHome)
            {
                return "A mapmaker continues his journey across the kingdom.";
            }
            else if (!PlayerValues.BoughtAllMaps())
            {
                return "Your map of the kingdom is incomplete.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            speedHelper = new SpeedHelper(GetModifier());
            speedHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (speedHelper != null)
            {
                speedHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private SpeedHelper speedHelper;

        /// <summary>
        /// The Explorer glyph increases the player's movement speed
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Explorer is a Common glyph, so its worth 1 notch
            // Sprintmaster increases movement speed by 20% for 1 notch, so this will do the same
            return 1.2f;
        }
    }
}