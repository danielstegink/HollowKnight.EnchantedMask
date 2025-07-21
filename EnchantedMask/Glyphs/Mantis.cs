using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Mantis : Glyph
    {
        public override string ID => "Mantis";
        public override string Name => "Glyph of Leadership";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the Mantis Tribe's swift and slender leaders.\n\n" +
                                                "Increases the bearer's affinity with the Mantis Tribe.";

        public override bool Unlocked()
        {
            return PlayerData.instance.defeatedMantisLords;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedMantisLords)
            {
                return "Three sisters sit undefeated upon their thrones.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.NailSlash.StartSlash += IncreaseNailLength;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.NailSlash.StartSlash -= IncreaseNailLength;
        }

        /// <summary>
        /// The Mantis glyph increases the length of nail attacks when MOP is equipped
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void IncreaseNailLength(On.NailSlash.orig_StartSlash orig, NailSlash self)
        {
            // Get the default scale
            Vector3 startingScale = self.scale;

            // Increase the scale temporarily, otherwise the effects can/will stack
            if (PlayerData.instance.equippedCharm_13)
            {
                float modifier = GetModifier();
                Vector3 newScale = new Vector3(startingScale.x * modifier, startingScale.y * modifier);
                self.scale = newScale;
                //SharedData.Log($"{ID} - Nail length increased by {modifier}");
            }

            // Perform the nail slash
            orig(self);
            self.scale = startingScale;
        }

        /// <summary>
        /// As an Uncommon glyph, Mantis increases MOP's value by 
        ///     2 notches, for a total of 5.
        /// At 3 notches MOP increases nail length by 25%, and at 2 notches
        ///     Longnail increases it by 15%, so an extra 2 notches would
        ///     be worth 20%, for a total of 45%.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // 1.25 * 1.16 = 1.45
            return 1.16f;
        }
    }
}