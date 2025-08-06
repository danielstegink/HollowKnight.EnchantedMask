using EnchantedMask.Helpers.BlockHelpers;

namespace EnchantedMask.Glyphs
{
    public class Royal : Glyph
    {
        public override string ID => "Royal";
        public override string Name => "Glyph of the King";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the king of Hallownest.\n\n" +
                                                "May create a royal aura that defends the bearer from worldly dangers.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotQueenFragment &&
                    PlayerData.instance.gotKingFragment;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.gotQueenFragment)
            {
                return "The Queen hides with a memory of her Wyrm.";
            }
            else if (!PlayerData.instance.gotKingFragment)
            {
                return "An echo of the King sits upon a memory of his throne.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new RoyalShield();
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
        private RoyalShield helper;
    }
}
