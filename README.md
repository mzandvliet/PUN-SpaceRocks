# PUN-SpaceRocks

[Win-64 Client Download](https://www.dropbox.com/s/p7rc31ovh0sd7ch/PUN-SpaceRocks.zip?dl=0) based on commit b860aa4980d04bcc90a7595bcefbbf2fb024e30a

![](https://i.imgur.com/pu2jmpk.jpeg)

Implementation notes

* Unity 2018.2.8f1
	* Should import in any 2018.2.x without issue
* Used a more recent Photon PUN package
* Used 2d physics components
	* Made a custom synchronization component for them
* Very peer-to-peer setup, which is what the PUN plugin is all about I guess
	* Except for the asteroid spawning
* Had never actually used PUN for implementing my netcode, so:
	* I didn't use any of the sample scripts, wanted to write the logic myself, get a feel for their core library
	* More on that below
	* I also didn't use the Photon Lobby system
		* Wanted to have a lobby that let player ships linger together while waiting for players
	* So I immediately connect to a room, which then has a Lobby stage and a Game stage
	* ... I didn't realize Photon had a Lobby concept built in until after I did it this way ¯\_(ツ)_/¯
	* This does mean players can pop into a match that is in progress, which I like
* GUI Color picker is from [this repository](https://github.com/judah4/HSV-Color-Picker-Unity) 

Be sure to check out [this networking prototype](https://bitbucket.org/m_zandvliet/rigidbodysync/src/default/Assets/Scripts/Player/) I worked on a few years ago. It features fast-moving space ships in an authoritative server setup, with the code for different network roles into split neatly into separate components. Prediction and correction are done using a custom forward-euler integrator that matches single Unity rigidbodies well enough.

I think I could actually take that principle and use it in this Photon-based project to really tighten it up, but I figured that out a bit late.
