using EnchantedMask.Helpers.UI;
using EnchantedMask.Settings;
using GlobalEnums;
using System;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Glyph
    {
        /// <summary>
        /// List of tiers for Masks.
        /// A mask's tier should reflect its power, cost, and how difficult it is to unlock.
        /// </summary>
        public enum Tiers
        {
            Default,
            Common, // Equivalent to a 1-notch mask
            Uncommon, // Equivalent to a 2-notch mask
            Rare, // Equivalent to a 3-notch mask
            Epic, // Equivalent to a 4-notch mask
            Legendary // Equivalent to a 5-notch mask
        }

        public virtual string ID => "";

        public virtual string Name => "";

        public virtual Tiers Tier => Tiers.Default;
        
        public virtual string Description => "";

        internal Sprite Icon;

        public virtual Sprite GetIcon()
        {
            // Get stored sprite if we haven't already
            if (Icon == null)
            {
                // Get icon image as sprite
                Icon = SpriteHelper.GetMaskSprite(ID);
            }

            return Icon;
        }

        /// <summary>
        /// Whether or not the player has bought the glyph from Mask Maker yet
        /// </summary>
        public virtual bool Bought()
        {
            //SharedData.Log($"Bought status of {ID} glyph: {SharedData.saveSettings.GlyphsBought.Contains(ID)}");
            return SharedData.saveSettings.GlyphsBought.Contains(ID);
        }

        /// <summary>
        /// Whether or not the player has unlocked the mask
        /// </summary>
        public virtual bool Unlocked()
        {
            return false;
        }

        /// <summary>
        /// Provides a clue on how the player can unlock this glyph
        /// </summary>
        /// <returns></returns>
        public virtual string GetClue() 
        {
            return "My creator toils in darkness.";
        }

        /// <summary>
        /// Equip the glyph and trigger its effects
        /// </summary>
        public virtual void Equip() 
        {
            // Set equipped mask
            //SharedData.Log($"Equipping the {Name} ({ID})");
            SharedData.saveSettings.EquippedGlyph = ID;

            // Apply hooks
        }

        /// <summary>
        /// Unequip the mask
        /// </summary>
        public virtual void Unequip()
        {
            SharedData.saveSettings.EquippedGlyph = "";

            // Remove hooks
        }

        /// <summary>
        /// Checks if a glyph is equipped
        /// </summary>
        /// <returns></returns>
        public bool IsEquipped()
        {
            return SharedData.saveSettings.EquippedGlyph.Equals(ID);
        }

        /// <summary>
        /// Gets the string representation of the given tier
        /// </summary>
        /// <param name="tier"></param>
        /// <returns></returns>
        public static string GetTier(Tiers tier)
        {
            switch (tier)
            {
                case Tiers.Common:
                    return "Common";
                case Tiers.Uncommon:
                    return "Uncommon";
                case Tiers.Rare:
                    return "Rare";
                case Tiers.Epic:
                    return "Epic";
                case Tiers.Legendary:
                    return "Legendary";
                default:
                    return "Default";
            }
        }

        /// <summary>
        /// Gets the cost of the glyph based on its tier
        /// </summary>
        /// <returns></returns>
        public int GetCost()
        {
            switch (Tier)
            {
                case Tiers.Common:
                    return 500;
                case Tiers.Uncommon:
                    return 1000;
                case Tiers.Rare:
                    return 2000;
                case Tiers.Epic:
                    return 4000;
                case Tiers.Legendary:
                    return 8000;
                default:
                    return 500;
            }
        }

        /// <summary>
        /// Almost all glyphs use a modifier to set their power level based on their tier.
        /// And some of them are superior versions of existing glyphs, so being able to 
        /// just overwrite their lesser versions would be helpful.
        /// </summary>
        /// <returns></returns>
        internal virtual float GetModifier()
        {
            return 1.0f;
        }

        /// <summary>
        /// Calculates the extra value (probly damage) that the modifier should apply (minimum 1)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        internal virtual int GetBonus(int baseValue)
        {
            return (int)Math.Max(1, baseValue * GetModifier());
        }

        /// <summary>
        /// Determines if the player can take damage
        /// </summary>
        /// <param name="hazardType"></param>
        /// <returns></returns>
        internal bool CanTakeDamage(int hazardType)
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
            }

            return canTakeDamage;
        }
    }
}