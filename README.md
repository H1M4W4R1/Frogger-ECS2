# About
This project is an example of "Frogger-styled game" implemented using Unity ECS.

Current state: W.I.P.

# Concept
General concept is to create a collectible game where player moves in 2.5D space (world is rendered in 3 dimensions, however player moves only in 2). Player can die various ways - either by drowning in blue hydrogen monoxide acid, hit by car (and more ideas which will appear during development).

## **Tasks**:
* Collect bugs
* Don't die
* Pass all levels

# Known issues / problems
Due to my lack of previous experience in DOTS / ECS I went a bit too old-styled on this and created systems that are way overcomplicated (see PlayerMovementSystem.cs). Next time it would be divided into three subsystems (MovementInput, MovementCalculation and MovementProcessing... maybe even fourth - PlayerAnimation). Lesson learned - keep systems as simple as possible.

Unfortunately due to my recent lack of patience I didn't do a proper system level design, so it ended up being as it is... Also working alone is not a good thing while developing a game, as other devs may have better ideas.

# Required plugins
* DOTween Pro
