using EnchantedMask.Helpers.UI;
using EnchantedMask.Settings;
using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UIElements;

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

            On.HealthManager.TakeDamage -= BuffNail;
            On.HeroController.TakeDamage -= RevengeStrike;
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
                int nailDamage = PlayerData.instance.nailDamage;
                int revengeDamage = GetBonus(nailDamage);
                HitInstance hit = new HitInstance
                {
                    DamageDealt = revengeDamage,
                    AttackType = AttackTypes.Generic,
                    IgnoreInvulnerable = true,
                    Source = self.gameObject,
                    Multiplier = 1f
                };

                // Most enemies aren't directly linked to their attacks, so instead we 
                //      need to compromise by targetting the closest enemy
                HealthManager[] enemies = UnityEngine.Component.FindObjectsOfType<HealthManager>();
                List<HealthManager> enemiesByDistance = enemies.OrderBy(x => GetDistance(x.transform, self.gameObject.transform))
                                                            .ToList();
                if (enemiesByDistance.Count > 0)
                {
                    HealthManager closestEnemy = enemiesByDistance[0];
                    closestEnemy.Hit(hit);
                    //SharedData.Log($"{ID} - Enemy {closestEnemy.gameObject.name} hit for {revengeDamage} damage.");
                }
            }
        }

        /// <summary>
        /// The modifier changes depending on the glyph
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            // Warrior is a Rare glyph, so its worth 3 notches.
            // Thorns of Agony is a 1-notch charm and deals 1x nail damage,
            //      so Warrior should deal triple that.
            if (PlayerData.instance.clothKilled)
            {
                return 3f;
            }
            else
            {
                // Traitor is Rare, so its worth 3 notches
                // Nail damage is worth a 10% boost per notch.
                return 0.3f;
            }
        }

        /// <summary>
        /// Use Pythagorean to get distance between 2 objects
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        internal float GetDistance(Transform enemy, Transform player)
        {
            float xDiff = Math.Abs(enemy.GetPositionX() - player.GetPositionX());
            float yDiff = Math.Abs(enemy.GetPositionY() - player.GetPositionY());
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }
}
