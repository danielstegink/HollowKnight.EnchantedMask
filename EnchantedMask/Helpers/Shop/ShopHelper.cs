using EnchantedMask.Settings;
using ItemChanger.Locations;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using ItemChanger;
using EnchantedMask.Glyphs;
using System.Linq;
using Modding;
using ItemChanger.Placements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace EnchantedMask.Helpers.Shop
{
    /// <summary>
    /// Handles all code and handlers related to the Mask Shop where the glyphs are purchased
    /// </summary>
    public static class ShopHelper
    {
        public static void Run()
        {
            BuildItemChanger();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += StockStore;
            ModHooks.LanguageGetHook += GetText;
            ModHooks.GetPlayerBoolHook += GetBools;
            ModHooks.SetPlayerBoolHook += SetBools;
        }

        /// <summary>
        /// Adds the shop and glyphs to ItemChanger
        /// </summary>
        private static void BuildItemChanger()
        {
            SharedData.Log("Adding glyphs and shop to ItemChanger");

            // Add Mask Maker's shop
            CustomShopLocation maskShop = new CustomShopLocation()
            {
                name = glyphShopName,
                sceneName = SceneNames.Room_Mask_Maker,
                objectName = "Maskmaker NPC",
                fsmName = "npc_control",
                flingType = FlingType.DirectDeposit,
            };
            Finder.DefineCustomLocation(maskShop);

            // Add glyphs
            foreach (Glyph glyph in SharedData.glyphs)
            {
                string itemName = GetGlyphItemName(glyph.ID);
                var item = new ItemChanger.Items.BoolItem()
                {
                    name = itemName,
                    fieldName = GetBoughtBoolName(glyph.ID),
                    UIDef = new MsgUIDef()
                    {
                        name = new LanguageString("UI", GetTextKey(glyph.ID, true)),
                        shopDesc = new LanguageString("UI", GetTextKey(glyph.ID, false)),
                        sprite = new ItemChangerSprite(itemName, glyph.GetIcon())
                    }
                };

                var mapModTag = item.AddTag<InteropTag>();
                mapModTag.Message = "RandoSupplementalMetadata";
                mapModTag.Properties["ModSource"] = "EnchantedMask";
                mapModTag.Properties["PoolGroup"] = "Glyphs";

                Finder.DefineCustomItem(item);
            }
        }

        /// <summary>
        /// Populates the glyph shop's stock
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private static void StockStore(Scene arg0, LoadSceneMode arg1)
        {
            SharedData.Log("Stocking store");

            // Need to initialize the settings so the placements save properly
            ItemChangerMod.CreateSettingsProfile(false, false);

            // Get the shop
            var maskShop = Finder.GetLocation(glyphShopName);
            ShopPlacement shopPlacement = maskShop.Wrap() as ShopPlacement;

            // Find all the glyphs that have been unlocked but not bought, and add them to the shop
            List<Glyph> glyphsToStock = SharedData.glyphs.Where(x => x.Unlocked() &&
                                                                !x.Bought())
                                                         .ToList();
            foreach (Glyph glyph in glyphsToStock)
            {
                AbstractItem glyphItem = Finder.GetItem(GetGlyphItemName(glyph.ID));
                int geoCost = glyph.GetCost();
                shopPlacement.AddItemWithCost(glyphItem, geoCost);
            }

            ItemChangerMod.AddPlacements(new List<AbstractPlacement> { shopPlacement }, PlacementConflictResolution.Replace);
        }

        /// <summary>
        /// Gets text related to the glyphs for ItemChanger
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sheetTitle"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static string GetText(string key, string sheetTitle, string orig)
        {
            if (key.StartsWith(textKeyPrefix))
            {
                // Key should have the form EnchantedMask_ID(_Desc)
                string[] parts = key.Split('_');
                string id = parts[1];
                Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                if (glyph != default)
                {
                    // Name, Description
                    if (sheetTitle.Equals("UI"))
                    {
                        if (key.EndsWith("Desc"))
                        {
                            return glyph.Description;
                        }
                        else
                        {
                            return glyph.Name;
                        }
                    }
                }
            }

            return orig;
        }

        /// <summary>
        /// Gets if a glyph has been bought
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        private static bool GetBools(string name, bool orig)
        {
            if (name.StartsWith(boughtBoolPrefix)) // Format: boughtGlyph_ID
            {
                string id = name.Split('_')[1];
                Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                if (glyph != default)
                {
                    return glyph.Bought();
                }
            }

            return orig;
        }

        /// <summary>
        /// Sets whether or not a glyph as been bought
        /// </summary>
        /// <param name="key"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static bool SetBools(string key, bool orig)
        {
            if (key.StartsWith(boughtBoolPrefix))
            {
                string id = key.Split('_')[1];
                Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                if (glyph != default)
                {
                    if (orig)
                    {
                        SharedData.saveSettings.GlyphsBought.Add(glyph.ID);
                    }
                    else
                    {
                        SharedData.saveSettings.GlyphsBought.RemoveAll(x => x.Equals(glyph.ID));
                    }
                }
            }

            return orig;
        }

        #region Variables
        /// <summary>
        /// Stores the name of the glyph shop from ItemChanger
        /// </summary>
        private static string glyphShopName = "Glyph_Shop";

        /// <summary>
        /// Prefix for the Bought boolean variable for a given glyph
        /// </summary>
        private static string boughtBoolPrefix = "boughtGlyph";

        /// <summary>
        /// Name of the boolean variable tracking if the given glyph has been bought
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private static string GetBoughtBoolName(string ID)
        {
            return $"{boughtBoolPrefix}_{ID}";
        }

        /// <summary>
        /// Gets the name of the given glyph in ItemChanger
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private static string GetGlyphItemName(string ID)
        {
            return $"Glyph_{ID}";
        }

        /// <summary>
        /// Stores the prefix used for text variables related to glyphs
        /// </summary>
        private static string textKeyPrefix = "EnchantedMask";

        /// <summary>
        /// Gets the name of the text variable key for the given glyph
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="isName"></param>
        /// <returns></returns>
        private static string GetTextKey(string ID, bool isName)
        {
            string key = $"{textKeyPrefix}_{ID}";

            // If this isn't a name key, its a description key
            if (!isName)
            {
                key = $"{key}_Desc";
            }

            return key;
        }
        #endregion
    }
}
