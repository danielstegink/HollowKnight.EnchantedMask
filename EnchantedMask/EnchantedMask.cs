using EnchantedMask.Helpers;
using EnchantedMask.Helpers.GlyphHelpers;
using EnchantedMask.Helpers.Shop;
using EnchantedMask.Helpers.UI;
using EnchantedMask.Settings;
using Modding;
using SFCore.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SaveSettings = EnchantedMask.Settings.SaveSettings;

namespace EnchantedMask
{
    public class EnchantedMask : Mod, ILocalSettings<SaveSettings>
    {
        public override string GetVersion() => "1.2.0.0";

        #region Settings
        public void OnLoadLocal(SaveSettings s)
        {
            SharedData.saveSettings = s;
            //SharedData.Log("Save settings loaded");
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

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("Fungus1_04_boss", "Hornet Boss 1/Sphere Ball"),
                ("GG_Uumuu", "Mega Jellyfish GG"),
                ("RestingGrounds_08", "Ghost revek"),
                ("Crossroads_04", "_Enemies/Zombie Hornhead")
                //("Abyss_06_Core", "Shade Sibling Spawner")
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

            On.HeroController.Start += OnPlayerDataLoaded;
            On.GameManager.ReturnToMainMenu += OnQuit;
            
            // Need to modify the Spell Control FSM on creation so Void can work
            //On.HeroController.Start += AddVoidSpell;

            SharedData.Log("Initialized");
        }

        private void OnPlayerDataLoaded(On.HeroController.orig_Start orig, HeroController self)
        {
            //SharedData.Log("Hero Controller started");
            if (!string.IsNullOrWhiteSpace(SharedData.saveSettings.EquippedGlyph))
            {
                Glyphs.Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(SharedData.saveSettings.EquippedGlyph))
                                                        .First();
                glyph.Equip();
            }

            orig(self);
        }

        private IEnumerator OnQuit(On.GameManager.orig_ReturnToMainMenu orig, GameManager self, GameManager.ReturnToMainMenuSaveModes saveMode, Action<bool> callback)
        {
            // Some glyphs, like Blessed, modify player data, so we have to reset them all
            // when we close a save file
            IconHelper.ResetGlyphs();

            //SharedData.Log("Closing save");
            return orig(self, saveMode, callback);
        }

        /// <summary>
        /// Adds the spell from the Void glyph to the FSM so it can be easily added or removed
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void AddVoidSpell(On.HeroController.orig_Start orig, HeroController self)
        {
            //SharedData.Log($"Adding Abyssal Wraiths to Spell FSM");
            PlayMakerFSM spellControl = HeroController.instance.spellControl;
            spellControl.CopyState("Scream Antic2", "EnchantedMask Scream2");
            spellControl.CopyState("Scream Burst 2", "EnchantedMask Scream2 Burst");
            spellControl.ChangeTransition("EnchantedMask Scream2", "FINISHED", "EnchantedMask Scream2 Burst");

            spellControl.RemoveAction("EnchantedMask Scream2 Burst", 8);
            spellControl.RemoveAction("EnchantedMask Scream2 Burst", 7);
            spellControl.RemoveAction("EnchantedMask Scream2 Burst", 3);
            spellControl.RemoveAction("EnchantedMask Scream2 Burst", 1);
            spellControl.RemoveAction("EnchantedMask Scream2 Burst", 0);
            spellControl.InsertMethod("EnchantedMask Scream2 Burst", () => VoidSpell.AbyssalWraiths(), 0);

            orig(self);
        }
    }
}