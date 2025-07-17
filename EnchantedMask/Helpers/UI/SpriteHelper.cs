using System.IO;
using System.Reflection;
using UnityEngine;

namespace EnchantedMask.Helpers.UI
{
    public static class SpriteHelper
    {
        /// <summary>
        /// Gets the sprite for the given mask
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Sprite GetMaskSprite(string name)
        {
            string fileName = $"Glyphs.{name}";
            return GetSprite(fileName);
        }

        /// <summary>
        /// Gets the given sprite from the embedded resources
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Sprite GetSprite(string name)
        {
            // Get the mod's assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            //string[] manifestedResourceNames = assembly.GetManifestResourceNames();
            //SharedData.Log(string.Join(", ", manifestedResourceNames));

            // Get the sprite from the assembly's embedded resources as a stream
            string resourceName = $"EnchantedMask.Resources.{name}.png";
            //SharedData.Log($"Getting sprite resource: {resourceName}");
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                // Convert stream to bytes
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                // Create texture from bytes
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes, true);

                return Sprite.Create(texture,
                                        new Rect(0, 0, texture.width, texture.height),
                                        new Vector2(0.5f, 0.5f));
            }
        }
    }
}
