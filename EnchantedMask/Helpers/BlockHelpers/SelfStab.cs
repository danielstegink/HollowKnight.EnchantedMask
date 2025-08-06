using DanielSteginkUtils.Helpers.Shields;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class SelfStab : ShieldHelper
    {
        /// <summary>
        /// Self Stab doesn't block damage, it blocks subsequent damage
        /// </summary>
        public override bool blockFirstDamage { get; set; } = false;

        public override bool CustomShieldCheck()
        {
            return true;
        }

        /// <summary>
        /// The Hollow glyph is inspired by the Hollow Knight's Self Stab ability, and gives the player increased I-Frames
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            SpriteFlash flash = ClassIntegrations.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");

            // Determine how long the I-Frames should last
            float modifier = GetModifier();
            float defaultIFramesLength = modifier * HeroController.instance.INVUL_TIME;

            // On a side note, Hollow is infection-inspired, so flashing orange would be nice
            // The infected flash lasts about 0.26 seconds, so we will split the I-Frames into 0.3 second increments 
            int loops = (int)(defaultIFramesLength / 0.3f);
            for (int i = 0; i < loops; i++)
            {
                flash.flashInfected();
                yield return new WaitForSeconds(0.3f);
            }
        }

        /// <summary>
        /// Gets how much to increase the I-Frames by
        /// </summary>
        /// <returns></returns>
        private float GetModifier()
        {
            // As an Epic glyph, Hollow is worth 4 notches
            // Per my Utils, 1 notch is worth a 35% increase in I-Frames
            // So for 4 notches, we should increase I-Frames by 140%, for a total of 3.12 seconds
            float modifier = 1 + 4 * NotchCosts.IFramesPerNotch();

            // However, if Stalwart Shell is equipped, we want to add to instead of multiply the
            // 35% boost already present for a total of 3.57 seconds
            if (PlayerData.instance.equippedCharm_4)
            {
                // 1.35 * X = 1 + (1.4 + 0.35)
                return (1 + 5 * NotchCosts.IFramesPerNotch()) / (1 + NotchCosts.IFramesPerNotch());
            }

            return modifier;
        }
    }
}