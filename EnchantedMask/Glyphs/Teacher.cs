using EnchantedMask.Helpers.GlyphHelpers.Components;
using EnchantedMask.Settings;
using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Teacher : Glyph
    {
        public override string ID => "Teacher";
        public override string Name => "Glyph of the Teacher";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the kingdom's greatest scholar.\n\n" +
                                                "Attracts a swarm of lumaflies that damage the nearest enemy.";

        public override bool Unlocked()
        {
            return PlayerData.instance.monomonDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.monomonDefeated)
            {
                return "The Teacher rests deep within her Archives.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            CreatePrefab();
            GameManager.instance.StartCoroutine(SpawnLumaflies());
        }

        public override void Unequip()
        {
            base.Unequip();
        }

        /// <summary>
        /// Stores custom lumafly swarm
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// Gets lightning swarm from preloads
        /// </summary>
        private void CreatePrefab()
        {
            // Get lightning swarm from preloads
            GameObject preload = SharedData.preloads["GG_Uumuu"]["Mega Jellyfish GG"];
            PlayMakerFSM fsm = preload.LocateMyFSM("Mega Jellyfish");
            SpawnObjectFromGlobalPool spawnAction = fsm.GetAction<SpawnObjectFromGlobalPool>("Gen", 2);
            GameObject lumaflies = spawnAction.gameObject.Value;
            lumaflies.layer = (int)PhysLayers.HERO_ATTACK;
            Satchel.GameObjectUtils.RemoveComponent<DamageHero>(lumaflies);
            lumaflies.AddComponent<LumaflyDamage>();

            // Copy preload into a prefab, then disable it so it doesn't shock us
            prefab = UnityEngine.GameObject.Instantiate(lumaflies);
            prefab.name = "EnchantedMask.Lumaflies";
            prefab.SetActive(false);
            UnityEngine.GameObject.DontDestroyOnLoad(prefab);
        }

        /// <summary>
        /// Teacher glyph spawns a lumafly swarm on the nearest enemy every 2 seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnLumaflies()
        {
            while (IsEquipped())
            {
                // Get the closest enemy
                GameObject closestEnemy = DanielSteginkUtils.Helpers.GetEnemyHelper.GetNearestEnemy();
                if (closestEnemy != null)
                {
                    Vector3 target = closestEnemy.gameObject.transform.position;

                    // Create a swarm
                    Vector3 source = HeroController.instance.transform.position;
                    GameObject swarm = UnityEngine.GameObject.Instantiate(prefab, target, Quaternion.identity);
                    swarm.SetActive(true);

                    // Only wait after we've summoned a swarm
                    // If it takes more than 2 seconds to find one, we should fire automatically
                    yield return new WaitForSeconds(2f);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}