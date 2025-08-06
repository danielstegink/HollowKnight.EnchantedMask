using DanielSteginkUtils.Utilities;
using System;
using System.Collections.Generic;

namespace EnchantedMask.Glyphs
{
    public class Grey : Glyph
    {
        public override string ID => "Grey";
        public override string Name => "Glyph of Loneliness";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of delusional infatuation.\n\n" +
                                                "Increases the power of the bearer's Desolate Dive spell.";

        public override bool Unlocked()
        {
            return PlayerData.instance.greyPrinceDefeats >= 4;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.zoteDead)
            {
                return "A self-proclaimed Knight has fallen to a superior foe.";
            }
            else if (!PlayerData.instance.zoteRescuedBuzzer)
            {
                return "A self-proclaimed Knight lies trapped in the jaws of a beast.";
            }
            else if (!PlayerData.instance.zoteRescuedDeepnest)
            {
                return "A self-proclaimed Knight is trapped in darkness and thread.";
            }
            else if (!PlayerData.instance.zoteSpokenColosseum)
            {
                return "A self-proclaimed Knight is once again trapped, this time caged by Fools.";
            }
            else if (!PlayerData.instance.zoteDefeated)
            {
                return "A self-proclaimed Knight tests his might in a grand arena.";
            }
            else if (!PlayerData.instance.brettaRescued)
            {
                return "A helpless damsel lies trapped among mushrooms and spikes.";
            }
            else if (PlayerData.instance.greyPrinceDefeats < 4)
            {
                return "A damsel's dreams transform a helpless knight into a mighty prince.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffDive;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffDive;
        }

        /// <summary>
        /// The Grey glyph increases Dive spell damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffDive(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.AttackType == AttackTypes.Spell &&
                diveDamageNames.Contains(hitInstance.Source.name))
            {
                //SharedData.Log($"{hitInstance.DamageDealt} damage dealt by {hitInstance.Source.name}");

                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - {hitInstance.Source.name} damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Gets the Dive spell damage modifier
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Grey is an Epic glyph worth 4 notches.
            // Per my Utils, this is worth a 200% increase in the damage of a single spell.
            return 4f * NotchCosts.SingleSpellDamagePerNotch();
        }

        /// <summary>
        /// List of damage objects for the Dive spells
        /// </summary>
        private List<string> diveDamageNames = new List<string>()
        {
            "Q Fall Damage",
            "Hit L",
            "Hit R"
        };
    }
}