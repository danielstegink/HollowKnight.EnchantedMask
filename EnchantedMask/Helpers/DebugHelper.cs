using DebugMod;
using EnchantedMask.Settings;

namespace EnchantedMask.Helpers
{
    public class DebugHelper
    {
        /// <summary>
        /// Adds GiveGlyphs and TakeGlyphs to DebugMod
        /// </summary>
        public void AddDebugOptions()
        {
            DebugMod.DebugMod.AddToKeyBindList(typeof(DebugHelper));
        }

        /// <summary>
        /// Adds all glyphs to the player's inventory
        /// </summary>
        [BindableMethod(name = "Get all glyphs", category = "Enchanted Mask")]
        public static void GiveGlyphs()
        {
            RemoveGlyphs();
            foreach (Glyphs.Glyph glyph in SharedData.glyphs)
            {
                SharedData.saveSettings.GlyphsBought.Add(glyph.ID);
            }
        }

        /// <summary>
        /// Removes all glyphs from the player's inventory
        /// </summary>
        [BindableMethod(name = "Remove all glyphs", category = "Enchanted Mask")]
        public static void TakeGlyphs()
        {
            RemoveGlyphs();
            foreach (Glyphs.Glyph glyph in SharedData.glyphs)
            {
                glyph.Unequip();
            }
        }

        /// <summary>
        /// Resets the player's glyph inventory
        /// </summary>
        private static void RemoveGlyphs()
        {
            SharedData.saveSettings.GlyphsBought = new System.Collections.Generic.List<string>();
        }
    }
}