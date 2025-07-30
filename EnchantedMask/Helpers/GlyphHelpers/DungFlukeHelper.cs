using EnchantedMask.Helpers.GlyphHelpers.Components;
using EnchantedMask.Settings;
using Modding.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Helpers
{
    /// <summary>
    /// Dung Flukes don't store damage like regular flukes. Instead, they explode creating a 
    ///     custom dung cloud similar to the ones created by Defender's Crest.
    /// My attempts to modify the FSMs creating these clouds have ended in failure, so instead 
    ///     I made this helper that uses coroutines to find fluke clouds as they are made,
    ///     then apply the modifier provided in the constructor.
    /// </summary>
    public class DungFlukeHelper
    {
        public bool isActive { get; set; } = false;

        public float modifier { get; set; } = 1f;

        public string featureName { get; set; } = "";

        public bool performLogging { get; set; } = false;

        public DungFlukeHelper(float modifier, string featureName, bool log = false)
        {
            this.modifier = modifier;
            this.featureName = featureName;
            performLogging = log;
        }

        /// <summary>
        /// Starts the coroutine that applies the modifier
        /// </summary>
        public void Start()
        {
            isActive = true;
            GameManager.instance.StartCoroutine(DungFlukeCheck());
        }

        /// <summary>
        /// Stops the coroutine
        /// </summary>
        public void Stop()
        {
            isActive = false;
            ResetDamage();
        }

        /// <summary>
        /// Coroutine used to find dung clouds and modify them
        /// </summary>
        /// <returns></returns>
        private IEnumerator DungFlukeCheck()
        {
            while (isActive)
            {
                // Get a list of all the active dung clouds
                List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                    .Where(x => x.name.StartsWith("Knight Dung Cloud"))
                                                                    .ToList();
                foreach (GameObject cloud in dungClouds)
                {
                    // Get or add a ModsApplied component
                    DungFlukeModded modsApplied = cloud.GetOrAddComponent<DungFlukeModded>();
                    if (performLogging)
                    {
                        SharedData.Log($"DungFlukeHelper - Mods applied: {string.Join("|", modsApplied.ModList.Keys)}, " +
                                        $"Base damage interval: {modsApplied.BaseValue}");
                    }

                    // Get the current value
                    DamageEffectTicker damageEffectTicker = cloud.GetComponent<DamageEffectTicker>();
                    float currentValue = damageEffectTicker.damageInterval;

                    // If we are modding for the first time, the base value will be default, 
                    // so we need to store the base value for future reference
                    if (modsApplied.BaseValue == default)
                    {
                        modsApplied.BaseValue = currentValue;
                    }

                    // If current value doesn't match the modded value, it most likely has reset
                    // In that scenario, we have nothing to worry about
                    float moddedValue = modsApplied.BaseValue * modsApplied.GetModifier();
                    if (moddedValue != currentValue)
                    {
                        // However, if the base value has changed, we need to do a full reset of the component
                        if (currentValue != modsApplied.BaseValue)
                        {
                            modsApplied.BaseValue = currentValue;
                            modsApplied.ModList = new Dictionary<string, (string, float)>();
                        }
                    }

                    // Now, with the base value established, we can check if this mod has been applied
                    // First check if a different feature from the same mod was used. If so, reset its bonus
                    if (modsApplied.ModList.ContainsKey(SharedData.modName))
                    {
                        (string, float) value = modsApplied.ModList[SharedData.modName];
                        if (!value.Item1.Equals(featureName))
                        {
                            modsApplied.ModList.Remove(SharedData.modName);
                            if (performLogging)
                            {
                                SharedData.Log($"DungFlukeHelper - Damage interval reduced by {value.Item2} to remove {value.Item1}");
                            }
                        }
                    }

                    // Then apply our feature if it isn't there already
                    if (!modsApplied.ModList.ContainsKey(SharedData.modName))
                    {
                        modsApplied.ModList.Add(SharedData.modName, (featureName, modifier));
                        if (performLogging)
                        {
                            SharedData.Log($"DungFlukeHelper - Damage interval increased by {modifier} for {featureName}");
                        }
                    }

                    damageEffectTicker.SetDamageInterval(modsApplied.BaseValue * modsApplied.GetModifier());
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        /// <summary>
        /// When a glyph is deactivated, we should remove its buff as well in case its ongoing
        /// </summary>
        private void ResetDamage()
        {
            // Get a list of all the active dung clouds
            List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                .Where(x => x.name.StartsWith("Knight Dung Trail(Clone)"))
                                                                .ToList();
            foreach (GameObject cloud in dungClouds)
            {
                // Get or add a ModsApplied component
                DungDamageModded modsApplied = cloud.GetOrAddComponent<DungDamageModded>();

                // Get the current damage rate
                DamageEffectTicker damageEffectTicker = cloud.GetComponent<DamageEffectTicker>();
                float interval = damageEffectTicker.damageInterval;

                // If we are modding for the first time, the base value will be default, 
                // so we need to store the base value for future reference
                if (modsApplied.BaseValue == default)
                {
                    modsApplied.BaseValue = interval;
                }

                // If current value doesn't match the modded value, it most likely has reset
                // In that scenario, we have nothing to worry about
                float moddedValue = modsApplied.BaseValue * modsApplied.GetModifier();
                if (moddedValue != interval)
                {
                    // However, if the base value has changed, we need to do a full reset of the component
                    if (interval != modsApplied.BaseValue)
                    {
                        modsApplied.BaseValue = interval;
                        modsApplied.ModList = new Dictionary<string, (string, float)>();
                    }
                }

                // Now, with the base value established, we can check if this mod has been applied
                if (modsApplied.ModList.ContainsKey(SharedData.modName))
                {
                    modsApplied.ModList.Remove(SharedData.modName);
                }

                damageEffectTicker.SetDamageInterval(modsApplied.BaseValue * modsApplied.GetModifier());
            }
        }
    }
}
