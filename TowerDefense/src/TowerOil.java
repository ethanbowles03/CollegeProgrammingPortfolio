package game;

import java.awt.Graphics;
import java.awt.Point;

public class TowerOil implements Animatable {
	private GameState state;
	private int x, y, oilWidth, oilHeight;
	private double timeSinceLastShot;

	public TowerOil(GameState state, int x, int y) {
		this.state = state;
		this.x = x;
		this.y = y;
		this.oilWidth = 24;
		this.oilHeight = 36;
		
		timeSinceLastShot = 0;
	}

	@Override
	public void update(double timeElapsed) {
		// Shoot oil at nearby enemies
		// Shoot oil at nearby enemies
		
				//See if enough time has pasted to fire
				
				timeSinceLastShot += timeElapsed;
				
				if(timeSinceLastShot < 1) {
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
				
				EffectOil eo = new EffectOil(state, towerPoint, e.getPosition());
				
				state.addGameObject(eo);
				
				timeSinceLastShot = 0;
	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("OilBarrel.png"), x, y, oilWidth, oilHeight, null);
	}

}
