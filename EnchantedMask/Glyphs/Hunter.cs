using System;

namespace EnchantedMask.Glyphs
{
    public class Hunter : Glyph
    {
        public override string ID => "Hunter";
        public override string Name => "Glyph of the Hunter";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of a mighty hunter.\n\n" +
                                                "Increases the bearer's insight, allowing them to hunt prey more easily.";

        public override bool Unlocked()
        {
            return PlayerData.instance.hasHuntersMark;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasJournal)
            {
                return "A veteran hunter waits to offer you a quest.";
            }
            else if (!PlayerData.instance.hasHuntersMark)
            {
                return "The Hunter's final reward remains unclaimed.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffDamage;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffDamage;
        }

        /// <summary>
        /// The Hunter glyph increases all damage dealt, or at least all nail and spell damage (pets are such a chore)
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            int bonusDamage = GetBonus(hitInstance.DamageDealt);
            hitInstance.DamageDealt += bonusDamage;
            //SharedData.Log($"{ID} - Damage increased by {bonusDamage}");

            orig(self, hitInstance);
        }

        /// <summary>
        /// As a Rare glyph, Hunter is worth 3 notches.
        /// That would be worth a 30% boost in nail strength or a 
        ///     45% boost in spell damage, but for all damage, the 
        ///     value should be much smaller.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.2f;
        }
    }
}