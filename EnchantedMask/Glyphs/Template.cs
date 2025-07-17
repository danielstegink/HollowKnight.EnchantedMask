namespace EnchantedMask.Glyphs
{
    public class Template : Glyph
    {
        public override string ID => "";
        public override string Name => "";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "";

        public override bool Unlocked()
        {
            //return PlayerData.instance.maskmakerMet &&
            //    PlayerData.instance.falseKnightDefeated;

            return true;
        }

        public override string GetClue()
        {
            string clue = base.GetClue();
            if (string.IsNullOrWhiteSpace(clue))
            {
                if (!PlayerData.instance.falseKnightDefeated)
                {
                    clue = "";
                }
            }

            return clue;
        }

        public override void Equip()
        {
            base.Equip();

            
        }

        public override void Unequip()
        {
            base.Unequip();

            
        }
    }
}
