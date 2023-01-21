package game;

import java.awt.Graphics;
import java.io.File;
import java.io.IOException;
import java.awt.Color;
import java.awt.Font;
import java.awt.FontFormatException;
import java.awt.GraphicsEnvironment;

public class Menu implements Animatable {
	private GameState state;
	private boolean objectsAdded;
	private GraphicsEnvironment ge;
	private Font pixellari;

	public Menu(GameState state) {
		this.state = state;

		this.objectsAdded = false;

		try {
			ge = GraphicsEnvironment.getLocalGraphicsEnvironment();
			pixellari = Font
					.createFont(Font.TRUETYPE_FONT,
							getClass().getClassLoader().getResourceAsStream("resources/Pixellari.ttf"))
					.deriveFont(Font.PLAIN, 35f);
			ge.registerFont(pixellari);

		} catch (IOException | FontFormatException e) {
			System.out.println(e);
		}
	}

	@Override
	public void update(double timeElapsed) {
		// Adds the objects if they have not been added and the game is not over
		if (!objectsAdded && state.getLives() > -1) {
			state.addGameObject(new TowerMenuOil(state, 450, 510));
			state.addGameObject(new TowerMenuWater(state, 454, 550));
		}
	}

	@Override
	public void draw(Graphics g, GameView v) {

		// Draws the menu overlay
		g.drawImage(ResourceLoader.getLoader().getImage("MenuOverlay.png"), 0, 0, null);

		// Set the font
		g.setFont(pixellari);

		// Show the score
		int score = state.getScore();
		g.drawString("" + score, 190, 73);

		// Show the amount of lives
		int lives = state.getLives();
		if (lives < 0) {
			lives = 0;
		}
		g.drawString("" + lives, 500, 73);

		// Show the amount of credits
		if (state.getCredits() == 0) {
			g.setColor(new Color(174, 204, 72));
			g.drawString("OUT OF CREDITS", 150, 539);
			g.drawString(": 10", 487, 540);
			g.drawString(": 5", 487, 585);
		} else {
			int credits = state.getCredits();
			g.setColor(new Color(174, 204, 72));
			g.drawString("" + credits, 150, 539);
			g.drawString(": 10", 487, 540);
			g.drawString(": 5", 487, 585);
		}
	}
}
