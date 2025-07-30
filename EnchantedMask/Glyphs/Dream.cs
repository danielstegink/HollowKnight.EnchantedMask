using EnchantedMask.Settings;
using System;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Dream : Glyph
    {
        public override string ID => "Dream";
        public override string Name => "Glyph of Dreams";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a forgotten master of dreams.\n\n" +
                                                "Allows the bearer to spend Essence to damage enemies with the Dream Nail.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mothDeparted;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.mothDeparted)
            {
                return "The master of dreams waits for you to complete your training.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.EnemyDreamnailReaction.RecieveDreamImpact += DreamAttack;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.EnemyDreamnailReaction.RecieveDreamImpact -= DreamAttack;
        }


        /// <summary>
        /// The Dream glyph spends Essence to deal damage to enemies upon Dream Nail hit
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DreamAttack(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
        {
            // Run the dream nail first 
            orig(self);

            // Confirm the enemy has a HealthManager for us to damage
            HealthManager enemy = self.gameObject.GetComponent<HealthManager>();
            if (enemy != default)
            {
                // Calculate the amount of damage to deal and how much Essence to use
                int essenceSpent = GetEssence();
                if (essenceSpent > 0)
                {
                    // Calculate the damage
                    float damagePerSoul = GetDamagePerSoul();
                    float soulPerEssence = GetSoulPerEssence();
                    float damage = damagePerSoul * soulPerEssence * essenceSpent;
                    int damageInt = (int)Math.Round(damage);

                    // Set up a spell attack
                    HitInstance dreamHit = new HitInstance
                    {
                        DamageDealt = damageInt,
                        AttackType = AttackTypes.Spell,
                        IgnoreInvulnerable = true,
                        Source = HeroController.instance.gameObject,
                        Multiplier = 1f
                    };

                    // Deal the damage
                    enemy.Hit(dreamHit);
                    //SharedData.Log($"{ID} - {damageInt} damage dealt for {essenceSpent} Essence");

                    // Remove the essence from our inventory and add them as 
                    // essence spent so the game knows to increase the chance of 
                    // us getting more
                    PlayerData.instance.dreamOrbs -= essenceSpent;
                    PlayerData.instance.dreamOrbsSpent += essenceSpent;
                }
            }
        }

        /// <summary>
        /// Dream effectively adds a spell cast to the Dream Nail,
        ///     so we need to pick a spell and calculate damage based
        ///     on it.
        /// </summary>
        /// <returns></returns>
        internal float GetDamagePerSoul()
        {
            // Dream uses Essence to deal extra damage when it normally wouldn't deal any.
            // For 2 notches, Glowing Womb uses 8 SOUL to deal 9 extra damage every 4 seconds.
            //      That's 1.125 damage per SOUL per second per notch.
            // Dream is an Uncommon glyph worth 2 notches. Additionally, this damage
            //      only applies when using a Dream Nail, which takes 1.75 seconds to charge.
            // That means we should deal 3.9375 damage per SOUL.
            return 3.938f;
        }

        /// <summary>
        /// Determines how much SOUL 1 Essence is worth
        /// </summary>
        /// <returns></returns>
        private float GetSoulPerEssence()
        {
            // Normally the user gets 11 SOUL per nail attack. Assuming the enemy can survive
            //      3 nail attacks (about 60 HP w/ Pure Nail), thats 33 SOUL per enemy.
            // In comparison, Essence has a drop rate of 1 per 300 enemes. Now, this glyph
            //      will result in us spending a lot of Essence, so the ratio will increase to
            //      1 per 60 enemies.
            // On paper, this makes it very valuable. However, we use SOUL constantly and 
            //      use Essence practically never, so it makes sense to seriously adjust
            //      the value.
            // I'm thinking Essence should be only 1% as valuable, since we only ever use it for
            //      Dream Gates.
            // 33 * 60 / 100 = 19.8
            return 19.8f;
        }

        /// <summary>
        /// Determines how much Essence to spend on extra damage
        /// </summary>
        /// <returns></returns>
        private int GetEssence()
        {
            // Dream uses Essence to deal extra damage when it normally wouldn't deal any.
            // So, how much SOUL would we spend for 1 notch?
            // DN is like a Nail Art, so we're adding a Spell to a Nail Art.
            // For 2 notches, Glowing Womb uses 8 SOUL to deal 9 extra damage every 4 seconds.
            //      That's 1 SOUL per second per notch.
            // Dream Nail takes 1.75 seconds to attack, and Dream is worth 2 notches, so
            //      we would spend 3.5 SOUL.
            // But we need to spend at least 1 Essence, so we'll spend 1 Essence for 19.8 SOUL.
            // DN is hard to pull off anyway, so this is probably fine.
            return Math.Min(1, PlayerData.instance.dreamOrbs);
        }
    }
}