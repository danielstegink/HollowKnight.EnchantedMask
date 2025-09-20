using DanielSteginkUtils.Helpers.Shields;
using DanielSteginkUtils.Utilities;
using System.Collections;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class GodShield : ShieldHelper
    {
        /// <summary>
        /// God uses 1 notch to block damage, making it worth a 7% chance of blocking
        /// </summary>
        /// <returns></returns>
        public override bool CustomShieldCheck()
        {
            int random = UnityEngine.Random.Range(1, 101);
            return random <= 7;
        }

        /// <summary>
        /// God flashes gold when triggered
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            SpriteFlash flash = ClassIntegrations.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");
            flash.flash(UnityEngine.Color.yellow, 0.5f, 0.5f, 0.01f, 0.75f);

            yield return base.CustomEffects();
        }
    }
}