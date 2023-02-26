# Snake Client
PS8 Snake Client by Conner Fisk & Ethan Bowles

Decision Process:

  After setting up the skeleton code, we decided to start on the model. We created a Snake, Wall, Powerup, and World class. In these classes, we added the necessary fields (and JsonProperty tags) for each object that the server sends and recieves. After this was all set up, we moved on to the controller. We created a GameController class which is used to actually communicate with the server. Next, we moved on to the view. We decided to create the model and controller before the view so we would be able to see immediately if the view was doing what it was supposed to. For the view, along with the necessary Maui classes, we created a WorldPanel class used for loading the needed images and drawing on the canvas when needed. 
  
View Features:

  For our client, we used the base wall and background images, but changed the snake color rotation and make our own explotion. For each snake's ID, we % it by 10, and use the remainder to determine which of the 10 colors that we added will fill the snake. For the explosion, we found a pixel art explosion image and loaded it as we did the walls and background. When the snake dies the explosion image starts small and gets bigger to give the user a more realistic explosion.

# Snake Server
PS9 Snake Server by Conner Fisk & Ethan Bowles

What is working:

  After finishing the PS8 Snake Client code we moved onto creating the server. Our first objective was to get a handshake working between server and the client. We made sure that we sent the correct information over the server when connecting and processed incoming messages from the client. In order to make sure this handshake is working we printed server and user connection information in the console. We then created a setting class to allow the user to change the setting through an xml file. This file is located in the Server class-library in the Setting folder in our project. After implementing these features we added missing, server-side information to our model and implemented movement and turn methods in the snake class. We then added an infinite event loop to update and send the world to the client over the server. To make our game behave the same as an actual snake game we made all the objects spawn and despawn on the screen when needed and added collision between every object. If the snake's head hits a wall or snake it dies and grows if it hits a powerup. For snake-on-snake collisions we tested to see if any segment of the snake was hitting another. To finish it up we implemented the respawning of objects in a way that only allowed them spawn on non-occupied areas. If a snake spawns on a powerup they collect it.  Lastly we added wrap around functionality and a new gamemode with super powerups. 

What is not working:
  - Currently our respawn delay is working. However, as more snakes connect the delay seems to decrease. 
  - Wrap around works in all directions. However, it only works when the snake makes it fully through the boundary. No fast turnarounds
  
  
Interesting Features: 
  We added a new gamemode that has a super powerup. This powerup increases the snakes speed temporarily and also increase the snakes length by 5 segments. This powerup is very rare, the user has only a 1 in 10 chance to get it. You can change the game mode in the setting file by changing the Mode setting from "basic" to "extra". If you want to view the extra game mode it is on line 374.
   
