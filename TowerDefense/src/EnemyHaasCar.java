package game;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.geom.AffineTransform;
import java.awt.image.BufferedImage;

public class EnemyHaasCar extends Enemy {

	// Fields
	private double degree;
	private GameState state;

	// Builds the HaasCar object with a specified position
	public EnemyHaasCar(double p, GameState state) {
		super(p);
		degree = 0;
		anchorX = 0;
		anchorY = 0;
		percentTravelled = p;
		this.state = state;
	}

	/**
	 * Rotates an image a certain number of degrees
	 * 
	 * @param icon   - takes in the picture/icon that needs to be rotated
	 * @param degree - the degree which to rotate the image
	 * @return - returns the rotated image
	 */
	public BufferedImage rotateIcon(BufferedImage icon, Double degree) {
		// Variables
		double sin, cos;
		int width, height, iconWidth, iconHeight;

		// Get Image Width and Height
		iconWidth = icon.getWidth();
		iconHeight = icon.getHeight();

		// Finds the sin and cos value then gets the new width and height of the icon
		sin = Math.abs(Math.sin(Math.toRadians(degree)));
		cos = Math.abs(Math.cos(Math.toRadians(degree)));
		width = (int) Math.round(iconWidth * cos + iconHeight * sin);
		height = (int) Math.round(iconWidth * sin + iconHeight * cos);

		// Creates new image with the new width and height
		BufferedImage rotatedIcon = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		Graphics2D g = rotatedIcon.createGraphics();

		// Find the point of rotation
		anchorX = (int) ((width - iconWidth) / 2);
		anchorY = (int) ((height - iconHeight) / 2);

		iconWidth /= 2;
		iconHeight /= 2;

		// Rotates the image around the anchor and draws it on the screen
		AffineTransform a = new AffineTransform();
		a.setToRotation(Math.toRadians(degree), anchorX + iconWidth, anchorY + iconHeight);
		a.translate(anchorX, anchorY);
		g.setTransform(a);
		g.drawImage(icon, 0, 0, null);
		g.dispose();

		return rotatedIcon;
	}

	@Override
	public void update(double timeElapsed) {
		percentTravelled = percentTravelled + 0.1 * timeElapsed;
		if (percentTravelled > 1) {
			state.removeGameObject(this);
			state.addLives(-1);
		}
	}

	@Override
	public void draw(Graphics g, GameView v) {

		// Checks the percent the car is at and makes sure it doesn't crash
		if (percentTravelled < 1) {
			// Gets the position on the path
			Point objPoint = ResourceLoader.getLoader().getPath("path.txt").getPathPosition(percentTravelled);

			// Gets the degree of the car in the path
			degree = ResourceLoader.getLoader().getPath("path.txt").getDegree(percentTravelled);

			// Gets the image of the car and then rotates it
			BufferedImage carImage = ResourceLoader.getLoader().getImage("HaasCar.png");
			BufferedImage car = rotateIcon(carImage, degree - 90);

			// Find the center point where the car is located and then draws image
			int xPos = (int) (objPoint.getX() - anchorX - 8);
			int yPos = (int) (objPoint.getY() - anchorY - 20);

			g.drawImage(car, xPos, yPos, null);
		}

	}

}
