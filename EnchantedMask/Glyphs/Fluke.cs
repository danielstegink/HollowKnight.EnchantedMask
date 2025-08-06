using DanielSteginkUtils.Helpers.Charms.Dung;
using DanielSteginkUtils.Helpers.Charms.Pets;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class Fluke : Glyph
    {
        public override string ID => "Fluke";
        public override string Name => "Glyph of Motherhood";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the Fluke's matriarch.\n\n" +
                                                "Increases the bearer's affinity with the Flukes.";

        public override bool Unlocked()
        {
            return PlayerData.instance.flukeMotherDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.flukeMotherDefeated)
            {
                return "A mother ferociously breeds above a pool of acid.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            flukeHelper = new FlukeHelper(GetModifier());
            flukeHelper.Start();

            dungFlukeHelper = new DungFlukeHelper(SharedData.modName, Name, 1 / GetModifier());
            dungFlukeHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (flukeHelper != null)
            {
                flukeHelper.Stop();
            }

            if (dungFlukeHelper != null)
            {
                dungFlukeHelper.Stop();
            }
        }

        /// <summary>
        /// Handles damage buff for Flukes
        /// </summary>
        private FlukeHelper flukeHelper;

        /// <summary>
        /// Handles damage buff for Dung Flukes
        /// </summary>
        private DungFlukeHelper dungFlukeHelper;

        /// <summary>
        /// The Fluke glyph increases the damage dealt by Flukenest
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As an Uncommon glyph, Fluke is worth 2 notches.
            // Flukenest uses 3 notches, so Fluke should increase its damage by about 67%.
            return 1f + 2f / 3f;
        }
    }
}
