using DanielSteginkUtils.Utilities;
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
        /// </summary>
        /// <returns></returns>
        internal override int GetBonus(int baseValue)
        {
            // Per my Utils, 1 notch is worth half nail damage.
            return (int)NotchCosts.DashDamagePerNotch();
        }
    }
}
