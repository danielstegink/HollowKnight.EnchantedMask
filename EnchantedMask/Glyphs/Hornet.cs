using EnchantedMask.Settings;
using GlobalEnums;
using Modding;
using System.Collections;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Hornet : Glyph
    {
        public override string ID => "Hornet";
        public override string Name => "Glyph of Protection";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => "The symbol of the sworn protector of Hallownest's ruins.\n\n" +
                                                "May summon a magic web that defends the bearer from damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.hornetOutskirtsDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hornet1Defeated)
            {
                return "The Gendered Child stands watch at Greenpath's end.";
            }
            else if (!PlayerData.instance.hornetOutskirtsDefeated)
            {
                return "The Gendered Child guards the King's legacy.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            InitializeWebShield();
            ModHooks.TakeHealthHook += WebBlock;
        }

        public override void Unequip()
        {
            base.Unequip();

            ModHooks.TakeHealthHook -= WebBlock;
        }

        #region Define Web prefab
        /// <summary>
        /// Stores custom web shield for easy creation later
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// Gets Hornet web from preloads and builds our custom version
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
        #endregion

        #region Spawn Web
        /// <summary>
        /// The Hornet glyph has a chance to produce a web that negates damage
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int WebBlock(int damage)
        {
            if (damage > 0)
            {
                int random = UnityEngine.Random.Range(1, 101);
                //SharedData.Log($"{ID} - Attempting to block: {random}");
                if (random <= BlockChance())
                {
                    GameManager.instance.StartCoroutine(SpawnWeb());
                    damage = 0;
                }
            }

            return damage;
        }

        /// <summary>
        /// As a Rare glyph, Sentinel is worth 3 notches and has a 22% chance of blocking.
        /// </summary>
        /// <returns></returns>
        internal int BlockChance()
        {
            return 22;
        }

        /// <summary>
        /// Spawns a Hornet web around the player
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private IEnumerator SpawnWeb()
        {
            Vector3 spawnPoint = HeroController.instance.transform.position;
            GameObject web = UnityEngine.GameObject.Instantiate(prefab, spawnPoint, Quaternion.identity);
            web.SetActive(true);

            yield return new WaitForSeconds(1f);
            UnityEngine.GameObject.Destroy(web);
        }
        #endregion
    }
}