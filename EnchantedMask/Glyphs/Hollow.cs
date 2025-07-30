using EnchantedMask.Helpers.BlockHelpers;

namespace EnchantedMask.Glyphs
{
    public class Hollow : Glyph
    {
        public override string ID => "Hollow";
        public override string Name => "Glyph of Sacrifice";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the kingdom's greatest hero.\n\n" +
                                                "After taking damage, a coating of Infection protects the bearer form damage for a short time.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedHollowKnight;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.lurienDefeated || 
                !PlayerData.instance.monomonDefeated || 
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (!PlayerData.instance.killedHollowKnight)
            {
                return "The greatest of knights holds the Infection at bay.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            selfStab.ApplyHook();
        }

        public override void Unequip()
        {
            base.Unequip();

            selfStab.RemoveHook();
        }

        /// <summary>
        /// Handles damage negation for the Hollow glyph inspired by 
        ///     the Hollow Knight's Self Stab ability.
        /// </summary>
        private SelfStab selfStab = new SelfStab();
    }
}
