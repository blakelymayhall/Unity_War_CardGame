The following is a brainstorm for generating initial set of issues to 'get started' in ticket management

Figure the best way to to develop 'smaller games' inside of the game to test built of functionality as we go. I will call these phaes

A list of needed functionality for the complete game is approximately: 

# UI
- Background 
- Card sprites
	- Background
	- One for each 
- Deck count 
- Score count

# Gameplay
- Deck prefab 
	- Deck object 
	- Card object 
- On game start, generate two shuffled decks

# Phases
## Phase 1 - Single Card Flip
- On game start, show the back of any random card 
- On card click, show the front of that card 
- By the end of this, you should have a card object with some logic that:
	- Holds card value
	- Sprites
- Somewhere the 'click' functionality is stored, but that should only be temporary function as we don't intend to click individual cards

## Phase 2 - Deck 
- Show a deck on left
- When clicked, card appears on right
- Every click refreshes the card 
- When no cards remain, deck spot is empty 
- Test with small amount of cards
- By the end of this, you should have a deck object with some logic that:
	- Holds max (starting) num cards
	- Holds remaining number of cards 
	- Has the on click functionality 