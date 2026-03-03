using DanielSteginkUtils.Utilities;
using EnchantedMask.Helpers.GlyphHelpers.Components;
using EnchantedMask.Settings;
using GlobalEnums;
using ItemChanger.Extensions;
using Modding;
using Satchel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Champion : Glyph
    {
        public override string ID => "Champion";
        public override string Name => "Glyph of Champions";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of Five Great Knights' champion and master.\n\n" +
                                                "Transforms the Vengeful Spirit spell into a symbol of the Pale King.";

        public override bool Unlocked()
        {
            return CallCleared(out _);
        }

        public override string GetClue()
        {
            if (!CallCleared(out string clue))
            {
                return clue;
            }

            return base.GetClue();
        }


        /// <summary>
        /// Checks if the player has cleared the Champion's Call in Pale Court
        /// </summary>
        /// <param name="clue"></param>
        /// <returns></returns>
        private bool CallCleared(out string clue)
        {
            if (SharedData.paleCourtMod == null)
            {
                clue = "You do not have Pale Court installed.";
                return false;
            }

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourtMod, "SaveSettings");
            int callClears = ClassIntegrations.GetField<object, int>(saveSettings, "ChampionsCallClears");
            if (callClears <= 0)
            {
                clue = "The greatest challenge of the Pale King's legacy remains unconquered.";
                return false;
            }

            clue = "";
            return true;
        }

        public override void Equip()
        {
            base.Equip();

            ReplaceFireball(true);
            On.HealthManager.Hit += IFrames;
        }

        public override void Unequip()
        {
            base.Unequip();

            ReplaceFireball(false);
            On.HealthManager.Hit -= IFrames;
        }

        #region FSM changes
        /// <summary>
        /// Stores the buzzsaw prefab
        /// </summary>
        internal static GameObject prefab;

        /// <summary>
        /// Name of the state replacing Vengeful Spirit
        /// </summary>
        internal static string level1State = "EnchantedMask.Saw1";

        /// <summary>
        /// Name of the state replacing Shade Soul
        /// </summary>
        internal static string level2State = "EnchantedMask.Saw2";

        /// <summary>
        /// Tracks all buzzsaw clones
        /// </summary>
        internal static Queue<Tuple<GameObject, Stopwatch>> clones = new Queue<Tuple<GameObject, Stopwatch>>();

        /// <summary>
        /// Modifies the spell FSM by replacing the VS/SS with a custom spell that shoots buzzsaws
        /// </summary>
        /// <param name="replace"></param>
        private void ReplaceFireball(bool replace)
        {
            PlayMakerFSM spellControl = HeroController.instance.spellControl;

            if (replace)
            {
                spellControl.ChangeTransition("Level Check", "LEVEL 1", level1State);
                spellControl.ChangeTransition("Level Check", "LEVEL 2", level2State);
            }
            else
            {
                spellControl.ChangeTransition("Level Check", "LEVEL 1", "Fireball 1");
                spellControl.ChangeTransition("Level Check", "LEVEL 2", "Fireball 2");
            }
        }

        /// <summary>
        /// Custom action to shoot a buzzsaw like a fireball
        /// </summary>
        /// <param name="isUpgraded"></param>
        public static void ShootSaw(bool isUpgraded)
        {
            if (prefab == null)
            {
                GameObject saw = SharedData.preloads["White_Palace_06"]["wp_saw (3)"];
                saw.SetActive(false);
                prefab = UnityEngine.GameObject.Instantiate(saw);
                prefab.layer = (int)PhysLayers.HERO_ATTACK;
                Satchel.GameObjectUtils.RemoveComponent<DamageHero>(prefab);
                prefab.AddComponent<SawDamage>();
                prefab.name = "EnchantedMask.ChampionSaw";
                prefab.SetActive(false);
                UnityEngine.GameObject.DontDestroyOnLoad(prefab);
            }

            // Want saw to spawn where the fireball normally would
            Vector3 position = HeroController.instance.transform.position;
            bool facingRight = HeroController.instance.cState.facingRight;
            position.x += facingRight ? 3 : -3;

            GameObject clone = UnityEngine.GameObject.Instantiate(prefab, position, Quaternion.identity);
            SawDamage sawDamage = clone.GetComponent<SawDamage>();
            sawDamage.damageDealt = SawDamage.GetDamage(isUpgraded);
            clone.SetActive(true);

            // After we make the clone, log when it was created so we can delete it after a short period of time
            Stopwatch timer = new Stopwatch();
            timer.Start();
            clones.Enqueue(new Tuple<GameObject, Stopwatch>(clone, timer));
        }

        /// <summary>
        /// To prevent damage stacking on enemies, the buzz saw will give I-Frames to attacked enemies
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        internal static void IFrames(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            // If the enemy is hit by a buzzsaw, check if they were recently hit by that same buzzsaw
            if (hitInstance.Source.name.StartsWith("EnchantedMask.ChampionSaw"))
            {
                SharedData.Log($"Attempting to deal {hitInstance.DamageDealt} spell damage");
                GameObject enemy = self.gameObject;
                SawIFrames iFrames = enemy.GetOrAddComponent<SawIFrames>();

                // If they were never hit, note that they were attacked at this time
                if (!iFrames.attackLog.ContainsKey(hitInstance.Source))
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    iFrames.attackLog.Add(hitInstance.Source, timer);
                }
                else
                {
                    // If it's been less than a second, negate the damage
                    if (iFrames.attackLog[hitInstance.Source].ElapsedMilliseconds < 1000)
                    {
                        hitInstance.DamageDealt = 0;
                    }
                    else // Otherwise, let the damage happen and reset the timer
                    {
                        iFrames.attackLog[hitInstance.Source].Restart();
                    }
                }
            }

            orig(self, hitInstance);
        }
        #endregion
    }
}