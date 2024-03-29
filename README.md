
# Rosie
A [RogueLike](https://en.wikipedia.org/wiki/Roguelike) named after my dog, which consists of modified code from my RogueLike repository. This is a work in progress and will change  a lot over time. I have no plan or structure for this, I'm making it up as I go along and enjoying myself as I do it.

It's barely an alpha and has has the following features:

* Maze generation
* Player Field of vision via recursive shadowcasting
* Monster AI (rudimentary: move towards player on sight, attack, wandering between rooms, following player scent)
* Combat system
* Messages
* Minimap
* Weapons 
* Take, drop, equip items 
* Armour effects on combat system
* Treasure placement and pickup
* Addded input handling system
* Added door open / close mechanics via the iOpenable interface
* Added stair cases / movement between levels
* Parameterised the Weapon, Armour and NPC data
* Added click event into InputHandler

## To Do
* Document NPC AI system in Wiki
* Inventory Management: throw
* Add items: potions, scrolls, rings
* Animations - projectiles
* Food
* Item effects
* Clean up movement between level code
* Add code to increase complexity of levels, variation of monster types related to depth
* Improve animation effects system - current the effects are a static list in the Game1.cs, and items are added to it directly

## Long Term
 * Add articles in wiki - note that at the momenet as it's currently in so much flux that there's a chance I might chop up and burn something that has been meticulously documented.
 * Improve weapons
 * Environmental features e.g. Lava, water
 * Improve Combat system
 * Make the game object model more coherent (ongoing)
 * Investigate [MonoGame Extended](https://www.monogameextended.net/) - Animation and Modal Windows

## Code Overview
(needs a lot more expansion)
Game structure
* RosieGame.cs - is the main container for the game
* InputHandler.cs - gathers keystrokes and when they match registered commands passed them to RosieGame.cs
* Player.cs - class representing the player (inherits from Actor.cs)
* Monster.cs - class representing monster (inherits from Actor.cs)
* Level.cs - contains levels level map, list of monsters
* Generator.cs - base class which is inherited by the level generators: CaveGenerator.cs, CorridorCaveGenerator.cs, IslandGenerator.cs, MapGenerator.cs
* Item.cs is the base class for dungeon items e.g. Armour.cs, Weapon.cs, GoldCoins.cs
* Roller.cs and Roll.cs - the former is class to represent dice rolls e.g. 2d4 and the latter rolls the dice
* Scheduler.cs - determines which actors move on each turn



## Notes

* This was built using Visual Studio 2019 with the [MonoGame](https://docs.monogame.net/index.html) plugin.
* The tileset is from [here](https://github.com/statico/rltiles) and a map of the images can be found [here](http://statico.github.io/rltiles/)
* [RogueSharpRLNetSamples](https://bitbucket.org/FaronBracy/roguesharprlnetsamples/src/master/) - a .Net roguelike which served as a helpful reference.
* [Rogue Basin](http://roguebasin.com/index.php/Main_Page) is a handy resource
* MineCraft font taken from [here](https://www.dafont.com/bitmap.php).
* 2D particle system derived from [here](http://rbwhitaker.wikidot.com/2d-particle-engine-1).
* Monster AI inspired by [this](https://www.reddit.com/r/roguelikes/comments/1eayb6/monster_ai_system_explained_part_1_of_5/).
* Single Keypress code taken from [here](https://www.dreamincode.net/forums/topic/365588-detect-single-keypress-in-xna/).
* Pixel font from [here](https://github.com/00-Evan/shattered-pixel-dungeon/blob/master/core/src/main/assets/fonts/pixel_font.ttf).
* [Monster Attacks](http://roguebasin.com/index.php/Monster_attacks) from RogueBasin
* [VS2022 Code Cleanup on save](https://devblogs.microsoft.com/visualstudio/bringing-code-cleanup-on-save-to-visual-studio-2022-17-1-preview-2/)
* [NotePad++ GraphViz plugin](https://github.com/jrebacz/NppGraphViz) was handy for testing the waypoints used by the Monster AI System