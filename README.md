# Integrated Project - CrimeLens

## Overview
### Brief Game Description
CrimeLens is a spy game, where you, the player, infiltrate a scam organization to learn more about how they trick their victims into believing false rumours, giving up personal details and how to get around methods that victims may use to protect themselves. Sneak in, grab the organization’s data and hand it over to the authorities without getting caught. Do you have what it takes…? 

### Genre
Stealth, Puzzle, RPG 

### Core Gameplay Loop
Players interact with activities, NPCs and their environment to earn points to place high on a leaderboard.   

### Target Audience
Young teenagers who spend lots of time online.  

## Intended Platform and Requirements
Platform: Windows 
Hardware: UHD Display (1920 x 1080) 

## Controls
Movement - WASD
Jump - Spacebar
Interact - E
Crouch - C

## Gameplay Instructions
### Game Flow / Tutorial
- Players will start on the menu
- When they hit start, their character will spawn in the lobby
- Players are free to explore the game however choose
- Quests will be given at each room to help guide them in the intended flow of the game.
- Complete the activities in each room
- Observe the numbers on the whiteboard and arrange them in the correct order
- Key the correct pin in the keypad to unlock ServerRoom Door
- Find the USB Stick in the room and plug it into computer to begin extracting data
- Evade the boss while waiting for the data to be finished downloading
- Escape the building and find the police station
- Enter the police station and hand over the USB Stick, ending the game 

### Key Mechanics
- Movement is essential: avoiding NPCs by crouching to reduce noise or simply running in the opposite direction is extremely important in having a good run
- Exploring their Environment: Use Interact key “E” to talk to NPCs and investigate the furniture to uncover clues to beat the game or attain a higher score.

## Limitations and Known Bugs
- Cabinet: Unable to make the player teleport into the cabinet to hide when interacting with it, player needs to manually walk into the cabinet before the door closes
- Sometimes when the player collides with the Chaser NPC, the player will be pushed outside of the map to the small platforms left from the modular build of the floor
- When interacting with NPCs, UI updates need to be delayed ensuring Unity elements have been rendered before the interaction 

## AI and Finite State Machines
### Short Explanation and Transitions
#### Idle
- The NPC stays in one spot for a set amount of time
- After the set amount of time, it will switch to patrol

#### Patrol
- The NPC moves to the next pre-set spot
- Once it arrives at the pre-set spot, it switches to Idle 

#### Chase
- The NPC chases the player until the player is out of reach
- If the player is out of reach, it switches to Idle 

#### Investigate
- The NPC will go move towards the last position where the player was heard
- The NPC will stay for 3 seconds to investigate the area
- If player is found within the area, NPC will give 1 of 3 strikes
- If all 3 strikes are used up, NPC switches to CallForBackup, then Chase
- Otherwise, the NPC will return to Patrol 

#### CallForBackup
- Spawns another NPC randomly in a 1-unit radius of the original NPC once all 3 strikes of Investigate are used up
- Spawned NPCs start off in Idle state.  

#### ShowDialogueLine
- Splits each dialogue line into individual words and iterates through them to display the full messahe
- Checks for the next line and repeats the action o Calls StopDialogue function if there are no more lines
- Called by StartDialogue function 

#### ShowOptions 
- Show choice buttons for current dialogue node and set the button text, awarding points if the player answers correctly
- Sets optionsPanel to active before running the state
- Calls for the next dialogue using OnOptionsSelected
- OnOptionsSelected checks for the next dialogue line, calling coroutine ShowOptions again if there’s another line
- Otherwise, it calls EndDIalogue, ending the interaction between the player and the NPC.  

### Notes and Implementation Details
#### Chaser
- Uses Unity’s NavMeshAgent for pathfinding between set points and chasing the player
- Patrol points are stored as transform references
- Investigate stores the player’s last known location and moves towards it
- Strikes are tracked using strikeCount
- After 3 strikes, switches state to CallForBackup and sends the player to the respawn point
- NPCs spawned in using CallForBackup use Instantiate() with a random offset of 1-unit radius

#### NPCDialogue
- Dialogue lines are stored in a DialogueNode class with text and next node references
- ShowDialogueLine runs a coroutine to type text word-by-word
- Within the coroutine, check if there are more dialogue lines
- Reruns the coroutine until are there no more lines, calling the StopDialogue funciotn 

#### NPCOptions
- Stores options and scores on top of existing text and next node references
- ShowOptions coroutine sets button text, adds click listeners and calls OnOptionSelected() when chosen
- When OnOptionSelected is called, it checks for the next node. If there’s more dialogue, ShowOptions is called once again. Otherwise it calls EndDialogue

## Puzzle Solutions
### List of puzzle solutions
Captcha: Type out the corresponding code generated
Phishing Email: In Order: fake, Legitimate, Fake, Fake
Deepfake Room: In Order: Right, Left, Left, Left, Right
Keypad Code: 2724

### How dynamic puzzles are generated
#### Captcha
- When the player presses the Get Code button, it generates a random 5 character long alphanumeric code using GenerateCode
- When the player clicks Confirm, it runs VerifyCode, which compares the player’s input to the previously stored GenerateCode
- If the player answers correctly, the UI will automatically close itself, signaling to the player that they’ve completed the activity
- If the player answers incorrectly, an error message will appear telling the player of their mistakes and GenerateCode will run again, allowing the player to retry the activity. 

## Credits and References
Character Controller: [Starter Assets: Caharcter Controller](https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961) 
Characters: [Mixamo, Police Cutscene](https://sketchfab.com/3d-models/police-man-sg-73ec215f59e94763b1acde2e78db7969)
Skybox: [Skybox Series Free | 2D Sky | Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633)
