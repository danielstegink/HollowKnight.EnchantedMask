using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers.Components
{
    /// <summary>
    /// Tracks which buzzsaws the game object is immune to and for how long
    /// </summary>
    public class SawIFrames : MonoBehaviour
    {
        /// <summary>
        /// Tracks the last time a given buzzsaw hit the game object
        /// </summary>
        internal Dictionary<GameObject, Stopwatch> attackLog = new Dictionary<GameObject, Stopwatch>();
    }
}