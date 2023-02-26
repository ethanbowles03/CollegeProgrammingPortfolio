// Authors: Conner Fisk, Ethan Bowles
// Class that contains the constructor and properties of a World object.
// Date: Nov 15, 2022

using SnakeGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnakeGame
{
    public class World
    {
        //Fields
        public Dictionary<int, Snake> Snakes { get; private set; }
        public Dictionary<int, Powerup> Powerups { get; private set; }
        public Dictionary<int, Wall> Walls { get; private set; }
        public int Size { get; private set; }

        public int powerupDelayCounter { get; set; }    

        /// <summary>
        /// Constructor for a World object.
        /// </summary>
        /// <param name="worldSize"></param>
        public World(int worldSize)
        {
            // Lock the World object.
            // Initialize and set the fields.
            Snakes = new Dictionary<int, Snake>();
            Powerups = new Dictionary<int, Powerup>();
            Walls = new Dictionary<int, Wall>();
            Size = worldSize;

        }

        /// <summary>
        /// This method is used to figure our what type of object each deserialized JSON is
        /// and add it to the correct dictionary if needed.
        /// </summary>
        /// <param name="item"></param>
        public void AddObj(object? item)
        {
            // If it is a Snake...
            if (item is Snake)
            {
                // Create a Snake object from item
                Snake snake = (Snake)item;
                // Lock the Snakes dictionary
                //If the snake has NOT disconnected...
                if (snake.dc == false)
                {
                    // Update the snake if it is already in the dictionary
                    if (Snakes.ContainsKey(snake.snake))
                        Snakes[snake.snake] = snake;
                    else // if not, add the snake to the dictionary
                        Snakes.Add(snake.snake, snake);
                }
                else // If the snake HAS disconnected...
                {
                    // Remove the snake from the Dictionary (if it is in there) if it
                    // has disconnected.
                    if (Snakes.ContainsKey(snake.snake))
                        Snakes.Remove(snake.snake);
                }
            }
            // If it is a Wall...
            else if (item is Wall)
            {
                // Create a Wall object from item
                Wall wall = (Wall)item;
                // Update the wall if it is already in the dictionary
                if (Walls.ContainsKey(wall.wall))
                    Walls[wall.wall] = wall;
                else // if not, add the Wall to the Dictionary
                    Walls.Add(wall.wall, wall);
            }
            // If it is a Powerup
            else if (item is Powerup)
            {
                // Create a Powerup object from item
                Powerup powerup = (Powerup)item;
                // Lock the Powerups dictionary
                // Update the Powerup if it is already in the dictionary
                if (Powerups.ContainsKey(powerup.power))
                    Powerups[powerup.power] = powerup;
                else // if not, add the Powerup to the dictionary
                    Powerups.Add(powerup.power, powerup);
            }
        }
    }
}