using EnchantedMask.Settings;
using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;
using SaveSettings = EnchantedMask.Settings.SaveSettings;
using EnchantedMask.Helpers.UI;
using EnchantedMask.Helpers.Shop;
using EnchantedMask.Helpers;

namespace EnchantedMask
{
    public class EnchantedMask : Mod, ILocalSettings<SaveSettings>
    {
        public override string GetVersion() => "1.0.0.0";

        #region Settings
        public void OnLoadLocal(SaveSettings s)
        {
            SharedData.saveSettings = s;
        }

        public SaveSettings OnSaveLocal()
        {
            return SharedData.saveSettings;
        }
        #endregion

        public EnchantedMask() : base(SharedData.modName)
        {
            PageHelper.AddPage();
        }

        public override List<ValueTuple<string, string>> GetPreloadNames()
        {
            return new List<ValueTuple<string, string>>
            {
                new ValueTuple<string, string>("Fungus1_04_boss", "Hornet Boss 1/Sphere Ball"),
                new ValueTuple<string, string>("GG_Uumuu", "Mega Jellyfish GG"),
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            SharedData.Log("Initializing");

            SharedData.preloads = preloadedObjects;

            SharedData.Log("Running helpers");
            ShopHelper.Run();
            PageHelper.Run();
            IconHelper.Run();

            IMod debugMod = ModHooks.GetMod("DebugMod");
            if (debugMod != null)
            {
                DebugHelper debugHelper = new DebugHelper();
                debugHelper.AddDebugOptions();
            }

            SharedData.Log("Initialized");
        }
    }
}