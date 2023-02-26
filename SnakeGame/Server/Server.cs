// Authors: Conner Fisk, Ethan Bowles, December 1, 2022
// This class holds the main method which starts the server and
// holds the infinite loop which keeps it running.

using SnakeGame;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SnakeGame
{
	internal class Server
	{
		static void Main(string[] args)
		{
			// Create a new DataContractSerializer & XmlReader
			DataContractSerializer ser = new DataContractSerializer(typeof(ServerSettings));
			XmlReader reader = XmlReader.Create("../../../settings/settings.xml");

			// Create a ServerSettings object from the given settings file by using the
			// created ser & reader.
			ServerSettings ss = (ServerSettings)ser.ReadObject(reader)!;

			// Create a ServerController and start the server with the parsed ServerSettings
			ServerController sc = new ServerController();
			sc.StartServer(ss);

			// Create a Stopwatch used to create a "delay" in the server
			// for the framerate.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Create an infinite loop to keep the server running.
			while (true)
			{
				// Wait until MSPerFrame milliseconds has passed.
				while (stopwatch.ElapsedMilliseconds < ss.MSPerFrame) { }
				// Restart the stopwatch.
				stopwatch.Restart();

				// Update and send the world.
				sc.UpdateWorld();
				sc.SendWorld();
			}
		}
	}
}