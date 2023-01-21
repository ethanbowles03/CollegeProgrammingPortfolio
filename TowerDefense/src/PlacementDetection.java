package game;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

public class PlacementDetection {
	private String fileName;
	public PlacementDetection(String fileName) {
		this.fileName = fileName;
	}
	
	public boolean checkColor(int mouseX, int mouseY){
		System.out.println(mouseX + " : " + mouseY);
		BufferedImage img;
		try {
			img = ImageIO.read(getClass().getResource("/resources/" + fileName));
			int pixel = img.getRGB(mouseX,mouseY);
			Color color = new Color(pixel, true);
	        //Retrieving the R G B values
	        int green = color.getGreen();
	        int blue = color.getBlue();
	        if(blue == 0 && green == 0) {
	        	return true;
	        }else {
	        	return false;
	        }
		} catch (IOException e) {
			// TODO Auto-generated catch block
			System.out.println(e);
			return false;
		}
	}
}
