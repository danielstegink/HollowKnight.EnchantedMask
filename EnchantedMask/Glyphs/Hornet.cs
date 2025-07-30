using EnchantedMask.Helpers.BlockHelpers;

namespace EnchantedMask.Glyphs
{
    public class Hornet : Glyph
    {
        public override string ID => "Hornet";
        public override string Name => "Glyph of Protection";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of the sworn protector of Hallownest's ruins.\n\n" +
                                                "May summon a magic web that defends the bearer from damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.hornetOutskirtsDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hornet1Defeated)
            {
                return "The Gendered Child stands watch at Greenpath's end.";
            }
            else if (!PlayerData.instance.hornetOutskirtsDefeated)
            {
                return "The Gendered Child guards the King's legacy.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            webBlock.ApplyHook();
        }

        public override void Unequip()
        {
            base.Unequip();

            webBlock.RemoveHook();
        }

        /// <summary>
        /// Handles damage negation for the Hornet glyph inspired by 
        ///     Hornet's web attack
        /// </summary>
        private WebBlock webBlock = new WebBlock();
    }
}