using DanielSteginkUtils.Helpers.Attributes;

namespace EnchantedMask.Helpers.GlyphHelpers
{
    public class HiveHealHelper : HealHelper
    {
        public HiveHealHelper(int healChance, bool performLogging = false) : base(healChance, performLogging)
        {
        }

        /// <summary>
        /// We only want this to trigger if Hiveblood is equipped
        /// </summary>
        /// <returns></returns>
        public override bool CustomCheck()
        {
            return PlayerData.instance.equippedCharm_29;
        }
    }
}