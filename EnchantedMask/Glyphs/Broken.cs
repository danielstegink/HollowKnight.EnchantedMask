using EnchantedMask.Settings;
using GlobalEnums;
using System;
using System.Linq;

namespace EnchantedMask.Glyphs
{
    public class Broken : Glyph
    {
        public override string ID => "Broken";
        public override string Name => "Broken Glyph";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The shattered symbol of a great warrior.\n\n" +
                                                "Increases the bearer's affinity with Monarch Wings.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedInfectedKnight;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.killedInfectedKnight)
            {
                return "A fallen comrade rests in caves below the kingdom.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.CanDoubleJump += AllowExtraJump;
            On.HeroController.DoDoubleJump += DoExtraJump;
            On.HeroController.BackOnGround += ResetJump;
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.CanDoubleJump -= AllowExtraJump;
            On.HeroController.DoDoubleJump -= DoExtraJump;
            On.HeroController.BackOnGround -= ResetJump;
        }

        /// <summary>
        /// Tracks if the 3rd jump is available
        /// </summary>
        private bool canExtraJump = true;

        /// <summary>
        /// The Broken glyph makes it so the bearer can jump 
        /// thrice with Monarch Wings instead of twice
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool AllowExtraJump(On.HeroController.orig_CanDoubleJump orig, HeroController self)
        {
            // If able to double-jump, do that first
            if (orig(self))
            {
                return true;
            }

            // Double jump has a number of factors it checks to see if the player can double-jump
            // The only one we really overwrite is whether they've double-jumped
            ActorStates[] landedStates = new ActorStates[] { ActorStates.no_input, ActorStates.hard_landing, ActorStates.dash_landing };
            bool inAir = !self.inAcid && 
                            !self.cState.dashing &&
                            !self.cState.wallSliding &&
                            !self.cState.backDashing &&
                            !self.cState.attacking &&
                            !self.cState.bouncing &&
                            !self.cState.shroomBouncing &&
                            !self.cState.onGround;
            return PlayerData.instance.hasDoubleJump &&
                    !self.controlReqlinquished &&
                    !landedStates.Contains(self.hero_state) &&
                    inAir &&
                    canExtraJump;
        }

        /// <summary>
        /// We need to add a special case to the double jump so it supports 
        /// the triple jump
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        public void DoExtraJump(On.HeroController.orig_DoDoubleJump orig, HeroController self)
        {
            // If we are not double-jumping, we need to reset the wings for graphics purposes
            // and note that we've used our extra jump
            bool isDoubleJumping = !SharedData.GetField<HeroController, bool>(self, "doubleJumped");
            if (!isDoubleJumping)
            {
                self.dJumpWingsPrefab.SetActive(false);
                self.dJumpFlashPrefab.SetActive(false);
                canExtraJump = false;
            }

            orig(self);
        }

        /// <summary>
        /// Once we've landed on the ground, triple-jump resets
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ResetJump(On.HeroController.orig_BackOnGround orig, HeroController self)
        {
            canExtraJump = true;
            orig(self);
        }
    }
}