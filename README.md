# Robo-Rush

## Overview
Robo Rush is a multiplayer battle-royale race-style obstacle course game where upto 8 players need to navigate floors of hexagonal tiles that disappear after being stepped on. The goal is to be the last player standing on the tile floors by strategically avoiding falling through the tiles to the ground beneath the floors of tiles. Top 3 ranking Players on the scoreboard gain XP for every match. The XP can be used to get new skins for the player characters (Future Implementation).

---

## Features

- Colorful Battle Royale: Players control customizable robo characters and compete in an arena of multiple floors of hexagonal tiles.

- Multiplayer Mayhem: Up to 8 players can join a lobby to face off in fun & chaotic game until one winner remains.

- Fast-Paced Strategy: The gameplay is a fast-paced where players jump across disappearing hexagonal tiles, strategizing to avoid falling through gaps and being eliminated.

- Customization Options: From Lobby Size to Character Skins, players can customize their experience with the game. They can unlock more skins to personalize their character (Future Implementation).

- Simple Controls: Easy-to-learn gameplay with basic camera controls, running & jumping mechanics.
  
- Ranking & Scoreboard: Players are ranked based on how long they survived on the tile floors. Top 3 ranks of the game earn XP.
  
---

## Implementation Details

### Design Patterns

Singleton Pattern: Used for managing global systems like GameManager to manage the game's state & act as mediator for the Services, EventBusManager to manage the events, SceneLoader to load specific scnenes both locally & on network, CanvasUIManager to manage all the UI in the same Canvas across all the scenes, Audio Manager to manage all the game audio, etc.. This ensures there's only one instance handling these operations across the game.

Service Locator Pattern: Centralized management of game services like Level, Player, UI etc. The Service Locator helps decouple the game components, making it easier to manage services and dependencies across different systems.

Observer Pattern: Used for event-driven interactions for managing various gameplay events. Components can "subscribe" to events (like the activation of the Hex Tiles for gameplay) and automatically update themselves when these events occur.

State Machine Pattern: Used to manage different game states (StartState, LobbyState, CharacterSelection, Gameplay & GameOver) by clearly defining transitions between each state. This ensures smooth handling of required services for each state & logic, as the game responds to user inputs and events based on its current state.

### Scriptable Objects

Used to store and manage data independently of game objects, making them ideal for managing data for Audio, Level, Player, Character, UI, Network, Environment & XP. This allows for easy customization and modification of attributes directly from the Unity Editor without needing to modify code.

### Design Patterns

Unity Netcode: Unity Netcode enables multiplayer functionality by synchronizing data across clients and servers, allowing real-time communication and interaction in networked games.

Unity Lobby Service: The Unity Lobby Service helps manage player matchmaking and game sessions, allowing players to create, join, and manage lobbies before entering the game.

Unity Relay Service: The Unity Relay service facilitate multiplayer connectivity in games without requiring developers to manage dedicated game servers or players to deal with network complexities like IP addresses and port forwarding.

---

## Architecture Block Diagram

![Architecture Block Diagram -  RoboRush - Battle Royale Obstacle Course Game](https://github.com/user-attachments/assets/86f7fbc4-1013-4131-914f-56deb0878f89)

---

## How to Play

Download the Game: A playable build of the game is available in the drive folder link attached below in the next section. Download the zip file & extract it.

Open the Game: Launch Robo Rush (Battle-Royale-Game application file) and choose the "Start Playing" option from the main menu.

Create Lobby: If you want to create a lobby, enter a lobby name, choose the lobby size/capacity, set the lobby to public for others to join or private if you want to play only with your friends.

Join Lobby: To join a lobby, select either the "Quick Join" option to enter any available public matchmaking lobby, or choose specific public lobbies from the list on the right-hand-side panel (if available) or join a specific lobby by entering lobby code if you're joining a friend's lobby.

Start the Game: Once everyone has joined the lobby, select a character skin & click on Ready Button. The game will automatically begin once every player in lobby is ready and you'll be dropped into the game arena.

Play the Game: Once the game start countdown is done, start running & avoid stepping on tiles too long as they disappear after each step, creating gaps to fall through. 

Strategize your Run & Jumps: Plan your jumps and movements carefully to land on the remaining tiles while preventing yourself from falling. Keep an eye on other players and try to outlast them by staying on higher levels longer.

Rank in Top 3 to Earn XP: The last player standing on the remaining tiles wins the round, while others are eliminated as they fall. The Top 3 ranking players on the scoreboard get XP as per their rank(100/75/50).

---

## Playable build

https://drive.google.com/drive/folders/1A9wcJ2_kldITUvh_xYDpg4qSdVkbLzHp

---

## Gameplay Video

https://youtu.be/bCgLa1W95lI
