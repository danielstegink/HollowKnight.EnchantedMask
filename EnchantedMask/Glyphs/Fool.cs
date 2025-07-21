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

            On.HeroController.StartMPDrain += SpeedHealing;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.StartMPDrain -= SpeedHealing;
        }

        /// <summary>
        /// Fool increases healing speed
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="time"></param>
        private void SpeedHealing(On.HeroController.orig_StartMPDrain orig, HeroController self, float time)
        {
            time *= GetModifier();
            //SharedData.Log($"{ID} - Healing speed set to {time}");
            orig(self, time);
        }

        /// <summary>
        /// As an Epic glyph, Fool is worth 4 notches.
        /// Quick Focus is worth 3 notches and speeds healing up by 33%,
        ///     so Fool will increase healing speed by 44%
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.56f;
        }
    }
}
