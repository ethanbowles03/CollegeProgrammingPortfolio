/*
. * Author: Ethan Bowles and Conner Fisk
 * Date: Tuesday, December 7, 2021
 * 
 * Class to control the background music of the game
 * Pause and Start Capabilities
 */

package wordle;

import java.io.File;
import java.io.IOException;

import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.sound.sampled.LineUnavailableException;
import javax.sound.sampled.UnsupportedAudioFileException;

public class MusicPlayer {
	//Fields
	private Clip clip;
	private long clipTime;

	/**
	 * Sets up the audio clip to be played and sets the time in the song to
	 * be 0
	 */
	public MusicPlayer(String name) {
		//Set up fields
		clipTime = 0;
		
		//Sets up the clip to be played
		File sound = new File("src/wordle/" + name);
	    try {
			clip = AudioSystem.getClip();
			clip.open(AudioSystem.getAudioInputStream(sound));
		} catch (LineUnavailableException | IOException | UnsupportedAudioFileException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	/**
	 * Plays the music starting from the time it was last played
	 */
	public void playMusic() {
		//Plays starting at last stopped
		 clip.setMicrosecondPosition(clipTime);
		 clip.start();
		 
		 //Loops
	     clip.loop(Clip.LOOP_CONTINUOUSLY);
	}
	
	/**
	 * Pause the game music and store the time of the stop
	 */
	public void pauseMusic() {
		//Stops and stores microseconds
		clipTime= clip.getMicrosecondPosition();
		clip.stop();
	}
}
