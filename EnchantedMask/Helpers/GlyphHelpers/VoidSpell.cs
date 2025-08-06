using EnchantedMask.Helpers.GlyphHelpers.Components;
using EnchantedMask.Settings;
using GlobalEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers
{
    public static class VoidSpell
    {
        /// <summary>
        /// Stores the Shade Sibling prefab
        /// </summary>
        private static GameObject prefab;

        /// <summary>
        /// List of Siblings already spawned
        /// </summary>
        private static List<GameObject> siblings = new List<GameObject>();

        /// <summary>
        /// Abyssal Wraiths summons Siblings that float to the nearest enemy and explode
        /// </summary>
        public static void AbyssalWraiths()
        {
            if (prefab == null)
            {
                GetSibling();
            }

            Vector3 heroLocation = HeroController.instance.transform.position;
            for (int i = 0; i < 4; i++)
            {
                Vector3 offset = new Vector3(-3 + 2 * i, 2, 0);
                GameObject sibling = UnityEngine.GameObject.Instantiate(prefab, heroLocation + offset, Quaternion.identity);
                sibling.SetActive(true);
                siblings.Add(sibling);
            }
        }

        /// <summary>
        /// Gets Shade Sibling from preloads
        /// </summary>
        /// <returns></returns>
        private static void GetSibling()
        {
            // Get prefab from preloads
            GameObject spawner = SharedData.preloads["Abyss_06_Core"]["Shade Sibling Spawner"];
            PersonalObjectPool siblingPool = spawner.GetComponent<PersonalObjectPool>();
            GameObject sibling = siblingPool.startupPool[0].prefab;
            prefab = UnityEngine.GameObject.Instantiate(sibling);
            prefab.SetActive(false);
            UnityEngine.GameObject.DontDestroyOnLoad(prefab);

            // Customize prefab
            prefab.name = "EnchantedMask.VoidSibling";
            prefab.layer = (int)PhysLayers.HERO_ATTACK;
            prefab.AddComponent<VoidDamage>();
            Satchel.GameObjectUtils.RemoveComponent<DamageHero>(prefab);
            Satchel.GameObjectUtils.RemoveComponent<EnemyHitEffectsShade>(prefab);
            Satchel.GameObjectUtils.RemoveComponent<LimitBehaviour>(prefab);
            HealthManager health = prefab.GetComponent<HealthManager>();
            health.IsInvincible = true;
            health.hp = 1;
        }

        /// <summary>
        /// Ensures Siblings move towards the nearest enemy
        /// </summary>
        /// <returns></returns>
        public static IEnumerator SiblingMovement()
        {
            // Confirm the coroutine is active
            while (siblings.Count > 0)
            {
                // Go through each Sibling spawned
                foreach (GameObject sibling in siblings)
                {
                    // If a sibling has been destroyed, get rid of it
                    if (sibling == null || 
                        !sibling.activeSelf)
                    {
                        siblings.Remove(sibling);
                        UnityEngine.GameObject.Destroy(sibling);
                        continue;
                    }

                    // Get the nearest enemy
                    GameObject closestEnemy = DanielSteginkUtils.Helpers.GetEnemyHelper.GetNearestEnemy();
                    if (closestEnemy != null)
                    {
                        Vector3 target = closestEnemy.gameObject.transform.position;
                        Vector3 source = sibling.transform.position;

                        // Send Sibling in the direction of the enemy
                        var rb2d = sibling.GetComponent<Rigidbody2D>();
                        rb2d.velocity = new Vector2(target.x - source.x, target.y - source.y);

                        // Adjust Sibling's speed so it is consistent regardless of distance
                        float modifier = 10f / rb2d.velocity.magnitude;
                        rb2d.velocity = new Vector2(rb2d.velocity.x * modifier, rb2d.velocity.y * modifier);
                    }
                    else // If there is no enemy, stay in place and don't move
                    {
                        var rb2d = sibling.GetComponent<Rigidbody2D>();
                        rb2d.velocity = Vector2.zero;
                    }
                }
            }

            yield return new WaitForSeconds(0f);
        }
    }
}
