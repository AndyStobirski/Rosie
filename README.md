
# Rosie
A [RogueLike](https://en.wikipedia.org/wiki/Roguelike) named after my dog, which consists of modified code from my RogueLike repository. This is a work in progress and will change  a lot over time. I have no plan or structure for this, I'm making it up as I go along and enjoying myself as I do it.

At the present time, it's more of a collection of the concepts that make a roguelike loosely strung together. It consists of one level whose random number generator has been hardcoded to 12345 (to aid testing) - (Generator.cs > _rnd)

## Features

### Current

* Maze generation
* Player Field of vision
* Monster AI (rudimentary: move towards player on sight, attack, wandering between rooms)
* Combat system
* Messages
* Minimap
* Weapons (Rudimentary)
* Take, drop, equip items 
* Armour effects to combat system
* Treasure placement and pickup
* Addded input handling system
* Added door open / close mechanics via the iOpenable interface

### To Do
* Inventory Management: throw
* Add items: potions, scrolls, rings
* Animations - projectiles
* Food
* Item effects

### Long Term
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
* Player.cs - class representing the player (inherits from Actor.cs)
* Monster.cs - class representing monster (inherits from Actor.cs)
* Level.cs - contains levels level map, list of monsters
* Generator.cs - base class which is inherited by the level generators: CaveGenerator.cs, CorridorCaveGenerator.cs, IslandGenerator.cs, MapGenerator.cs
* Item.cs is the base class for: Armour.cs, Weapon.cs, GoldCoins.cs
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