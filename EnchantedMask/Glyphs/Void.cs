using EnchantedMask.Helpers.GlyphHelpers;
using EnchantedMask.Settings;
using GlobalEnums;
using Satchel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Void : Glyph
    {
        public override string ID => "Void";
        public override string Name => "Glyph of Destruction";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of the master of darkness and destruction.\n\n" +
                                                "Transforms the Howling Wraiths spell into a team of deadly Siblings.";

        public override bool Unlocked()
        {
            return PlayerData.instance.screamLevel == 2 &&
                    PlayerData.instance.killedVoidIdol_1;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.screamLevel < 2)
            {
                return "The dark secrets of spellcraft remain hidden from you.";
            }
            else if (!PlayerData.instance.visitedGodhome)
            {
                return "A colony of pilgrims attune themselves while buried in garbage.";
            }
            else if (!PlayerData.instance.killedVoidIdol_1)
            {
                return "There are gods who sit undefeated in their hall.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            AddSpell();
            GameManager.instance.StartCoroutine(VoidSpell.SiblingMovement());
        }

        public override void Unequip()
        {
            base.Unequip();

            RemoveSpell();
        }

        /// <summary>
        /// Void replaces Shriek with a new spell, Abyssal Wraiths
        /// </summary>
        private void AddSpell()
        {
            PlayMakerFSM spellControl = HeroController.instance.spellControl;
            spellControl.ChangeTransition("Level Check 3", "LEVEL 2", "EnchantedMask Scream2");
        }

        /// <summary>
        /// Resets the Spell Control FSM when the glyph is removed
        /// </summary>
        private void RemoveSpell()
        {
            try
            {
                PlayMakerFSM spellControl = HeroController.instance.spellControl;
                spellControl.ChangeTransition("Level Check 3", "LEVEL 2", "Scream Antic2");
            }
            catch { } // Possible that the hero controller won't be fully initialized yet
        }        
    }
}