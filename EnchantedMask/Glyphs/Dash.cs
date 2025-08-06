using DanielSteginkUtils.Helpers.Abilities;
using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Glyphs
{
    public class Dash : Glyph
    {
        public override string ID => "Dash";
        public override string Name => "Glyph of the Past";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a ghost from the kingdom's past.\n\n" +
                                                "Allows the bearer to dash more often.";

        public override bool Unlocked()
        {
            return PlayerData.instance.hornet1Defeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hornet1Defeated)
            {
                return "The Gendered Child stands watch at Greenpath's end.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            if (dashHelper == null)
            {
                dashHelper = new DashHelper(GetModifier(), GetModifier());
            }

            dashHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (dashHelper != null)
            {
                dashHelper = new DashHelper(GetModifier(), GetModifier());
                dashHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private DashHelper dashHelper;

        /// <summary>
        /// The Dash glyph reduces the cooldown of dash abilities
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As a Common glyph, Dash is worth 1 notch.
            // Per my Utils, 1 notch is worth a 16.5% decrease in dash cooldown
            return 1 - NotchCosts.GetDashCooldownPerNotch();
        }
    }
}