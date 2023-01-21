package game;

import java.io.File;
import java.io.IOException;

import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.sound.sampled.LineUnavailableException;
import javax.sound.sampled.UnsupportedAudioFileException;

public class MusicPlayer {
	private Clip clip;
	private long clipTime;

	public MusicPlayer() {
		clipTime = 0;
		File sound = new File(getClass().getResource("/resources/F!.wav").getFile());
	    try {
			clip = AudioSystem.getClip();
			clip.open(AudioSystem.getAudioInputStream(sound));
		} catch (LineUnavailableException | IOException | UnsupportedAudioFileException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void playMusic() {
		 clip.setMicrosecondPosition(clipTime);
		 clip.start();
	     clip.loop(Clip.LOOP_CONTINUOUSLY);
	}
	
	public void pauseMusic() {
		clipTime= clip.getMicrosecondPosition();
		clip.stop();
	}
}
