using EnchantedMask.Settings;
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
                                                "Increases the bearer's affinity with Weavers.";

        public override bool Unlocked()
        {
            return PlayerData.instance.hegemolDefeated;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hegemolDefeated)
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
        /// The Beast glyph is Uncommon, so its worth 2 notches. 
        /// Weaversong is a 1 notch charm, so we can justify
        ///     tripling the number of weaverlings.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private GameObject SpawnWeaverlings(GameObject gameObject)
        {
            if (gameObject.name.Contains("Weaverling") &&
                PlayerData.instance.equippedCharm_39)
            {
                _ = GameObject.Instantiate(gameObject,
                    new Vector3(HeroController.instance.transform.GetPositionX(),
                    HeroController.instance.transform.GetPositionY()),
                    Quaternion.identity);
                _ = GameObject.Instantiate(gameObject,
                    new Vector3(HeroController.instance.transform.GetPositionX(),
                    HeroController.instance.transform.GetPositionY()),
                    Quaternion.identity);
            }

            //SharedData.Log($"{ID} - Extra weaverlings spawned");
            return gameObject;
        }
    }
}
