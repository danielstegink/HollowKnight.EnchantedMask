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
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - Damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// As a Rare glyph, Sad is worth 3 notches.
        /// That would be worth a 30% boost in nail strength,
        ///     but for all damage, the value should be much smaller.
        /// On the other hand, this glyph only takes effect if the 
        ///     player is at full health, so that inflates the value
        ///     somewhat.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.3f;
        }
    }
}
