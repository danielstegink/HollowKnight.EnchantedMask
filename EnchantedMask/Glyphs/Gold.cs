using System;

namespace EnchantedMask.Glyphs
{
    public class Gold : Glyph
    {
        public override string ID => "Gold";
        public override string Name => "Glyph of Greed";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a vain and greedy creature.\n\n" +
                                                "Increases Geo the bearer finds.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedGorgeousHusk;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.killedGorgeousHusk)
            {
                return "A greedy and selfish creature hides behind its poorer counterparts.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.AddGeo += AddGeo;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.AddGeo -= AddGeo;
        }

        /// <summary>
        /// The Gold glyph increase geo gained
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="amount"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
        {
            int bonus = GetBonus(amount);
            amount += bonus;
            //SharedData.Log($"{ID} - Geo increased by {bonus}");
            orig(self, amount);
        }

        /// <summary>
        /// As a Common glyph, Gold is worth 1 notch.
        /// Golden Touch is 2 notches, but without fragility its more like a 4-notch.
        /// If 4 notches is worth a 20% boost, then 1 notch would be a 5% boost.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.05f;
        }
    }
}
