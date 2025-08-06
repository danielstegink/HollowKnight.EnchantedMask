using DanielSteginkUtils.Utilities;
using System.Collections;
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

            GameManager.instance.StartCoroutine(GiveSoul());
        }

        public override void Unequip()
        {
            base.Unequip();
        }

        /// <summary>
        /// The Snail glyph gives passive SOUL regeneration, like Kingsoul.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GiveSoul()
        {
            while (IsEquipped())
            {
                // Snail is an Uncommon glyph, making it worth 2 notches.
                // Per my Utils, 1 notch is worth 1 SOUL every 2.5 seconds
                // So Snail will give 1 SOUL every 1.25 seconds
                HeroController.instance.AddMPCharge(1);
                yield return new WaitForSeconds(NotchCosts.PassiveSoulTime() / 2);
                //SharedData.Log($"{ID} - 1 SOUL added");
            }
        }
    }
}
