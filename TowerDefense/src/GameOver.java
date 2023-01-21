package game;

import java.awt.Graphics;

public class GameOver implements Animatable {

	@Override
	public void update(double timeElapsed) {
		// TODO Auto-generated method stub

	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("GameOver.png"), 0, 0, null);
	}
}
