namespace EnchantedMask.Glyphs
{
    public class Explorer : Glyph
    {
        public override string ID => "Explorer";
        public override string Name => "Glyph of Exploration";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a clever mapmaker.\n\n" +
                                                "Increases the running speed of the bearer, allowing them to avoid danger.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mapAllRooms;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.corniferAtHome)
            {
                return "The mapmaker continues to his journey.";
            }
            else if (!PlayerData.instance.mapAllRooms)
            {
                return "Your map of the kingdom is incomplete.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.Move += SpeedBoost;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.Move -= SpeedBoost;
        }

        /// <summary>
        /// The Explorer glyph increases movement speed.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="move_direction"></param>
        private void SpeedBoost(On.HeroController.orig_Move orig, HeroController self, float move_direction)
        {
            move_direction *= GetModifier();
            //SharedData.Log($"{ID} - Speed increased to {move_direction}");
            orig(self, move_direction);
        }

        /// <summary>
        /// Explorer is a Common glyph, so its worth 1 notch.
        /// Sprintmaster increases movement speed by 20% for 1 notch,
        ///     so this will do the same.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 1.2f;
        }
    }
}