package comprehensive;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Scanner;
import java.lang.*;

/**
 * This class holds the constructor for the FileReader object as well as the
 * buildHashMap method
 * 
 * @author Ethan Bowles and Conner Fisk
 * @version April 25, 2022
 */

public class FileReader {
	// Fields
	private HashMap<String, ArrayList<String>> hm;
	private Scanner scanner;

	/**
	 * Constructs a HashMap with non-terminals being keys and an ArrayList of its
	 * production rules being its value using the given file.
	 * 
	 * @param str
	 */
	public FileReader(String str) {
		// Initialize the HashMap
		hm = new HashMap<String, ArrayList<String>>();

		// Try and create a scanner which goes through the given file
		try {
			scanner = new Scanner(new File(str));
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	/**
	 * This method builds the HashMap used to store all of the necessary data from
	 * the file to fill in the non terminals in each random phrase.
	 * 
	 * @return -> created HashMap
	 */
	public HashMap<String, ArrayList<String>> buildHashMap() {
		// Initialize needed variables
		String key = "";
		String line = "";
		ArrayList<String> connectedLines = new ArrayList<String>();

		// Continues while there is something next in the file
		while (scanner.hasNext()) {
			while (scanner.hasNext() && !(scanner.next().equals("{"))) {
				// Skips until the start
			}
			// Exits the while loop if there is nothing left in the file
			if (scanner.hasNext() == false) {
				break;
			}
			// Initializes connectedLines as a new ArrayList
			connectedLines = new ArrayList<String>();
			//Set the key to the next value
			key = scanner.next();
			//Go to the file's next line
			scanner.nextLine();
			//Set the line variable to the file's next line
			line = scanner.nextLine();
			//Goes until the given {} enclosed section is completed
			while (!(line.equals("}"))) {
				connectedLines.add(line);
				line = scanner.nextLine();
			}
			//Set the HashMap value with the found data
			hm.put(key, connectedLines);
		}
		//Returns the created HashMap
		return hm;
	}
}
