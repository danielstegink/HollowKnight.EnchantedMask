using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Bound : Glyph
    {
        public override string ID => "Bound";
        public override string Name => "Glyph of Binding";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of a powerful sealing spell.\n\n" +
                                                "Increases the bearer's affinity with thorns.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedBindingSeal;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.visitedWhitePalace)
            {
                return "A memory of the king's palace hides in a servant's dreams.";
            }
            else if (!PlayerData.instance.killedBindingSeal)
            {
                return "A secret memory is sealed behind a gauntlet of thorns and saws.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffThorns;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffThorns;
        }

        /// <summary>
        /// Bound increases damage dealt by Thorns of Agony
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void BuffThorns(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.transform.parent != null)
            {
                Transform parent = hitInstance.Source.transform.parent;
                if (parent.name.Equals("Thorn Hit"))
                {
                    int bonusDamage = GetBonus(hitInstance.DamageDealt);
                    hitInstance.DamageDealt += bonusDamage;
                }
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Bound is a Legendary glyph worth 5 notches.
        /// TOG is a 1-notch charm, so we can increase its damage 6-fold.
        /// </summary>
        internal override float GetModifier()
        {
            return 5f;
        }
    }
}