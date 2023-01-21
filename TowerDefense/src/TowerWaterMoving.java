package game;

import java.awt.Graphics;

public class TowerWaterMoving implements Animatable {
	private GameState state;
	private int waterWidth, waterHeight;

	public TowerWaterMoving(GameState state) {
		this.state = state;
		waterWidth = 15;
		waterHeight = 40;
	}

	@Override
	public void update(double timeElapsed) {
		if (state.isMouseClicked()) {
			state.removeGameObject(this);

			// Check to make sure it out of the menu area before placing
			if (state.validMousePos()) {
				state.addGameObject(new TowerWater(state, state.getMouseX()-waterWidth/2, state.getMouseY()-waterHeight/2));
				state.addCredits(-5);
			}
		}

	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("WaterBottle.png"), state.getMouseX()-waterWidth/2, state.getMouseY()-waterHeight/2, waterWidth,
				waterHeight, null);
	}

}
