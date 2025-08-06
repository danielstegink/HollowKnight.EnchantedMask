using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Glyphs
{
    public class Gold : Glyph
    {
        public override string ID => "Gold";
        public override string Name => "Glyph of Greed";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a vain and greedy creature.\n\n" +
                                                "Increases Geo the bearer finds.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedGorgeousHusk;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.killedGorgeousHusk)
            {
                return "A greedy and selfish creature hides behind its poorer counterparts.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new GeoHelper(GetModifier());
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
        private GeoHelper helper;

        /// <summary>
        /// Gold increases geo gained from all sources
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As a Common glyph, Gold is worth 1 notch
            // Per my Utils, Geo gain is worth about 5% per notch
            return 1 + NotchCosts.GeoPerNotch();
        }
    }
}
