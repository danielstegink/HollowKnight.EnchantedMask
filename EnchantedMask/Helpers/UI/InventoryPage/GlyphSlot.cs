using UnityEngine;

namespace EnchantedMask.Helpers.UI.InventoryPage
{
    /// <summary>
    /// Handles the "slot" holding a given glyph in the Glyphs Inventory page
    /// </summary>
    public class GlyphSlot
    {
        public readonly GameObject slotObject;
        public readonly GameObject icon;
        public readonly SpriteRenderer spriteRenderer;
        public readonly GameObject halo;

        public GlyphSlot(GameObject slotObject, GameObject icon, SpriteRenderer spriteRenderer, GameObject halo)
        {
            this.slotObject = slotObject;
            this.icon = icon;
            this.spriteRenderer = spriteRenderer;
            this.halo = halo;
        }
    }
}
