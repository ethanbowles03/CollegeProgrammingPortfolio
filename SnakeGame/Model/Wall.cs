// Authors: Conner Fisk, Ethan Bowles
// Class that contains the constructor and properties of a Wall object.
// Date: Nov 15, 2022

using Newtonsoft.Json;
using SnakeGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SnakeGame
{
    [DataContract(Name = "Wall", Namespace = "")]
    public class Wall
    {
        //Fields, add the JsonProperty tag to each.
        [JsonProperty("wall")]
        [DataMember (Name = "ID")]
        public int wall { get; private set; }

        [JsonProperty]
		[DataMember]
		public Vector2D p1 { get; private set; }

        [JsonProperty]
		[DataMember]
		public Vector2D p2 { get; private set; }

        /// <summary>
        /// Constructor for the wall object.
        /// </summary>
        public Wall(int id, int firstX, int firstY, int secX, int secY)
        {
            // Set the fields
            wall = id;
            p1 = new Vector2D(firstX, firstY);
            p2 = new Vector2D(secX, secY);
        }
    }
}