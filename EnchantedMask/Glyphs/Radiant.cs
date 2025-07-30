using EnchantedMask.Settings;
using Satchel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnchantedMask.Glyphs
{
    public class Radiant : Glyph
    {
        public override string ID => "Radiant";
        public override string Name => "Glyph of Light";
        public override Tiers Tier => Tiers.Epic;
        public override string Description => "The symbol of the source of the Infection.\n\n" +
                                                "Imbues the bearer's nail with the power to cut through the Light.";

        public override bool Unlocked()
        {
            return PlayerData.instance.killedFinalBoss;
        }

        public override string GetClue()
        {
            if (!PlayerData.instance.hasDreamNail)
            {
                return "The power of a forgotten tribe waits in the graveyard of heroes.";
            }
            else if (!PlayerData.instance.lurienDefeated ||
                !PlayerData.instance.monomonDefeated ||
                !PlayerData.instance.hegemolDefeated)
            {
                return "The Dreamers' seal holds strong.";
            }
            else if (PlayerData.instance.royalCharmState != 4)
            {
                return "A dark truth sleeps beneath the shells of your kin.";
            }
            else if (!PlayerData.instance.killedFinalBoss)
            {
                return "A great light hides in a knight's dreams.";
            }

            return base.GetClue();
        }

        public override void Equip()
        {
            base.Equip();

            GetPrefabs();
            GameManager.instance.StartCoroutine(LightNail());
        }

        public override void Unequip()
        {
            base.Unequip();
        }

        #region Get Prefabs
        /// <summary>
        /// Stores dream particles for VFX purposes
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// Stores the Dream Nail sound effect
        /// </summary>
        private AudioClip soundFx;

        /// <summary>
        /// Gets the VFX and audio prefabs
        /// </summary>
        private void GetPrefabs()
        {
            // Get Dream particles VFX from preloaded Revek
            GameObject revek = SharedData.preloads["RestingGrounds_08"]["Ghost revek"];
            PlayMakerFSM fsm = revek.LocateMyFSM("Appear");
            prefab = fsm.FsmVariables.FindFsmGameObject("Idle Pt").Value;

            // Get the Dream Nail sound from the Dream Nail FSM
            GameObject knight = HeroController.instance.gameObject;
            fsm = knight.LocateMyFSM("Dream Nail");
            HutongGames.PlayMaker.FsmState state = fsm.GetValidState("Slash");
            HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle audioStep = state.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(6);
            soundFx = (AudioClip)audioStep.audioClip.Value;
        }
        #endregion

        /// <summary>
        /// The Radiant glyph gives regular nail attacks the ability to destroy attacks from the Radiance
        /// </summary>
        private IEnumerator LightNail()
        {
            while (IsEquipped())
            {
                // Get a list of all active colliders
                Collider2D[] colliders = UnityEngine.Component.FindObjectsOfType<Collider2D>()
                                                                .Where(x => x.enabled)
                                                                .ToArray();
                GameObject[] colliderObjects = colliders.Select(x => x.gameObject).ToArray();

                // Get a list of all nail attacks
                GameObject[] nailAttacks = colliderObjects.Where(x => SharedData.nailAttackNames.Contains(x.name))
                                                            .ToArray();
                //SharedData.Log($"{ID} - {nailAttacks.Length} active nail attacks found");

                // Get a list of all Radiant attacks
                GameObject[] radiantAttacks = colliderObjects.Where(x => IsRadiantAttack(x))
                                                                .ToArray();
                //SharedData.Log($"{ID} - {radiantAttacks.Length} active Radiance attacks found");

                // Go through each Radiant attack
                foreach (GameObject attack in radiantAttacks)
                {
                    Collider2D collider = attack.GetComponent<Collider2D>();

                    // Go through each nail attack
                    foreach (GameObject nailAttack in nailAttacks)
                    {
                        Collider2D other = nailAttack.GetComponent<Collider2D>();
                        //SharedData.Log($"{ID} - Comparing {attack.name} ({collider.bounds}) to {nailAttack.name} ({other.bounds})");

                        // If the 2 overlap, destroy the attack
                        if (Overlap(collider.bounds, other.bounds))
                        {
                            // Radiant Spikes have to be briefly disabled instead of destroyed
                            if (attack.name.StartsWith("Radiant Spike"))
                            {
                                //SharedData.Log($"{ID} - Disabling {attack.name}");
                                GameManager.instance.StartCoroutine(Destroy(attack, true));
                            }
                            else
                            {
                                //SharedData.Log($"{ID} - Destroying {attack.name}");
                                GameManager.instance.StartCoroutine(Destroy(attack));
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(0f);
        }

        /// <summary>
        /// Checks if a game object is a Radiance attack
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private bool IsRadiantAttack(GameObject gameObject)
        {
            // Due to the complexity, I've decided that Radiant Beams won't be affected
            if (gameObject.name.StartsWith("Radiant Nail") ||
                gameObject.name.StartsWith("Radiant Spike") ||
                gameObject.name.StartsWith("Radiant Orb"))
            {
                return true;
            }
            else
            {
                //SharedData.Log($"{ID} - Attack {gameObject.name} not recognized.");
                return false;
            }
        }

        /// <summary>
        /// Check if the bounds of two colliders overlap
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        private bool Overlap(Bounds b1, Bounds b2)
        {
            // Determine which one is "left" of the other
            Bounds left = b1.min.x <= b2.min.x ? b1 : b2;
            Bounds right = b1.min.x <= b2.min.x ? b2 : b1;

            // Confirm the left overlaps with the right on the horizontal scale
            if (left.max.x < right.min.x)
            {
                return false;
            }

            // Determine which one is "above" the other
            Bounds top = b1.min.y >= b2.min.y ? b1 : b2;
            Bounds bottom = b1.min.y >= b2.min.y ? b2 : b1;

            // Confirm the top overlaps with the bottom on the vertical scale
            if (bottom.max.y < top.min.y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Destroys the given object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="disable">Flag to only briefly disable the object</param>
        /// <returns></returns>
        private IEnumerator Destroy(GameObject gameObject, bool disable = false)
        {
            GameManager.instance.StartCoroutine(DisplayVFX(gameObject));
            gameObject.SetActive(false);

            // If we're only disabling the attack, re-enable it after 1 second
            if (disable)
            {
                yield return new WaitForSeconds(1f);
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Performs special effects to give the destruction of the Radiance's attacks more flair
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayVFX(GameObject gameObject)
        {
            // Create a copy of the dream particles prefab
            // Prefab is actually pretty small, so we'll make multiple
            List<GameObject> vfxList = new List<GameObject>();
            Vector3[] offsets = new Vector3[]
            {
                new Vector3(0.25f, 0.25f),
                new Vector3(0.25f, -0.25f),
                new Vector3(-0.25f, -0.25f),
                new Vector3(-0.25f, 0.25f),

                new Vector3(-0.5f, -0.5f),
                new Vector3(0f, -0.5f),
                new Vector3(0.5f, -0.5f),
                new Vector3(-0.5f, 0f),
                new Vector3(0f, 0f),
                new Vector3(0.5f, 0f),
                new Vector3(-0.5f, 0.5f),
                new Vector3(0f, 0.5f),
                new Vector3(0.5f, 0.5f),
            };
            foreach (Vector3 offset in offsets)
            {
                Vector3 position = gameObject.transform.position + new Vector3(0f, -1f) + offset;
                GameObject vfx = UnityEngine.GameObject.Instantiate(prefab, position, Quaternion.identity);

#pragma warning disable CS0618 // Per Lost Artifacts, we have to use the obsolete option below
                vfx.GetComponent<ParticleSystem>().enableEmission = true;
#pragma warning restore CS0618

                vfxList.Add(vfx);
            }

            // Play the Dream Nail sound effect
            HeroController.instance.GetComponent<AudioSource>().PlayOneShot(soundFx, 1f);

            // Wait before destroying the VFX
            yield return new WaitForSeconds(1.5f);
            foreach( GameObject vfx in vfxList)
            {
                UnityEngine.GameObject.Destroy(vfx);
            }
        }
    }
}
