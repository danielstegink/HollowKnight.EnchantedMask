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
                return "A shaman's remains lie trapped in stone";
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

            isActive = true;
            GameManager.instance.StartCoroutine(GiveSoul());
        }

        public override void Unequip()
        {
            base.Unequip();

            isActive = false;
        }

        /// <summary>
        /// Tracks if the glyph is active
        /// </summary>
        private bool isActive = false;

        /// <summary>
        /// The Snail glyph gives passive SOUL regeneration, like Kingsoul.
        /// Snail is an Uncommon glyph, making it worth 2 notches.
        /// Kingsoul gives 4 SOUL every 2 seconds for 5 notches, averaging 2 SOUL per second.
        /// So Snail will give 1 SOUL every 1.25 seconds.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GiveSoul()
        {
            while (isActive)
            {
                HeroController.instance.AddMPCharge(1);
                yield return new WaitForSeconds(1.25f);
                //SharedData.Log($"{ID} - 1 SOUL added");
            }
        }
    }
}
