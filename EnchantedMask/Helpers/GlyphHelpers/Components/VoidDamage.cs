using EnchantedMask.Settings;
using GlobalEnums;
using UnityEngine;

namespace EnchantedMask.Helpers.GlyphHelpers.Components
{
    /// <summary>
    /// Damage component for the Siblings created by the Void glyph
    /// </summary>
    public class VoidDamage : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            // Verify that the collider exists and that its an enemy
            if (other == null ||
                other.gameObject.layer != (int)PhysLayers.ENEMIES)
            {
                return;
            }
            SharedData.Log($"Void - Collision: {other.gameObject.name}");

            // Void is a Legendary glyph worth 5 notches.
            // Siblings are able to travel at a slow speed as opposed to Scream which doesn't move.
            // So Void turns HW into a slower version of Shade Soul.
            //      SS deals 30 damage and moves about twice as fast as the Siblings, but they're
            //          virtually guaranteed a hit, so they roughly balance out.
            //      So together the 4 Siblings should deal about 30 damage.
            // Flukenest boosts SS damage by about 100% for 3 notches and a 50% range reduction.
            //      MOP increases nail range by 25% for 3 notches, so a 50% reduction is worth 6 notches.
            //      A 100% increase in a single spell's damage is worth 9 notches, or 11.11% per notch.
            // That increases the Sibling's total damage to 46.67, or 11.67 damage each.
            // Shaman Stone also increases that damage by 50% if equipped
            float siblingDamage = 30f * (1f + 5f / 9f) / 4f;
            if (PlayerData.instance.equippedCharm_19)
            {
                siblingDamage *= 1.5f;
            }

            HitInstance hit = new HitInstance
            {
                DamageDealt = (int)siblingDamage,
                AttackType = AttackTypes.SharpShadow,
                IgnoreInvulnerable = true,
                Source = gameObject,
                Multiplier = 1f
            };
            HealthManager enemy = other.gameObject.GetComponent<HealthManager>();
            enemy.Hit(hit);
            SharedData.Log($"Void - Enemy hit for: {(int)siblingDamage} damage");

            // Destroy Sibling
            HealthManager self = transform.parent.gameObject.GetComponent<HealthManager>();
            self.IsInvincible = false;

            SharedData.Log("Void - Destroying Sibling");
            HitInstance selfDestruct = new HitInstance()
            {
                DamageDealt = self.hp,
                AttackType = AttackTypes.Spell,
            };
            self.Hit(selfDestruct);
        }
    }
}
