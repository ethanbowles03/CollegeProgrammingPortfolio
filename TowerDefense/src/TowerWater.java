package game;

import java.awt.Graphics;
import java.awt.Point;

public class TowerWater implements Animatable {
	private GameState state;
	private int x, y, waterWidth, waterHeight;
	private double timeSinceLastShot;

	public TowerWater(GameState state, int x, int y) {
		this.state = state;
		this.x = x;
		this.y = y;
		this.waterWidth = 15;
		this.waterHeight  = 40;
		
		timeSinceLastShot = 0;
	}

	@Override
	public void update(double timeElapsed) {
		// Shoot oil at nearby enemies
		
		//See if enough time has pasted to fire
		
		timeSinceLastShot += timeElapsed;
		
		if(timeSinceLastShot < 1.5) {
			return;
		}
		
		//Find Nearest Enemy
		Point towerPoint = new Point(x,y);
		
		Enemy e = state.findNearestEnemy(towerPoint);
		
		if(e == null) {
			return;
		}
		
		if(towerPoint.distance(e.getPosition()) > 100) {
			return;
		}
		
		EffectWater ew = new EffectWater(state, towerPoint, e.getPosition());
		
		state.addGameObject(ew);
		
		timeSinceLastShot = 0;
	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("WaterBottle.png"), x, y, waterWidth, waterHeight, null);
	}

}
