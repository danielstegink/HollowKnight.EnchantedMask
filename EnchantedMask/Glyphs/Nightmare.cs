using EnchantedMask.Helpers.BlockHelpers;
using EnchantedMask.Helpers.UI;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Nightmare : Glyph
    {
        public override string ID => "Nightmare";
        public override string Name => $"Glyph of {(PlayerData.instance.destroyedNightmareLantern ? "Friendship" : "Nightmares")}";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => PlayerData.instance.destroyedNightmareLantern ?
                                                "The symbol of a friend awoken from a bad dream.\n\n" +
                                                    "Increases the bearer's affinity with protective music." :
                                                "The symbol of a nightmare brought to life.\n\n" +
                                                    "Increases the bearer's affinity with scarlet flames.";
        public override Sprite GetIcon()
        {
            if (PlayerData.instance.destroyedNightmareLantern)
            {
                return SpriteHelper.GetMaskSprite("Friendship");
            }
            else
            {
                return base.GetIcon();
            }
        }

        public override bool Unlocked()
        {
            return PlayerData.instance.destroyedNightmareLantern ||
                    PlayerData.instance.defeatedNightmareGrimm;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.nightmareLanternLit)
            {
                return "A beacon sits unlit among howling winds.";
            }
            else if (PlayerData.instance.grimmChildLevel < 3)
            {
                return "A child of nightmare craves power.";
            }
            else if (!PlayerData.instance.destroyedNightmareLantern &&
                        !PlayerData.instance.defeatedNightmareGrimm)
            {
                return "Two fates stand before you. Will you complete the ritual, or will you end the nightmare forever?";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            if (PlayerData.instance.destroyedNightmareLantern)
            {
                friendshipShield.ApplyHook();
            }
            else
            {
                On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += BuffGrimmchild;
            }
        }

        public override void Unequip()
        {
            base.Unequip();

            friendshipShield.RemoveHook();
            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter -= BuffGrimmchild;
        }

        #region Nightmare
        /// <summary>
        /// Nightmare increases the damage dealt by Grimmchild
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void BuffGrimmchild(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntOperator self)
        {
            if (self.Fsm.Name.Equals("Attack") &&
                    self.Fsm.GameObject.name.Equals("Enemy Damager") &&
                    self.Fsm.GameObject.transform.parent.gameObject.name.Contains("Grimmball") &&
                    self.State.Name.Equals("Hit"))
            {
                int baseDamage = self.Fsm.GetFsmInt("Damage").Value;
                self.Fsm.GetFsmInt("Damage").Value += GetBonus(baseDamage);
                //SharedData.Log($"{ID} - Damage increased from {baseDamage} to {self.Fsm.GetFsmInt("Damage").Value}");
            }

            orig(self);
        }

        /// <summary>
        /// Nightmare is an Epic glyph worth 4 notches.
        /// Grimmchild is worth 2 notches, so Nightmare can triple the damage.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 2f;
        }
        #endregion

        /// <summary>
        /// Handles damage negation for the Friendship glyph, inspired by
        ///     Carefree Melody
        /// </summary>
        private FriendshipShield friendshipShield = new FriendshipShield();
    }
}