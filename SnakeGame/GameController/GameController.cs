// Authors: Conner Fisk, Ethan Bowles
// Class that contains the controller used for out Snake Client.
// Date: Nov 16, 2022

using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameController
    {
        // Below are controller events that the view uses for communication
        public delegate void MessageHandler(IEnumerable<string> messages);
        public event MessageHandler? MessagesArrived;

        public delegate void ConnectedHandler();
        public event ConnectedHandler? Connected;

        public delegate void ErrorHandler(string err);
        public event ErrorHandler? Error;

        //Fields for the player name, world, player ID, and hasConnected boolean
        private string? name;
        public World? world;
        public string? pId = null;
        public bool hasConnected;

        /// <summary>
        /// Create a SocketState which represents the connection with the server.
        /// </summary>
        SocketState? theServer = null;

        /// <summary>
        /// This metho starts the process of connecting to the server.
        /// </summary>
        /// <param name="addr"></param>
        public void Connect(string addr, string name)
        {
            //Set the player's name.
            this.name = name;
            //Connect to the server with the OnConnect callback, the given address, and necessary port.
            Networking.ConnectToServer(OnConnect, addr, 11000);
        }


        /// <summary>
        /// The callback method which is invoked when a connection is made by the networking library.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            //If there was an error in the connection process...
            if (state.ErrorOccurred)
            {
                // Tell the view to show an error.
                Error?.Invoke("Error connecting to server");
                return;
            }

            // Set the hasConnected boolean to true if there was no error.
            hasConnected = true;

            // If there was no error in the connection process, tell the view.
            Connected?.Invoke();

            //Set theServer to the given state.
            theServer = state;

            //Send the name
            Networking.Send(theServer.TheSocket, name + "\n");

            // Begin recieving messages from the server by starting an event loop.
            state.OnNetworkAction = ReceiveMessage;

            // Get the data from the sever.
            Networking.GetData(state);
        }

        /// <summary>
        /// The method that is invoked by the networking library when data is available.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveMessage(SocketState state)
        {
            //If there is an error in recieving the message...
            if (state.ErrorOccurred)
            {
                // Inform the view of the error.
                Error?.Invoke("Lost connection to server");
                return;
            }

            //If there was not an error, process the messages that are being recieved.
            ProcessMessages(state);

            // Continue the event loop to keep recieving messages when they arrive.
            Networking.GetData(state);
        }

        /// <summary>
        /// Seperate each message at \n and process them.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessMessages(SocketState state)
        {
            // Store all of the data into a string.
            string totalData = state.GetData();

            // Create a string array of that splits up the data at each \n
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Create a list for newMessages
            List<string> newMessages = new List<string>();

            // Keep processing the data until every string in parts has been processed.
            foreach (string p in parts)
            {
                // If the current string p is empty, ignore it.
                if (p.Length == 0)
                    continue;
                // If the last string does not end with "\n", ignore it.
                if (p[p.Length - 1] != '\n')
                    break;

                // If p has passed the checks up until this point, add it to the newMessages which
                // will be shared with the view.
                newMessages.Add(p);

                // If pId has not been set yet (this is the first message recieved by the server)...
                if (pId is null)
                {
                    // Set pId to p without the trailing "\n"
                    pId = p.Substring(0, p.Length - 1);
                }
                // If pId has been set, but world has not (this is the second message recieved by the server)...
                else if (!(pId is null) && world is null)
                {
                    // Parse p without the trailing "\n" as an int called size.
                    int.TryParse(p.Substring(0, p.Length - 1), out int size);
                    // Set world to a new World with the size gathered in the previous line.
                    world = new World(size);
                }

                // Remove p from the SocketState's growable buffer as it has just been processed.
                state.RemoveData(0, p.Length);
            }

            // Let the view know of the newMessages.
            MessagesArrived?.Invoke(newMessages);

            // Add the objects found in the server's messages to the world.
            AddObjectsToWorld(newMessages);

        }

        /// <summary>
        /// This method goes through the given list of strings (Json objects), deserializes them,
        /// and adds them to the world.
        /// </summary>
        /// <param name="objs"></param>
        private void AddObjectsToWorld(List<String> objs)
        {
            // Lock the world
            lock (world!)
            {
                // Loops through every string in objs
                foreach (String obj in objs)
                {
                    // Set an object variable for each object that will be deserialized.
                    object? asset;

                    // If the current string contains the word "wall"...
                    if (obj.Contains("wall"))
                    {
                        // Set asset equal to the current string deserialized as a wall.
                        asset = JsonConvert.DeserializeObject<Wall>(obj);

                        // Add asset to the world.
                        world?.AddObj(asset);
                    }
                    // If the current string contains the word "snake"...
                    else if (obj.Contains("snake"))
                    {
                        // Set asset equal to the current string deserialized as a snake.
                        asset = JsonConvert.DeserializeObject<Snake>(obj);

                        // Add asset to the world.
                        world?.AddObj(asset);
                    }
                    // If the current string contains the word "power"...
                    else if (obj.Contains("power"))
                    {
                        // Set asset equal to the current string deserialized as a snake.
                        asset = JsonConvert.DeserializeObject<Powerup>(obj);

                        // Add asset to the world.
                        world?.AddObj(asset);
                    }
                }
            }
        }

        /// <summary>
        /// This method closes the connection with the server.
        /// </summary>
        public void Close()
        {
            // Close the socket.
            theServer?.TheSocket.Close();
        }

        /// <summary>
        /// This method sends a message to the server
        /// </summary>
        /// <param name="message"></param>
        public void MessageEntered(string message)
        {
            // If the server is not null...
            if (theServer is not null)
            {
                // Send the given message with a trailing "\n".
                Networking.Send(theServer.TheSocket, message + "\n");
            }
        }

        /// <summary>
        /// Getter method that returns the world.
        /// </summary>
        /// <returns></returns>
        public World? GetWorld()
        {
            return world;
        }

    }
}

