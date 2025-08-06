using DanielSteginkUtils.Helpers.Attributes;

namespace EnchantedMask.Glyphs
{
    public class Green : Glyph
    {
        public override string ID => "Green";
        public override string Name => "Glyph of Unn";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of the master of Greenpath.\n\n" +
                                                "Increases the bearer's affinity with Unn.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotCharm_28;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.gotCharm_28)
            {
                return "The mother of the Green Children sleeps beneath her lake.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new SpeedHelper(1f, GetModifier());
            helper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (helper != null)
            {
                helper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private SpeedHelper helper;

        /// <summary>
        /// Green increases movement speed while healing with Shape of Unn
        /// </summary>
        internal override float GetModifier()
        {
            // Green is a Common glyph, so its worth 1 notch.
            // SOU gives the speed boost it does for 2 notches, so a 50% increase makes sense.
            float modifier = 1.5f;

            // SOU also has visual synergies with Baldur Shell and Spore Shroom, but no actual synergies,
            // so Green will include them in its calculations.

            // Spore Shroom
            if (PlayerData.instance.equippedCharm_17)
            {
                modifier += 0.05f;
            }

            // Baldur Shell
            if (PlayerData.instance.equippedCharm_5)
            {
                modifier += 0.1f;
            }

            return modifier;
        }
    }
}