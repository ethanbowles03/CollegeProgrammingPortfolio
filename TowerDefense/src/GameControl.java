/**
 * This class holds the constructor and methods for the GameControl object.
 * 
 * @author Conner Fisk & Ethan Bowles
 * @date December 7, 2021
 */
package game;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.Timer;

public class GameControl implements Runnable, ActionListener {

	// Fields
	private GameView view;
	private GameState state;
	private int count, carCount, gameOverCount;
	private long previousTime;
	private boolean isStarted;
	private Timer timer;

	/**
	 * Constructor for the GameControl object
	 */
	public GameControl() {
		isStarted = false;
	}

	/**
	 * Builds the GameState and "runs" the game
	 */
	public void run() {

		// Build the game state.

		state = new GameState();

		// Build a view.

		view = new GameView(state);

		// Adds a menu and a HaasCar to start
		state.addGameObject(new Menu(state));
		state.addGameObject(new EnemyHaasCar(0, state));

		// Starts timer
		timer = new Timer(16, this);
		timer.start();

		// Keep track of the start time of the first tick
		previousTime = System.nanoTime();

		count = 0;
	}

	/**
	 * Called whenever an action event happens, and we are listening to that event.
	 * The timer automatically sets us 'this' up as the listener above.
	 * 
	 * @param e -> ActionEvent object
	 */
	@Override
	public void actionPerformed(ActionEvent e) {
		if (state.isGameStarted()) {
			// Checks to see if the game is over
			if (state.getLives() < 0 && gameOverCount < 1) {
				long currentTime = System.nanoTime();
				double elapsedTime = (currentTime - previousTime) / 1_000_000_000.0;
				// Updates all objects
				state.updateAll(elapsedTime);
				// Removes all objects
				state.deleteGame();
				// Adds only a GameOver object
				state.addGameObject(new GameOver());
				// Paints the GameOver object
				view.repaint();
				// Update the gameOverCount
				gameOverCount = 1;
			} else { // Continues if it is not
				if (gameOverCount != 1) {
					count++;
					if (count % 28 == 0) {
						// Adds one truck for every three cars
						if (carCount % 3 == 0) {
							state.addGameObject(new EnemyHaasTruck(0, state));
							carCount++;
						} else {
							state.addGameObject(new EnemyHaasCar(0, state));
							carCount++;
						}
					}
				}

				// Calculate elapsed time since last update

				long currentTime = System.nanoTime();
				double elapsedTime = (currentTime - previousTime) / 1_000_000_000.0;

				previousTime = currentTime;

				// Update the game objects

				state.updateAll(elapsedTime);

				// Make sure the mouse click is consumed
				state.consumeMouseClick();

				// Draw the game objects

				view.repaint();

			}
		}
	}
}