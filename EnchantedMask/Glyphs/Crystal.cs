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
        /// As a Common glyph, Crystal should be equivalent to a 1-notch charm.
        /// CDash is a niche dash, much like DDash.
        /// Sharp Shadow uses 2 notches to give DDash damage equal to our nail.
        /// So for 1 notch, Crystal can increase CDash damage by an amount
        ///     equal to half our current nail damage.
        /// </summary>
        /// <returns></returns>
        internal override int GetBonus(int baseValue)
        {
            return PlayerData.instance.nailDamage / 2;
        }
    }
}
