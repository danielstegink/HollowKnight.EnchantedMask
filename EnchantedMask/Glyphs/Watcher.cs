using EnchantedMask.Helpers;
using EnchantedMask.Settings;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using SFCore.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Watcher : Glyph
    {
        public override string ID => "Watcher";
        public override string Name => "Glyph of the Watcher";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the City's guardian protector.\n\n" +
                                                "Increases the bearer's charisma, causing their companions to deal increased damage.";

        public override bool Unlocked()
        {
            return PlayerData.instance.lurienDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.lurienDefeated)
            {
                return "The Watcher dreams from atop his Spire.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += BuffGrimmchild;
            On.HealthManager.Hit += BuffHatchlings;
            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += BuffWeaverlings;
            ModHooks.ObjectPoolSpawnHook += BuffFlukes;
            dungFlukeHelper = new DungFlukeHelper(1 - GetModifier());
            dungFlukeHelper.Start();

        }

        public override void Unequip()
        {
            base.Unequip();

            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter -= BuffGrimmchild;
            On.HealthManager.Hit -= BuffHatchlings;
            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter -= BuffWeaverlings;
            ModHooks.ObjectPoolSpawnHook -= BuffFlukes;
            if (dungFlukeHelper != null)
            {
                dungFlukeHelper.Stop();
            }
        }

        /// <summary>
        /// Grimmchild fires Grimmballs that store damage from Grimmchild's FSM. Modifying Grimmchild's FSM hasn't
        /// proven effective, but what has worked is going into the Hit state of the Grimmball's Enemy Damager child
        /// object's Attack FSM, and modifying the Damage variable there.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void BuffGrimmchild(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntOperator self)
        {
            //SharedData.Log($"Entering Int Operator: {self.Fsm.Name}, {self.Fsm.GameObjectName}, {self.Fsm.GameObject.transform.parent.gameObject.name}, {self.State.Name}");

            if (self.Fsm.Name.Equals("Attack") &&
                self.Fsm.GameObject.name.Equals("Enemy Damager") &&
                self.Fsm.GameObject.transform.parent.gameObject.name.Contains("Grimmball") &&
                self.State.Name.Equals("Hit"))
            {
                int baseDamage = self.Fsm.GetFsmInt("Damage").Value;
                self.Fsm.GetFsmInt("Damage").Value += GetBonus(baseDamage);
                //SharedData.Log($"Grimmball Hit state found. Damage: {self.Fsm.GetFsmInt("Damage")}");
            }

            orig(self);
        }

        private void BuffHatchlings(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
        {
            string parentName = "";
            if (hitInstance.Source.gameObject.name.Equals("Damager"))
            {
                // Check if the parent of the Damager object is a Knight Hatchling
                try
                {
                    parentName = hitInstance.Source.gameObject.transform.parent.name;
                }
                catch { }
                
                if (parentName.Contains("Knight Hatchling"))
                {
                    int baseDamage = hitInstance.DamageDealt;
                    hitInstance.DamageDealt += GetBonus(baseDamage);
                }
            }

            orig(self, hitInstance);
            //SharedData.Log($"{self.gameObject.name} " +
            //                $"hit by {hitInstance.Source.name} ({parentName}) " +
            //                $"for {hitInstance.DamageDealt}");
        }

        /// <summary>
        /// Weaverlings, like Grimmballs, store their damage in an Enemy Damager object
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void BuffWeaverlings(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntOperator self)
        {
            int baseDamage = 0;
            if (self.Fsm.Name.Equals("Attack") &&
                self.Fsm.GameObject.name.Equals("Enemy Damager") &&
                self.Fsm.GameObject.transform.parent.gameObject.name.Contains("Weaverling") &&
                self.State.Name.Equals("Hit"))
            {
                baseDamage = self.Fsm.GetFsmInt("Damage").Value;
                self.Fsm.GetFsmInt("Damage").Value += GetBonus(baseDamage);
                //SharedData.Log($"Weaverling damage set to: {self.Fsm.GetFsmInt("Damage")}");
            }

            orig(self);

            // Reset damage afterwards; Weaverling FSMs don't reset like Grimmballs
            if (baseDamage > 0)
            {
                self.Fsm.GetFsmInt("Damage").Value = baseDamage;
            }
        }

        /// <summary>
        /// Used for handling damage buff for Dung Flukes
        /// </summary>
        private DungFlukeHelper dungFlukeHelper;

        /// <summary>
        /// Increases damage dealt by Flukenest
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffFlukes(GameObject gameObject)
        {
            // Spell Flukes die pretty fast, so we don't really need to worry about
            //  removing the bonus damage if the player removes the charm after
            //  casting the spell
            if (gameObject.name.StartsWith("Spell Fluke") &&
               gameObject.name.Contains("Clone"))
            {
                // Dung Flukes have to be handled separately
                if (!gameObject.name.Contains("Dung"))
                {
                    SpellFluke fluke = gameObject.GetComponent<SpellFluke>();

                    // Fluke damage is stored in a private variable, so we need to
                    //  get the field the variable is stored in
                    int baseDamage = SharedData.GetField<SpellFluke, int>(fluke, "damage");
                    int bonusDamage = (int)Math.Max(1, baseDamage * GetModifier());

                    SharedData.SetField(fluke, "damage", baseDamage + bonusDamage);
                    //SharedData.Log($"{ID} - Damage increased by {bonusDamage}");
                }
            }

            return gameObject;
        }

        /// <summary>
        /// Watcher glyph is Uncommon, so its worth 2 notches. 
        /// Normally this would be worth 20% nail damage, but pet damage is
        ///     niche and requires multiple charms, so this can be increased.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.3f;
        }
    }
}