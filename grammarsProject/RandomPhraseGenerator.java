package comprehensive;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Random;
import java.lang.*;

/**
 * This class holds the randomPhrase method and the main method which runs
 * randomPhrase.
 * 
 * @author Ethan Bowles and Conner Fisk
 * @version April 25, 2022
 */

public class RandomPhraseGenerator {
	// Fields
	private static HashMap<String, ArrayList<String>> table;
	private static Random rng;
	private static ArrayList<String> keysList;
	private static String line;
	private static String[] splitLine;

	/**
	 * The main method which runs and prints out the random phrase.
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		// Initialize the random object
		rng = new Random();

		// Build a table out of the given file
		FileReader fr = new FileReader(args[0]);
		table = fr.buildHashMap();

		// Print out a random phrase as many times as prompted to
		for (int i = 0; i < Integer.valueOf(args[1]); i++) {
			System.out.println(randomPhrase("<start>"));
		}
	}

	/**
	 * Method that randomly creates a phrase from the given file.
	 * 
	 * @param key
	 * @return -> random phrase
	 */
	public static String randomPhrase(String key) {
		// Create a new StringBuilder to construct the phrase
		StringBuilder finalLine = new StringBuilder("");

		// Initialize needed variables
		keysList = table.get(key);
		line = keysList.get(rng.nextInt(keysList.size()));
		splitLine = line.split("((?=<)|(?<=>))");

		// For each loop that goes through each string in a line
		for (String s : splitLine) {
			// Finds non-terminals through detecting caret and picks a random
			// production rule from that non-terminal
			if (s.contains("<") && s.contains(">")) {
				finalLine.append(randomPhrase(s));
			} else {
				// Adds the constants to the phrase
				finalLine.append(s);
			}
		}
		// Returns the random phrase
		return finalLine.toString();
	}
}
