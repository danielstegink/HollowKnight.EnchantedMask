using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Glyphs
{
    public class Fool : Glyph
    {
        public override string ID => "Fool";
        public override string Name => "Glyph of Fools";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the Colosseum's greatest Fool.\n\n" +
                                                "Increases the speed of focusing SOUL.";

        public override bool Unlocked()
        {
            return PlayerData.instance.colosseumGoldCompleted;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.littleFoolMet)
            {
                return "There is a place where Fools gather for death and glory.";
            }
            else if (!PlayerData.instance.colosseumGoldCompleted)
            {
                return "A great Fool waits at the end of three trials.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new HealingSpeedHelper(GetModifier());
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
        private HealingSpeedHelper helper;

        /// <summary>
        /// Fool increases healing speed
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As an Epic glyph, Fool is worth 4 notches
            // Per my Utils, Healing speed can be increased by 11% per notch, so we will increase it by 44%
            return 1 - 4 * NotchCosts.HealingSpeedPerNotch();
        }
    }
}
