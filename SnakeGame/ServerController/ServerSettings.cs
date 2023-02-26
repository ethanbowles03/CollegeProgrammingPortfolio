// Authors: Conner Fisk, Ethan Bowles
// This class holds the empty constructor and necessary DataMember
// fields to be filled in with the settings file.
// Date: December 1, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SnakeGame
{
	[DataContract(Name = "GameSettings", Namespace = "")]
	public class ServerSettings
	{
		// Settings Fields
		[DataMember(Order = 0)]
		public int FramesPerShot { get; private set; }
		[DataMember(Order = 1)]
		public int MSPerFrame { get; private set; }
		[DataMember(Order = 2)]
		public int RespawnRate { get; private set; }
		[DataMember(Order = 3)]
		public int UniverseSize { get; private set; }
		[DataMember(Order = 4)]
		public int SnakeSpeed { get; private set; }
		[DataMember(Order = 5)]
		public int SnakeStartingLength { get; private set; }
		[DataMember(Order = 6)]
		public int SnakeGrowth { get; private set; }
		[DataMember(Order = 7)]
		public int MaxPowerups { get; private set; }
		[DataMember(Order = 8)]
		public int MaxPowerupDelay { get; private set; }
		[DataMember(Order = 9)]
		public string Mode { get; private set; }
		[DataMember(Order = 10)]
		public List<Wall>? Walls { get; private set; }

		/// <summary>
		/// Empty construtor for the server settings.
		/// </summary>
		public ServerSettings() { }
	}
}
