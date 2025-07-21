namespace EnchantedMask.Glyphs
{
    public class Old : Glyph
    {
        public override string ID => "Old";
        public override string Name => "Glyph of Wisdom";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a bug that has lived a long, full life.\n\n" +
                                                "Sharpens the bearer's mind, allowing them to perceive everything in slow motion.";

        public override bool Unlocked()
        {
            return PlayerData.instance.elderbugGaveFlower;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.xunFlowerGiven)
            {
                return "A mournful knight sits with a flower in hand.";
            }
            else if (!PlayerData.instance.elderbugGaveFlower)
            {
                return "A great elder unknowingly waits for a gift.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.Update += SlowTime;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.Update -= SlowTime;
            if (timeScale >= 0)
            {
                TimeController.GenericTimeScale = timeScale;
                //SharedData.Log($"{ID} - Time scale reset to {timeScale}");
                timeScale = -1;
            }
        }

        /// <summary>
        /// Tracks whether or not the buff has been applied;
        /// </summary>
        private float timeScale = -1f;

        private void SlowTime(On.HeroController.orig_Update orig, HeroController self)
        {
            //SharedData.Log($"Current time scale: {TimeController.GenericTimeScale}");
            if (timeScale < 0)
            {
                timeScale = TimeController.GenericTimeScale;
            }

            TimeController.GenericTimeScale = timeScale * GetModifier();
            //SharedData.Log($"{ID} - Time scale set to {TimeController.GenericTimeScale}");
            orig(self);
        }

        /// <summary>
        /// Old is an Uncommon glyph, so its worth 2 notches.
        /// However, it is kind of hard to balance something like this, so I will use 
        ///     Fyrenest's SlowTime charm as a reference, then go off my gut.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.8f;
        }
    }
}
