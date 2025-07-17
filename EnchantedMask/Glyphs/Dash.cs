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

            BuffDashCooldown();
        }

        public override void Unequip()
        {
            base.Unequip();

            ResetDashCooldown();
        }

        /// <summary>
        /// Tracks whether or not the buff has been applied;
        /// </summary>
        private bool buffApplied = false;

        /// <summary>
        /// Broken glyph reduces the Dash cooldown
        /// </summary>
        private void BuffDashCooldown()
        {
            if (!buffApplied)
            {
                float modifier = GetModifier();
                HeroController.instance.DASH_COOLDOWN *= modifier;
                HeroController.instance.DASH_COOLDOWN_CH *= modifier;
                HeroController.instance.SHADOW_DASH_COOLDOWN *= modifier;
                buffApplied = true;
                //SharedData.Log($"{ID} - Dash cooldown reduced: {HeroController.instance.DASH_COOLDOWN}, " +
                //                                                $"{HeroController.instance.DASH_COOLDOWN_CH}, " +
                //                                                $"{HeroController.instance.SHADOW_DASH_COOLDOWN}");
            }
        }

        /// <summary>
        /// Of course, we need to reset the cooldowns when the glyph is removed
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResetDashCooldown()
        {
            if (buffApplied)
            {
                float modifier = GetModifier();
                HeroController.instance.DASH_COOLDOWN /= modifier;
                HeroController.instance.DASH_COOLDOWN_CH /= modifier;
                HeroController.instance.SHADOW_DASH_COOLDOWN /= modifier;
                buffApplied = false;
                //SharedData.Log($"{ID} - Dash cooldown reset: {HeroController.instance.DASH_COOLDOWN}, " +
                //                                                $"{HeroController.instance.DASH_COOLDOWN_CH}, " +
                //                                                $"{HeroController.instance.SHADOW_DASH_COOLDOWN}");
            }
        }

        /// <summary>
        /// As a Common glyph, Dash is worth 1 notch.
        /// Dashmaster is worth 2 notches and reduces Dash
        ///     cooldown by 33%, so Dash will do 16.5%.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.835f;
        }
    }
}