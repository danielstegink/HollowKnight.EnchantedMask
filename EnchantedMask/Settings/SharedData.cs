using EnchantedMask.Glyphs;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EnchantedMask.Settings
{
    public static class SharedData
    {
        public static string modName = "EnchantedMask";

        public static SaveSettings saveSettings { get; set; } = new SaveSettings();

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
            //new Void(),
            //new God(),
        };

        /// <summary>
        /// Logs message to the shared mod log at AppData\LocalLow\Team Cherry\Hollow Knight\ModLog.txt
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Modding.Logger.Log($"[EnchantedMask] {message}");
        }

        public static Dictionary<string, Dictionary<string, GameObject>> preloads;

        #region Accessing Private Fields/Methods
        /// <summary>
        /// Gets a private field from the given input class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <param name="isStaticOrConst"></param>
        /// <returns></returns>
        public static O GetField<I, O>(I input, string fieldName, bool isStaticOrConst = false)
        {
            BindingFlags typeFlag = BindingFlags.Instance;
            if (isStaticOrConst)
            {
                typeFlag = BindingFlags.Static;
            }

            FieldInfo fieldInfo = input.GetType()
                                       .GetField(fieldName, BindingFlags.NonPublic | typeFlag);
            return (O)fieldInfo.GetValue(input);
        }

        /// <summary>
        /// Sets a private field from the given input class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="isStaticOrConst"></param>
        public static void SetField<I, O>(I input, string fieldName, O value, bool isStaticOrConst = false)
        {
            BindingFlags typeFlag = BindingFlags.Instance;
            if (isStaticOrConst)
            {
                typeFlag = BindingFlags.Static;
            }

            FieldInfo fieldInfo = input.GetType()
                                       .GetField(fieldName, BindingFlags.NonPublic | typeFlag);
            fieldInfo.SetValue(input, value);
        }

        /// <summary>
        /// Calls a private method from the given input class
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <param name="parameters"></param>
        /// <param name="isStaticOrConst"></param>
        /// <returns></returns>
        public static O CallFunction<I, O>(I input, string fieldName, object[] parameters, bool isStaticOrConst = false)
        {
            BindingFlags typeFlag = BindingFlags.Instance;
            if (isStaticOrConst)
            {
                typeFlag = BindingFlags.Static;
            }

            MethodInfo methodInfo = input.GetType()
                                            .GetMethod(fieldName, BindingFlags.NonPublic | typeFlag);
            return (O)methodInfo.Invoke(input, parameters);
        }
        #endregion

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