# Uncontrol Room

This prototype bases entanglement on lots of different buttons that do one work. You look at the projected screen and press buttons to achieve a certain goal. It's upto the player to find out which buttons do what.

## Inputs

* WASD to move
* Mouse to Look
* In front of a button, press Space to call that button's function

## How it works

There's a second camera outside the Uncontrol Room that renders to the Control Screen. The buttons are all prefab variants that have the same 'button' script. Each variant calls a different function on the Target Object (on the screen) when it is pressed. Figuring out the buttons, press them around to reach the goal. This can be extended to have levers, pull chains, pressure pads, knobs, basically anything you can think of. And the target goal can be set however you want. Whether it's as simple as an obstacle course, or picking up objects and placing them, if it's imaginable, it can be done.