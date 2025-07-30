using System.Collections.Generic;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers.Components
{
    /// <summary>
    /// Used for tracking which mods have altered a given game object
    /// </summary>
    public class ModsApplied<T> : MonoBehaviour
    {
        /// <summary>
        /// List of mods/features that have altered the property
        /// </summary>
        public Dictionary<string, (string, float)> ModList = new Dictionary<string, (string, float)>();

        /// <summary>
        /// Stores the original modded value
        /// </summary>
        public T BaseValue = default;

        /// <summary>
        /// Gets the cumulative modifier of all the mods applied
        /// </summary>
        /// <returns></returns>
        public float GetModifier()
        {
            float modifier = 1f;
            foreach (KeyValuePair<string, (string, float)> mod in ModList)
            {
                modifier *= mod.Value.Item2;
            }

            return modifier;
        }
    }
}
