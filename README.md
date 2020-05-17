# Common terrarium

Implementation of ant-like species for the common terrarium project of DD2438.

## How to setup ?

Switch to the Comparisons branch and pull.

### Prefabs

We used the pig prefab for the creature, but you can use any other, it doesn't really matter.

+ Attach a rigidbody and collider to your prefab
+ Attach to it the PigCreature script
	+ Rememberance parameter should be set to 0.7, or greater.
+ Attach the GroupT1RodAntBoarV2 script
	+Herbivore
	+ MaxEnergy : 1500 or less
	+ MaxSpeed : 10
	+ Size : 4
+ Set its tag to "herbivore"

You can now point out to the game Manager (in the game area prefab in the scene) that this prefab has to be spawned

### Trace system

+ Create a GameObject in the scene and set its tag to "trace"
+ Attach to it the "Trace.cs" script and set the values xMin,xMax,zMin,zMax such that it covers the entire game area.
	+ write 50 for nXCells and nZCells
	+ Let updateFrequency to 0
	+ Set evaporation to 0.1

When playing, a black cross is drawn, it should cover the whole terrain, make sure of it.