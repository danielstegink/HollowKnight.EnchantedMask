using EnchantedMask.Helpers;
using EnchantedMask.Settings;
using Modding;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Fluke : Glyph
    {
        public override string ID => "Fluke";
        public override string Name => "Glyph of Motherhood";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the Fluke's matriarch.\n\n" +
                                                "Increases the bearer's affinity with the Flukes.";

        public override bool Unlocked()
        {
            return PlayerData.instance.flukeMotherDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.flukeMotherDefeated)
            {
                return "A mother ferociously breeds in a cramped, damp space.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            dungFlukeHelper = new DungFlukeHelper(1 - GetModifier());
            dungFlukeHelper.Start();
            ModHooks.ObjectPoolSpawnHook += BuffFlukes;
        }

        public override void Unequip()
        {
            base.Unequip();

            if (dungFlukeHelper != null)
            {
                dungFlukeHelper.Stop();
            }
            ModHooks.ObjectPoolSpawnHook -= BuffFlukes;
        }

        /// <summary>
        /// Used for handling damage buff for Dung Flukes
        /// </summary>
        private DungFlukeHelper dungFlukeHelper;

        /// <summary>
        /// The Fluke glyph increases damage dealt by Flukenest.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject BuffFlukes(GameObject gameObject)
        {
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
                    int bonusDamage = GetBonus(baseDamage);

                    SharedData.SetField(fluke, "damage", baseDamage + bonusDamage);
                    //SharedData.Log($"{ID} - Damage increased by {bonusDamage}");
                }
            }

            return gameObject;
        }

        /// <summary>
        /// As an Uncommon glyph, Fluke is worth 2 notches.
        /// Per Shaman Stone, this would normally be worth a 30% increase.
        /// However, it only boosts 1 spell, so we can increase the boost.
        /// </summary>
        /// <returns></returns>
        internal override float GetModifier()
        {
            return 0.4f;
        }
    }
}
