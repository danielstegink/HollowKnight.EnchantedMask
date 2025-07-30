using EnchantedMask.Settings;
using GlobalEnums;
using System;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Helpers.BlockHelpers
{
    public class WebBlock : BlockHelper
    {
        /// <summary>
        /// Stores custom web shield
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// Gets Hornet web from preloads and customizes it
        /// </summary>
        private void InitializeWebShield()
        {
            // Get base web from preloads
            GameObject preload = SharedData.preloads["Fungus1_04_boss"]["Hornet Boss 1/Sphere Ball"];

            // Copy preload into a prefab, then disable it so it doesn't attack us
            prefab = UnityEngine.GameObject.Instantiate(preload);
            prefab.SetActive(false);
            UnityEngine.GameObject.DontDestroyOnLoad(prefab);

            // Customize prefab
            prefab.name = "EnchantedMask.WebBlock";
            prefab.layer = (int)PhysLayers.HERO_ATTACK;
            UnityEngine.GameObject.Destroy(prefab.GetComponent<DamageHero>());
        }

        public override void ApplyHook()
        {
            InitializeWebShield();

            base.ApplyHook();
        }

        /// <summary>
        /// As a Rare glyph, Hornet is worth 3 notches and has a 22% chance of blocking
        /// </summary>
        /// <returns></returns>
        public override bool CustomBlockCheck()
        {
            int random = UnityEngine.Random.Range(1, 101);
            //SharedData.Log($"Hornet - Checking {random} against 22");
            return random <= 22;
        }

        /// <summary>
        /// Hornet creates a web object when blocking
        /// </summary>
        /// <returns></returns>
        public override IEnumerator CustomEffects()
        {
            Vector3 spawnPoint = HeroController.instance.transform.position;
            GameObject web = UnityEngine.GameObject.Instantiate(prefab, spawnPoint, Quaternion.identity);
            web.SetActive(true);

            yield return base.CustomEffects();
            UnityEngine.GameObject.Destroy(web);
        }
    }
}
