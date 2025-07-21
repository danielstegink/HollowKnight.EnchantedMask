using Modding;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Shroom : Glyph
    {
        public override string ID => "Shroom";
        public override string Name => "Glyph of Spores";
        public override Tiers Tier => Tiers.Common;
        public override string Description => "The symbol of Hallownest's most mysterious fungi.\n\n" +
                                                "Increases the bearer's affinity with the Mushroom Clan.";

        public override bool Unlocked()
        {
            return PlayerData.instance.mrMushroomState == 8;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.lurienDefeated ||
                !PlayerData.instance.monomonDefeated ||
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (PlayerData.instance.mrMushroomState < 8)
            {
                return "An elusive fungus traverses the kingdom.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            ModHooks.ObjectPoolSpawnHook += BuffShroom;
        }

        public override void Unequip()
        {
            base.Unequip();

            ModHooks.ObjectPoolSpawnHook -= BuffShroom;
        }

        /// <summary>
        /// Stores the original damage interval of the spore cloud
        /// </summary>
        private float sporeDamage = -1f;

        /// <summary>
        /// The Shroom glyph increases the damage dealt by Spore Shroom
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffShroom(GameObject gameObject)
        {
            if (gameObject.name.Equals("Knight Spore Cloud(Clone)") &&
                PlayerData.instance.equippedCharm_17)
            {
                StoreOriginalDamage(gameObject);

                gameObject.GetComponent<DamageEffectTicker>().damageInterval = sporeDamage * GetModifier();
                //SharedData.Log($"{ID} - Damage interval {gameObject.GetComponent<DamageEffectTicker>().damageInterval}");
            }

            return gameObject;
        }

        /// <summary>
        /// Stores the original damage for the spore cloud so that 
        /// we can safely upgrade it only once
        /// </summary>
        /// <param name="gameObject"></param>
        private void StoreOriginalDamage(GameObject gameObject)
        {
            if (sporeDamage < 0)
            {
                sporeDamage = gameObject.GetComponent<DamageEffectTicker>().damageInterval;
            }
        }

        /// <summary>
        /// As a Common glyph, Shroom is worth 1 notch.
        /// Spore Shroom is only worth 1 notch normally, so we
        ///     can increase its damage rate by 100%.
        /// </summary>
        internal override float GetModifier()
        {
            return 0.5f;
        }
    }
}