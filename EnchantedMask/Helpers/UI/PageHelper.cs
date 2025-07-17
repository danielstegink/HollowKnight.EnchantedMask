using EnchantedMask.Helpers.UI.InventoryPage;
using EnchantedMask.Settings;
using Modding;
using SFCore;
using System;

namespace EnchantedMask.Helpers.UI
{
    /// <summary>
    /// Handles code related to the Glyphs page in the Inventory
    /// </summary>
    public static class PageHelper
    {
        private static Tuple<string, string> customText = new Tuple<string, string>("ENCHANTED_MASKS_MENU", "Glyphs");

        /// <summary>
        /// Custom bool we set in the get bool hook so we control it when the game looks for it
        /// </summary>
        private static string menuBooleanName = "glyphsEnabled";

        /// <summary>
        /// Adds the Glyphs page to Inventory
        /// </summary>
        public static void AddPage()
        {
            SharedData.Log("Adding Glyphs page to inventory");
            InventoryHelper.AddInventoryPage(InventoryPageType.Empty, "Glyphs", customText.Item1,
                                                "ENCHANTED_MASK_EVENT", menuBooleanName,
                                                GlyphsPage.instance.Build);
        }

        public static void Run()
        {
            ModHooks.GetPlayerBoolHook += GetBools;
            ModHooks.LanguageGetHook += GetText;
            On.HeroController.Update += UpdateInventoryPage;
        }

        /// <summary>
        /// Controls if the Glyphs page is enabled
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static bool GetBools(string name, bool orig)
        {
            if (name.Equals(menuBooleanName))
            {
                return true;
            }

            return orig;
        }

        /// <summary>
        /// Wrapping this to hard-code Glyphs page name
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetTitle"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static string GetText(string key, string sheetTitle, string orig)
        {
            if (customText.Item1.Equals(key))
            {
                return customText.Item2;
            }

            return orig;
        }

        /// <summary>
        /// Ensures that the Inventory page updates whenever we buy a new glyph
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void UpdateInventoryPage(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);
            GlyphsPage.instance.UpdatePage();
        }
    }
}
