using GlobalEnums;
using System;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Grub : Glyph
    {
        public override string ID => "Grub";
        public override string Name => "Glyph of Childhood";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of a small child full of potential.\n\n" +
                                                "Increases the bearer's affinity with grubs.";

        public override bool Unlocked()
        {
            return PlayerData.instance.finalGrubRewardCollected;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.grubsCollected == 0)
            {
                return "There is a small child in need of rescue.";
            }
            else if (PlayerData.instance.grubsCollected < 46)
            {
                return "There are more children trapped throughout the kingdom.";
            }
            else if (!PlayerData.instance.finalGrubRewardCollected)
            {
                return "The father waits to give you his final reward.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.TakeDamage += Grubsong;
            On.HealthManager.TakeDamage += Elegy;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.TakeDamage -= Grubsong;
            On.HealthManager.TakeDamage -= Elegy;
        }

        /// <summary>
        /// The Grub glyph increases the SOUL gained from Grubsong.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Grubsong(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            orig(self, go, damageSide, damageAmount, hazardType);

            if (PlayerData.instance.equippedCharm_3 &&
                damageAmount > 0)
            {
                float baseSoul = self.GRUB_SOUL_MP;
                int newSoul = (int)Math.Floor(baseSoul * GetGrubsongModifier());
                self.AddMPCharge(newSoul);
                //SharedData.Log($"Grub - Soul increased by {newSoul}");
            }
        }

        /// <summary>
        /// As a Rare glyph, Grub is worth 3 notches. However, it affects 2 charms. As such, 
        ///     each charm will receive 75% of the bonus it usually would.
        /// Grubsong grants 15 SOUL when damaged for 1 notch, so Grub will increase this to 
        ///     15 * 3 * 0.75 = 33.75 SOUL, rounded down to 33.
        /// </summary>
        /// <returns></returns>
        internal float GetGrubsongModifier()
        {
            return 1.25f;
        }

        /// <summary>
        /// Grub also increases the damage dealt by Grubberfly's Elegy
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void Elegy(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.name.Contains("Grubberfly"))
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"Grub - Elegy damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// For 3 notches, Grubberfly's Elegy adds an extra attack with 50% damage
        ///     and roughly 300% range while at full health.
        /// If 1 notch is worth a 10% increase in nail damage, the damage boost is worth 5 notches.
        /// Additionally, MOP increases range by 25% for 3 notches, so a 200% increase in range
        ///     is worth about 24 notches. However, this only applies to the Elegy beam which
        ///     only deals 1/3 of the total damage, so that drops the bonus down to 8 notches.
        ///     Let's say 7 since the travel time means the beam can miss.
        /// So the extra damage (and the extra range on that damage) is worth about 12 notches.
        ///     But requiring us to be at fully health reduces the value to 3 notches.
        /// So, the 2.25 notch boost should be multiplied by 4 to 9 notches, which is worth
        ///     a roughly 90% increase in nail damage on top of the original 50% for a total of
        ///     140%.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // 0.5 + 0.5x = 1.4, X = 1.8
            return 1.8f;
        }
    }
}
