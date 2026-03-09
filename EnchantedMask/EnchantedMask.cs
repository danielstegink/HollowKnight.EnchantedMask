using EnchantedMask.Helpers;
using EnchantedMask.Helpers.Shop;
using EnchantedMask.Helpers.UI;
using EnchantedMask.Settings;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SaveSettings = EnchantedMask.Settings.SaveSettings;
using EnchantedMask.Glyphs;
using System.Diagnostics;

namespace EnchantedMask
{
    public class EnchantedMask : Mod, ILocalSettings<SaveSettings>
    {
        public override string GetVersion() => "1.5.1.0";

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
                ("Crossroads_04", "_Enemies/Zombie Hornhead"),
                ("White_Palace_06", "wp_saw (3)")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            SharedData.Log("Initializing");

            SharedData.preloads = preloadedObjects;

            SharedData.paleCourtMod = ModHooks.GetMod("Pale Court");

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
            On.HeroController.Update += OnUpdate;

            SharedData.Log("Initialized");
        }

        /// <summary>
        /// Once the HeroController starts, the player data and save settings will be loaded, so we can set up the glyphs
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void OnPlayerDataLoaded(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);

            //SharedData.Log("Hero Controller started");
            AddCustomFireball();

            if (!string.IsNullOrWhiteSpace(SharedData.saveSettings.EquippedGlyph))
            {
                Glyphs.Glyph glyph = SharedData.glyphs.Where(x => x.ID.Equals(SharedData.saveSettings.EquippedGlyph))
                                                        .First();
                glyph.Equip();
            }
        }

        /// <summary>
        /// Adds states and actions so that Champion can replace Vengeful Spirit
        /// </summary>
        private void AddCustomFireball()
        {
            // First, we need to make 2 new states to replace Vengeful Spirit and Shade Soul
            PlayMakerFSM spellControl = HeroController.instance.spellControl;
            Satchel.FsmUtil.CopyState(spellControl, "Fireball 1", Champion.level1State);
            Satchel.FsmUtil.CopyState(spellControl, "Fireball 2", Champion.level2State);

            // Then, we need to replace the code in the new custom states
            Satchel.FsmUtil.RemoveAction(spellControl, Champion.level1State, 3);
            Satchel.FsmUtil.RemoveAction(spellControl, Champion.level2State, 3);
            Satchel.FsmUtil.InsertCustomAction(spellControl, Champion.level1State, () => Champion.ShootSaw(false), 3);
            Satchel.FsmUtil.InsertCustomAction(spellControl, Champion.level2State, () => Champion.ShootSaw(true), 3);

            //SharedData.Log("FSM ready for modification");
        }

        /// <summary>
        /// Some glyphs, like Blessed, modify player data, so we have to reset them all when we close a save file
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="saveMode"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator OnQuit(On.GameManager.orig_ReturnToMainMenu orig, GameManager self, GameManager.ReturnToMainMenuSaveModes saveMode, Action<bool> callback)
        {
            IconHelper.ResetGlyphs();

            //SharedData.Log("Closing save");
            return orig(self, saveMode, callback);
        }

        /// <summary>
        /// Need to periodically check the Champions saws for deletion, but
        /// avoid coroutines to mitigate data usage
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void OnUpdate(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);

            List<Tuple<GameObject, Stopwatch>> saws = new List<Tuple<GameObject, Stopwatch>>();
            while (Champion.clones.Count > 0)
            {
                saws.Add(Champion.clones.Dequeue());
            }

            foreach (Tuple<GameObject, Stopwatch> saw in saws)
            {
                if (saw.Item2.ElapsedMilliseconds > 5000)
                {
                    UnityEngine.GameObject.Destroy(saw.Item1);
                }
                else
                {
                    Champion.clones.Enqueue(saw);
                }
            }
        }
    }
}