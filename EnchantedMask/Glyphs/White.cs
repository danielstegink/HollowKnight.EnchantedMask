using EnchantedMask.Helpers.GlyphHelpers;

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

            dungTrailHelper = new DungTrailHelper(ID, GetSizeModifier(), GetModifier());
            dungTrailHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (dungTrailHelper != null)
            {
                dungTrailHelper.Stop();
            }
        }

        /// <summary>
        /// Used for handling size of Defender's Crest clouds
        /// </summary>
        private DungTrailHelper dungTrailHelper;

        /// <summary>
        /// White is a Rare glyph worth 3 notches, but it has 2 features: size boost, and damage boost.
        /// The size boost is more convenient, so we will use only 1 notch for that, so Brown is still viable.
        /// Per Brown, 1 notch is worth a 50% size increase.
        /// </summary>
        /// <returns></returns>
        private float GetSizeModifier()
        {
            return 1.5f;
        }

        /// <summary>
        /// We used 1 notch to increase the cloud's size, so we have 2 notches left to modify damage.
        /// Defender's Crest is only worth 1 notch normally, so we can increase its damage rate by 200%.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 1f / 3f;
        }
    }
}