using GlobalEnums;
using UnityEngine;

namespace EnchantedMask.Helpers
{
    /// <summary>
    /// Lumafly Damage is the damage component for the lumafly swarm from the
    ///     Teacher glyph.
    /// The Teacher is an Uncommon glyph, making it worth 2 notches. So it should
    ///     do twice as much damage per second as Defender's Crest.
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

            // By default, we want to deal only a little damage
            // The lumaflies are a hazard (1), so the damage should be Generic
            HitInstance hit = new HitInstance
            {
                DamageDealt = 15,
                AttackType = AttackTypes.Generic,
                IgnoreInvulnerable = true,
                Source = gameObject,
                Multiplier = 1f
            };

            HealthManager enemy = other.gameObject.GetComponent<HealthManager>();
            enemy.Hit(hit);
            //SharedData.Log($"Teacher - Lumaflies dealt {hit.DamageDealt} damage.");
        }
    }
}
