
# Entanglement Prototype

## How to Use

* Attatch a PROTO_Quantum_Object script to an object with a rigidbody. 
* Assign another object with a rigidbody component to the Rb Partner field. (These two objects will be linked)

## Values

* Sync Positions will sync the beta object's position to it's partner, allowing it to phase through terrain. Disabling this will sync the objets based on velocity.
* Rb Partner is the object's partner that it will sync with
* Switch Threshold dictates whether you switch the beta object manually (set to 0) or if they influence each other equally (set to any number > 0)
* Beta shows if the current object is following it's partner. If Switch Threshold is set to 0, you can toggle this and switch the object to beta/leader on command

## How it works

The Quantum Object starts off as the leader, making the beta object copy it's velocity & positional change information.

The beta object is influenced completely by the leader.

The beta can become the leader if a large enough force or positional change occurs (dictated by Switch Threshold, still in development)

