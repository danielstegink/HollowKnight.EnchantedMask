using EnchantedMask.Settings;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class FriendshipShield : BlockHelper
    {
        /// <summary>
        /// Friendship is an Epic glyph worth 4 notches.
        /// CM gives a 22% chance of blocking for 3 notches, 
        ///     so Friendship should increase the chance by 29%, for a total of 51%.
        /// (1 - 0.22) * (1 - X) = (1 - 0.51), X = 37%.
        /// </summary>
        /// <returns></returns>
        public override bool CustomBlockCheck()
        {
            // Of course, we require that Carefree Melody be equipped
            int carefreeId = GetCarefreeId();
            if (carefreeId <= 0 ||
                !PlayerData.instance.GetBool($"equippedCharm_{carefreeId}"))
            {
                return false;
            }

            int random = UnityEngine.Random.Range(1, 101);
            return random <= 37;
        }

        /// <summary>
        /// Friendship triggers the CM shield when it blocks
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            GameObject carefreeShield = HeroController.instance.carefreeShield;
            if (carefreeShield != null)
            {
                carefreeShield.SetActive(true);
            }

            yield return base.CustomEffects();
        }

        /// <summary>
        /// Gets the charm ID for Carefree Melody
        /// </summary>
        /// <returns></returns>
        private int GetCarefreeId()
        {
            int grimmchildLevel = PlayerData.instance.grimmChildLevel;
            if (grimmchildLevel == 5)
            {
                return 40;
            } // todo - implement carefree grimm at some point, or set up the code to find carefree melody
            else
            {
                return -1;
            }
        }
    }
}
