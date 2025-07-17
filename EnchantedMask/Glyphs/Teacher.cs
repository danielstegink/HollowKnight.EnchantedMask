using EnchantedMask.Helpers;
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
                                                "Attracts a swarm of lumaflies that damage nearby enemies.";

        public override bool Unlocked()
        {
            return PlayerData.instance.monomonDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.monomonDefeated)
            {
                return "The Teacher rests deep within her Archives.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            isActive = true;
            CreatePrefab();
            GameManager.instance.StartCoroutine(SpawnLumaflies());
        }

        public override void Unequip()
        {
            base.Unequip();

            isActive = false;
        }

        /// <summary>
        /// Tracks if the swarms should keep spawning
        /// </summary>
        private bool isActive = false;

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
            GameObject preload = SharedData.preloads["GG_Uumuu"]["Mega Jellyfish GG"]
                                             .LocateMyFSM("Mega Jellyfish")
                                            .GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value;
            preload.layer = (int)PhysLayers.HERO_ATTACK;
            Satchel.GameObjectUtils.RemoveComponent<DamageHero>(preload);
            preload.AddComponent<LumaflyDamage>();

            // Copy preload into a prefab, then disable it so it doesn't shock us
            prefab = UnityEngine.GameObject.Instantiate(preload);
            prefab.name = "EnchantedMask.Lumaflies";
            prefab.SetActive(false);
            UnityEngine.GameObject.DontDestroyOnLoad(prefab);
        }

        /// <summary>
        /// Teacher glyph spawns a lumafly swarm roughly every 2 seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnLumaflies()
        {
            while (isActive)
            {
                GameObject swarm = UnityEngine.GameObject.Instantiate(prefab, HeroController.instance.transform.position, Quaternion.identity);
                swarm.SetActive(true);

                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
