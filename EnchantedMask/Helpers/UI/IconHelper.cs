using EnchantedMask.Glyphs;
using EnchantedMask.Settings;
using GlobalEnums;
using Modding;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Helpers.UI
{
    /// <summary>
    /// Handles code related to the icon that appears in the UI
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// Stores the Icon
        /// </summary>
        private static GameObject glyphIcon;

        /// <summary>
        /// Stores the canvas we draw the Icon on
        /// </summary>
        private static GameObject canvas;

        /// <summary>
        /// Temporary storage for the currently equipped glyph
        /// </summary>
        private static string equippedGlyph = "";

        /// <summary>
        /// Stores an empty sprite for when no glyph is equipped
        /// </summary>
        private static Sprite blankSprite = SpriteHelper.GetSprite("Blank");

        public static void Run()
        {
            On.HeroController.Update += CheckIcon;
        }

        /// <summary>
        /// We want to check the icon whenever there is an update
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private static void CheckIcon(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);

            // Create the canvas if it doesn't already exist
            if (canvas == null)
            {
                canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920f, 1080f));
                canvas.name = "Glyph Canvas";
                canvas.transform.parent = GameCameras.instance.hudCanvas.transform;
            }

            // Create the icon if it doesn't already exist
            if (glyphIcon == null)
            {
                CanvasUtil.RectData canvasRect = new CanvasUtil.RectData(new Vector2(50f, 50f), new Vector2(0.155f, 0.78f),
                                                                         new Vector2(0.155f, 0.78f), new Vector2(0.155f, 0.78f));
                glyphIcon = CanvasUtil.CreateImagePanel(canvas, blankSprite, canvasRect);
                glyphIcon.name = "Glyph Icon";

                UnityEngine.UI.Image iconPicture = glyphIcon.GetComponent<UnityEngine.UI.Image>();
                iconPicture.preserveAspect = false;
                iconPicture.type = UnityEngine.UI.Image.Type.Filled;
                iconPicture.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
                iconPicture.fillAmount = 1f;
            }

            if (GameManager.instance == null ||
                GameManager.instance.gameState != GameState.PLAYING || // Hides the icon during transitions and cutscenes
                InInventory() || // Hide the icon while our inventory is open
                GameCameras.instance.hudCanvas.transform.position.y >= 50) // This hides the icon during conversations
            {
                CanvasRenderer render = glyphIcon.GetComponent<CanvasRenderer>();
                render.SetAlpha(0f);
            }
            else // Otherwise, don't hide it
            {
                CanvasRenderer render = glyphIcon.GetComponent<CanvasRenderer>();
                render.SetAlpha(1f);
            }

            // Make sure to apply the equipped glyph's icon
            Sprite iconSprite = GetIconSprite(SharedData.saveSettings.EquippedGlyph);
            UnityEngine.UI.Image iconImage = glyphIcon.GetComponent<UnityEngine.UI.Image>();
            iconImage.sprite = iconSprite;
        }

        /// <summary>
        /// Checks if the Inventory is open
        /// </summary>
        /// <returns></returns>
        private static bool InInventory()
        {
            GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory Top");
            if (inventoryObject == null)
            {
                return false;
            }

            PlayMakerFSM inventoryFsm = FSMUtility.LocateFSM(inventoryObject, "Inventory Control");
            if (inventoryFsm == null)
            {
                return false;
            }

            HutongGames.PlayMaker.FsmBool isInventoryOpen = inventoryFsm.FsmVariables.GetFsmBool("Open");
            return isInventoryOpen != null &&
                    isInventoryOpen.Value;
        }

        /// <summary>
        /// Gets the sprite to show in the Icon
        /// </summary>
        /// <param name="glyphId"></param>
        /// <returns></returns>
        private static Sprite GetIconSprite(string glyphId)
        {
            if (string.IsNullOrEmpty(glyphId))
            {
                return blankSprite;
            }
            else
            {
                Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(glyphId)).FirstOrDefault();
                return glyph.GetIcon();
            }
        }

        /// <summary>
        /// When a new save is loaded, we need to reset the glyphs to use the new save data
        /// </summary>
        public static void ResetGlyphs()
        {
            //SharedData.Log("Resetting glyphs");

            // Store the currently equipped glyph so the reset doesn't wipe it out
            equippedGlyph = SharedData.saveSettings.EquippedGlyph;

            foreach (Glyph glyph in SharedData.glyphs)
            {
                glyph.Unequip();
            }

            SharedData.saveSettings.EquippedGlyph = equippedGlyph;
        }
    }
}
