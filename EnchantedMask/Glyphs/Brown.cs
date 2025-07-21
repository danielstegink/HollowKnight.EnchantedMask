using Modding;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Brown : Glyph
    {
        public override string ID => "Brown";
        public override string Name => "Glyph of Honour";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of a loyal knight.\n\n" +
                                                "Increases the bearer's affinity with heroic odours.";

        public override bool Unlocked()
        {
            return PlayerData.instance.defeatedDungDefender;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.defeatedDungDefender)
            {
                return "A great knight sleeps surrounded by filth.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            ModHooks.ObjectPoolSpawnHook += BuffDungCloud;
        }

        public override void Unequip()
        {
            base.Unequip();

            dungDamage = -1f;
            ModHooks.ObjectPoolSpawnHook -= BuffDungCloud;
        }

        /// <summary>
        /// Stores the original damage rate of the dung cloud
        /// </summary>
        private float dungDamage = -1f;

        /// <summary>
        /// The Brown glyph increases the damage rate of Defender's Crest.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffDungCloud(GameObject gameObject)
        {
            if (gameObject.name.Equals("Knight Dung Trail(Clone)") &&
                PlayerData.instance.equippedCharm_10)
            {
                StoreOriginalDamage(gameObject);

                gameObject.GetComponent<DamageEffectTicker>().damageInterval = dungDamage * GetModifier();
                //SharedData.Log($"{ID} - Damage interval set to {gameObject.GetComponent<DamageEffectTicker>().damageInterval}");
            }

            return gameObject;
        }

        /// <summary>
        /// Stores the original damage rate for the dung cloud so that 
        /// we can safely upgrade it only once
        /// </summary>
        /// <param name="gameObject"></param>
        private void StoreOriginalDamage(GameObject gameObject)
        {
            if (dungDamage < 0)
            {
                dungDamage = gameObject.GetComponent<DamageEffectTicker>().damageInterval;
            }
        }

        /// <summary>
        /// As an Uncommon glyph, Brown is worth 2 notches.
        /// Defender's Crest is only worth 1 notch normally, so we
        ///     can increase its damage rate by 200%.
        /// Except Defender's Crest uses DOT, so it stacks easily.
        /// </summary>
        internal override float GetModifier()
        {
            return 0.3f;
        }
    }
}
