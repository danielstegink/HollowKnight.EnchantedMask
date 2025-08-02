# Enchanted Mask
Inspired by Lost Artifacts, this mod adds a whole new type of power to the game: Glyphs. 
As you traverse the game, you will unlock different glyphs which can be purchased from the Map Maker. 
You can only equip 1 glyph at a time, which will display as an icon in the top-left corner of your screen.

Each glyph has been assigned a tier based on how difficult it is to unlock. Higher-tier glyphs are more expensive and more powerful. 
 - Common glyphs are the easiest to unlock. They cost 500 geo and are as powerful as 1-notch charms.
 - Uncommon glyphs are a little harder, roughly mid-game difficulty. They cost 1000 geo and are as powerful as 2-notch charms.
 - Rare glyphs are challenging, generally acquired in late game. They cost 2000 geo and are as powerful as 3-notch charms.
 - Epic glyphs are end-game difficulty. They cost 4000 geo and are as powerful as 4-notch charms.
 - Legendary glyphs are considered the greatest challenges in the game. They cost 8000 geo and are as valuable as 5-notch charms.

The Glyphs page in your inventory will show all the glyphs you've acquired. 
The logic for equipping/unequipping a glyph is the same as that of charms. A sound effect will play each time.
Each one will display its name, tier, and a description. 
If a glyph hasn't been unlocked, the description will provide a clue regarding how to unlock it.

You can find a detailed list of each glyph, its abilities, and how to get it in the SPOILERS file.

Additionally, the mod is has a sub-menu in Debug Mod for adding/removing glyphs from one's inventory.

## Thanks
Thank you Hadanelith, BenjaLP211 and Dwarfwoot for testing and feedback.

## Patch Notes
1.1.1.0
- Fixed Love's icon to be the correct color
- Modified save logic so glyph effects don't persist after closing the game
- Reduced damage from Nail
- Reduced SOUL and damage gained from Childhood

1.1.0.0
- Added new glyphs for the expansions (Godhome is still a work in progress)
- Glyphs page now plays sound effects when glyphs are equipped/unequipped
- Glyphs properly reset between saves
- Balance patches and documentation changes for various glyphs
	- Exploration requirements now require getting a map of every region
	- Childhood deals more damage with Elegy beams and doesn't require that Grubberfly's Elegy be equipped
	- Hunter deals more damage
	- Love has been upgraded from Common to Uncommon, giving 2 notches instead of 1
	- Honour doesn't make dung clouds as big, but it should now be compatible with Pale Court
	- Crystal increases CDash damage by half of the player's nail damage instead of by 50%
	- Dream's effect has been replaced with Light's old effect of making Dream Nail do damage using Essence.
	- Broken gives 2 extra jumps instead of 1
	- Nail deals more damage
	- Hornet uses new logic to mimic base-game I-Frames
	- Life has a reduced chance of giving Lifeblood masks
	- Fluke deals more damage
	- Watcher deals more damage
	- Teacher spawns lumaflies on the nearest enemy instead of the player
	- Beast doesn't require that Weaversong be equipped
	- Sacrifice gives immunity for less time and uses new I-Frame logic
	- Warrior uses simpler logic for finding nearest enemy
	- King uses new I-Frame logic
	- Wisdom slows time more
	- Mourning deals more damage
	- Darkness doesn't require that Sharp Shadow be equipped
	- Light has a new effect: basic nail attacks destroy most of the Radiance's attacks upon contact
	- Spores doesn't require that Spore Shroom be equipped

1.0.1.0
- Rebalanced Beast to double Weaverlings instead of tripling them
- Fixed Dream to stop giving Essence when unequipped
- Rebalanced Dream to give Essence less often
- Modified Hornet to check if the player can even take damage, and to give 1 second of immunity when triggered
- Rebalanced Radiant to deal more damage
- Modified Royal to perform the same "Can Take Damage" check as Hornet
- Modified Snail and Teacher to check if the glyph is equipped, rather than track active status via variable
- Modified clues to be more descriptive while keeping some mystery
