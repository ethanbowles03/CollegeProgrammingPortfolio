package game;

import java.awt.Graphics;

public class Hitmarker implements Animatable {

	private int x, y;
	private double age;
	private GameState state;
	
	public Hitmarker(GameState state, int x, int y) {
		this.x = x;
		this.y = y;
		this.state = state;
		age = 0;
	}
	
	@Override
	public void update(double timeElapsed) {
		age += timeElapsed;
		if(age > 0.2) {
			state.removeGameObject(this);
		}
	}

	@Override
	public void draw(Graphics g, GameView v) {
		// TODO Auto-generated method stub
		v.drawCenteredImage(g,"Hitmarker.png", (int) x, (int) y, 0.04);
	}

}
