using DanielSteginkUtils.Helpers.Attributes;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using System;

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
                // Get the theoretical max damage we wish to do
                float maxDamage = GetMaxDamage();

                // Get how much essence we wish to spend
                int essence = GetEssence(maxDamage);
                if (essence > 0)
                {
                    // Calculate the true damage
                    float soul = essence * Calculations.SoulPerEssence();
                    float damage = soul * Calculations.DamagePerSoul(Calculations.SpellType.AbyssShriek);
                    //SharedData.Log($"{ID} - {essence} -> {soul} SOUL -> {damage} damage");

                    // Perform a spell attack to do the damage
                    DamageHelper.DealDamage(enemy, (int)damage, AttackTypes.Spell, HeroController.instance.gameObject);

                    // Lastly, change the Essence counts in our player data so the game knows to adjust the drop rate
                    PlayerData.instance.dreamOrbs -= essence;
                    PlayerData.instance.dreamOrbsSpent += essence;
                }
            }
        }

        /// <summary>
        /// Dream effectively adds a spell cast to the Dream Nail, so we need to 
        /// calculate the upper limit of how much damage we want to do
        /// </summary>
        /// <returns></returns>
        private float GetMaxDamage()
        {
            // Dream is an Uncommon glyph worth 2 notches
            // Dream Nail does no damage of its own, so we will use others to build a base
            // Per my Utils, we know that would be worth 20% nail damage, or 4 damage assuming Pure Nail
            float nailDamage = 2 * NotchCosts.NailDamagePerNotch() * PlayerData.instance.nailDamage;
            //SharedData.Log($"{ID} - Nail damage: {nailDamage}");

            // However, Dream Nail takes much longer than a regular nail attack, so its damage should be inflated
            // Per my Utils, Dream Nail should deal 4.26 times as much damage, for a total of about 17 damage
            float dreamNailDamage = Calculations.NailDamageToDreamNailDamage(nailDamage);
            //SharedData.Log($"{ID} - Dream Nail damage: {dreamNailDamage}");

            // Dream uses Essence like SOUL to deal damage, so we should actually treat this like a spell attack
            // Per my Utils, spell damage is 2.73 times as valuable as nail damage, meaning we want to hit for about 47 damage
            float spellDamage = Calculations.NailDamageToSpellDamage(dreamNailDamage);
            //SharedData.Log($"{ID} - Spell damage: {spellDamage}");

            return spellDamage;
        }

        /// <summary>
        /// Determines how much Essence to spend on damage
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        private int GetEssence(float damage)
        {
            // The damage is based on the Abyss Shriek spell, so we can use its Damage per SOUL to
            // calculate how much SOUl this kind of damage would cost, about 26 SOUL in total
            float soul = damage / Calculations.DamagePerSoul(Calculations.SpellType.AbyssShriek);
            //SharedData.Log($"{ID} - SOUL for {damage} damage: {soul}");

            // Per my Utils, 1 Essence is worth about 20 SOUL
            float essence = soul / Calculations.SoulPerEssence();
            //SharedData.Log($"{ID} - Essence for {soul} SOUL: {essence}");

            // If my math is right, we will want to spend about 1 Essence, but to make things fun I will round up to 2
            // To be safe, we want to make sure we try to spend at least 1 Essence
            int essenceToSpend = Math.Max(1, (int)Math.Ceiling(essence));
            //SharedData.Log($"{ID} - Max essence to spend: {essenceToSpend}");

            // Lastly, make sure we have that much essenceToSpend
            essenceToSpend = Math.Min(essenceToSpend, PlayerData.instance.dreamOrbs);
            //SharedData.Log($"{ID} - Essence to spend: {essenceToSpend}");

            return essenceToSpend;
        }
    }
}