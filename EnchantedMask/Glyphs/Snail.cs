using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Snail : Glyph
    {
        public override string ID => "Snail";
        public override string Name => "Glyph of Shamanism";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of Hallownest's greatest spellcasters.\n\n" +
                                                "Helps the bearer passively absorb SOUL from their surroundings.";

        public override bool Unlocked()
        {
            return PlayerData.instance.fireballLevel == 2 &&
                    PlayerData.instance.screamLevel == 2 &&
                    PlayerData.instance.quakeLevel == 2 &&
                    PlayerData.instance.gotCharm_21;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.fireballLevel == 0)
            {
                return "A reclusive shaman sits in his ancestral home.";
            }
            else if (PlayerData.instance.quakeLevel == 0)
            {
                return "A scorned sorcerer pursues the mysteries of the soul.";
            }
            else if (PlayerData.instance.quakeLevel < 2)
            {
                return "A shaman's remains lie trapped in crystal.";
            }
            else if (!PlayerData.instance.gotCharm_21)
            {
                return "A tired shaman rests eternally among heroes.";
            }
            else if (PlayerData.instance.fireballLevel < 2)
            {
                return "A tortured shaman begs for release from twisted experiments.";
            }
            else if (PlayerData.instance.screamLevel == 0)
            {
                return "A vocal shaman sings no more.";
            }
            else if (PlayerData.instance.screamLevel < 2)
            {
                return "Spirits long dead cry out to be heard.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            soulTimer.Restart();
            On.HeroController.Update += GiveSoul;
        }

        public override void Unequip()
        {
            base.Unequip();

            soulTimer.Stop();
            On.HeroController.Update -= GiveSoul;
        }

        Stopwatch soulTimer = new Stopwatch();

        /// <summary>
        /// The Snail glyph gives passive SOUL regeneration, like Kingsoul.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void GiveSoul(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);

            // Snail is an Uncommon glyph, making it worth 2 notches.
            // Per my Utils, 1 notch is worth 1 SOUL every 2.5 seconds
            // So Snail will give 1 SOUL every 1.25 seconds
            if (soulTimer.ElapsedMilliseconds >= 1000 * NotchCosts.PassiveSoulTime() / 2)
            {
                HeroController.instance.AddMPCharge(1);
                soulTimer.Restart();
            }
        }
    }
}
