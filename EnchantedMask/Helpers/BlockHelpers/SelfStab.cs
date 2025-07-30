using EnchantedMask.Settings;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class SelfStab : BlockHelper
    {
        public override bool blockFirstDamage { get; set; } = false;

        /// <summary>
        /// As an Epic glyph, Hollow is worth 4 notches. 
        /// Stalwart Shell is a 2-notch charm with 2 effects, one of which is increasing I-Frames from
        ///     1.3 seconds to 1.75, a 35% increase. If we assume the effects are equal, that means
        ///     1 notch is worth a 35% boost in I-Frames.
        /// So for 4 notches, we should increase I-Frames by 140%, for a total of 3.12 seconds.
        /// On a side note, Hollow is infection-inspired, so flashing orange would be nice.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            SpriteFlash flash = SharedData.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");

            // I realize this is slightly off, but this way we can do an even number of flashes
            for (int i = 0; i < 9; i++)
            {
                flash.flashInfected();
                yield return new WaitForSeconds(0.35f);
            }
        }
    }
}
