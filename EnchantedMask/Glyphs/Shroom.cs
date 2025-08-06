using DanielSteginkUtils.Helpers.Charms.Shroom;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class Shroom : Glyph
    {
        public override string ID => "Shroom";
        public override string Name => "Glyph of Spores";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of Hallownest's most mysterious fungi.\n\n" +
                                                "Increases the bearer's affinity with the Mushroom Clan.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mrMushroomState == 8;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.lurienDefeated ||
                !PlayerData.instance.monomonDefeated ||
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (PlayerData.instance.mrMushroomState < 8)
            {
                return "An elusive fungus traverses the kingdom.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new SporeDamageHelper(SharedData.modName, ID, 1 / GetModifier());
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
        private SporeDamageHelper helper;

        /// <summary>
        /// The Shroom glyph increases the damage rate of Spore Shroom
        /// </summary>
        internal override float GetModifier()
        {
            // As a Common glyph, Shroom is worth 1 notch.
            // Spore Shroom is only worth 1 notch normally, so we can increase its damage rate by 100%.
            return 2f;
        }
    }
}