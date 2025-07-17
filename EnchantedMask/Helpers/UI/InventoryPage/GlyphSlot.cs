using UnityEngine;

namespace EnchantedMask.Helpers.UI.InventoryPage
{
    /// <summary>
    /// Handles the "slot" holding a given glyph in the Glyphs Inventory page
    /// </summary>
    public class GlyphSlot
    {
        public readonly GameObject slotOjbect;
        public readonly GameObject icon;
        public readonly SpriteRenderer spriteRenderer;

        public GlyphSlot(GameObject slotOjbect, GameObject icon, SpriteRenderer spriteRenderer)
        {
            this.slotOjbect = slotOjbect;
            this.icon = icon;
            this.spriteRenderer = spriteRenderer;
        }
    }
}
