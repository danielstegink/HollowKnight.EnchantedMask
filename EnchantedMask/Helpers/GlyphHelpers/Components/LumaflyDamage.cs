using DanielSteginkUtils.Helpers.Attributes;
using GlobalEnums;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers.Components
{
    /// <summary>
    /// Lumafly Damage is the damage component for the lumafly swarm from the Teacher glyph.
    /// </summary>
    public class LumaflyDamage : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            // Verify that the collider exists and that its an enemy
            if (other == null ||
                other.gameObject.layer != (int)PhysLayers.ENEMIES)
            {
                return;
            }

            HealthManager enemy = other.gameObject.GetComponent<HealthManager>();

            // Teacher is an Uncommon glyph worth 2 notches.
            // Grimmchild also costs 2 notches and deals 11 damage to the nearest enemy every 2 seconds
            // The lumaflies are a hazard, so the damage should be Generic
            DamageHelper.DealDamage(enemy, 11, AttackTypes.Generic, gameObject);
            //SharedData.Log($"Teacher - Lumaflies dealt {hit.DamageDealt} damage.");
        }
    }
}
