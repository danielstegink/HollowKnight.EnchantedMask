using DanielSteginkUtils.Helpers.Charms.Dung;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class Brown : Glyph
    {
        public override string ID => "Brown";
        public override string Name => "Glyph of Honour";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a loyal knight.\n\n" +
                                                "Increases the bearer's affinity with heroic odours.";

        public override bool Unlocked()
        {
            return PlayerData.instance.defeatedDungDefender;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedDungDefender)
            {
                return "A great knight sleeps surrounded by filth.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new DungSizeHelper(SharedData.modName, ID, GetModifier());
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
        private DungSizeHelper helper;

        /// <summary>
        /// The Brown glyph increases the size of clouds created by Defender's Crest
        /// </summary>
        internal override float GetModifier()
        {
            // As an Uncommon glyph, Brown is worth 2 notches
            // So Defender's Crest would be worth 3 notches, equivalent to tripling its damage rate.
            // Tripling the damage rate could mean increasing the damage speed, but it can also
            // mean increasing the range to hit triple the enemies.

            // The size of the cloud, which only slightly extends past the player, can typically accomodate only 1 enemy.
            // For the cloud to accomodate 3x the enemies, assuming the enemies surround the player,
            // doubling the size of the cloud seems appropriate.
            return 2f;
        }
    }
}
