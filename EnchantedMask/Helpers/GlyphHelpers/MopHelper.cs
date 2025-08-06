using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;

namespace EnchantedMask.Helpers.GlyphHelpers
{
    public class MopHelper : NailRangeHelper
    {
        /// <summary>
        /// This should only work if MOP is equipped
        /// </summary>
        /// <returns></returns>
        public override bool CustomCheck()
        {
            return PlayerData.instance.equippedCharm_13;
        }

        public override float GetModifier()
        {
            // As an Uncommon glyph, Mantis increases MOP's value by 2 notches
            // Per my Utils, an increase in nail range is worth 8.33% per notch
            // So MOP's range should be increased to a total of 42.33%
            // 1.25 * X = 1.4233
            return (1.25f + 2 * NotchCosts.NailRangePerNotch()) / 1.25f;
        }
    }
}
