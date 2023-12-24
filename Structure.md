# Structure

## Start Up

How this game starts up for the curious amongst you:

Game1.cs
 * Initialize
   * Basic MONO configuration
   * Starts InputHandler
   * Sets Game Seed		
 * LoadContent
   * Loads textures
   * Instantiates the game class RosieGame()

## Code Structure

 * Game1.cs - top level container
	* RosieGame.cs - the game
	* InputHandler.cls - handles input and if recognises, passes to RosieGame.cs

## Interactions



The game is contained in *RosieGame.cs*


 Input (mouse click, key presses) are handled by *InputHandler.cs*

 Which has registered list of commands and if it recognises the keypress, it passes them to RosieGame.cs