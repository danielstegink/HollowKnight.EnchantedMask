using DanielSteginkUtils.Utilities;
using EnchantedMask.Helpers.GlyphHelpers;

namespace EnchantedMask.Glyphs
{
    public class Hive : Glyph
    {
        public override string ID => "Hive";
        public override string Name => "Glyph of Honey";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of the kingdom's busiest neighbors.\n\n" +
                                                "Increases the bearer's affinity with the Hive.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedHiveKnight;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.killedHiveKnight)
            {
                return "The Hive's greatest champion safeguards his liege.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new HiveHealHelper(GetHealChance());
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
        private HiveHealHelper helper;

        /// <summary>
        /// Hive has a chance to heal the player for extra health when Hiveblood is equipped
        /// </summary>
        /// <returns></returns>
        internal int GetHealChance()
        {
            // As a Rare glyph, Hive is worth 3 notches.
            // Per my Utils, healing an extra mask is worth 8 notches.
            // So this will have a 3/8 chance of healing an extra mask.
            return (int)(100 * 3 / NotchCosts.NotchesPerHeal());
        }
    }
}