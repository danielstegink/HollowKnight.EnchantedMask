using EnchantedMask.Settings;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Green : Glyph
    {
        public override string ID => "Green";
        public override string Name => "Glyph of Unn";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of the master of Greenpath.\n\n" +
                                                "Increases the bearer's affinity with Unn.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotCharm_28;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.gotCharm_28)
            {
                return "The mother of the Green Children sleeps beneath her lake.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            GameManager.instance.StartCoroutine(SpeedBoost());
        }

        public override void Unequip()
        {
            base.Unequip();
        }

        /// <summary>
        /// Tracks whether or not we want to increase speed
        /// </summary>
        private bool applyBuff = true;

        /// <summary>
        /// Green increases movement speed while healing with Shape of Unn equipped.
        /// SOU affects the 2D body instead of using the HeroController's move event,
        ///     so we need to use this coroutine to monitor it instead.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpeedBoost()
        {
            // If the glyph has been unequipped, we can stop
            while (IsEquipped())
            {
                // We only need to apply it if we're healing and SOU is equipped
                if (PlayerData.instance.equippedCharm_28 &&
                    HeroController.instance.cState.focusing)
                {
                    Rigidbody2D rb2d = SharedData.GetField<HeroController, Rigidbody2D>(HeroController.instance, "rb2d");
                    if (rb2d.velocity.x == 0) // If we've stopped, the movement has reset and we need to re-apply the buff
                    {
                        applyBuff = true;
                    }
                    else if (applyBuff) 
                    {
                        rb2d.velocity = new Vector2(rb2d.velocity.x * GetModifier(), rb2d.velocity.y);
                        //SharedData.Log($"{ID} - RB2D speed: {rb2d.velocity}");
                        applyBuff = false; // Once we've applied the buff, we don't want to keep applying it
                    }
                }
                else // If we stopped focusing or SOU is unequipped, that is a reset
                {
                    applyBuff = true;
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }

            // Once the coroutine stops, that is also a reset
            applyBuff = true;
        }

        /// <summary>
        /// Green is a Common glyph, so its worth 1 notch.
        /// SOU gives the speed boost it does for 2 notches, so a 50% increase makes sense.
        /// SOU also has visual synergies with Baldur Shell and Spore Shroom, but no actual 
        ///     synergies, so Green will include them in its calculations.
        /// </summary>
        internal override float GetModifier()
        {
            float modifier = 1.5f;

            // Spore Shroom
            if (PlayerData.instance.equippedCharm_17)
            {
                modifier += 0.05f;
            }

            // Baldur Shell
            if (PlayerData.instance.equippedCharm_5)
            {
                modifier += 0.1f;
            }

            //SharedData.Log($"{ID} - modifier: {modifier}");
            return modifier;
        }
    }
}
