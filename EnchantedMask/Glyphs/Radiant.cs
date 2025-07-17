using EnchantedMask.Settings;
using System;

namespace EnchantedMask.Glyphs
{
    public class Radiant : Glyph
    {
        public override string ID => "Radiant";
        public override string Name => "Glyph of Light";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the source of the Infection.\n\n" +
                                                "Allows the bearer to spend Essence to damage enemies with the Dream Nail.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedFinalBoss;
        }

        public override string GetClue()
        {
            if (PlayerData.instance.royalCharmState != 4)
            {
                return "A dark truth remains hidden.";
            }
            else if (!PlayerData.instance.killedFinalBoss)
            {
                return "The source of Infection hides in the knight's dreams.";
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
        /// The Radiant glyph spends Essence to deal extra damage to enemies
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
                // Get the amount of essence we can spend
                int essenceCount = GetEssence();
                if (essenceCount > 0)
                {
                    // Calculate the damage
                    int damage = (int)(essenceCount * GetModifier());

                    // Set up a spell attack
                    HitInstance dreamHit = new HitInstance
                    {
                        DamageDealt = damage,
                        AttackType = AttackTypes.Spell,
                        IgnoreInvulnerable = true,
                        Source = HeroController.instance.gameObject,
                        Multiplier = 1f
                    };

                    // Deal the damage
                    enemy.Hit(dreamHit);
                    //SharedData.Log($"{ID} - {damage} dealt from {essenceCount} Essence");

                    // Remove the essence from our inventory and add them as 
                    // essence spent so the game knows to increase the chance of 
                    // us getting more
                    PlayerData.instance.dreamOrbs -= essenceCount;
                    PlayerData.instance.dreamOrbsSpent += essenceCount;
                }
            }
        }

        /// <summary>
        /// Determines how much Essence to spend on extra damage
        /// </summary>
        /// <returns></returns>
        private int GetEssence()
        {
            // Radiant is an Epic glyph, making it worth 4 notches.
            // If this were a nail attack, that would be a 40% increase in damage.
            // However, need to consider how many nail attacks a Dream Nail is worth.
            // Dream Nail takes 1.75 seconds to attack and generates 33 SOUL, which is
            //      18.86 SOUL per second.
            // Pure Nail takes 0.41 seconds to deal 21 damage and gain 11 SOUL, which is
            //      26.83 SOUL and 51.22 damage per second.
            // It's moments like this that really drive home the fact that Dream Nail is
            //      only meant to be used against staggered enemies.
            // 33 / 1.75

            // Soul Catcher increases SOUL from nail strikes by 3 for 2 notches, and
            //      Soul Eater increases it by 8 for 4 notches, which means 1 notch is
            //      worth about 1.75 SOUL per nail strike, or 4.27 SOUL per second.
            // Per my own logic regarding Fragile Strength, 1 notch is worth a 10%
            //      increase in nail damage, or 5.12 damage per second.
            //  So 1 SOUL is worth 1.2 damage.
            // 5.122 / (1.75 / 0.41)

            // We've been treating Dream Nail like a nail attack due to its range so far,
            //      but since we're going to spend Essence like SOUL to power it, we need
            //      to treat Dream Nail like a spell. 
            // The spell most like Dream Nail is Abyss Shriek due to its short range and
            //      lack of i-frames.
            // Shaman Stone increases its damage by about 50%, but Shaman Stone affects
            //      all spells.
            // Flukenest increases only fireball spells, and increases their damage by
            //      roughly 100% for 3 notches.
            // It also deserves noting that Quick Slash increases nail speed by
            //      39%, which affects both its damage and its SOUL gain.

            // So, we have a nail-like attack that needs a spell-like damage boost.
            // Dream Nail doesn't do damage, but it does do 18.86 SOUL per second, which
            //      approximates to 22.63 damage per second.
            // So at 4 notches, with a boost of 33% per notch, Radiant should give
            //      Dream Nail an extra 30.17 damage per second, which equates to
            //      52.8 damage per swing.

            // Per below, 1 Essence is worth 12 damage, so the closest value is
            //      48 in exchange for 4 Essence.
            // Of course, we won't spend Essence we don't have.
            return Math.Min(4, PlayerData.instance.dreamOrbs);
        }

        /// <summary>
        /// Determines how much damage the player can deal in 
        /// exchange for 1 Essence
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Since we are treating Essence as a disposable resource like SOUL, it makes
            //      sense to calculate the damage like a Spell.
            // Level 2 Fireball (Shade Soul) deals 30 damage for 33 SOUL.
            float damagePerSoul = 30f/33f;

            // Now to calculate how valuable Essence is compared to SOUL.
            // Normally the user gets 11 SOUL per nail attack. Assuming the enemy being
            //      attacked can survive 2 nail attacks, thats about 22 SOUL per enemy.
            // In comparison, Essence has a drop rate of 1 per 300 enemes. Now, this glyph
            //      will result in us spending a lot of Essence, and in that scenario, the 
            //      ratio will increase to 1 per 60 enemies.
            // On paper, this makes it very valuable. However, we use SOUL constantly and 
            //      use Essence practically never, so it makes sense to seriously adjust
            //      the value.
            // I'm thinking Essence should be only 1% as valuable, since we only ever use it for
            //      Dream Gates.
            float soulPerEssence = 22f * 60f / 100f;

            // So under this new logic, 1 Essence should be worth about 12 damage.
            return damagePerSoul * soulPerEssence;
        }
    }
}
