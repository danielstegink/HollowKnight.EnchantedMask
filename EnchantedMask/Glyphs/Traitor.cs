using EnchantedMask.Helpers.GlyphHelpers;
using EnchantedMask.Helpers.UI;
using EnchantedMask.Settings;
using GlobalEnums;
using System;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Traitor : Glyph
    {
        public override string ID => "Traitor";

        public override string Name => $"Glyph of {(PlayerData.instance.clothKilled ? "Warriors" : "Betrayal")}";
        public override Tiers Tier => Tiers.Rare;
        public override string Description => PlayerData.instance.clothKilled ?
                                                "The symbol of a brave warrior.\n\n" +
                                                    "When the bearer takes damage, their nearest attacker takes damage as well." :
                                                "The symbol of one who betrayed their tribe.\n\n" +
                                                    "Increases the bearer's strength, allowing them to do more damage with a nail.";
        public override Sprite GetIcon()
        {
            if (PlayerData.instance.clothKilled)
            {
                return SpriteHelper.GetMaskSprite("Warrior");
            }
            else
            {
                return base.GetIcon();
            }
        }

        public override bool Unlocked()
        {
            return PlayerData.instance.killedTraitorLord;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.savedCloth &&
                        !PlayerData.instance.killedTraitorLord)
            {
                return "You stand at a crossroads: a traitor in exile stalks worthy prey " +
                        "while a warrior cowers in caves below the kingdom. Which will you seek first?";
            }
            else if (!PlayerData.instance.killedTraitorLord)
            {
                return "A traitor in exile stalks worthy prey.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            if (PlayerData.instance.clothKilled)
            {
                On.HeroController.TakeDamage += RevengeStrike;
            }
            else
            {
                On.HealthManager.TakeDamage += BuffNail;
            }
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.TakeDamage -= RevengeStrike;
            On.HealthManager.TakeDamage -= BuffNail;
        }

        /// <summary>
        /// Traitor increases nail damage. 
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="hitInstance"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuffNail(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            bool isNailAttack = SharedData.nailAttackNames.Contains(hitInstance.Source.name) ||
                               SharedData.nailArtNames.Contains(hitInstance.Source.name) ||
                               hitInstance.Source.name.Contains("Grubberfly");
            if (isNailAttack)
            {
                int bonusDamage = GetBonus(hitInstance.DamageDealt);
                hitInstance.DamageDealt += bonusDamage;
                //SharedData.Log($"{ID} - Nail damage increased by {bonusDamage}");
            }

            orig(self, hitInstance);
        }

        /// <summary>
        /// Warrior causes a nearby enemy to take damage when the player does, 
        /// mimicking Cloth's killing blow against the Traitor Lord.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="damageSide"></param>
        /// <param name="damageAmount"></param>
        /// <param name="hazardType"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RevengeStrike(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            bool canTakeDamage = SharedData.CallFunction<HeroController, bool>(self, "CanTakeDamage", new object[] { });

            orig(self, go, damageSide, damageAmount, hazardType);

            // If the attacker is an actual enemy and not an acid pool
            //      or something, hit back.
            // Also, ignore it if the player can't take damage right now, 
            //      such as if they still have I-Frames.
            if (hazardType == 1 && canTakeDamage)
            {
                // Most enemies aren't directly linked to their attacks, so instead we 
                //      need to compromise by targetting the closest enemy
                GameObject closestEnemy = GetEnemyHelper.GetNearestEnemy();
                if (closestEnemy != null)
                {
                    int nailDamage = PlayerData.instance.nailDamage;
                    int revengeDamage = GetBonus(nailDamage);
                    HitInstance attack = new HitInstance
                    {
                        DamageDealt = revengeDamage,
                        AttackType = AttackTypes.Generic,
                        IgnoreInvulnerable = true,
                        Source = self.gameObject,
                        Multiplier = 1f
                    };

                    HealthManager enemyHealth = closestEnemy.GetComponent<HealthManager>();
                    enemyHealth.Hit(attack);
                    //SharedData.Log($"{ID} - Enemy {closestEnemy.name} hit for {revengeDamage} damage.");
                }
            }
        }

        /// <summary>
        /// The modifier changes depending on the glyph
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Warrior/Traitor are a Rare glyph, so they're worth 3 notches.

            // Thorns of Agony is a 1-notch charm and deals 1x nail damage,
            //      so Warrior should deal triple that.
            if (PlayerData.instance.clothKilled)
            {
                return 3f;
            }
            else
            {
                // Unbreakable Strength increase nail damage by 50% for 3 notches.
                // Per Greed, I've decided that Unbreakable is worth 2 extra notches.
                // So nail damage is worth about a 10% boost per notch.
                return 0.3f;
            }
        }
    }
}