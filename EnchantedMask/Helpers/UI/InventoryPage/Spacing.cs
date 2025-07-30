using EnchantedMask.Settings;
using System;
using UnityEngine;

namespace EnchantedMask.Helpers.UI.InventoryPage
{
    public class Spacing
    {
        /// <summary>
        /// X-coordinate of the leftmost mask
        /// </summary>
        private float x = 0;

        /// <summary>
        /// Y-coordinate of the topmost mask
        /// </summary>
        private readonly float y = 0;

        /// <summary>
        /// Total space between masks in a row
        /// </summary>
        public float ColumnSpace = 0;

        /// <summary>
        /// Total space between masks in a row
        /// </summary>
        public float RowSpace = 0;

        /// <summary>
        /// Modifies size of the mask sprites
        /// </summary>
        private readonly float scale = 0;

        /// <summary>
        /// Stores the maximum number of masks per row
        /// </summary>
        public int MasksPerRow = 0;

        /// <summary>
        /// Creates spacing for the slots in the Glyph Inventory page
        /// </summary>
        /// <param name="masksPerRow"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Spacing(int masksPerRow)
        {
            x = -10.98f;
            y = 4;

            MasksPerRow = masksPerRow;
            ColumnSpace = 14f / MasksPerRow;
            RowSpace = 14f / MasksPerRow;
            scale = 8f / MasksPerRow;
        }

        /// <summary>
        /// Gets the X coordinate of the mask slot
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float X(int columnIndex)
        {
            return x + ColumnSpace * columnIndex;
        }

        /// <summary>
        /// Gets the y coordinate of the mask slot
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Y(int rowIndex)
        {
            return y - RowSpace * rowIndex;
        }

        /// <summary>
        /// Scales the sprite to fit in the mask slot
        /// </summary>
        /// <returns></returns>
        public Vector3 Scale()
        {
            return new Vector3(scale, scale, scale);
        }
    }
}
