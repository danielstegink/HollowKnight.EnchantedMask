using DanielSteginkUtils.Helpers.Charms.Pets;
using DanielSteginkUtils.Helpers.Shields;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Helpers.UI;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Nightmare : Glyph
    {
        public override string ID => "Nightmare";
        public override string Name => $"Glyph of {(PlayerData.instance.destroyedNightmareLantern ? "Friendship" : "Nightmares")}";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => PlayerData.instance.destroyedNightmareLantern ?
                                                "The symbol of a friend awoken from a bad dream.\n\n" +
                                                    "Increases the bearer's affinity with protective music." :
                                                "The symbol of a nightmare brought to life.\n\n" +
                                                    "Increases the bearer's affinity with scarlet flames.";
        public override Sprite GetIcon()
        {
            if (PlayerData.instance.destroyedNightmareLantern)
            {
                return SpriteHelper.GetMaskSprite("Friendship");
            }
            else
            {
                return base.GetIcon();
            }
        }

        public override bool Unlocked()
        {
            return PlayerData.instance.destroyedNightmareLantern ||
                    PlayerData.instance.defeatedNightmareGrimm;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.nightmareLanternLit)
            {
                return "A beacon sits unlit among howling winds.";
            }
            else if (PlayerData.instance.grimmChildLevel < 3)
            {
                return "A child of nightmare craves power.";
            }
            else if (!PlayerData.instance.destroyedNightmareLantern &&
                        !PlayerData.instance.defeatedNightmareGrimm)
            {
                return "Two fates stand before you. Will you complete the ritual, or will you end the nightmare forever?";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            if (PlayerData.instance.destroyedNightmareLantern)
            {
                carefreeHelper = new CarefreeHelper(GetShieldChance());
                carefreeHelper.Start();
            }
            else
            {
                grimmchildHelper = new GrimmchildHelper(GetModifier(), 1f, false);
                grimmchildHelper.Start();
            }
        }

        public override void Unequip()
        {
            base.Unequip();

            if (carefreeHelper != null)
            {
                carefreeHelper.Stop();
            }

            if (grimmchildHelper != null)
            {
                grimmchildHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private CarefreeHelper carefreeHelper;

        /// <summary>
        /// Friendship adds a second chance of CM triggering
        /// </summary>
        /// <returns></returns>
        private int GetShieldChance()
        {
            // Friendship is an Epic glyph worth 4 notches
            // Per my Utils, blocking an attack is worth a 7.49% chance per notch, or 29% for 4 notches.
            float bonus = 4 * NotchCosts.ShieldChancePerNotch();

            // We then feed into Calculations as this is meant to pair with the default CM shield
            // Per my own math, a 29% boost to CM should result in a second shield with a chance of about 38%
            return Calculations.GetSecondMelodyShield(bonus);
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private GrimmchildHelper grimmchildHelper;

        /// <summary>
        /// Nightmare increases damage dealt by Grimmchild
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Nightmare is an Epic glyph worth 4 notches
            // Grimmchild is worth 2 notches, so Nightmare can triple the damage
            return 3f;
        }
    }
}