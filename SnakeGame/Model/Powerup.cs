// Authors: Conner Fisk, Ethan Bowles
// Class that contains the constructor and properties of a Powerup object.
// Date: Nov 15, 2022

using Newtonsoft.Json;
using SnakeGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class Powerup
    {
        // Fields, add the JsonProperty tag to each.
        [JsonProperty]
        public int power { get; private set; }

        [JsonProperty]
        public Vector2D loc { get; private set; }

        [JsonProperty]
        public bool died { get; set; }

        /// <summary>
        /// Constructor for a Powerup
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Powerup(int pId, int x, int y)
        {
            // Set the Powerup's ID
            power = pId;
            // Create a new Vector2D with the given x and y and store it as the Powerup's loc
            loc = new Vector2D(x, y);
        }

        /// <summary>
        /// This method is used to check if the Powerup is colliding with a given obejct.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool isColliding(Object o)
        {
            // If o is a Snake...
            if (o is Snake)
            {
				// Initialize o as a Snake and get the necessary values.
				Snake s = (Snake)o;
                Vector2D head = s.body[s.body.Count - 1];

                // If the head of the snake collidies with the powerup, return true.
                if((head - loc).Length() < 5 + 5)
                    return true;
            }
            // If o is a Wall...
            else if(o is Wall)
            {
				// Initialize o as a Wall.
				Wall w = (Wall)o;

                // If the powerup location overlays with the wall segment, return true.
                if((loc.X > w.p1.X - 25 && loc.X < w.p2.X + 25) && (loc.Y > w.p1.Y -25 && loc.Y < w.p2.Y + 25) ||
					(loc.X > w.p2.X - 25 && loc.X < w.p1.X + 25) && (loc.Y > w.p2.Y - 25 && loc.Y < w.p1.Y + 25))
                {
                    return true;
                }
			}
            // If o is a Powerup...
			else if(o is Powerup)
            {
                // Initialize o as a Powerup
                Powerup p = (Powerup)o;

                // If a powerup's location is the same or is within the graphical border
                // of the powerup, return true.
				if ((p.loc - loc).Length() < 5 + 5)
					return true;
			}

            // If the program gets to the end of the method, return false.
            return false;
        }
	}
}