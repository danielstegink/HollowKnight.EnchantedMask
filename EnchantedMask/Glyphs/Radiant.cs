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
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.lurienDefeated ||
                !PlayerData.instance.monomonDefeated ||
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (PlayerData.instance.royalCharmState != 4)
            {
                return "A dark truth sleeps beneath the shells of your kin.";
            }
            else if (!PlayerData.instance.killedFinalBoss)
            {
                return "A great light hides in a knight's dreams.";
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
                // Calculate the amount of damage to deal and how much Essence to use
                int essenceSpent = GetEssence();
                if (essenceSpent > 0)
                {
                    // Calculate the damage
                    float damagePerSoul = GetDamagePerSoul();
                    float soulPerEssence = GetSoulPerEssence();
                    float damage = damagePerSoul * soulPerEssence * (float)essenceSpent;
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
        /// Radiant effectively adds a spell cast to the Dream Nail,
        ///     so we need to pick a spell and calculate damage based
        ///     on it.
        /// </summary>
        /// <returns></returns>
        internal float GetDamagePerSoul()
        {
            // DN has short range and a long cast time. So of all 
            //      the spells, it is most similar to Abyss Shriek.
            // Abyss Shriek creates 4 bursts, so we can Radiant
            //      as a single burst at 1/4 the regular SOUL cost.
            float baseDamage = 20f;
            float soulCost = 33f / 4f;

            return baseDamage / soulCost;
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
            // Radiant uses Essence to deal extra damage when it 
            //      normally wouldn't deal any.
            // So, how much SOUL would we spend for 1 notch?
            // DN is like a Nail Art, so we're adding a Spell to a
            //      Nail Art, doubling our DPS.
            // A 3-notch charm like Shaman Stone or Quick Slash increases
            //      DPS by 40%, and casting an extra spell would be like
            //      adding 100%, which would be worth 7.5 notches.
            // However, Dream Nail and Nail Arts are difficult to pull off.
            //      Spells and regular Nail attacks take about 0.41 seconds
            //      to trigger, while Dream Nail takes 1.75 seconds, 426.82%
            //      as long.
            // So, if we factor in that the spell is spread out over a much
            //      longer period of time, it actually becomes worth about
            //      1.75 notches
            // Radiant is an Epic glyph, making it worth 4 notches. So
            //      Radiant should cast 227% of a spell
            float spellsPerDreamNail = 1.75f / 0.41f;
            float notchesPerSpell = 7.5f / spellsPerDreamNail;
            float baseSoul = 33f;
            float totalSoul = baseSoul * 4f / notchesPerSpell;
            //SharedData.Log($"{ID} - Total SOUL spent: {totalSoul}");

            // Finally, calculate the total essence based on the total SOUL
            float soulPerEssence = GetSoulPerEssence();
            float maxEssence = totalSoul / soulPerEssence;
            //SharedData.Log($"{ID} - Max essence: {maxEssence}");

            // Remember to check how much Essence we actually have to spend
            int maxEssenceInt = (int)Math.Round(maxEssence);
            return Math.Min(maxEssenceInt, PlayerData.instance.dreamOrbs);
        }
    }
}
