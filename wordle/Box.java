package wordle;

import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;

public class Box {
	private int x,y,w,h;
	private String letter;
	private Color boxColor;
	
	public Box(int x, int y, int w, int h) {
		this.x =x;
		this.y = y;
		this.w = w;
		this.h = h;
		this.letter = "";
	}
	
	public void setBoxColor(Color color) {
		this.boxColor = color;
	}
	
	public void setLetter(String letter) {
		this.letter = letter.toUpperCase();
	}
	
	public String getLetter() {
		return this.letter;
	}
	
	public void drawBoxes(Graphics g, boolean test, boolean finished, int lineY, int yPointer) {
		if(finished && yPointer < lineY) {
			g.setColor(this.boxColor);
			g.fillRect(x, y, w, h);
			g.setColor(Color.BLACK);
			g.drawRect(x, y, w, h);
			g.setFont(new Font("Times New Roman", Font.BOLD, 40));
			g.drawString(this.letter,x + 10,y + 40);
		}else {
			if(test) {
				g.setColor(Color.RED);
				g.drawRect(x, y, w, h);
				g.setColor(Color.BLACK);
				g.setFont(new Font("Times New Roman", Font.BOLD, 40));
				g.drawString(this.letter,x + 10,y + 40);
			}else {
				g.setColor(Color.BLACK);
				g.drawRect(x, y, w, h);
				g.setFont(new Font("Times New Roman", Font.BOLD, 40));
				g.drawString(this.letter,x + 10,y + 40);
			}
		}
	}
}
