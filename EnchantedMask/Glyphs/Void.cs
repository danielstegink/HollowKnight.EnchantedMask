using DanielSteginkUtils.Utilities;
using GlobalEnums;
using System;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Void : Glyph
    {
        public override string ID => "Void";
        public override string Name => "Glyph of Destruction";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of the god of darkness.\n\n" +
                                                "Envelops the bearer in Void, increasing the damage they deal while also increasing the damage they take.";

        public override bool Unlocked()
        {
            return PlayerData.instance.screamLevel == 2 &&
                    PlayerData.instance.killedVoidIdol_3;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.screamLevel < 2)
            {
                return "The dark secrets of spellcraft remain hidden from you.";
            }
            else if (!PlayerData.instance.visitedGodhome)
            {
                return "A colony of pilgrims attune themselves while buried in garbage.";
            }
            else if (!PlayerData.instance.killedVoidIdol_3)
            {
                return "There are gods who sit undefeated in their hall.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.TakeDamage += TakeDoubleDamage;
            On.HealthManager.TakeDamage += BuffDamage;
        }

        public override void Unequip()
        {
            base.Unequip();
            On.HeroController.TakeDamage -= TakeDoubleDamage;
            On.HealthManager.TakeDamage -= BuffDamage;
        }

        /// <summary>
        /// The Void glyph causes the player to take double damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        private void TakeDoubleDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            damageAmount *= 2;
            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// The Void glyph increases damage the player deals
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            int baseDamage = hitInstance.DamageDealt;
            int bonusDamage = GetBonus(baseDamage);
            hitInstance.DamageDealt += bonusDamage;
            //SharedData.Log($"{ID} - {baseDamage} damage increased by {bonusDamage}");

            orig(self, hitInstance);
        }

        /// <summary>
        /// Gets the damage modifier
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // As a Legendary glyph, Void is worth 5 notches
            int notchValue = 5;
            // However, we also take double damage, which is like having half the usual health
            float maxHealth = PlayerData.instance.GetInt("maxHealth");
            int healthLost = (int)Math.Floor(maxHealth / 2);
            // Per my Utils, 1 health is worth 2 notches
            notchValue += (int)(NotchCosts.NotchesPerMask() * healthLost);

            // Per my Utils, 1 notch is worth a 6.67% increase in all damage dealt, which comes out to about 87% at the default max 9 health
            float modifier = notchValue * NotchCosts.DamagePerNotch();
            //SharedData.Log($"{ID} - Total health: {maxHealth}, Total notches: {notchValue}, Damage modifier: {modifier}");
            return modifier;
        }
    }
}