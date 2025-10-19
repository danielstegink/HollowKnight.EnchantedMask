using EnchantedMask.Glyphs;
using EnchantedMask.Settings;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantedMask.Helpers.UI.InventoryPage
{
    /// <summary>
    /// Handles the Inventory UI for the glyphs
    /// </summary>
    public class GlyphsPage
    {
        public static readonly GlyphsPage instance = new GlyphsPage();

        #region Variables
        /// <summary>
        /// FSM for the glyphs page
        /// </summary>
        private PlayMakerFSM pageFsm;

        /// <summary>
        /// Used to display the glyph's name
        /// </summary>
        private GameObject nameBox;

        /// <summary>
        /// Used to display the glyph's rarity
        /// </summary>
        private GameObject tierBox;

        /// <summary>
        /// Used to display the glyph's description or clues on how to
        /// unlock it
        /// </summary>
        private GameObject descriptionBox;

        /// <summary>
        /// Stores the slots holding the glyphs
        /// </summary>
        private List<GlyphSlot>[] slots;

        /// <summary>
        /// Number of rows in the slots grid
        /// </summary>
        private int slotRows = -1;

        /// <summary>
        /// Max number of columns in each row of the slots grid
        /// </summary>
        private int slotCols = -1;

        /// <summary>
        /// Default sprite for when a glyph hasn't been bought yet
        /// </summary>
        private Sprite hiddenSprite = null;

        /// <summary>
        /// Index of the left arrow
        /// </summary>
        private const int leftArrowIndex = -2;

        /// <summary>
        /// Index of the right arrow
        /// </summary>
        private const int rightArrowIndex = -3;

        /// <summary>
        /// Current row selected on the slot grid
        /// </summary>
        private int selectedRow = -1;

        /// <summary>
        /// Current column selected on the slot grid
        /// </summary>
        private int selectedColumn = -1;

        /// <summary>
        /// Sound bite played when a glyph is equipped
        /// </summary>
        private AudioClip equipSound;

        /// <summary>
        /// Sound bite played when a glyph is removed
        /// </summary>
        private AudioClip unequipSound;
        #endregion

        /// <summary>
        /// Creates all of the page's components and initializes them
        /// </summary>
        /// <param name="glyphsPage"></param>
        public void Build(GameObject glyphsPage)
        {
            try
            {
                // Set defaults
                selectedRow = -1;
                selectedColumn = -1;
                hiddenSprite = SpriteHelper.GetSprite("Hidden");
                equipSound = SoundsHelper.GetEquipSound();
                unequipSound = SoundsHelper.GetUnequipSound();

                // The icon, name, tier and description should all populate the right 1/3 of the screen
                // In order: Name, Tier, Description
                BuildName(glyphsPage);
                BuildTier(glyphsPage);
                BuildDescription(glyphsPage);

                // Slots should occupy the left 2/3 of the screen in a roughly square array
                BuildSlots(glyphsPage);

                // Customize the FSM
                CustomizeFsm(glyphsPage);

                // Call Update to set everything according to the save data
                glyphsPage.SetActive(false);
                UpdatePage();
            }
            catch (Exception e)
            {
                SharedData.Log($"Error while building glyphs page: {e}");
            }
        }

        #region Build Page
        /// <summary>
        /// Initializes the Name box
        /// </summary>
        /// <param name="glyphsPage"></param>
        private void BuildName(GameObject glyphsPage)
        {
            // 0, 0 occupies the center of the top of the screen
            // Entire page is approximately 20x10 in size
            // So the corners are (-10, 0), (10, 0), (10, -10), and (-10, -10)

            nameBox = UnityEngine.Object.Instantiate(GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Charms/Text Name").gameObject);
            nameBox.name = "Glyph Name";
            nameBox.transform.SetParent(glyphsPage.transform);
            nameBox.transform.position = new Vector3(9, -2, 0);
            nameBox.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            nameBox.GetComponent<TextMeshPro>().text = "";
        }

        /// <summary>
        /// Initializes the Tier box
        /// </summary>
        /// <param name="glyphsPage"></param>
        private void BuildTier(GameObject glyphsPage)
        {
            tierBox = UnityEngine.Object.Instantiate(GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Charms/Text Desc").gameObject);
            tierBox.name = "Glyph Tier";
            tierBox.transform.SetParent(glyphsPage.transform);
            tierBox.transform.position = new Vector3(9f, -2.25f, 0);
            tierBox.GetComponent<TextMeshPro>().text = "";
            tierBox.GetComponent<TextContainer>().rect = new Rect(0, 0, 7, 10); // Unclear what the x and y do? no visible offset
            tierBox.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;
        }

        /// <summary>
        /// Initializes the Description box
        /// </summary>
        /// <param name="glyphsPage"></param>
        private void BuildDescription(GameObject glyphsPage)
        {
            descriptionBox = UnityEngine.Object.Instantiate(GameObject.Find("_GameCameras").transform.Find("HudCamera/Inventory/Charms/Text Desc").gameObject);
            descriptionBox.name = "Glyph Description";
            descriptionBox.transform.SetParent(glyphsPage.transform);
            descriptionBox.transform.position = new Vector3(9f, -3f, 0);
            descriptionBox.GetComponent<TextMeshPro>().text = "";
            descriptionBox.GetComponent<TextContainer>().rect = new Rect(0, 0, 7, 10);
        }

        /// <summary>
        /// Populates the slots holding the glyphs
        /// </summary>
        /// <param name="glyphsPage"></param>
        private void BuildSlots(GameObject glyphsPage)
        {
            // We want the slots sorted into a grid roughly 3:4 in scale
            // So if 3x * 4x >= totalGlyphs, then x is the square root of totalGlyphs divided by 12
            double glyphsDivided = (double)SharedData.glyphs.Count / 12;
            //SharedData.Log($"{SharedData.glyphs.Count} glyphs -> x/12 = {glyphsDivided}");
            double squareRoot = Math.Sqrt(glyphsDivided);
            //SharedData.Log($"Sqrt({glyphsDivided}) = {squareRoot}");
            slotCols = (int)Math.Ceiling(squareRoot * 4);
            slotRows = (int)Math.Ceiling((double)SharedData.glyphs.Count / slotCols);
            SharedData.Log($"{SharedData.glyphs.Count} glyphs -> {slotRows} rows by {slotCols} columns");
            slots = new List<GlyphSlot>[slotRows];
            for (int i = 0; i < slotRows; i++)
            {
                slots[i] = new List<GlyphSlot>();
            }

            Spacing spacing = new Spacing(slotCols);
            int rowIndex = 0; 
            int columnIndex = 0;
            for (int i = 0; i < SharedData.glyphs.Count; i++)
            {
                // When we've reached the maximum slots for a given row, move to the next one
                if (columnIndex >= slotCols)
                {
                    rowIndex++;
                    columnIndex = 0;
                }

                // Define the game object holding the slot in place
                GameObject slotObject = new GameObject($"Glyph Slot {i} - Object");
                slotObject.transform.SetParent(glyphsPage.transform);
                slotObject.layer = glyphsPage.layer;
                slotObject.transform.position = new Vector3(spacing.X(columnIndex), spacing.Y(rowIndex), -3f);

                // Helps the cursor know where to stop
                BoxCollider2D bc2d = slotObject.AddComponent<BoxCollider2D>();
                bc2d.offset = new Vector2(0, 0);
                bc2d.size = spacing.Scale();
                slotObject.SetActive(true);

                // Defines the size and location of the display icon
                GameObject icon = new GameObject($"Glyph Slot {i} - Icon");
                icon.transform.SetParent(slotObject.transform);
                icon.transform.localPosition = new Vector3(0, 0, 0);
                icon.transform.localScale = spacing.Scale();
                icon.layer = glyphsPage.layer;

                // Decides what the icon looks like
                SpriteRenderer spriteRenderer = icon.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = SharedData.glyphs[i].GetIcon();
                spriteRenderer.sortingLayerID = 629535577;
                spriteRenderer.sortingLayerName = "HUD";

                // Add the background halo to show if a slot is equipped
                GameObject equippedHalo = new GameObject($"Glyph Slot {i} - Halo");
                equippedHalo.transform.SetParent(icon.transform);
                equippedHalo.layer = icon.layer;
                equippedHalo.transform.localPosition = new Vector3(0, 0, 5);
                equippedHalo.transform.localScale = 1.5f * spacing.Scale();

                // Define the halo's appearance
                SpriteRenderer spriteRenderer2 = equippedHalo.AddComponent<SpriteRenderer>();
                spriteRenderer2.sprite = GetHaloSprite(SharedData.glyphs[i].Tier);
                spriteRenderer2.sortingLayerID = 629535577;
                spriteRenderer2.sortingLayerName = "HUD";
                equippedHalo.SetActive(false);

                slots[rowIndex].Add(new GlyphSlot(slotObject, icon, spriteRenderer, equippedHalo));
                columnIndex++;
            }
        }

        private Sprite GetHaloSprite(Glyph.Tiers tier)
        {
            switch (tier)
            {
                case Glyph.Tiers.Common:
                    return SpriteHelper.GetSprite("WhiteHalo");
                case Glyph.Tiers.Uncommon:
                    return SpriteHelper.GetSprite("GreenHalo");
                    case Glyph.Tiers.Rare:
                    return SpriteHelper.GetSprite("BlueHalo");
                case Glyph.Tiers.Epic:
                    return SpriteHelper.GetSprite("PurpleHalo");
                case Glyph.Tiers.Legendary:
                    return SpriteHelper.GetSprite("YellowHalo");
                default:
                    return SpriteHelper.GetSprite("WhiteHalo");
            }
        }

        /// <summary>
        /// Modifies the page's default FSM to fit the new UI
        /// </summary>
        /// <param name="glyphsPage"></param>
        private void CustomizeFsm(GameObject glyphsPage)
        {
            pageFsm = InitializeFsm(glyphsPage);

            AddKeyPressHandlers();

            AddSlotStates();

            AddKeyPressEvents();

            AddArrows();

            //LogFsm(fsm);
            //fsm.Fsm.StateChanged += LogStateChanges;
        }

        /// <summary>
        /// Updates the page
        /// </summary>
        public void UpdatePage()
        {
            for (int rowIndex = 0; rowIndex < slots.Length; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < slots[rowIndex].Count; columnIndex++)
                {
                    GlyphSlot slot = slots[rowIndex][columnIndex];
                    Glyphs.Glyph glyph = SharedData.glyphs[GetGlyphIndex(rowIndex, columnIndex)];

                    slot.spriteRenderer.sprite = glyph.Bought() ? glyph.GetIcon() : hiddenSprite;
                    slot.halo.SetActive(glyph.IsEquipped());
                }
            }
        }
        #endregion

        #region Customize FSM
        /// <summary>
        /// Creates the page FSM from the default Empty UI
        /// </summary>
        /// <param name="glyphsPage"></param>
        /// <returns></returns>
        private PlayMakerFSM InitializeFsm(GameObject glyphsPage)
        {
            //SharedData.Log("Creating FSM");
            PlayMakerFSM fsm = glyphsPage.LocateMyFSM("Empty UI");

            // Remove default commands for left and right arrow
            fsm.GetState("L Arrow").RemoveTransitionsTo("R Arrow");
            fsm.GetState("R Arrow").RemoveTransitionsTo("L Arrow");

            // Set up initial state
            FsmState initState = fsm.GetState("Init Heart Piece");
            initState.Name = "Init Glyphs";
            initState.RemoveTransitionsTo("L Arrow");
            initState.AddLastAction(new Lambda(() =>
            {
                fsm.SendEvent("FINISHED");
            }));

            return fsm;
        }

        /// <summary>
        /// Adds handlers for the arrow keys and equipping a glyph
        /// </summary>
        /// <param name="fsm"></param>
        private void AddKeyPressHandlers()
        {
            //SharedData.Log("Adding key bindings");
            pageFsm.AddState(new FsmState(pageFsm.Fsm)
            {
                Name = "Up Press",
                Actions = [new Lambda(() => HandleUpPress(pageFsm))]
            });

            pageFsm.AddState(new FsmState(pageFsm.Fsm)
            {
                Name = "Down Press",
                Actions = [new Lambda(() => HandleDownPress(pageFsm))]
            });

            pageFsm.AddState(new FsmState(pageFsm.Fsm)
            {
                Name = "Left Press",
                Actions = [new Lambda(() => HandleLeftPress(pageFsm))]
            });

            pageFsm.AddState(new FsmState(pageFsm.Fsm)
            {
                Name = "Right Press",
                Actions = [new Lambda(() => HandleRightPress(pageFsm))]
            });

            pageFsm.AddState(new FsmState(pageFsm.Fsm)
            {
                Name = "Equip",
                Actions = [new Lambda(() => HandleEquip(pageFsm))]
            });
        }

        /// <summary>
        /// Adds states to the FSM for each glyph slot
        /// </summary>
        /// <param name="fsm"></param>
        private void AddSlotStates()
        {
            //SharedData.Log("Adding slots to FSM");
            FsmState rArrow = pageFsm.GetState("R Arrow");
            PlayMakerFSM updateCursor = pageFsm.gameObject.LocateMyFSM("Update Cursor");

            for (int rowIndex = 0; rowIndex < slots.Length; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < slots[rowIndex].Count; columnIndex++)
                {
                    string stateName = GetSlotState(rowIndex, columnIndex);
                    pageFsm.AddState(new FsmState(pageFsm.Fsm)
                    {
                        Name = stateName,
                        Actions =
                        [
                            new Lambda(() => SelectGlyph(stateName)),
                        ]
                    });

                    rArrow.AddTransition(GetSlotEvent(rowIndex, columnIndex), stateName);
                }
            }

            // Default screen to start on first glyph
            FsmState initState = pageFsm.GetState("Init Glyphs");
            initState.AddTransition("FINISHED", GetSlotState(0, 0));
        }

        /// <summary>
        /// Adds FSM events to link key presses to their handlers
        /// </summary>
        /// <param name="fsm"></param>
        private void AddKeyPressEvents()
        {
            //SharedData.Log($"Adding transitions");

            // Start with the arrow keys
            string[] directions = new string[] { "Up", "Down", "Left", "Right" };
            foreach (string direction in directions)
            {
                string stateName = $"{direction} Press";
                FsmState directionState = pageFsm.GetState(stateName);

                for (int rowIndex = 0; rowIndex < slots.Length; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < slots[rowIndex].Count; columnIndex++)
                    {
                        // Add transition from the direction state to the slot state
                        string eventName = GetSlotEvent(rowIndex, columnIndex);
                        string slotStateName = GetSlotState(rowIndex, columnIndex);
                        directionState.AddTransition(eventName, slotStateName);

                        // Add transition from slot state to direction state
                        FsmState slotState = pageFsm.GetState(slotStateName);
                        slotState.AddTransition($"UI {direction.ToUpper()}", stateName);
                    }
                }

                // Add transitions for getting to page arrows
                directionState.AddTransition("OUT LEFT", "L Arrow");
                directionState.AddTransition("OUT RIGHT", "R Arrow");
            }

            // Then do the equip glyph key
            FsmState equipState = pageFsm.GetState("Equip");
            for (int rowIndex = 0; rowIndex < slots.Length; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < slots[rowIndex].Count; columnIndex++)
                {
                    // Add transition from the equip state to the slot state
                    string eventName = GetSlotEvent(rowIndex, columnIndex);
                    string slotStateName = GetSlotState(rowIndex, columnIndex);
                    equipState.AddTransition(eventName, slotStateName);

                    // Add transition from slot state to equip state
                    FsmState slotState = pageFsm.GetState(slotStateName);
                    slotState.AddTransition("UI CONFIRM", "Equip");
                }
            }
        }

        /// <summary>
        /// Adds FSM transitions for the page arrows
        /// </summary>
        /// <param name="fsm"></param>
        private void AddArrows()
        {
            //SharedData.Log("Adding arrow events");
            FsmState lArrow = pageFsm.GetState("L Arrow");
            lArrow.AddLastAction(new Lambda(() => SetSelectedGlyph(lArrow.Name)));
            lArrow.AddTransition("UI RIGHT", GetSlotState(0, 0));

            FsmState rArrow = pageFsm.GetState("R Arrow");
            rArrow.AddLastAction(new Lambda(() => SetSelectedGlyph(rArrow.Name)));
            rArrow.AddTransition("UI LEFT", "Left Press");
        }
        #endregion

        #region Key Press Handlers
        /// <summary>
        /// Handles when the up key is pressed
        /// </summary>
        /// <param name="fsm"></param>
        private void HandleUpPress(PlayMakerFSM fsm)
        {
            //SharedData.Log($"Up Pressed: {selectedRow}, {selectedColumn}");

            // By default, go to row above
            string eventName = GetSlotEvent(selectedRow - 1, selectedColumn);

            // If on first row, go to last row
            if (selectedRow == 0)
            {
                int lastRow = slots.Length - 1;

                // If there isn't a matching column on the last row,
                //   default to the last slot of that row
                int lastColumn = slots[lastRow].Count - 1;
                eventName = GetSlotEvent(lastRow, Math.Min(selectedColumn, lastColumn));
            }

            //SharedData.Log($"Sending event: {eventName}");
            fsm.SendEvent(eventName);
        }

        /// <summary>
        /// Handles when the down key is pressed
        /// </summary>
        /// <param name="fsm"></param>
        private void HandleDownPress(PlayMakerFSM fsm)
        {
            //SharedData.Log($"Down Pressed: {selectedRow}, {selectedColumn}");

            // By default, go to the first row
            string eventName = GetSlotEvent(0, selectedColumn);

            // If not on last row, go to the next row
            int lastRow = slots.Length - 1;
            if (selectedRow < lastRow)
            {
                // If there isn't a matching column on the next row,
                //   default to the last slot of that row
                int lastColumn = slots[selectedRow + 1].Count - 1;
                eventName = GetSlotEvent(selectedRow + 1, Math.Min(selectedColumn, lastColumn));
            }

            //SharedData.Log($"Sending event: {eventName}");
            fsm.SendEvent(eventName);
        }

        /// <summary>
        /// Handles when the left key is pressed
        /// </summary>
        /// <param name="fsm"></param>
        private void HandleLeftPress(PlayMakerFSM fsm)
        {
            //SharedData.Log($"Left Pressed: {selectedRow}, {selectedColumn}");

            // By default, go to rightmost slot of the top row
            int lastColumn = slots[0].Count - 1;
            string eventName = GetSlotEvent(0, lastColumn);

            // If on a slot, go to the left slot
            if (selectedRow >= 0)
            {
                // If on leftmost slot of a row, go to left arrow instead
                if (selectedColumn == 0)
                {
                    eventName = "OUT LEFT";
                }
                else
                {
                    eventName = GetSlotEvent(selectedRow, selectedColumn - 1);
                }
            }

            //SharedData.Log($"Sending event: {eventName}");
            fsm.SendEvent(eventName);
        }

        /// <summary>
        /// Handles when the right key is pressed
        /// </summary>
        /// <param name="fsm"></param>
        private void HandleRightPress(PlayMakerFSM fsm)
        {
            //SharedData.Log($"Right Pressed: {selectedRow}, {selectedColumn}");

            // By default, go to first slot of the top row
            string eventName = GetSlotEvent(0, 0);

            // If on a slot, go to the right slot
            if (selectedRow >= 0)
            {
                // If on rightmost slot of a row, go to right arrow instead
                int lastColumn = slots[selectedRow].Count - 1;
                if (selectedColumn == lastColumn)
                {
                    eventName = "OUT RIGHT";
                }
                else
                {
                    eventName = GetSlotEvent(selectedRow, selectedColumn + 1);
                }
            }

            //SharedData.Log($"Sending event: {eventName}");
            fsm.SendEvent(eventName);
        }

        /// <summary>
        /// Handles when the equip key is pressed
        /// </summary>
        /// <param name="fsm"></param>
        private void HandleEquip(PlayMakerFSM fsm)
        {
            //SharedData.Log($"Equip Pressed: {selectedRow}, {selectedColumn}");

            // If on a slot, equip the associated glyph
            if (selectedRow >= 0)
            {
                int glyphIndex = GetGlyphIndex(selectedRow, selectedColumn);
                Glyphs.Glyph glyph = SharedData.glyphs[glyphIndex];

                // Can only equip a glyph if its been bought
                if (glyph.Bought())
                {
                    // If glyph is already equipped, remove it instead
                    if (SharedData.saveSettings.EquippedGlyph.Equals(glyph.ID))
                    {
                        glyph.Unequip();
                        HeroController.instance.GetComponent<AudioSource>().PlayOneShot(unequipSound, 1f);
                    }
                    else
                    {
                        // Unequip the currently equipped glyph
                        string equippedId = SharedData.saveSettings.EquippedGlyph;
                        if (!string.IsNullOrWhiteSpace(equippedId))
                        {
                            Glyphs.Glyph oldGlyph = SharedData.glyphs.Where(x => x.ID.Equals(equippedId)).FirstOrDefault();
                            oldGlyph.Unequip();
                        }

                        glyph.Equip();
                        HeroController.instance.GetComponent<AudioSource>().PlayOneShot(equipSound, 1f);
                    }

                    //SharedData.Log("Updating page");
                    UpdatePage();
                }
            }

            string eventName = GetSlotEvent(selectedRow, selectedColumn);
            //SharedData.Log($"Sending event: {eventName}");
            fsm.SendEvent(eventName);
        }
        #endregion

        #region FSM Helpers
        /// <summary>
        /// Gets a Glyph from the Glyph list based on the grid coordinates of its slot
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int GetGlyphIndex(int rowIndex, int columnIndex)
        {
            //SharedData.Log($"Getting glyph index: {rowIndex}, {columnIndex}");
            // Each row (except the last one) will store X glyphs
            // The first glyph is index 0, so the last glyph of the first row is X-1,
            //  while the last glyph of the 2nd row is 2X-1
            int finalGlyphFromLastRow = (rowIndex * slotCols) - 1;
            //SharedData.Log($"Index of last glyph: {finalGlyphFromLastRow}");

            // Remember that column index starts from 0, so need to increment our final index by 1
            int glyphIndex = finalGlyphFromLastRow + columnIndex + 1;
            //SharedData.Log($"Glyph index: {glyphIndex}");
            return glyphIndex;
        }

        /// <summary>
        /// Gets the name of the FSM state of the given glyph slot
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private string GetSlotState(int rowIndex, int columnIndex)
        {
            return $"Glyph {rowIndex} {columnIndex}";
        }

        /// <summary>
        /// Handles the act of moving the cursor to a given glyph
        /// </summary>
        /// <param name="stateName"></param>
        private void SelectGlyph(string stateName)
        {
            //SharedData.Log($"Selecting glyph: {stateName}");
            string[] stateParts = stateName.Split(' '); // Glyph ROW COLUMN
            selectedRow = int.Parse(stateParts[1]);
            selectedColumn = int.Parse(stateParts[2]);

            PlayMakerFSM updateCursor = pageFsm.gameObject.LocateMyFSM("Update Cursor");
            updateCursor.FsmVariables.FindFsmGameObject("Item").Value = slots[selectedRow][selectedColumn].slotObject;
            updateCursor.SendEvent("UPDATE CURSOR");

            SetSelectedGlyph(stateName);
        }

        /// <summary>
        /// Gets the name of the event to select the given glyph slot
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private string GetSlotEvent(int rowIndex, int columnIndex)
        {
            return $"GLYPH_{rowIndex}_{columnIndex}";
        }

        /// <summary>
        /// Updates the page when a given glyph is selected
        /// </summary>
        /// <param name="stateName"></param>
        private void SetSelectedGlyph(string stateName)
        {
            // If selecting the left or right arrow, no glyph is selected
            if (!stateName.StartsWith("Glyph"))
            {
                nameBox.GetComponent<TextMeshPro>().text = "";
                tierBox.GetComponent<TextMeshPro>().text = "";
                descriptionBox.GetComponent<TextMeshPro>().text = "";

                selectedRow = -1;
                selectedColumn = leftArrowIndex;
                if (stateName.StartsWith("R"))
                {
                    selectedColumn = rightArrowIndex;
                }
            }
            else // Slot event
            {
                string[] stateParts = stateName.Split(' '); // Glyph ROW COLUMN
                selectedRow = int.Parse(stateParts[1]);
                selectedColumn = int.Parse(stateParts[2]);

                int glyphIndex = GetGlyphIndex(selectedRow, selectedColumn);
                Glyphs.Glyph glyph = SharedData.glyphs[glyphIndex];

                // If we have the glyph, display its info
                if (glyph.Bought())
                {
                    nameBox.GetComponent<TextMeshPro>().text = glyph.Name;
                    tierBox.GetComponent<TextMeshPro>().text = Glyphs.Glyph.GetTier(glyph.Tier);
                    tierBox.GetComponent<TextMeshPro>().color = GetColor(glyph.Tier);
                    descriptionBox.GetComponent<TextMeshPro>().text = glyph.Description;
                }
                else
                {
                    nameBox.GetComponent<TextMeshPro>().text = "???";
                    tierBox.GetComponent<TextMeshPro>().text = "???";
                    tierBox.GetComponent<TextMeshPro>().color = GetColor(Glyphs.Glyph.Tiers.Default);

                    string clue = new Glyph().GetClue();
                    if (!glyph.Unlocked())
                    {
                        clue = glyph.GetClue();
                    }
                    descriptionBox.GetComponent<TextMeshPro>().text = clue;
                }
            }
        }

        /// <summary>
        /// Gets the color for the Tier text
        /// </summary>
        /// <param name="tier"></param>
        /// <returns></returns>
        private UnityEngine.Color GetColor(Glyphs.Glyph.Tiers tier)
        {
            switch (tier)
            {
                case Glyphs.Glyph.Tiers.Common:
                    return new UnityEngine.Color(255, 255, 255); // White
                case Glyphs.Glyph.Tiers.Uncommon:
                    return new UnityEngine.Color(0, 255, 0); // Green
                case Glyphs.Glyph.Tiers.Rare:
                    return new UnityEngine.Color(0, 50, 255); // Blue
                case Glyphs.Glyph.Tiers.Epic:
                    return new UnityEngine.Color(255, 0, 255); // Purple
                case Glyphs.Glyph.Tiers.Legendary:
                    return new UnityEngine.Color(255, 255, 0); // Yellow
                default:
                    return new UnityEngine.Color(255, 255, 255); // White
            }
        }
        #endregion

        #region Logging
        private void LogFsm(PlayMakerFSM fsm)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("FSM Summary");
            sb.AppendLine($"Events: {string.Join(", ", fsm.FsmEvents.Select(x => x.Name))}");
            sb.AppendLine($"States: {string.Join(", ", fsm.FsmStates.Select(x => x.Name))}");

            List<string> transitions = new List<string>();
            foreach (FsmState state in fsm.FsmStates)
            {
                foreach (FsmTransition transition in state.Transitions)
                {
                    transitions.Add($"{state.Name} -({transition.EventName})-> {transition.ToState}");
                }
            }
            sb.Append($"Transitions: {string.Join("\n", transitions)}");

            SharedData.Log(sb.ToString());
        }

        private void LogStateChanges(FsmState state)
        {
            SharedData.Log($"FSM changing to {state.Name}");
        }
        #endregion
    }
}