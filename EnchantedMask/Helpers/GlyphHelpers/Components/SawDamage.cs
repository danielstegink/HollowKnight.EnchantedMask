using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Helpers.GlyphHelpers.Components
{
    /// <summary>
    /// Handles damage dealt by Champion buzzsaws
    /// </summary>
    public class SawDamage : DamageEnemies
    {
        public SawDamage()
        {
            attackType = AttackTypes.Spell; // Saws are conjured via magic
            damageDealt = 0;
            direction = 2;
            magnitudeMult = 1f;
        }

        /// <summary>
        /// Calculates the amount of damage SawDamage should do
        /// </summary>
        /// <param name="isUpgraded"></param>
        /// <returns></returns>
        internal static int GetDamage(bool isUpgraded)
        {
            // This ability replaces Vengeful Spirit, but its stationary hitbox is closer to Howling Wraiths
            // HW and AS both deal damage split into parts, reasonably doing 30 and 60 dmg respectively
            int damage = isUpgraded ? 60 : 30;

            // Per my Utils, 1 notch is worth a +50% bonus to a single spell
            float bonusPerNotch = NotchCosts.SingleSpellDamagePerNotch();

            // The Champion glyph is a Legendary which makes it worth 5 notches for a total of +250%
            float modifier = bonusPerNotch * 5;

            // However, the buzzsaw lasts 5 seconds and hits an enemy every second, allowing up to 5 hits on a
            // stationary target
            modifier /= 3;

            // Finally, attach the bonus to the base 100% damage we do already
            modifier += 1;

            // Apply charm synergy bonuses
            // Flukenest increases spell damage by up to 100% in exchange for reduced accuracy
            if (PlayerData.instance.equippedCharm_11)
            {
                modifier *= 2;
            }
            // Shaman Stone increases spell damage by an average of 50% if it doesn't affect spell size
            if (PlayerData.instance.equippedCharm_19)
            {
                modifier *= 1.5f;
            }

            return (int)(damage * modifier);
        }
    }
}