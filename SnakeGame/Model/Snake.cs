// Authors: Conner Fisk, Ethan Bowles
// Class that contains the constructor and properties of a Snake object.
// Date: Nov 15, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SnakeGame;
using Newtonsoft.Json;
using System.Diagnostics.SymbolStore;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading;

namespace SnakeGame
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Snake
	{
		//Fields, add the JsonProperty tag if necessary.
		[JsonProperty]
		public int snake { get; private set; }

		[JsonProperty]
		public string name { get; set; }

		[JsonProperty]
		public List<Vector2D> body { get; private set; }

		[JsonProperty]
		public Vector2D dir { get; private set; }

		[JsonProperty]
		public int score { get; set; }

		[JsonProperty]
		public bool died { get; set; }

		[JsonProperty]
		public bool alive { get; set; }

		[JsonProperty]
		public bool dc { get; private set; }

		[JsonProperty]
		public bool join { get; private set; }

		public int framesSinceDeath { get; set; }
		public bool respawning { get; set; }
		public bool isGrowing { get; set; }
		public bool isSpeeding { get; set; }
		public int growingTimer { get; set; }
		public int speedingTimer { get; set; }

		public int startingLength;
		public int speed;
		private int wallPassthrough;

		/// <summary>
		/// Constructor for the snake object.
		/// </summary>
		public Snake(int sId, int x, int y)
		{
			// Set the fields
			snake = sId;
			dir = new Vector2D(x, y);
			score = 0;
			alive = true;
			died = false;
			name = "";
			body = new List<Vector2D>();
		}

		/// <summary>
		/// This method is used to initialize a snake with the given parameters.
		/// </summary>
		/// <param name="speed"></param>
		/// <param name="startingLength"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Init(int speed, int startingLength, int x, int y)
		{
			// Initialize the needed snake variables
			this.body = new List<Vector2D>();
			this.alive = true;
			this.died = false;
			this.speed = speed;
			this.score = 0;
			this.startingLength = startingLength;
			this.isSpeeding = false;

			// Create a head and tail Vector2D and add it to the snake's body.
			Vector2D head = new Vector2D(x, y);
			Vector2D tail = new Vector2D(x + (dir.X * startingLength), y + (dir.Y * startingLength));
			this.body.Add(tail);
			this.body.Add(head);
		}

		/// <summary>
		/// This method is used to turn the snake.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="changeInDir"></param>
		public void Turn(Vector2D changeInDir)
		{
			// Get the snakes head and point just before the head.
			Vector2D head = body[body.Count - 1];
			Vector2D befHead = body[body.Count - 2];

			// Don't allow the snake to turn in on itself.
			if ((Math.Abs(head.X - befHead.X) < 10 && Math.Abs(head.Y - befHead.Y) < 10) ||
				(dir.X * -1 == changeInDir.X && dir.Y * -1 == changeInDir.Y))
			{
				return;
			}

			// Duplicate the snake's head and add it to the body.
			Vector2D dupHead = new Vector2D(body[body.Count - 1].X, body[body.Count - 1].Y);
			body.Add(dupHead);

			// Set the snake's direction to the new direction.
			this.dir = changeInDir;
		}

		/// <summary>
		/// This method is used to move the snake across the map.
		/// </summary>
		public void Move(int worldSize)
		{
			// Get the needed Vector2D values.
			Vector2D head = body[body.Count - 1];
			Vector2D befHead = body[body.Count - 2];
			Vector2D tail = body[0];
			Vector2D nextTail = body[1];

			// Update the snake's head depending on the direction it is moving
			// and its speed.
			if (isSpeeding)
			{
				head.X = head.X - (dir.X * (speed * 2));
				head.Y = head.Y - (dir.Y * (speed * 2));
			}
			else
			{
				head.X = head.X - (dir.X * speed);
				head.Y = head.Y - (dir.Y * speed);
			}

			// Check if the snake needs to wrap around from top/bottom...
			if ((head.Y > (worldSize) / 2) || (head.Y < -(worldSize) / 2))
			{
				Vector2D transferPoint = new Vector2D(head.X, head.Y);
				transferPoint.Y = -transferPoint.Y;
				if (transferPoint.Y < 0)
				{
					// Set the wallPassthrough to remove when needed.
					wallPassthrough = 1;
				}
				else
				{
					// Set the wallPassthrough to remove when needed.
					wallPassthrough = 0;
				}
				this.body.Add(transferPoint);
				Vector2D newHead = new Vector2D(transferPoint.X, transferPoint.Y);
				this.body.Add(newHead);
			}

			// Check if the snake needs to wrap around from right/left...
			if ((head.X > (worldSize) / 2) || (head.X < -(worldSize) / 2))
			{
				Vector2D transferPoint = new Vector2D(head.X, head.Y);
				transferPoint.X = -transferPoint.X;
				if (transferPoint.X < 0)
				{
					// Set the wallPassthrough to remove when needed.
					wallPassthrough = 2;
				}
				else
				{
					// Set the wallPassthrough to remove when needed.
					wallPassthrough = 3;
				}
				// Add the transferPoint to the body then create a new head with the transferPoint (x,y)
				this.body.Add(transferPoint);
				Vector2D newHead = new Vector2D(transferPoint.X, transferPoint.Y);
				this.body.Add(newHead);
			}

			// The following 'if' statements check and make sure the tail is removed correctly during a passthrough.
			if (this.body.Count > 2 && body[0].Y < (worldSize / 2) && body[1].Y > (worldSize / 2) && body[0].Y < body[1].Y && wallPassthrough == 0)
			{
				this.body.RemoveAt(0);
			}
			if (this.body.Count > 2 && body[0].Y > (worldSize / 2) && body[1].Y < (worldSize / 2) && body[1].Y < body[0].Y && wallPassthrough == 1)
			{
				this.body.RemoveAt(0);
			}
			if (this.body.Count > 2 && body[0].X < (worldSize / 2) && body[1].X > (worldSize / 2) && body[0].X < body[1].X && wallPassthrough == 3)
			{
				this.body.RemoveAt(0);
			}
			if (this.body.Count > 2 && body[0].X > (worldSize / 2) && body[1].X < (worldSize / 2) && body[1].X < body[0].X && wallPassthrough == 2)
			{
				this.body.RemoveAt(0);
			}

			// If the snake is NOT currently growing from a powerup...
			if (!isGrowing)
			{
				// Find the difference between the tail and nextTail Vector2Ds
				double difX = nextTail.X - tail.X;
				double difY = nextTail.Y - tail.Y;

				// Depending on difX and difY, move the tail to the next point
				if (difX < 0 && difY == 0)
				{
					tail.X = tail.X - (1 * speed);
				}
				else if (difX > 0 && difY == 0)
				{
					tail.X = tail.X - (-1 * speed);
				}
				else if (difY > 0 && difX == 0)
				{
					tail.Y = tail.Y - (-1 * speed);
				}
				else if (difY < 0 && difX == 0)
				{
					tail.Y = tail.Y - (1 * speed);
				}

				// If the tail has caught up to the next point, remove it from the body.
				if ((Math.Abs(tail.X - nextTail.X) < 1 && Math.Abs(tail.Y - nextTail.Y) < 1) && this.body.Count > 2)
				{
					this.body.RemoveAt(0);
				}
			}
		}

		/// <summary>
		/// This method is used to determine if a snake's head is colliding with a given object.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool isColliding(Object o)
		{
			// If o is a List of Vector2Ds
			if (o is List<Vector2D>)
			{
				// Initialize o as a List<Vector2D> and get the necessary values.
				List<Vector2D> oBody = (List<Vector2D>)o;
				Vector2D head = this.body[body.Count - 1];
				Vector2D prevSeg = oBody[0];

				// For every Vector2D in the list...
				foreach (Vector2D seg in oBody)
				{
					if ((seg.X + prevSeg.X == 0) || (seg.Y + prevSeg.Y == 0))
					{
						prevSeg = seg;
						continue;
					}
					// Check if the snake's head collides with segment.
					if (CreateAndCheckRec(seg, prevSeg, head))
						return true;
					prevSeg = seg;
				}
			}
			// If o is a Snake
			if (o is Snake)
			{
				// Initialize o as a Snake and get the necessary values.
				Snake s = (Snake)o;
				Vector2D head = this.body[body.Count - 1];
				Vector2D prevSeg = s.body[0];

				// For every segment in the Snake s body...
				foreach (Vector2D seg in s.body)
				{
					// Check if the snake's head collides with the current segment of the s snake.
					if (CreateAndCheckRec(seg, prevSeg, head))
						return true;
					prevSeg = seg;
				}
			}
			// If o is a Wall
			else if (o is Wall)
			{
				// Initialize o as a Wall and get the necessary values.
				Wall w = (Wall)o;
				Vector2D head = this.body[this.body.Count - 1];

				// Check if the snake's head is colliding with the current wall segment.
				if ((head.X > w.p1.X - 25 && head.X < w.p2.X + 25) && (head.Y > w.p1.Y - 25 && head.Y < w.p2.Y + 25) ||
					(head.X > w.p2.X - 25 && head.X < w.p1.X + 25) && (head.Y > w.p2.Y - 25 && head.Y < w.p1.Y + 25))
				{
					return true;
				}
			}
			// If o is a Powerup
			else if (o is Powerup)
			{
				// Initialize o as a Powerup and get the necessary values.
				Powerup p = (Powerup)o;
				Vector2D head = this.body[this.body.Count - 1];

				// Check if the snake's head is colliding with the current Powerup.
				if ((p.loc - head).Length() < 5 + 5)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This is a helper method used to create a "rectangle boundary" around two segments in order
		/// to check of the given head is in that boundary.
		/// </summary>
		/// <param name="seg"></param>
		/// <param name="prevSeg"></param>
		/// <param name="head"></param>
		/// <returns></returns>
		private bool CreateAndCheckRec(Vector2D seg, Vector2D prevSeg, Vector2D head)
		{
			// If the two segments are the same, return false.
			if (prevSeg.X == seg.X && prevSeg.Y == seg.Y)
				return false;
			else
			{
				// Find the minimum & maximum X & Y values.
				double minX = Math.Min(seg.X, prevSeg.X);
				double maxX = Math.Max(seg.X, prevSeg.X);
				double minY = Math.Min(seg.Y, prevSeg.Y);
				double maxY = Math.Max(seg.Y, prevSeg.Y);

				// If the given head is in the border, return true.
				if (isInRectangle(minX, maxX, minY, maxY, head))
					return true;
			}
			// Return false if it has gotten to this point.
			return false;
		}

		/// <summary>
		/// This method is used to check if a snake is colliding with itself.
		/// </summary>
		/// <returns></returns>
		public bool isSelfColliding()
		{
			// Initiialze the headDir Vector2D and create a variable for
			// a curDir Vector2D that will be set later on.
			Vector2D headDir = this.dir;
			Vector2D curDir;

			// Initialize a int used to determine the highest index in the snake's
			// body that it can self collide on.
			int highestValidCol = 0;

			// Loop through the snake from the head to the tail.
			for (int i = this.body.Count - 1; i > 0; i--)
			{
				// Get the current Vector2D and get the angle between it and the Vector2D
				// behind it.
				Vector2D cur = this.body[i];
				float angle = Vector2D.AngleBetweenPoints(cur, this.body[i - 1]);

				// Depending on the value of the angle float, set curDir to the direction
				// cur and the Vector2D before it are travelling in.
				if (angle == -90)
				{
					curDir = new Vector2D(1, 0);
				}
				else if (angle == 90)
				{
					curDir = new Vector2D(-1, 0);
				}
				else if (angle == 180)
				{
					curDir = new Vector2D(0, -1);
				}
				else
				{
					curDir = new Vector2D(0, 1);
				}

				// If curDir is the opposite of the snake's head direction, set highestValidCol to i and break the loop.
				if (((curDir.X != headDir.X) && (curDir.Y == headDir.Y)) || ((curDir.X == headDir.X) && (curDir.Y != headDir.Y)))
				{
					highestValidCol = i;
					break;
				}
			}

			// Create a list of all the Vector2Ds of the snake up
			// until highestValidCol.
			List<Vector2D> checkCol = new List<Vector2D>();
			for (int i = 0; i <= highestValidCol; i++)
			{
				checkCol.Add(this.body[i]);
			}

			// Check if the snake collidies with any of the segments in the checkCol list
			// and return the bool value of that check.
			return this.isColliding(checkCol);
		}


		/// <summary>
		/// This method is used to check where or not the given curPoint is in a rectangle border.
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="curPoint"></param>
		/// <returns></returns>
		private bool isInRectangle(double minX, double maxX, double minY, double maxY, Vector2D curPoint)
		{
			// If the curPoint is in the rectangle border, return true.
			if ((minX - 10 < curPoint.X && curPoint.X < maxX + 10) && (minY - 10 < curPoint.Y && curPoint.Y < maxY + 10))
				return true;
			else // If curPoint is NOT in the rectangle border, return false.
				return false;
		}

		/// <summary>
		/// This method is used to check if the snake's body is colliding with a given object.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="worldSize"></param>
		/// <returns></returns>
		public bool isBodyColliding(Object o, int worldSize)
		{
			// If o is a Snake.
			if (o is Snake)
			{
				// Initialize o as a Snake and get the necessary values.
				Snake s = (Snake)o;
				Vector2D prevPoint = s.body[0];
				Vector2D nTail = this.body[0];
				Vector2D nHead = this.body[1];

				// For every Vector2D in the snake's body...
				foreach (Vector2D point in s.body)
				{
					// Skip the first iteration to ensure it has two different points.
					if (prevPoint.Equals(point))
					{
						prevPoint = point;
						continue;
					}
					// If the bodyCollidingHelper returns true, do the same
					if (bodyCollidingHelper(nTail, nHead, prevPoint, point, worldSize))
						return true;

					// If the above lines don't return true, set prevPoint to point
					// and go onto the next iteration.
					prevPoint = point;
				}
			}
			// If o is a Wall
			else if (o is Wall)
			{
				// Initialize o as a Wall and get the necessary values.
				Wall w = (Wall)o;
				Vector2D nTail = this.body[0];
				Vector2D nHead = this.body[1];
				double segMinX = Math.Min(w.p1.X, w.p2.X);
				double segMaxX = Math.Max(w.p1.X, w.p2.X);
				double segMinY = Math.Min(w.p1.Y, w.p2.Y);
				double segMaxY = Math.Max(w.p1.Y, w.p2.Y);

				// If nHead overlaps with the segment, return true.
				if ((nHead.X > segMinX - 25 && nHead.X < segMaxX + 25) && (nHead.Y > segMinY - 25 && nHead.Y < segMaxY + 25))
					return true;

				// If nTail overlaps with the segment, return true.
				if ((nTail.X > segMinX - 25 && nTail.X < segMaxX + 25) && (nTail.Y > segMinY - 25 && nTail.Y < segMaxY + 25))
					return true;

				// If bodyCollidingHelper returns true, do the same.
				if (bodyCollidingHelper(nTail, nHead, w.p1, w.p2, worldSize))
					return true;
			}
			// If o is a Powerup
			else if (o is Powerup)
			{
				// Initialize o as a Powerup and get the necessary values.
				Powerup p = (Powerup)o;
				Vector2D nTail = this.body[0];
				Vector2D nHead = this.body[1];
				double snakeMinX = Math.Min(nTail.X, nHead.X);
				double snakeMaxX = Math.Max(nTail.X, nHead.X);
				double snakeMinY = Math.Min(nTail.Y, nHead.Y);
				double snakeMaxY = Math.Max(nTail.Y, nTail.Y);

				// If the location of the powerup overlaps with the snake, return true.
				if ((p.loc.X > snakeMinX && p.loc.X < snakeMaxX) &&
					(p.loc.Y > snakeMinY && p.loc.Y < snakeMaxY))
				{
					return true;
				}
			}
			// Return false if the program gets to the end of the method.
			return false;
		}

		/// <summary>
		/// This is a helper method used to clean up code and check whether a segment of a snake
		/// is overlapping with the given points.
		/// </summary>
		/// <param name="nTail"></param>
		/// <param name="nHead"></param>
		/// <param name="prevPoint"></param>
		/// <param name="point"></param>
		/// <param name="worldSize"></param>
		/// <returns></returns>
		private bool bodyCollidingHelper(Vector2D nTail, Vector2D nHead, Vector2D prevPoint, Vector2D point, int worldSize)
		{
			// Retreive the needed min/max values of the snake.
			double snakeMinX = Math.Min(nTail.X, nHead.X);
			double snakeMaxX = Math.Max(nTail.X, nHead.X);
			double snakeMinY = Math.Min(nTail.Y, nHead.Y);
			double snakeMaxY = Math.Max(nTail.Y, nTail.Y);

			// Retreive the needed min/max values of the segment.
			double segMinX = Math.Min(prevPoint.X, point.X);
			double segMaxX = Math.Max(prevPoint.X, point.X);
			double segMinY = Math.Min(prevPoint.Y, point.Y);
			double segMaxY = Math.Max(prevPoint.Y, point.Y);

			// If the snake is out of bounds of the world, return true.
			if (snakeMinX < -worldSize / 2 || snakeMaxX > worldSize / 2 || snakeMaxY > worldSize / 2 || snakeMinY < -worldSize / 2)
				return true;

			if (snakeMinX == snakeMaxX) // The new snake is vertical.
			{
				if (segMinY == segMaxY) // The compared segment is horizontal.
				{
					// If there is overlay between the vertical snake and horizontal segment, return true.
					if ((segMinY > snakeMinY && segMinY < snakeMaxY) &&
						(segMinX < snakeMinX && segMaxX > snakeMaxX))
					{
						return true;
					}
				}
			}
			else // The new snake is horizontal.
			{
				if (segMinX == segMaxX) // The compared segment is vertical.
				{
					// If there is overlay between the horizontal snake and vertical segment, return true.
					if ((snakeMinY > segMinY && snakeMinY < segMaxY) &&
						(snakeMinX < segMinX && snakeMaxX > segMaxX))
					{
						return true;
					}
				}
			}
			// Return false if the program gets to the end of the method.
			return false;
		}
	}
}