using Modding;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Beast : Glyph
    {
        public override string ID => "Beast";
        public override string Name => "Glyph of the Beast";
        public override Tiers Tier => Tiers.Uncommon;
        public override string Description => "The symbol of the ruler of the Weaver tribe.\n\n" +
                                                "Increases the bearer's affinity with the Weavers.";

        public override bool Unlocked()
        {
            return PlayerData.instance.GetBool("hegemolDefeated");
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.GetBool("hasDreamNail"))
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.GetBool("hegemolDefeated"))
            {
                return "The Beast sleeps in the depths of her Den.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            ModHooks.ObjectPoolSpawnHook += SpawnWeaverlings;
        }

        public override void Unequip()
        {
            base.Unequip();

            ModHooks.ObjectPoolSpawnHook -= SpawnWeaverlings;
        }

        /// <summary>
        /// The Beast glyph increases the number of weaverlings produced by Weaversong
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnWeaverlings(GameObject gameObject)
        {
            if (gameObject.name.Contains("Weaverling"))
            {
                // Beast is an Uncommon glyph, so its worth 2 notches
                // Weaversong is a 2 notch charm, so we can justify doubling the number of weaverlings
                _ = GameObject.Instantiate(gameObject,
                    new Vector3(HeroController.instance.transform.GetPositionX(),
                    HeroController.instance.transform.GetPositionY()),
                    Quaternion.identity);
                //SharedData.Log($"{ID} - Extra weaverlings spawned");
            }

            return gameObject;
        }
    }
}
