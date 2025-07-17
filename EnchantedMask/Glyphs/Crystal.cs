using EnchantedMask.Settings;
using System;

namespace EnchantedMask.Glyphs
{
    public class Crystal : Glyph
    {
        public override string ID => "Crystal";
        public override string Name => "Glyph of Crystals";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The sparkling symbol of a crystallised miner.\n\n" +
                                                "Increases the bearer's affinity with the Crystal Heart.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killsMegaBeamMiner == 0;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedMegaBeamMiner)
            {
                return "An infected miner sleeps peacefully.";
            }
            else if (PlayerData.instance.killsMegaBeamMiner > 0)
            {
                return "The infected miner demands a rematch.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HealthManager.TakeDamage += BuffCDash;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HealthManager.TakeDamage -= BuffCDash;
        }

        /// <summary>
        /// The Crystal glyph increases Crystal Heart damage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        private void BuffCDash(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (hitInstance.Source.name.Equals("SuperDash Damage"))
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - CDash damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// As a Common glyph, the CDash should be equivalent to a 1-notch charm.
        /// This would be worth a 10% increase in nail damage. However, CDash is a 
        ///     very niche attack, so I'm comfortable going much higher.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.5f;
        }
    }
}
