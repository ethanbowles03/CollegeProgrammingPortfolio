package game;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;
import java.util.TreeMap;

public class ResourceLoader {
	// Fields
	private Map<String, BufferedImage> map;
	static private ResourceLoader instance;

	static public ResourceLoader getLoader() {
		if (instance == null) {
			instance = new ResourceLoader();
		}
		return instance;
	}

	private ResourceLoader() {
		map = new HashMap<String, BufferedImage>();
	}

	public BufferedImage getImage(String name) {
		// Sets the name to the correct folder location
		String locationName = "resources/" + name;

		// Checks to see if the image is already in the map
		if (map.containsKey(name)) {
			BufferedImage image = map.get(name);
			return image;
		} else {
			// Tries to load the image
			try {
				// Creates backdrop object
				ClassLoader loader = this.getClass().getClassLoader();
				InputStream is = loader.getResourceAsStream(locationName);
				BufferedImage image = javax.imageio.ImageIO.read(is);
				map.put(name, image);
				return image;
			} catch (IOException e) {
				System.out.println("Could not load the image.");
				System.exit(0);
				return null;
			}
		}
	}

	public Path getPath(String name) {
		// Sets the name to the correct folder location
		String locationName = "resources/" + name;

		// Gets the path
		ClassLoader loader = this.getClass().getClassLoader();
		Scanner pathScanner = new Scanner(loader.getResourceAsStream(locationName));
		Path path = new Path(pathScanner);

		// Return the path
		return path;
	}
}
