using DanielSteginkUtils.Helpers.Abilities;
using DanielSteginkUtils.Utilities;
using EnchantedMask.Settings;
using GlobalEnums;
using Modding;

namespace EnchantedMask.Glyphs
{
    public class Abyss : Glyph
    {
        public override string ID => "Abyss";
        public override string Name => "Glyph of the Abyss";
        public override Tiers Tier => Tiers.Legendary;
        public override string Description => "The symbol of the kingdom's depths.\n\n" +
                                                "Allows the bearer to dash from anywhere, and to use Shade Cloak more often.";

        public override bool Unlocked()
        {
            return ClearedTemple(out _);
        }

        public override string GetClue()
        {
            if (!ClearedTemple(out string clue))
            {
                return clue;
            }

            return base.GetClue();
        }

        /// <summary>
        /// Checks if the player has cleared the Abyssal Temple in Pale Court
        /// </summary>
        /// <param name="clue"></param>
        /// <returns></returns>
        private bool ClearedTemple(out string clue)
        {
            if (SharedData.paleCourtMod == null)
            {
                clue = "You do not have Pale Court installed.";
                return false;
            }

            object saveSettings = ClassIntegrations.GetProperty<IMod, object>(SharedData.paleCourtMod, "SaveSettings");
            bool[] gotCharms = ClassIntegrations.GetField<object, bool[]>(saveSettings, "gotCharms");
            if (!gotCharms[3])
            {
                clue = "The flower of a forgotten temple lies steeped in darkness.";
                return false;
            }

            clue = "";
            return true;
        }

        public override void Equip()
        {
            base.Equip();

            On.HeroController.CanDash += CanDash;
            if (dashHelper == null)
            {
                dashHelper = new DashHelper(1, 0.5f);
            }

            dashHelper.Start();
        }

        public override void Unequip()
        {
            base.Unequip();

            On.HeroController.CanDash -= CanDash;
            if (dashHelper != null)
            {
                dashHelper.Stop();
            }
        }

        /// <summary>
        /// Utils helper
        /// </summary>
        private DashHelper dashHelper;

        /// <summary>
        /// Removes restrictions on dashing while in mid-air
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        private bool CanDash(On.HeroController.orig_CanDash orig, HeroController self)
        {
            if (self.hero_state != ActorStates.no_input &&
                self.hero_state != ActorStates.hard_landing)
            {
                if (!self.cState.dashing &&
                    !self.cState.backDashing &&
                    !self.cState.preventDash &&
                    !self.cState.hazardDeath)
                {
                    float dashCooldownTimer = ClassIntegrations.GetField<HeroController, float>(self, "dashCooldownTimer");
                    float attack_time = ClassIntegrations.GetField<HeroController, float>(self, "attack_time");
                    if (dashCooldownTimer <= 0f &&
                        (!self.cState.attacking || attack_time <= self.ATTACK_RECOVERY_TIME) &&
                        PlayerData.instance.GetBool("canDash"))
                    {
                        return true;
                    }
                }
            }

            return orig(self);
        }
    }
}