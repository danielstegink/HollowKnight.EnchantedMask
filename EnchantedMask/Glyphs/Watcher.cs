using DanielSteginkUtils.Helpers.Charms.Pets;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;

namespace EnchantedMask.Glyphs
{
    public class Watcher : Glyph
    {
        public override string ID => "Watcher";
        public override string Name => "Glyph of the Watcher";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the City's guardian protector.\n\n" +
                                                "Increases the bearer's charisma, causing their companions to deal increased damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.lurienDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.lurienDefeated)
            {
                return "The Watcher dreams from atop his Spire.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            helper = new AllPetsHelper(SharedData.modName, ID, GetModifier(), false);
            helper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (helper != null)
            {
                helper.Stop();
            }
        }

        /// <summary>
        /// Pet Utils
        /// </summary>
        private AllPetsHelper helper;

        /// <summary>
        /// The Watcher glyph increases damage dealt by pets such as Grimmchild
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Watcher glyph is an Uncommon glyph worth 2 notches
            // Per my Utils, 1 notch is worth a 16.67% increase in pet damage
            return 1 + 2 * NotchCosts.PetDamagePerNotch();
        }
    }
}