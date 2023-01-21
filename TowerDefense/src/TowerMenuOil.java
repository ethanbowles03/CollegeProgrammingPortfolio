package game;

import java.awt.Graphics;

public class TowerMenuOil implements Animatable {
	private GameState state;
	private int x, y;

	public TowerMenuOil(GameState state, int x, int y) {
		this.state = state;
		this.x = x;
		this.y = y;
	}

	@Override
	public void update(double timeElapsed) {
		// Make sure there are enough credits
		if (state.getCredits() >= 10) {
			if (state.getMouseX() >= x && state.getMouseX() < x + 50 && state.getMouseY() >= y
					&& state.getMouseY() < y + 50 && state.isMouseClicked()) {
				state.addGameObject(new TowerOilMoving(state));
				state.setMouseClicked(false);
			}
		}
	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("OilBarrel.png"), x, y, 24, 36, null);
	}

}
