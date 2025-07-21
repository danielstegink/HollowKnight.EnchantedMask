using System.Collections;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Royal : Glyph
    {
        public override string ID => "Royal";
        public override string Name => "Glyph of the King";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the king of Hallownest.\n\n" +
                                                "May create a royal aura that defends the bearer from worldly dangers.";

        public override bool Unlocked()
        {
            return PlayerData.instance.gotQueenFragment &&
                    PlayerData.instance.gotKingFragment;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.gotQueenFragment)
            {
                return "The Queen hides with a memory of her Wyrm.";
            }
            else if (!PlayerData.instance.gotKingFragment)
            {
                return "An echo of the King sits upon a memory of his throne.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.TakeDamage += HazardShield;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.TakeDamage -= HazardShield;
        }

        /// <summary>
        /// Checks if the player is currently immune to damage
        /// </summary>
        private bool isImmune = false;

        /// <summary>
        /// Royal has a chance to negate damage from environmental hazards such as thorns and buzzsaws
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        private void HazardShield(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, GlobalEnums.CollisionSide damageSide,
            int damageAmount, int hazardType)
        {
            // Confirm that damage was taken and the damage type wasn't Enemies (1)
            if (CanTakeDamage(hazardType) &&
                hazardType != 1)
            {
                // Check if currently immune
                if (isImmune)
                {
                    damageAmount = 0;
                }
                else // Otherwise, run the numbers
                {
                    int random = UnityEngine.Random.Range(1, 101);
                    //SharedData.Log($"{ID} - Checking shield: {random}");
                    if (random <= BlockChance())
                    {
                        damageAmount = 0;
                        isImmune = true;
                        GameManager.instance.StartCoroutine(StayImmune());
                    }
                }
            }

            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// As an Epic glyph, Royal is worth 4 notches.
        /// If this was defending against regular damage, 
        ///     that would be worth a 29% chance.
        /// Since this only defends against environmental damage,
        ///     the value can be much higher.
        /// </summary>
        /// <returns></returns>
        private int BlockChance()
        {
            return 36;
        }

        /// <summary>
        /// The player could potentially end up getting damaged immediately after 
        /// ignoring damage due to collision rules. So, the player will get 1 second 
        /// of immunity to escape to safety.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StayImmune()
        {
            HeroController.instance.GetComponent<SpriteFlash>().flashWhiteLong();
            yield return new WaitForSeconds(1f);
            isImmune = false;
        }
    }
}
