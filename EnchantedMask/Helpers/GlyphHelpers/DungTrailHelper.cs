using EnchantedMask.Helpers.GlyphHelpers.Components;
using EnchantedMask.Settings;
using ItemChanger.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers
{
    /// <summary>
    /// Dung Clouds from Defender's Crest get spawned and recycled multiple times, making it hard to track which 
    ///     ones have been buffed.
    /// Additionally, Pale Court straight up destroys the original object pool and creates its own clouds, so it's 
    ///     just easier to search for the clouds periodically, buff them, and then add a component so we know 
    ///     a given cloud has been modified.
    /// </summary>
    public class DungTrailHelper
    {
        public float damageModifier { get; set; } = 1f;

        public float sizeModifier { get; set; } = 1f;

        private bool isActive = false;

        public string featureName { get; set; } = "";

        public bool performLogging { get; set; } = false;

        public DungTrailHelper(string featureName, float sizeModifier = 1f, float damageModifier = 1f, bool log = false)
        {
            this.featureName = featureName;
            this.damageModifier = damageModifier;
            this.sizeModifier = sizeModifier;
            performLogging = log;
        }

        public void Start()
        {
            isActive = true;
            GameManager.instance.StartCoroutine(BuffSize());
            GameManager.instance.StartCoroutine(BuffDamage());
        }

        public void Stop()
        {
            isActive = false;
            ResetSize();
            ResetDamage();
        }

        #region Size
        /// <summary>
        /// Handles modifications to the size of the clouds
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private IEnumerator BuffSize()
        {
            while (isActive)
            {
                // Get a list of all the active dung clouds
                List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                    .Where(x => x.name.StartsWith("Knight Dung Trail(Clone)"))
                                                                    .ToList();
                foreach (GameObject cloud in dungClouds)
                {
                    // Get or add a ModsApplied component
                    DungSizeModded modsApplied = cloud.GetOrAddComponent<DungSizeModded>();
                    if (modsApplied == null)
                    {
                        throw new System.Exception($"Error: Unable to add ModsApplied component to {cloud.name}");
                    }

                    if (performLogging)
                    {
                        SharedData.Log($"DungTrailHelper.BuffCloudSize - Mods applied: {string.Join("|", modsApplied.ModList.Keys)}, " +
                                        $"Base value: {modsApplied.BaseValue}");
                    }

                    // Get the current cloud size
                    Vector3 currentScale = cloud.transform.localScale;

                    // If we are modding for the first time, the base value will be default, 
                    // so we need to store the base value for future reference
                    if (modsApplied.BaseValue == default)
                    {
                        modsApplied.BaseValue = currentScale;
                    }

                    // If current value doesn't match the modded value, it most likely has reset
                    // In that scenario, we have nothing to worry about
                    Vector3 moddedValue = modsApplied.BaseValue * modsApplied.GetModifier();
                    if (moddedValue != currentScale)
                    {
                        // However, if the base value has changed, we need to do a full reset of the component
                        if (currentScale != modsApplied.BaseValue)
                        {
                            modsApplied.BaseValue = currentScale;
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
                                SharedData.Log($"DungTrailHelper.BuffCloudSize - Reduced by {value.Item2} to remove {value.Item1}");
                            }
                        }
                    }

                    // Then apply our feature if it isn't there already
                    if (!modsApplied.ModList.ContainsKey(SharedData.modName))
                    {
                        modsApplied.ModList.Add(SharedData.modName, (featureName, sizeModifier));
                        if (performLogging)
                        {
                            SharedData.Log($"DungTrailHelper.BuffCloudSize - Increased by {sizeModifier} for {featureName}");
                        }
                    }

                    cloud.transform.localScale = modsApplied.BaseValue * modsApplied.GetModifier();
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        /// <summary>
        /// When a glyph is deactivated, we should remove its size buff as well in case its ongoing
        /// </summary>
        private void ResetSize()
        {
            // Get a list of all the active dung clouds
            List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                .Where(x => x.name.StartsWith("Knight Dung Trail(Clone)"))
                                                                .ToList();
            foreach (GameObject cloud in dungClouds)
            {
                // Get or add a ModsApplied component
                DungSizeModded modsApplied = cloud.GetOrAddComponent<DungSizeModded>();

                // Get the current cloud size
                Vector3 currentScale = cloud.transform.localScale;

                // If we are modding for the first time, the base value will be default, 
                // so we need to store the base value for future reference
                if (modsApplied.BaseValue == default)
                {
                    modsApplied.BaseValue = currentScale;
                }

                // If current value doesn't match the modded value, it most likely has reset
                // In that scenario, we have nothing to worry about
                Vector3 moddedValue = modsApplied.BaseValue * modsApplied.GetModifier();
                if (moddedValue != currentScale)
                {
                    // However, if the base value has changed, we need to do a full reset of the component
                    if (currentScale != modsApplied.BaseValue)
                    {
                        modsApplied.BaseValue = currentScale;
                        modsApplied.ModList = new Dictionary<string, (string, float)>();
                    }
                }

                // Now, with the base value established, we can check if this mod has been applied
                if (modsApplied.ModList.ContainsKey(SharedData.modName))
                {
                    modsApplied.ModList.Remove(SharedData.modName);
                }

                cloud.transform.localScale = modsApplied.BaseValue * modsApplied.GetModifier();
            }
        }
        #endregion

        #region Damage
        /// <summary>
        /// Handles modifications to the damage rate of the clouds
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private IEnumerator BuffDamage()
        {
            while (isActive)
            {
                // Get a list of all the active dung clouds
                List<GameObject> dungClouds = UnityEngine.GameObject.FindObjectsOfType<GameObject>()
                                                                    .Where(x => x.name.StartsWith("Knight Dung Trail(Clone)"))
                                                                    .ToList();
                foreach (GameObject cloud in dungClouds)
                {
                    // Get or add a ModsApplied component
                    DungDamageModded modsApplied = cloud.GetOrAddComponent<DungDamageModded>();
                    if (modsApplied == null)
                    {
                        throw new System.Exception($"Error: Unable to add ModsApplied component to {cloud.name}");
                    }

                    if (performLogging)
                    {
                        SharedData.Log($"DungTrailHelper.BuffDamage - Mods applied: {string.Join("|", modsApplied.ModList.Keys)}, " +
                                        $"Base value: {modsApplied.BaseValue}");
                    }

                    // Get the current damage rate
                    DamageEffectTicker damageEffectTicker = cloud.GetComponent<DamageEffectTicker>();
                    float interval = damageEffectTicker.damageInterval;

                    // If we are modding for the first time, the base value will be default, 
                    // so we need to store the base value for future reference
                    if (modsApplied.BaseValue == default)
                    {
                        modsApplied.BaseValue = interval;
                    }

                    // If current value doesn't match the modded value, the cloud has most likely reset
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
                    // First check if a different feature from the same mod was used. If so, reset its bonus
                    if (modsApplied.ModList.ContainsKey(SharedData.modName))
                    {
                        (string, float) value = modsApplied.ModList[SharedData.modName];
                        if (!value.Item1.Equals(featureName))
                        {
                            modsApplied.ModList.Remove(SharedData.modName);
                            if (performLogging)
                            {
                                SharedData.Log($"DungTrailHelper.BuffDamage - Reduced by {value.Item2} to remove {value.Item1}");
                            }
                        }
                    }

                    // Then apply our feature if it isn't there already
                    if (!modsApplied.ModList.ContainsKey(SharedData.modName))
                    {
                        modsApplied.ModList.Add(SharedData.modName, (featureName, damageModifier));
                        if (performLogging)
                        {
                            SharedData.Log($"DungTrailHelper.BuffDamage - Increased by {damageModifier} for {featureName}");
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
        #endregion
    }
}