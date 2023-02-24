package wordle;

import java.awt.Color;
import java.awt.Point;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.io.File;
import java.io.FileNotFoundException;
import java.util.Arrays;
import java.util.Random;
import java.util.Scanner;

import javax.swing.JFrame;

public class Wordle extends JFrame implements KeyListener{
	private WordlePanel panel;
	private File file;
	private Scanner fileReader;
	private String[] words;
	private String answer;
	
	public Wordle() {
		setUpWordsList();
		setUpAnswer();
		setFrame();
		setContentPane(panel);
		addKeyListener(this);
		
		setVisible(true);
	}

	public static void main(String[] args) {
		new Wordle();
	}
	
	private void setUpWordsList(){
		this.file = new File("src/wordle/WordsTyped.txt");
		try {
			this.fileReader = new Scanner(this.file);
			int wordCount = 0;
			while(fileReader.hasNextLine()) {
				fileReader.nextLine();
				wordCount++;
			}
			this.words = new String[wordCount];
			wordCount = 0;
			this.fileReader = new Scanner(this.file);
			while(fileReader.hasNextLine()) {
				words[wordCount] = fileReader.nextLine();
				wordCount++;
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}
	
	private void setUpAnswer(){
		Random number = new Random();
		int wordCount = 0;
		this.answer = "";
		this.file = new File("src/wordle/Answers.txt");
		try {
			this.fileReader = new Scanner(this.file);
			while(fileReader.hasNextLine()) {
				fileReader.nextLine();
				wordCount++;
			}
			
			int rng = number.nextInt(wordCount);
			wordCount = 0;
			this.fileReader = new Scanner(this.file);
			while(fileReader.hasNextLine()) {
				if(wordCount == rng) {
					answer = fileReader.next();
					wordCount++;
				}else {
					fileReader.next();
					wordCount++;
				}
			}
			System.out.println(answer);
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}
	
	private void setFrame() {
		this.panel = new WordlePanel(50,50, words, answer);
		this.panel.setBackground(Color.LIGHT_GRAY);
		setBackground(Color.LIGHT_GRAY);
		setSize(500,700);
		setResizable(false);
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setLocation(new Point(500,500));
	}

	@Override
	public void keyPressed(KeyEvent e) {
		int code = e.getKeyCode();
		if (code==KeyEvent.VK_ENTER){
            panel.updateLine();
        }else if(code==KeyEvent.VK_BACK_SPACE){
        	panel.delete();
        }else if(code >= 'A' && code <= 'Z'){
        	panel.updateBox(String.valueOf(e.getKeyChar()));
        }
	}

	@Override
	public void keyTyped(KeyEvent e) {}
	
	@Override
	public void keyReleased(KeyEvent e) {}

}
