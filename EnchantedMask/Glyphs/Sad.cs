using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using System;

namespace EnchantedMask.Glyphs
{
    public class Sad : Glyph
    {
        public override string ID => "Sad";
        public override string Name => "Glyph of Mourning";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of lost love.\n\n" +
                                                "When the bearer is at full health, they will deal increased damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.xunRewardGiven;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.xunRewardGiven)
            {
                return "A mournful knight sits with a flower in hand.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffDamage;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffDamage;
        }

        /// <summary>
        /// The Sad glyph increases all damage dealt while the player is at full health.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (PlayerData.instance.health == PlayerData.instance.CurrentMaxHealth)
            {
                int baseDamage = hitInstance.DamageDealt;
                int bonusDamage = GetBonus(baseDamage);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - {baseDamage} damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Gets the damage modifier
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As a Rare glyph, Sad is worth 3 notches.
            // Per my Utils, a universal damage buff should be 6.67% per notch.
            // Per my Utils, however, requiring full health makes a bonus 4 times as valuable,
            // for a total of about 80%
            float modifier = 3f * NotchCosts.DamagePerNotch() * NotchCosts.FullHealthModifier();
            //SharedData.Log($"{ID} - Damage modifier: {modifier}");

            return modifier;
        }
    }
}
