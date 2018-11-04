# PUN-SpaceRocks

[Win-64 Client Download](https://www.dropbox.com/s/p7rc31ovh0sd7ch/PUN-SpaceRocks.zip?dl=0) based on commit b860aa4980d04bcc90a7595bcefbbf2fb024e30a

![](https://i.imgur.com/pu2jmpk.jpeg)

Implementation notes

* Used a more recent Photon PUN package
  * Noticed there was an Asteroids demo game in there, took a quick look but threw it out. You'll notice mine is different.
  * If this *has* to be PUN 1.91 let me know and I'll change it
  * PUN app id is my own
 	* Again, I can change that if needed. Presumed some other folks might be using the one you sent.
* Used 2d physics components
  * Made a custom synchronization component for them
* Very peer-to-peer setup, which is what the PUN plugin is all about I guess
  * Except for the asteroid spawning
* Had never actually used this Photon plugin for networking, so:
  * I didn't use any of the sample scripts, wanted to write the logic myself, get a feel for their core library
  * More on that below
  * I also didn't use the Photon Lobby system
	* Wanted to have a lobby that let player ships linger together while waiting for players
  * So I immediately connect to a room, which then has a Lobby stage and a Game stage
  * ... I didn't realize Photon had a Lobby concept built in until after I did it this way ¯\_(ツ)_/¯
	* This does mean players can pop into a match that is in progress, which I like
* GUI Color picker is from [this repository](https://github.com/judah4/HSV-Color-Picker-Unity) 

After using Photon to build this thing, here's some of my impressions:
* Every object is responsible for synchronizing itself
	* Would make it difficult to reason about synchronizing compositions of objects efficiently
	* Trashes cpu cache because of discontinuous memory access patterns
	* It's a consequence of Unity's old game object architecture
  * Using the new ECS would make this orders of magnitude more efficient
* Code for Server/Client/Owner/Remote roles tends to get mushed together
  * Brings lots of branching based on view.isMine ownership
	* We decoupled this in one of our own networking libraries, makes the code for complex networked objects more manageable
* Peer2Peer by default
  * Makes it pretty trivial to write cheats and exploits
  * For processes not associated with a specific player, not clear where they should live
    * such as the process that spawns asteroids
* Photon library uses classic managed dotnet code
	* Such as their Hashmap implementation, used for synhronizing per-player state like scores
  * As a project grows, the garbage generated like this will slow it down

The rest of the weekend I'll be trying out Unity's [new network transport layer](https://github.com/Unity-Technologies/multiplayer), which is in early testing. I've been impressed with the new ECS and multithreaded job systems, and the new transport layer is built entirely for that.
