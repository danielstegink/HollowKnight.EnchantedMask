using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class Sibling : Glyph
    {
        public override string ID => "Sibling";
        public override string Name => "Glyph of Darkness";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of an abandoned child immersed in darkness.\n\n" +
                                                "Increases the bearer's affinity with darkness.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotShadeCharm;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasKingsBrand)
            {
                return "The King's legacy waits beyond the kingdom's reach.";
            }
            else if (!PlayerData.instance.gotQueenFragment || 
                        !PlayerData.instance.gotKingFragment)
            {
                return "The king's power remains fragmented.";
            }
            else if (!PlayerData.instance.gotShadeCharm)
            {
                return "A dark truth sleeps beneath the shells of your kin.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += SSDamage;
        }

        public override void Unequip()
        {
            base.Unequip();
            On.HealthManager.TakeDamage -= SSDamage;
        }

        /// <summary>
        /// The Sibling glyph increases damage dealt using Sharp Shadow.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void SSDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.name.Equals("Sharp Shadow"))
            {
                int baseDamage = hitInstance.DamageDealt;
                int bonusDamage = GetBonus(baseDamage);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - {baseDamage} damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Gets the bonus damage for Sharp Shadow
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        internal override int GetBonus(int baseValue)
        {
            // Sibling is a Common glyph, so its worth 1 notch.
            // Per my Utils, 1 notch is worth a Dash damage increase equal to 50% nail damage.
            return (int)NotchCosts.DashDamagePerNotch();
        }
    }
}