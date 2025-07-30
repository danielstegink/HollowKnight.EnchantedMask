using EnchantedMask.Settings;
using System;

namespace EnchantedMask.Glyphs
{
    public class Nailsage : Glyph
    {
        public override string ID => "Nailsage";
        public override string Name => "Glyph of the Nail";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a true master of the nail.\n\n" +
                                                "Increases the bearer's mastery of Nail Arts, allowing them to do more damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotCharm_26;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasAllNailArts)
            {
                return "Three masters of the nail train in isolation.";
            }
            else if (!PlayerData.instance.gotCharm_26)
            {
                return "The true master of the nail will complete your training.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffNailArts;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffNailArts;
        }

        /// <summary>
        /// The Nailmaster glyph increases Nail Art damage. 
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffNailArts(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (SharedData.nailArtNames.Contains(hitInstance.Source.name))
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - Nail Art damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// For 1 notch, NMG reduces Nail Art charge time by 44%. If you include the 0.4 second animation time,
        ///     that means it increased the DPS of Nail Arts by 52%.
        /// As an Uncommon glyph, Nailsage is worth 2 notches, so its worth a 104% increase.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 1.04f;
        }
    }
}