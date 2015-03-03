After the team members met and discussed the main ideas for the game, the decisions were made and the game was chosen for implementation - "Falling Chars". The following document represents the implementation course (planning) the team complies with.

## "Falling Chars" implementation course
It's a game combination, which consists of two smaller games - a word game and a falling rocks game.

- Word catalog/dictionary (as a string array), which is read from a file (not hard-coded), represents the dictionary againts which every word, composed by the player is checked for validity
- Falling letters generator
- The player should be represented as a griffin (3-letter string), which moves at the screen bottom left- or rightwards
- Random word generator from the dictionary (obsolete)
- After/before each frame has been redrawn, a collision check should be performed
- - If no collision - cycle continues
- - If collision was found
- - - the letter in collision is to be added to the word buffer (consisted of captured letters)
- - - the word field at the right of the screen is to be updated at the next frame
- After enaugh letters are collected by the player it is passed to the word checker for validation (by pressing a button)
- - If a valid word is passed to the checker - player wins a number of points, equal to the length of the collected word
- - If non-valid word is passed to the chekcer - player loses number of points, which is calculated as a percent of the points, which would be given, if the word was correct
- The game has no play time limit and ends when the player decides to, so he/she has as much time to prove him-/herself as he/she needs
- The player should collect only letters, that forms a meaningful word
- The player has no info about the contents of the predefined dictionary, so he/she decides what the word will be
- It is possible that the player collects unneeded letter(s) in his/her word buffer - for the player to be able to continue the game and construct some meaningful word, he/she has to collect some bonus(es) which have the ability to manipulate the buffer
- After the game ends (or in-game) the player is presented the score-list, containing the 5 (or 10) best and the 5 (or 10) worst player rankings