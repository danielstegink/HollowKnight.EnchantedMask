using EnchantedMask.Helpers.BlockHelpers;
using EnchantedMask.Settings;
using System;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class God : Glyph
    {
        public override string ID => "God";
        public override string Name => "Glyph of the Godseekers";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of those who seek power through attunement.\n\n" +
                                                "Attunes to the bearer, increasing their basic abilities.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedGodseekerMask;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.visitedGodhome)
            {
                return "A colony of pilgrims attune themselves while buried in garbage.";
            }
            else if (!AllBindingsCompleted())
            {
                return "The pantheons' challenges remain unconquered.";
            }
            else if (!PlayerData.instance.killedGodseekerMask)
            {
                return "A memory of a godless land hides within a dream.";
            }
            
            return base.GetClue();
        }

        /// <summary>
        /// Determines if the player has beaten each pantheon binding in Godhome
        /// </summary>
        /// <returns></returns>
        private bool AllBindingsCompleted()
        {
            return PlayerData.instance.bossDoorStateTier1.boundNail &&
                    PlayerData.instance.bossDoorStateTier1.boundSoul &&
                    PlayerData.instance.bossDoorStateTier1.boundCharms &&
                    PlayerData.instance.bossDoorStateTier1.boundShell &&
                    PlayerData.instance.bossDoorStateTier2.boundNail &&
                    PlayerData.instance.bossDoorStateTier2.boundSoul &&
                    PlayerData.instance.bossDoorStateTier2.boundCharms &&
                    PlayerData.instance.bossDoorStateTier2.boundShell &&
                    PlayerData.instance.bossDoorStateTier3.boundNail &&
                    PlayerData.instance.bossDoorStateTier3.boundSoul &&
                    PlayerData.instance.bossDoorStateTier3.boundCharms &&
                    PlayerData.instance.bossDoorStateTier3.boundShell &&
                    PlayerData.instance.bossDoorStateTier4.boundNail &&
                    PlayerData.instance.bossDoorStateTier4.boundSoul &&
                    PlayerData.instance.bossDoorStateTier4.boundCharms &&
                    PlayerData.instance.bossDoorStateTier4.boundShell &&
                    PlayerData.instance.bossDoorStateTier5.boundNail &&
                    PlayerData.instance.bossDoorStateTier5.boundSoul &&
                    PlayerData.instance.bossDoorStateTier5.boundCharms &&
                    PlayerData.instance.bossDoorStateTier5.boundShell;
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.Move += SpeedBoost;
            BuffDashCooldown();
            On.HealthManager.TakeDamage += BuffNail;
            On.HealthManager.TakeDamage += BuffSpells;
            godShield.Start();
            GameManager.instance.StartCoroutine(GiveSoul());
        }

        public override void Unequip()
        {
            base.Unequip();

            ResetDashCooldown();
            On.HeroController.Move -= SpeedBoost;
            On.HealthManager.TakeDamage -= BuffNail;
            On.HealthManager.TakeDamage -= BuffSpells;
            godShield.Stop();
        }

        #region Speed
        /// <summary>
        /// The God glyph increases movement speed.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="move_direction"></param>
        private void SpeedBoost(On.HeroController.orig_Move orig, HeroController self, float move_direction)
        {
            move_direction *= GetSpeedModifier();
            orig(self, move_direction);
        }

        /// <summary>
        /// God uses 1/2 notch to increase movement speed, making it a 10% boost (half of Sprintmaster)
        /// </summary>
        /// <returns></returns>
        private float GetSpeedModifier()
        {
            return 1.1f;
        }
        #endregion

        #region Dash
        /// <summary>
        /// Tracks whether or not the buff has been applied;
        /// </summary>
        private bool dashApplied = false;

        /// <summary>
        /// God glyph reduces the Dash cooldown
        /// </summary>
        private void BuffDashCooldown()
        {
            if (!dashApplied)
            {
                float modifier = GetDashModifier();
                HeroController.instance.DASH_COOLDOWN *= modifier;
                HeroController.instance.DASH_COOLDOWN_CH *= modifier;
                HeroController.instance.SHADOW_DASH_COOLDOWN *= modifier;
                dashApplied = true;
            }
        }

        /// <summary>
        /// Of course, we need to reset the cooldowns when the glyph is removed
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResetDashCooldown()
        {
            if (dashApplied)
            {
                float modifier = GetDashModifier();
                HeroController.instance.DASH_COOLDOWN /= modifier;
                HeroController.instance.DASH_COOLDOWN_CH /= modifier;
                HeroController.instance.SHADOW_DASH_COOLDOWN /= modifier;
                dashApplied = false;
            }
        }

        /// <summary>
        /// God uses 1/2 notch to modify dash cooldown, making it worth 8.25% (1/4 of Dashmaster)
        /// </summary>
        /// <returns></returns>
        private float GetDashModifier()
        {
            return 0.9175f;
        }
        #endregion

        #region Nail
        /// <summary>
        /// God increases nail damage. 
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffNail(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            bool isNailAttack = SharedData.nailAttackNames.Contains(hitInstance.Source.name) ||
                               SharedData.nailArtNames.Contains(hitInstance.Source.name) ||
                               hitInstance.Source.name.Contains("Grubberfly");
            if (isNailAttack)
            {
                int bonusDamage = GetNailBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - Nail damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// God uses 1 notch to increase nail damage, making it worth a 10% increase.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int GetNailBonus(int damage)
        {
            return (int)Math.Max(1, damage * 0.1f);
        }
        #endregion

        #region Spells
        /// <summary>
        /// God increases spell damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffSpells(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.AttackType == AttackTypes.Spell)
            {
                int bonusDamage = GetSpellBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// God uses 1 notch to increase spell damage, making it worth 15% (1/3 Shaman Stone)
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int GetSpellBonus(int damage)
        {
            return (int)Math.Max(1, damage * 0.15f);
        }
        #endregion

        /// <summary>
        /// Handles damage negation for the God glyph
        /// </summary>
        private GodShield godShield = new GodShield();

        /// <summary>
        /// God gives SOUL passively over time.
        /// God uses 1 notch, so it gives 1 SOUL every 2.5 seconds.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GiveSoul()
        {
            while (IsEquipped())
            {
                HeroController.instance.AddMPCharge(1);
                yield return new WaitForSeconds(2.5f);
            }
        }
    }
}