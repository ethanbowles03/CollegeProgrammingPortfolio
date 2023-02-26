package wordle;

import java.awt.*;
import java.util.Arrays;
import java.util.Random;

import javax.swing.*;

public class WordlePanel extends JPanel{
	private int x,y,left;
	private Box[][] boxes;
	private int pointerX, pointerY;
	private String character, answer;
	private String[] userWords;
	private boolean finshedLine, win;
	private MusicPlayer mp;
	
	public WordlePanel(int startX, int startY, String[] words, String answer) {
		this.x = startX;
		this.y = startY;
		this.left = startX;
		this.pointerX = 0;
		this.pointerY = 0;
		this.character = "";
		this.userWords = words;
		this.answer = answer;
		this.finshedLine = false;
		this.win = false;
		
		mp = new MusicPlayer("waiting-music-116216.wav");
		mp.playMusic();
		
		setUpBoxes();
	}
	
	private void setUpBoxes() {
		boxes = new Box[6][6];
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 6; j++) {
				boxes[i][j] = new Box(x,y,50,50);
				x += 87;
  			}
			x = left;
			y += 87;
		}
	}
	
	private void paintBoxes(Graphics g, int indexX, int indexY, boolean finished) {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 5; j++) {
				if(i == indexY && j == indexX) {
					boxes[i][j].setLetter(this.character);
					boxes[i][j].drawBoxes(g, true, finished, pointerY, i);
				}else {
					boxes[i][j].drawBoxes(g, false, finished, pointerY, i);
				}
			}
		}
	}
	
	private void updatePointer() {
		if(pointerX < 5) {
			pointerX++;
		}
	}
	
	public void updateLine() {
		if(pointerX == 5) {
			boolean check = false;
			String word = checkWord().toLowerCase();
			for(int i = 0; i < this.userWords.length; i++) {
				if(this.userWords[i].equals(word)) {
					check = true;
				}
			}
			if(check) {
				if(checkWin(word)) {
					win = true;
					repaint();
				}else {
					char[] chArr = word.toCharArray();
					setBoxColors(chArr);
					pointerX = 0;
					pointerY++;
					Random rng = new Random();
					repaint();
				}
			}else {
				System.out.println("Not a real word");
			}
		}
		if(pointerY == 6) {
			System.out.println("You lose");
			System.exit(0);
		}
	}
	
	private void setBoxColors(char[] arr) {
		boolean[] containsList = new boolean[5];
		Color[] boxColors = new Color[5];
		char[] answerArr = this.answer.toLowerCase().toCharArray();
		for(int i = 0; i < 5; i++) {
			if(answerArr[i] ==  arr[i]) {
				boxColors[i] = Color.GREEN;
			}else {
				for(int j = 0; j < 5; j++) {
					if(answerArr[i] == arr[j]) {
						containsList[j] = true;
					}
				}
			}
		}
		for(int i = 0; i < 5; i++) {
			if(boxColors[i] ==  null) {
				boxColors[i] = Color.LIGHT_GRAY;
			}
		}
		for(int i = 0; i < 5; i++) {
			if(containsList[i]) {
				boxColors[i] = Color.YELLOW;
				boxes[pointerY][i].setBoxColor(boxColors[i]);
			}else {
				boxes[pointerY][i].setBoxColor(boxColors[i]);
			}
		}
		this.finshedLine = true;
	}
	
	public void updateBox(String a) {
		boxes[pointerY][pointerX].setLetter(a);
		updatePointer();
		this.repaint();
	}
	
	private String checkWord() {
		String word = "";
		for(int i = 0; i < 5; i++) {
			word += boxes[pointerY][i].getLetter();
		}
		return word;
	}
	
	private boolean checkWin(String userIn) {
		if(userIn.equals(this.answer.toLowerCase())) {
			return true;
		}
		return false;
	}
	
	public void delete() {
		if(pointerX > 0) {
			pointerX--;
			boxes[pointerY][pointerX].setLetter("");
			repaint();
		}
	}
	
	public void paint(Graphics g) {
		if(win) {
			setBackground(Color.GREEN);
			g.setFont(new Font("Times New Roman", Font.BOLD, 50));
			g.drawString("You win", 75,300);
		}else {
			paintBoxes(g, this.pointerX, this.pointerY, this.finshedLine);
			g.drawLine(0, 575, 500, 575);
		}
	}
}
