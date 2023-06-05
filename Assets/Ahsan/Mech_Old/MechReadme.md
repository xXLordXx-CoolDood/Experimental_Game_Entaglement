
# Mech Prototype

## Inputs

Inputs are mappable directly from the Mech prefab. As of the commit, 

* Z and C raise the legs 
* A and D turns the body
* Spacebar brings it back to balance.

## Values

* Sway rate makes it more wiggly
* Sway length makes it tip further
* Return speed sets how quickly it goes back straight when lowering the foot
* Balance value determines how quickly the sway dies down (Holding it for longer makes it sway negatively). This value needs to be highter than the sway length
* Balance Max is the limit for the Balance value. Being above this wont lower the foot, so the player has to bring back balance (Using the mapped input)

* Turn Speed is just turn speed

## How it works

The Mech has 4 states:

* RightUp
* LeftUp
* Based
* Orienting

When a foot is pressed, it triggers an animation to raise it. Then the mech starts to tip around.
When that foot is raised, no other inputs can be given except balancing. The raising sets off a balance value that oscillates, and that oscillation increases with a Balance Offset.
The Balance Input subtracts the offset, so the oscillation decreases. When the Balance is within +- the Balance Max, the Mech returns to its original position, and lowers the foot.
As requested, when both feet are lowered, the mech can be turned around.

