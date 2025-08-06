using EnchantedMask.Helpers.GlyphHelpers;

namespace EnchantedMask.Glyphs
{
    public class Mantis : Glyph
    {
        public override string ID => "Mantis";
        public override string Name => "Glyph of Leadership";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the Mantis Tribe's swift and slender leaders.\n\n" +
                                                "Increases the bearer's affinity with the Mantis Tribe.";

        public override bool Unlocked()
        {
            return PlayerData.instance.defeatedMantisLords;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedMantisLords)
            {
                return "Three sisters sit undefeated upon their thrones.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new MopHelper();
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
        private MopHelper helper;
    }
}