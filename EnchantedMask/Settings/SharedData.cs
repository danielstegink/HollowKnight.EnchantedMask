using EnchantedMask.Glyphs;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace EnchantedMask.Settings
{
    public static class SharedData
    {
        /// <summary>
        /// Name of the mod (passed to helpers for logging purposes)
        /// </summary>
        public static string modName = "EnchantedMask";

        /// <summary>
        /// Save settings for the mod
        /// </summary>
        public static SaveSettings saveSettings { get; set; } = new SaveSettings();

        /// <summary>
        /// Stores the PC mod for ease of reference
        /// </summary>
        public static IMod paleCourtMod;

        /// <summary>
        /// List of all the masks in the game
        /// </summary>
        public static List<Glyph> glyphs = new List<Glyph>()
        {
            new Explorer(),
            new Grub(),
            new False(),
            new Snail(),
            new Hunter(),
            new Dash(),
            new Blessed(),
            new Mantis(),
            new Soul(),
            new Brown(),
            new Crystal(),
            new Dream(),
            new Gold(),
            new Broken(),
            new Nailsage(),
            new Hornet(),
            new Blue(),
            new Fluke(),
            new Watcher(),
            new Teacher(),
            new Beast(),
            new Hollow(),
            new Green(),
            new Fool(),
            new Traitor(),
            new Royal(),
            new Old(),
            new Sad(),
            new Sibling(),
            new Radiant(),
            new Shroom(),
            new White(),
            new Grey(),
            new Nightmare(),
            new Bound(),
            new Hive(),
            new Void(),
            new God(),
            new Abyss(),
            new Champion(),
        };

        /// <summary>
        /// Logs message to the shared mod log at AppData\LocalLow\Team Cherry\Hollow Knight\ModLog.txt
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Modding.Logger.Log($"[EnchantedMask] {message}");
        }

        /// <summary>
        /// Stored preloaded objects for ease of reference
        /// </summary>
        public static Dictionary<string, Dictionary<string, GameObject>> preloads;

        #region GameObject Names
        /// <summary>
        /// List of the object names of the regular nail attacks
        /// </summary>
        public static List<string> nailAttackNames = new List<string>()
        {
            "Slash",
            "AltSlash",
            "UpSlash",
            "DownSlash",
        };

        /// <summary>
        /// List of the object names of the Nail Art attacks
        /// </summary>
        public static List<string> nailArtNames = new List<string>()
        {
            "Cyclone Slash",
            "Great Slash",
            "Dash Slash",
            "Hit L",
            "Hit R"
        };
        #endregion
    }
}