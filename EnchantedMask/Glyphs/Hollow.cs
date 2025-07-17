using EnchantedMask.Settings;
using GlobalEnums;
using Modding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Hollow : Glyph
    {
        public override string ID => "Hollow";
        public override string Name => "Glyph of Sacrifice";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the kingdom's greatest hero.\n\n" +
                                                "After taking damage, a coating of Infection protects the bearer form damage for a short time.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedHollowKnight;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.lurienDefeated || 
                !PlayerData.instance.monomonDefeated || 
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (!PlayerData.instance.killedHollowKnight)
            {
                return "The greatest of knights holds the Infection at bay.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.TakeDamage += SelfStab;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.TakeDamage -= SelfStab;
        }

        /// <summary>
        /// Stores if the player is currently immune to damage
        /// </summary>
        private bool isImmune = false;

        /// <summary>
        /// The Hollow glyph negates all damage for a short time after taking damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        private void SelfStab(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            if (damageAmount > 0)
            {
                if (isImmune)
                {
                    damageAmount = 0;
                }
                else
                {
                    GameManager.instance.StartCoroutine(StartDamageShield());
                }
            }

            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// As an Epic glyph, Hollow is worth 4 notches. 
        /// There honestly isn't a good way to calculate what's balanced, so I'm going to 
        ///     just play it by ear.
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartDamageShield()
        {
            SharedData.Log($"{ID} - Immunity started");
            Stopwatch timer = Stopwatch.StartNew();

            isImmune = true;
            SpriteFlash flash = SharedData.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");

            for (int i = 0; i < 8; i++)
            {
                flash.flashInfected();
                yield return new WaitForSeconds(0.5f);

                flash.flashInfected();
                yield return new WaitForSeconds(0.5f);
            }

            isImmune = false;
            SharedData.Log($"{ID} - Immunity ended. Total time: {timer.ElapsedMilliseconds}ms");
        }
    }
}
