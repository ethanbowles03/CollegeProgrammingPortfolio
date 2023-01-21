package game;

import java.awt.Point;

abstract public class Enemy implements Animatable {
	protected int anchorX, anchorY;
	protected double percentTravelled, position;

	public Enemy(double p) {
		position = p;
	}

	public Point getPosition() {
		Point objPoint = ResourceLoader.getLoader().getPath("path.txt").getPathPosition(percentTravelled);
		System.out.println(position);
		int xPos = (int) (objPoint.getX() - anchorX - 8);
		int yPos = (int) (objPoint.getY() - anchorY - 20);

		Point currentPos = new Point(xPos, yPos);

		return objPoint;
	}
}
