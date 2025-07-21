namespace EnchantedMask.Glyphs
{
    public class Blessed : Glyph
    {
        public override string ID => "Blessed";
        public override string Name => "Glyph of Love";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of one who is overflowing with love.\n\n" +
                                                "Allows the bearer to use more charms.";

        public override bool Unlocked()
        {
            return PlayerData.instance.salubraBlessing;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.metCharmSlug)
            {
                return "A lover of antiquity eagerly awaits you in her home.";
            }
            else if (!PlayerData.instance.salubraBlessing)
            {
                return "The lover waits for you to claim her blessing.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            if (!applied)
            {
                PlayerData.instance.charmSlots++;
                applied = true;
                //SharedData.Log($"{ID} - Charm slots increased");
            }
            GameManager.instance.RefreshOvercharm();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (applied)
            {
                PlayerData.instance.charmSlots--;
                applied = false;
                //SharedData.Log($"{ID} - Charm slots reduced");
            }
            GameManager.instance.RefreshOvercharm();
        }

        /// <summary>
        /// The Blessed glyph adds charm notches.
        /// Blessed is a Common glyph worth 1 notch, so 
        ///     it gives 1 notch.
        /// When unequipping the glyph, we need a way
        ///     to confirm that we've added a notch.
        /// </summary>
        private bool applied = false;
    }
}
