using EnchantedMask.Settings;
using GlobalEnums;
using System;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class BlockHelper
    {
        /// <summary>
        /// Stores if the player is currently immune to damage
        /// </summary>
        private bool isImmune = false;

        /// <summary>
        /// Whether or not we negate damage the first time the block is triggered
        /// </summary>
        public virtual bool blockFirstDamage { get; set; } = true;

        /// <summary>
        /// Hooks into the TakeDamage event so we can negate damage if needed
        /// </summary>
        public virtual void ApplyHook()
        {
            On.HeroController.TakeDamage += Block;
        }

        /// <summary>
        /// Unhooks from the TakeDamage event when we don't want to block damage anymore
        /// </summary>
        public void RemoveHook()
        {
            On.HeroController.TakeDamage -= Block;
        }

        /// <summary>
        /// Performs a check to see if we can block damage, and handles the 
        ///     blocking process
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Block(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            if (CanTakeDamage(damageAmount, hazardType))
            {
                if (isImmune)
                {
                    damageAmount = 0;
                }
                else if (CustomBlockCheck())
                {
                    if (blockFirstDamage)
                    {
                        damageAmount = 0;
                    }

                    GameManager.instance.StartCoroutine(Invincibility());
                }
            }

            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// Determines if the player can take damage
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        /// <returns></returns>
        public virtual bool CanTakeDamage(int damageAmount, int hazardType)
        {
            // Damage is calculated by the HeroController's TakeDamage method
            // First, run the CanTakeDamage check from the HeroController
            bool canTakeDamage = SharedData.CallFunction<HeroController, bool>(HeroController.instance, "CanTakeDamage", null);
            if (canTakeDamage)
            {
                // There is an additional check for I-Frames and shadow dashing
                if (hazardType == 1)
                {
                    if (HeroController.instance.damageMode == DamageMode.HAZARD_ONLY ||
                        HeroController.instance.cState.shadowDashing ||
                        HeroController.instance.parryInvulnTimer > 0f)
                    {
                        canTakeDamage = false;
                    }
                }
                else if (HeroController.instance.cState.invulnerable ||
                            PlayerData.instance.isInvincible)
                {
                    canTakeDamage = false;
                }
                else if (damageAmount <= 0)
                {
                    canTakeDamage = false;
                }
            }

            return canTakeDamage;
        }

        /// <summary>
        /// Placeholder for special logic determining if we can block the next attack
        /// </summary>
        /// <returns></returns>
        public virtual bool CustomBlockCheck()
        {
            return true;
        }

        /// <summary>
        /// Provides imitation for the I-Frames player usually gets between attacks
        /// </summary>
        /// <returns></returns>
        public IEnumerator Invincibility()
        {
            isImmune = true;

            yield return CustomEffects();

            isImmune = false;
        }

        /// <summary>
        /// Placeholder for special I-Frame effects unique to the calling class
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator CustomEffects()
        {
            yield return new WaitForSeconds(1.3f);
        }
    }
}
