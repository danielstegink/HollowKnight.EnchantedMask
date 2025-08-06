using DanielSteginkUtils.Helpers.Abilities;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using System;

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

            jumpHelper = new JumpHelper(SharedData.modName, "Broken", GetExtraJumps());
            jumpHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            if (jumpHelper != null)
            {
                jumpHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private JumpHelper jumpHelper;

        /// <summary>
        /// The Broken glyph increases the number of jumps possible with Monarch Wings
        /// </summary>
        private int GetExtraJumps()
        {
            // Broken is an Uncommon glyph worth 2 notches

            // Jumping is kind of like dashing upwards
            // An extra jump means the player can "dash" twice per stretch, which is equivalent to a 50% reduction
            // Per Dashmaster, that would be worth 3 notches
            float extraJumpAsDashCooldown = 0.5f;
            float notchesPerJump = extraJumpAsDashCooldown / NotchCosts.GetDashCooldownPerNotch();

            // However, it only applies to upward "dashes", reducing it by about 1/3 its value to about 1 notch
            notchesPerJump /= 3f;
            //SharedData.Log($"{ID} - Notches per jump: {notchesPerJump}");

            // So we should give 2 jumps for 2 notches
            int extraJumps = (int)Math.Max(1, 2 / notchesPerJump);
            //SharedData.Log($"{ID} - Extra jumps: {extraJumps}");

            return extraJumps;
        }
    }
}