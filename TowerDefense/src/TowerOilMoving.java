package game;

import java.awt.Graphics;

public class TowerOilMoving implements Animatable {
	private GameState state;
	private int oilWidth, oilHeight;

	public TowerOilMoving(GameState state) {
		this.state = state;
		this.oilWidth = 24;
		this.oilHeight = 36;
	}

	@Override
	public void update(double timeElapsed) {
		if (state.isMouseClicked()) {
			state.removeGameObject(this);

			// Check to make sure it out of the menu area before placing
			if (state.validMousePos()) {
				state.addGameObject(new TowerOil(state, state.getMouseX()-oilWidth/2, state.getMouseY()-oilHeight/2));
				state.addCredits(-10);
			}
		}
	}

	@Override
	public void draw(Graphics g, GameView v) {
		g.drawImage(ResourceLoader.getLoader().getImage("OilBarrel.png"), state.getMouseX()-oilWidth/2, state.getMouseY()-oilHeight/2, oilWidth,
				oilHeight, null);
	}

}
