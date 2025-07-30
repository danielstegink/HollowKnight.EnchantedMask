using System;

namespace EnchantedMask.Glyphs
{
    public class Soul : Glyph
    {
        public override string ID => "Soul";
        public override string Name => "Glyph of Sorcery";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the Soul Sanctum's twisted obsession.\n\n" +
                                                "Increases the power of the bearer's spells.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mageLordDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.mageLordDefeated)
            {
                return "A scorned sorcerer pursues the mysteries of the soul.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffSpells;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffSpells;
        }

        /// <summary>
        /// The Soul glyph increases spell damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffSpells(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.AttackType == AttackTypes.Spell)
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - Spell damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Shaman Stone, a 3-notch charm, boosts spell damage by an average of 44% across the various spells.
        /// As an Uncommon glyph, Soul is worth a 2-notch charm, and thus boosts spell damage by 30%.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.3f;
        }
    }
}