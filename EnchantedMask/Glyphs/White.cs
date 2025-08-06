using DanielSteginkUtils.Helpers.Charms.Dung;
using EnchantedMask.Helpers.GlyphHelpers;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class White : Glyph
    {
        public override string ID => "White";
        public override string Name => "Glyph of Bravery";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of a loyal knight's past glory.\n\n" +
                                                "Increases the bearer's affinity for heroic odours as they become weaker.";

        public override bool Unlocked()
        {
            return PlayerData.instance.whiteDefenderDefeats == 5;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedDungDefender)
            {
                return "A great knight sleeps surrounded by filth.";
            }
            else if (!PlayerData.instance.lurienDefeated ||
                        !PlayerData.instance.monomonDefeated ||
                        !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (PlayerData.instance.whiteDefenderDefeats < 5)
            {
                return "A great knight dreams of glory long past.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            sizeHelper = new DungSizeHelper(SharedData.modName, ID, GetSizeModifier());
            sizeHelper.Start();

            damageHelper = new DungDamageHelper(SharedData.modName, ID, 1 / GetModifier());
            damageHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (sizeHelper != null)
            {
                sizeHelper.Stop();
            }

            if (damageHelper != null)
            {
                damageHelper.Stop();
            }
        }

        /// <summary>
        /// Cloud size helper
        /// </summary>
        private DungSizeHelper sizeHelper;

        /// <summary>
        /// Cloud damage helper
        /// </summary>
        private DungDamageHelper damageHelper;

        /// <summary>
        /// White is a Rare glyph worth 3 notches.
        /// It uses 1 notch to increase the size of the dung clouds.
        /// </summary>
        /// <returns></returns>
        private float GetSizeModifier()
        {
            // Per Brown, 1 notch is worth a 50% size increase
            return 1.5f;
        }

        /// <summary>
        /// White spends 2 notches increasing the damage rate of dung clouds.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Defender's Crest is worth 1 notch, so we can increase its damage rate by 200%
            return 3f;
        }
    }
}