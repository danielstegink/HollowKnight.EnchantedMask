using DanielSteginkUtils.Helpers.Libraries;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
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
            if (PlayerData.instance.equippedCharm_3 &&
                Logic.CanTakeDamage(damageAmount, hazardType))
            {
                float baseSoul = self.GRUB_SOUL_MP;
                int newSoul = (int)Math.Floor(baseSoul * GetGrubsongModifier());
                SoulHelper.GainSoul(newSoul, false);
            }

            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// Gets the modifier for Grubsong
        /// </summary>
        /// <returns></returns>
        internal float GetGrubsongModifier()
        {
            // As a Rare glyph, Grub is worth 3 notches.However, it affects 2 charms.
            // As such, each charm will receive 75% of the bonus it usually would.

            // Grubsong grants 15 SOUL when damaged for 1 notch, so Grub will increase 
            // this to 15 * 3 * 0.75 = 33.75 SOUL, rounded down to 33.
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
            if (hitInstance.Source.name.Contains("Grubberfly"))
            {
                int baseDamage = hitInstance.DamageDealt;
                int bonusDamage = GetBonus(baseDamage);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - {baseDamage} damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Gets the damage modifier for Grubberfly's Elegy
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Per my Utils, Grubberfly's Elegy costs only 1/4 of its actual value due to its Full Health requirement.
            // So the 2.25 notches we use to modify its damage get multipled to 9.
            float notchValue = 3 * 0.75f * NotchCosts.FullHealthModifier();

            // So we need to increase damage dealt by Elegy beams by 90%, for a total of 140%
            float glyphModifier = NotchCosts.NailDamagePerNotch() * notchValue;
            float totalModifier = 0.5f + glyphModifier;

            // 0.5 + 0.5x = 1.4, x = 1.8
            float modifier = (totalModifier - 0.5f) / 0.5f;
            //SharedData.Log($"{ID} - Elegy modifier: {modifier}");

            return modifier;
        }
    }
}