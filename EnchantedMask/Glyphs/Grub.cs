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
        /// Grubsong grants 15 SOUL when damaged for 1 notch, so Grub will grant 
        ///     15 * 3 * 0.75 = 33.75 SOUL, rounded down to 33
        /// </summary>
        /// <returns></returns>
        internal float GetGrubsongModifier()
        {
            return 2.25f;
        }

        /// <summary>
        /// Grub also increases the damage dealt by Grubberfly's Elegy
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void Elegy(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.name.Contains("Grubberfly") &&
                PlayerData.instance.equippedCharm_35)
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"Grub - Elegy damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Grubberfly's Elegy deals nail damage, which is normally 10% per notch.
        /// Elegy beams are more niche, but they also have tradeoff advantages,
        ///     so they still only get a 10% bonus per notch.
        /// So final tally of 3 * 0.75 notches times 10% per notch comes out to a 
        ///     22.5% boost in nail damage
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.225f;
        }
    }
}
