
# FPS Prototype

## Inputs

* Mouse to look, left click to shoot a web
* WASD are used for movement, space is for jumping

## Values

* Player camera is the camera transform
* shot spawn is where the web is spawned
* Mouse smooth time is used to smooth the player's looking
* Cursors lock locks the cursor to the center of the screen & hides it
* Mouse sensitivity controls the sensitivity of the mouse
* Speed determines the player's move speed
* Surf speed determines how fast the player surfs on webs
* Surf With Movement determines if the direction of a surf is dictated by the player's movement direction or the direction they are looking (camera direction)
* Move Smooth Time controls the acceleration of the player
* Gravity controls the gravity of the player
* Ground check is the gameobject that checks for ground & web collisions
* Ground is the input for layers which are considered ground
* Webs is the input for layers which are considered webs
* Jump Height controls how high the player jumps

## How it works

The player has 2 states, surfing & walking

When walking, the player can move in 4 lateral directions and can jump.

When surfing, the player can only look around and jump while surfing along the web they are currently riding. Jumping cancels the surf and returns the player to the walkings tate.

The direction of a surf is based on either the player's current movement or where they are looking.

Webs are only created when the player shoots a physical object (so the web can "attatch" to it)

If the player hits a web from any other angle, they die (position reset to 0,0,0)

