// Authors: Conner Fisk, Ethan Bowles
// Class that contains the panel that displays all the game info
// Date: Nov 15, 2022

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using IImage = Microsoft.Maui.Graphics.IImage;
#if MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#else
using Microsoft.Maui.Graphics.Win2D;
#endif
using Color = Microsoft.Maui.Graphics.Color;
using System.Reflection;
using Microsoft.Maui;
using System.Net;
using Font = Microsoft.Maui.Graphics.Font;
using SizeF = Microsoft.Maui.Graphics.SizeF;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Cryptography;
using WinRT;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Maui.Graphics.Text;
using Windows.Media.Devices;
using System.Diagnostics.Metrics;

namespace SnakeGame;
public class WorldPanel : IDrawable
{
    // A delegate for DrawObjectWithTransform
    // Methods matching this delegate can draw whatever they want onto the canvas  
    public delegate void ObjectDrawer(object o, ICanvas canvas);

    //Image fields
    private IImage wall;
    private IImage background;
    private IImage explosion;

    //Field used for explosion animationi
    private int counter = 0;

    //World object
    private World theWorld;

    //Player id
    private int pId;

    //Grahpics view and initial drawings
    private GraphicsView graphicsView = new();
    private bool initializedForDrawing = false;

    /// <summary>
    /// Path finder for the IImage
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
#if MACCATALYST
    private IImage loadImage(string name)
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string path = "SnakeGame.Resources.Images";
        return PlatformImage.FromStream(assembly.GetManifestResourceStream($"{path}.{name}"));
    }
#else
    private IImage loadImage(string name)
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string path = "SnakeGame.Resources.Images";
        var service = new W2DImageLoadingService();
        return service.FromStream(assembly.GetManifestResourceStream($"{path}.{name}"));
    }
#endif

    /// <summary>
    /// Sets the world to be used
    /// </summary>
    /// <param name="w"></param>
    public void SetWorld(World w)
    {
        theWorld = w;
    }

    /// <summary>
    /// Sets the player ID to be used
    /// </summary>
    /// <param name="id"></param>
    public void SetPId(int id)
    {
        pId = id;
    }

    /// <summary>
    /// Sets up the imagees in the files to be drawen
    /// </summary>
    private void InitializeDrawing()
    {
        wall = loadImage("WallSprite.png");
        background = loadImage("Background.png");
        explosion = loadImage("Explosion.png");
        initializedForDrawing = true;
    }

    /// <summary>
    /// This method performs a translation and rotation to draw an object.
    /// </summary>
    /// <param name="canvas">The canvas object for drawing onto</param>
    /// <param name="o">The object to draw</param>
    /// <param name="worldX">The X component of the object's position in world space</param>
    /// <param name="worldY">The Y component of the object's position in world space</param>
    /// <param name="angle">The orientation of the object, measured in degrees clockwise from "up"</param>
    /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
    private void DrawObjectWithTransform(ICanvas canvas, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
    {
        // "push" the current transform
        canvas.SaveState();

        canvas.Translate((float)worldX, (float)worldY);
        canvas.Rotate((float)angle);
        drawer(o, canvas);

        // "pop" the transform
        canvas.RestoreState();
    }

    /// <summary>
    /// Main draw method that is called every frame
    /// Sets up the images to be drawen
    /// Then loops through all the snake, walls and powerups
    /// and redraws all of them in the correct position on the screen
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (!initializedForDrawing)
            InitializeDrawing();

        canvas.ResetState();

        if (theWorld != null)
        {
            //sets the location of the canvas view to be centered around the player snake
            if (theWorld.Snakes.ContainsKey(pId))
            {
                Snake mySnake = theWorld.Snakes[pId];
                List<Vector2D> mySnakeBody = mySnake.body;

                float playerX = (float)mySnakeBody[mySnakeBody.Count - 1].X;
                float playerY = (float)mySnakeBody[mySnakeBody.Count - 1].Y;

                canvas.Translate(-playerX + (900 / 2), -playerY + (900 / 2));
            }
            canvas.DrawImage(background, -theWorld.Size / 2, -theWorld.Size / 2, theWorld.Size, theWorld.Size);
            lock (theWorld)
            {
                //Loops through the walls and draws them
                foreach (var wall in theWorld.Walls.Values)
                {
                    // For vertical walls...
                    if (wall.p1.X == wall.p2.X)
                    {
                        //Checks which point is the top
                        if (wall.p1.Y > wall.p2.Y)
                        {
                            double difference = wall.p1.Y - wall.p2.Y;
                            for (int i = 0; i <= (int)difference / 50; i++)
                            {
                                DrawObjectWithTransform(canvas, wall, wall.p1.X, wall.p1.Y - (i * 50), 0, WallDrawer);
                            }
                        }
                        else
                        {
                            double difference = wall.p2.Y - wall.p1.Y;
                            for (int i = 0; i <= (int)difference / 50; i++)
                            {
                                DrawObjectWithTransform(canvas, wall, wall.p2.X, wall.p2.Y - (i * 50), 0, WallDrawer);
                            }
                        }
                    }
                    else //Horizontal walls
                    {
                        //Checks which point is the right
                        if (wall.p1.X > wall.p2.X)
                        {
                            double difference = wall.p1.X - wall.p2.X;
                            for (int i = 0; i <= (int)difference / 50; i++)
                            {
                                DrawObjectWithTransform(canvas, wall, wall.p1.X - (i * 50), wall.p1.Y, 0, WallDrawer);
                            }
                        }
                        else
                        {
                            double difference = wall.p2.X - wall.p1.X;
                            for (int i = 0; i <= (int)difference / 50; i++)
                            {
                                DrawObjectWithTransform(canvas, wall, wall.p2.X - (i * 50), wall.p2.Y, 0, WallDrawer);
                            }
                        }
                    }
                }

                //Draws all of the snakes in the frame
                foreach (var snake in theWorld.Snakes.Values)
                {
                    if (snake.alive == false)
                    {
                        //Draw Explosion
                        List<Vector2D> snakeSegments = snake.body;

                        Vector2D head = snakeSegments[snakeSegments.Count - 1];

                        canvas.DrawImage(explosion, (float)head.X - (25 + counter) / 2, (float)head.Y - (25 + counter) / 2, 25 + counter, 25 + counter);

                        if (counter < 75)
                        {
                            counter += 2;
                        }

                        continue;
                    }
                    else
                    {
                        counter = 0;
                    }

                    //Gets the color and sends it
                    int snakeCol = snake.snake % 10;
                    Color snakeColor = findSnakeColor(snakeCol);
                    canvas.StrokeColor = snakeColor;

                    //Loops through the segments of the snake and draws them
                    List<Vector2D> segments = snake.body;
                    Vector2D prev = segments[0];

                    foreach (Vector2D curVec in segments)
                    {
                        //Finds the difference between the 2 vectors and normalizes it
                        Vector2D vec = prev - curVec;
                        int length = (int)vec.Length();
                        vec.Normalize();

                        //Doesnt draw the segment if it is across the world
                        if (length < theWorld.Size)
                        {
                            //Draws the segment in the direction of the angle
                            float ang = vec.ToAngle();
                            DrawObjectWithTransform(canvas, length, curVec.X, curVec.Y, ang, SnakeDrawer);
                        }
                        prev = curVec;
                    }

                    //Draws the name of the player below the snake
                    canvas.FontColor = Colors.White;
                    canvas.FontSize = 12;
                    canvas.DrawString(snake.name + ": " + snake.score.ToString(), (float)prev.X, (float)prev.Y + 20, HorizontalAlignment.Center);
                }

                //Loops through and draws all of the powerups
                foreach (var power in theWorld.Powerups.Values)
                {

                    if (!power.died)
                    {
                        Vector2D location = power.loc;
                        DrawObjectWithTransform(canvas, power, location.X, location.Y, 0, PowerupDrawer);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Helper method to draw the walls on the screen in the correct location
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void WallDrawer(object o, ICanvas canvas)
    {
        float width = 50;
        float height = 50;

        canvas.DrawImage(wall, -width / 2, -height / 2, width, height);
    }

    /// <summary>
    /// Helper method to draw each segment of the snake
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void SnakeDrawer(object o, ICanvas canvas)
    {
        int snakeSegmentLength = (int)o;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeSize = 10;
        canvas.DrawLine(0, 0, 0, -snakeSegmentLength);
    }

    /// <summary>
    /// Helper method that draws the powerup in the correct location
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void PowerupDrawer(object o, ICanvas canvas)
    {
        int width = 16;
        canvas.FillColor = Colors.OrangeRed;

        // Ellipses are drawn starting from the top-left corner.
        // So if we want the circle centered on the powerup's location, we have to offset it
        // by half its size to the left (-width/2) and up (-height/2)
        canvas.FillEllipse(-(width / 2), -(width / 2), width, width);
    }

    /// <summary>
    /// Helper method which draws a different color for every 10 snakes
    /// </summary>
    /// <param name="mod10"></param>
    /// <returns></returns>
    private Color findSnakeColor(int mod10)
    {
        if (mod10 == 1)
            return Colors.Red;
        if (mod10 == 2)
            return Colors.Orange;
        if (mod10 == 3)
            return Colors.LemonChiffon;
        if (mod10 == 4)
            return Colors.Green;
        if (mod10 == 5)
            return Colors.Blue;
        if (mod10 == 6)
            return Colors.Purple;
        if (mod10 == 7)
            return Colors.BlanchedAlmond;
        if (mod10 == 8)
            return Colors.Brown;
        if (mod10 == 9)
            return Colors.Black;
        else
            return Colors.Pink;
    }
}