using EnchantedMask.Settings;
using System.Collections;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class RoyalShield : BlockHelper
    {
        /// <summary>
        /// The Royal glyph only triggers on environmental damage
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        /// <returns></returns>
        public override bool CanTakeDamage(int damageAmount, int hazardType)
        {
            return base.CanTakeDamage(damageAmount, hazardType) && 
                    hazardType != 1;
        }

        /// <summary>
        /// As an Epic glyph, Royal is worth 4 notches. Normally, that would be worth a 29% chance.
        /// Since it only affects hazard damage, its value can be increased. However, you would 
        ///     realistically only wear this during parkour challenges where there are few, if any,
        ///     enemies. So it shouldn't be worth much more. I'll go with a 25% boost for now.
        /// </summary>
        /// <returns></returns>
        public override bool CustomBlockCheck()
        {
            int random = UnityEngine.Random.Range(1, 101);
            return random <= 36;
        }

        /// <summary>
        /// Royal flashes white when triggered
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            SpriteFlash flash = SharedData.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");
            flash.flashWhitePulse();

            yield return base.CustomEffects();
        }
    }
}
