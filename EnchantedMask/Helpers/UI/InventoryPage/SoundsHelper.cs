using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;

namespace EnchantedMask.Helpers.UI.InventoryPage
{
    public static class SoundsHelper
    {
        /// <summary>
        /// Gets the sound for equipping a glyph
        /// </summary>
        /// <returns></returns>
        public static AudioClip GetEquipSound()
        {
            GameObject charmsPage = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;
            PlayMakerFSM charmFsm = charmsPage.LocateMyFSM("UI Charms");
            FsmState tweenState = charmFsm.GetValidState("Tween Up");
            AudioPlayerOneShotSingle soundAction = tweenState.GetAction<AudioPlayerOneShotSingle>(1);
            return (AudioClip)soundAction.audioClip.Value;
        }

        /// <summary>
        /// Gets the sound for removing a glyph
        /// </summary>
        /// <returns></returns>
        public static AudioClip GetUnequipSound()
        {
            GameObject charmsPage = GameCameras.instance.hudCamera.transform.Find("Inventory").Find("Charms").gameObject;
            PlayMakerFSM charmFsm = charmsPage.LocateMyFSM("UI Charms");
            AudioPlayerOneShotSingle soundAction = charmFsm.GetAction<AudioPlayerOneShotSingle>("Set", 2);
            return (AudioClip)soundAction.audioClip.Value;
        }
    }
}
