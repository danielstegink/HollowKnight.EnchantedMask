using HutongGames.PlayMaker;
using Modding;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class False : Glyph
    {
        public override string ID => "False";
        public override string Name => "Glyph of Deceit";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of a lowly creature who stole power they could not control.\n\n" +
                                                "Increases the bearer's strength, allowing them to cripple great foes.";

        public override bool Unlocked()
        {
            return PlayerData.instance.falseKnightDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.falseKnightDefeated)
            {
                return "A thief hides among ruined highways, protected by stolen armor.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            ModHooks.OnEnableEnemyHook += ExtraStagger;
        }

        public override void Unequip()
        {
            base.Unequip();

            ModHooks.OnEnableEnemyHook -= ExtraStagger;
        }

        /// <summary>
        /// False reduces the number of hits required to stagger a boss.
        /// For 2 notches, Heavy Blow reduces stagger by 1 and increases knockback.
        /// So we can reduce stagger by 1 for 1 notch.
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="isAlreadyDead"></param>
        /// <returns></returns>
        private bool ExtraStagger(GameObject enemy, bool isAlreadyDead)
        {
            // If the enemy is already dead, no need to adjust the stagger rate
            if (isAlreadyDead)
            {
                return isAlreadyDead;
            }

            // Get the Stun or Stun Control FSM
            PlayMakerFSM stunFsm = enemy.LocateMyFSM("Stun");
            if (stunFsm == null)
            {
                stunFsm = enemy.LocateMyFSM("Stun Control");
                if (stunFsm == null)
                {
                    //SharedData.Log("This enemy cannot be staggered.");
                    return isAlreadyDead;
                }
            }

            // Get the Heavy Blow state of the FSM
            FsmState heavyBlowState = stunFsm.FsmStates.FirstOrDefault(x => x.Name.Equals("Heavy Blow"));
            if (heavyBlowState == default)
            {
                return isAlreadyDead;
            }

            // The FSM will have 2 values of note: Stun Combo and Stun Hit Max. Both need to be reduced
            stunFsm.FsmVariables.FindFsmInt("Stun Hit Max").Value--;
            stunFsm.FsmVariables.FindFsmInt("Stun Combo").Value--;

            return isAlreadyDead;
        }
    }
}