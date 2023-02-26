// Authors: Conner Fisk, Ethan Bowles
// This class holds the constructor and needed methods
// for the ServerController
// Date: December 1, 2022

using NetworkUtil;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SnakeGame
{
	[DataContract(Name = "GameSettings", Namespace = "")]
	public class ServerController
	{
		// Fields
		private Dictionary<long, SocketState>? clients;
		public World? theWorld;
		private bool nameRecieved;
		private ServerSettings? ss;
		private string? clientName;
		private Random? rng;
		private int powerupId;
		private int curPowerupDelay;

		/// <summary>
		/// Begin accepting Tcp sockets connections from the clients.
		/// </summary>
		public void StartServer(ServerSettings ss)
		{
			// Initialize the needed variables.
			rng = new Random();
			this.clients = new Dictionary<long, SocketState>();
			this.theWorld = new World(ss.UniverseSize);
			this.ss = ss;

			// Make a random powerup delay from the retrieved max delay from the settings.
			curPowerupDelay = rng.Next(0, ss.MaxPowerupDelay);

			// Add the walls from the settings to theWorld dictionary.
			foreach (Wall wall in ss.Walls!)
			{
				theWorld.Walls.Add(wall.wall, wall);
			}
			// Start the server
			Networking.StartServer(NewClientConnected, 11000);
			Console.WriteLine("Server started...");
		}

		/// <summary>
		/// Method that is invoked by the networking libarary
		/// when there is a new client that connects.
		/// </summary>
		/// <param name="state"></param>
		public void NewClientConnected(SocketState state)
		{
			// If there was an error, do nothing and return.
			if (state.ErrorOccurred)
			{
				return;
			}

			// Generate a random remainder and use that value to determine which
			// direction the user's snake will be started in.
			int remainder = rng!.Next(0, 5) % 4;
			lock (theWorld!)
			{
				if (remainder == 1)
					theWorld!.Snakes.Add((int)state.ID, new Snake((int)state.ID, 0, 1));
				else if (remainder == 2)
					theWorld!.Snakes.Add((int)state.ID, new Snake((int)state.ID, 0, -1));
				else if (remainder == 3)
					theWorld!.Snakes.Add((int)state.ID, new Snake((int)state.ID, 1, 0));
				else
					theWorld!.Snakes.Add((int)state.ID, new Snake((int)state.ID, -1, 0));

				// Get the user's snake out of theWorld's Snakes dictionary and call snakeRandomPos
				// to set the snakes coordinates at a random valid position.
				Snake curSnake = theWorld.Snakes[(int)state.ID];
				snakeRandomPos(curSnake);
			}

			// Send the client's snake ID and the world size to the client.
			Networking.Send(state.TheSocket, state.ID.ToString() + "\n");
			Networking.Send(state.TheSocket, theWorld!.Size.ToString() + "\n");

			// Send each of the walls as a JSON to the client.
			foreach (Wall wall in theWorld.Walls.Values)
			{
				string wallJson = JsonConvert.SerializeObject(wall) + "\n";
				Networking.Send(state.TheSocket, wallJson);
			}

			// Lock and save the client state
			lock (clients!)
			{
				clients[state.ID] = state;
			}

			// Write that the specific client has connected.
			Console.WriteLine("Client " + state.ID + " has connected.");

			// Update the state's network action so we can process data
			// when something happens
			state.OnNetworkAction = ReceiveMessage;

			// Get ready to recieve data from the client.
			Networking.GetData(state);
		}

		/// <summary>
		/// This method takes in a snake and sets it position to a random valid spot
		/// on the map.
		/// </summary>
		/// <param name="curSnake"></param>
		private void snakeRandomPos(Snake curSnake)
		{
			// Create the snakeCollCheck bool
			bool snakeCollCheck = false;

			// While the snake is still not in a valid position
			while (!snakeCollCheck)
			{
				// Generate a random (x,y) position
				int x = rng!.Next(-theWorld!.Size / 2, theWorld.Size / 2);
				int y = rng.Next(-theWorld.Size / 2, theWorld.Size / 2);

				// Initialize the snake with the random values created above.
				curSnake.Init(ss!.SnakeSpeed, ss.SnakeStartingLength, x, y);

				// For each wall, check if curSnake is colliding with any of them.
				foreach (Wall wall in theWorld.Walls.Values)
				{
					if (curSnake.isBodyColliding(wall, ss.UniverseSize))
					{
						// Set snakeCollCheck to true if there is a collision
						snakeCollCheck = true;
					}
					else
						continue;
				}
				// If there was a collision, reset snakeCollCheck and continue the while loop
				// to get a new (x,y)
				if (snakeCollCheck)
				{
					snakeCollCheck = false;
					continue;
				}
				// For each snake, check if curSnake is colliding with any of them.
				foreach (Snake snake in theWorld.Snakes.Values)
				{
					if (curSnake.isBodyColliding(snake, ss.UniverseSize))
					{
						// Set snakeCollCheck to true if there is a collision
						snakeCollCheck = true;
					}
					else
						continue;
				}
				// If there was a collision, reset snakeCollCheck and continue the while loop
				// to get a new (x,y)
				if (snakeCollCheck)
				{
					snakeCollCheck = false;
					continue;
				}
				else // The (x,y) is valid, so break the while loop.
				{
					break;
				}
			}
		}

		/// <summary>
		/// This method created a Powerup at a random position.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private Powerup powerupRandomPos(int id)
		{
			// Create a powerupCollCheck bool to flag collisions.
			bool powerupCollCheck = false;

			// Create a powerup to edit
			Powerup p = new Powerup(0, 0, 0);

			// While the powerup is not in a valid place...
			while (!powerupCollCheck)
			{
				// Get the size of the world.
				int size = theWorld!.Size;

				// Generate a random (x,y) coordinate and set p's location
				// to that.
				int x = rng!.Next(-size / 2, theWorld.Size / 2);
				int y = rng.Next(-theWorld.Size / 2, theWorld.Size / 2);
				p = new Powerup(id, x, y);

				// For every wall in the world, check if p collides with it.
				foreach (Wall wall in theWorld.Walls.Values)
				{
					if (p.isColliding(wall)) // If there is a collision, set powerupCollCheck to true.
						powerupCollCheck = true;
					else
						continue;
				}
				// If there is a collision, set powerupCollCheck to false to get a new random (x,y).
				if (powerupCollCheck)
					powerupCollCheck = false;
				else
					break;
			}

			// Return the created Powerup.
			return p;
		}

		/// <summary>
		/// Method that is invoked by the networking library when there
		/// is a netwrok action that occurs.
		/// </summary>
		/// <param name="state"></param>
		private void ReceiveMessage(SocketState state)
		{
			// Remove the client if they aren't still connected
			if (state.ErrorOccurred)
			{
				RemoveClient(state.ID);
				return;
			}

			// Process the message.
			ProcessMessage(state);

			// Continue the event loop that receives messages from this client.
			Networking.GetData(state);
		}

		/// <summary>
		/// This method is used to process messages recieved from the client.
		/// </summary>
		/// <param name="state"></param>
		private void ProcessMessage(SocketState state)
		{
			// Get the whole string the client sent and split it at new lines.
			string totalData = state.GetData();
			string[] parts = Regex.Split(totalData, @"(?<=[\n])");

			// For every part in the client's message...
			foreach (string p in parts)
			{
				// Ignore empty strings added by the regex splitter
				if (p.Length == 0)
					continue;

				// Ignore the last string if it doesn't end with a '\n'
				if (p[p.Length - 1] != '\n')
					break;

				// If the message is a movement command...
				if (p[0] == '{')
				{
					lock (theWorld!)
					{
						Snake thisSnake = theWorld!.Snakes[(int)state.ID];
						if (p.Contains("none"))
						{
							// Don't turn the snake
						}
						else if (p.Contains("up")) // Turn the snake up.
						{
							thisSnake.Turn(new Vector2D(0, 1));
						}
						else if (p.Contains("left")) // Turn the snake left.
						{
							thisSnake.Turn(new Vector2D(1, 0));
						}
						else if (p.Contains("down")) // Turn the snake down.
						{
							thisSnake.Turn(new Vector2D(0, -1));
						}
						else if (p.Contains("right")) // Turnt he snake right.
						{
							thisSnake.Turn(new Vector2D(-1, 0));
						}
					}
				}
				else
				{
					// Handle the client's message as a name.
					theWorld!.Snakes[(int)state.ID].name = p.Substring(0, p.Length - 1);
				}

				// Remove the data as it has been handled by this point.
				state.RemoveData(0, p.Length);
			}
		}

		/// <summary>
		/// Removes a client from the clients dictionary when called.
		/// </summary>
		/// <param name="id">The client's ID</param>
		private void RemoveClient(long id)
		{
			// Display that the specific client has disconnected.
			Console.WriteLine("Client " + id + " disconnected.");

			// Lock the clients dictionary and remove the specified client.
			lock (clients!)
			{
				clients.Remove(id);
			}

			// Lock the world and remove the client's snake.
			lock (theWorld!)
			{
				theWorld.Snakes[(int)id].alive = false;
				theWorld.Snakes[(int)id].died = false;
				SendWorld();
				theWorld.Snakes.Remove((int)id);
			}
		}

		/// <summary>
		/// This method is used to update the world and make sure everything is
		/// in the state that it needs to be before it gets sent to the client.
		/// </summary>
		public void UpdateWorld()
		{
			// Lock theWorld while we update it.
			lock (theWorld!)
			{
				// For every snake in theWorld...
				foreach (Snake snake in theWorld!.Snakes.Values)
				{
					// If snake is alive, call move on it.
					if (snake.alive == true)
					{
						snake.Move(ss!.UniverseSize);
					}
					// Loop through every snake and check if the current snake collides with any of them (or iteself).
					foreach (Snake s in theWorld!.Snakes.Values)
					{
						if ((snake.isColliding(s) || !snake.alive) && s.snake != snake.snake && (s.died == false && s.alive == true))
						{
							KillSnake(snake); // Kill the current snake if there is a collision.
						}
						else if (s.snake == snake.snake) // If checking a self collision, use the isSelfColliding method
						{
							if (snake.isSelfColliding())
								KillSnake(snake); // Kill the current snake if there is a collision.
						}
					}
					// Loop through every wall and check if the current snake collides with any of them.
					foreach (Wall wall in theWorld!.Walls.Values)
					{
						if (snake.isColliding(wall))
						{
							KillSnake(snake); // Kill the current snake if there is a collision.
						}
					}
					// Loop through every powerup and check if the snake is colliding with it.
					foreach (Powerup powerup in theWorld!.Powerups.Values)
					{
						// If there is a collision between the snake and powerup, grow the snake, remove the powerup, and increment
						// the client's score.
						if (snake.isColliding(powerup) || snake.isBodyColliding(powerup, ss!.UniverseSize))
						{
							// SPECIAL FEATURE:
							// 1 out of 10 chance that a powerup will increase the Snake's speed for 10x the 
							// server setting's snakeGrowth amount. It will also +5 to the player's score instead
							// of only +1.
							int rand = 0;
							if (!ss!.Mode.Equals("basic"))
							{
								rand = rng!.Next(0, 11);
							}
							if (rand == 10)
							{
								snake.isSpeeding = true;
								snake.score += 5;
							}
							else
							{
								snake.isGrowing = true;
								snake.score++;
							}
							powerup.died = true;
							SendWorld();
							theWorld!.Powerups.Remove(powerup.power);
						}
					}
					// If the snake is currently growing...
					if (snake.isGrowing)
					{
						// Grow the snake for the given time.
						if (snake.growingTimer < ss!.SnakeGrowth)
						{
							snake.growingTimer++;
						}
						else // Once that time is up, stop the snake growth.
						{
							snake.isGrowing = false;
							snake.growingTimer = 0;
						}
					}

					// If the snake is currently speeding...
					if (snake.isSpeeding)
					{
						// Grow the snake for the given time.
						if (snake.speedingTimer < ss!.SnakeGrowth * 10)
						{
							snake.speedingTimer++;
						}
						else // Once that time is up, stop the snake growth.
						{
							snake.isSpeeding = false;
							snake.speedingTimer = 0;
						}
					}
				}
				// If there is less that the maximum amount of powerups in the world and the powerup delay is up,
				// add a new powerup at a random position in the world.
				if (theWorld.Powerups.Count < ss!.MaxPowerups && theWorld.powerupDelayCounter >= curPowerupDelay)
				{
					theWorld.powerupDelayCounter = 0;
					curPowerupDelay = rng!.Next(0, ss.MaxPowerupDelay);
					theWorld.Powerups.Add(powerupId, powerupRandomPos(powerupId));
					powerupId++;
				}
				// Increment the powerupDelayCounter.
				theWorld.powerupDelayCounter++;
			}
		}

		/// <summary>
		/// This method is used to respawn a snake.
		/// </summary>
		/// <param name="o"></param>
		public void Respawn(Snake snake)
		{
			// Set the snake to a random position, then send the world.
			snakeRandomPos(snake);
			SendWorld();

			// Set the Snake's respawning bool to false after it has respawned.
			snake.respawning = false;
		}

		/// <summary>
		/// This method sends the client the data of the Powerups and Snakes in the game
		/// in order for the client to stay updated to the current state.
		/// </summary>
		public void SendWorld()
		{
			// Lock the clients
			lock (clients!)
			{
				// For every client...
				foreach (SocketState client in clients!.Values)
				{
					// Lock theWorld
					lock (theWorld!)
					{
						// For every snake in the world, send the client the JSON for that snake.
						foreach (Snake snake in theWorld!.Snakes.Values)
						{
							string snakeJson = JsonConvert.SerializeObject(snake) + "\n";
							Networking.Send(client.TheSocket, snakeJson);
						}
						// For every powerup in the world, send the client the JSON for that powerup.
						foreach (Powerup powerup in theWorld!.Powerups.Values)
						{
							string powerupJson = JsonConvert.SerializeObject(powerup) + "\n";
							Networking.Send(client.TheSocket, powerupJson);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method is used to kill a specified snake.
		/// </summary>
		/// <param name="snake"></param>
		private void KillSnake(Snake snake)
		{
			// If the snake is not already respawning...
			if (!snake.respawning)
			{
				// If this is the first frame since the snake has been 
				// called to be killed...
				if (snake.framesSinceDeath == 0)
				{
					// Set the snake's died to true, then send the world.
					snake.died = true;
					SendWorld();
					// Set the snake's alive to false
					snake.alive = false;
				}
				else // If it's not the first frame since the kill call...
				{
					// Make sure the snake's died is false.
					snake.died = false;
				}
				// Increment the snake's framesSinceDeath counter.
				snake.framesSinceDeath++;

				// Once the snake has been dead long enough according to the settings file...
				if (snake.framesSinceDeath >= ss!.RespawnRate)
				{
					// Get the snake ready to respawn and call the Respawn method.
					snake.respawning = true;
					snake.framesSinceDeath = 0;
					Respawn(snake);
				}
			}
		}
	}
}