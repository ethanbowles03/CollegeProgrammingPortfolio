package comprehensive;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Random;

/**
 * This class collects running times for the comparing the LinkedListStack class
 * and the ArrayStack class.
 * 
 * @author Erin Parker, Conner Fisk, & Ethan Bowles
 * @version April 20, 2022
 */

public class ComprehensiveTimer {
	public static void main(String[] args) {
		// Do 10000 lookups and use the average running time
		int timesToLoop = 100;
		
		RandomPhraseGenerator rpg = new RandomPhraseGenerator();
		
		// For each problem size n . . .
		for (int n = 1000; n <= 10000; n += 1000) {
			
			long startTime, midpointTime, stopTime;

			// First, spin computing stuff until one second has gone by
			// This allows this thread to stabilize
			startTime = System.nanoTime();
			while (System.nanoTime() - startTime < 1000000000) { // empty block
			}

			// Now, run the test
			startTime = System.nanoTime();

			for (int k = 0; k < timesToLoop; k++) {
				String[] arr = new String[] {"src/comprehensive/assignment_extension_request.g", String.valueOf(n)};
				rpg.main(arr);
			}

			midpointTime = System.nanoTime();

			// Run a loop to capture the cost of running the "timesToLoop" loop
			for (int i = 0; i < timesToLoop; i++) {
				String[] arr = new String[] {"src/comprehensive/assignment_extension_request.g", String.valueOf(n)};
			}

			stopTime = System.nanoTime();

			// Compute the time, subtract the cost of running the loop
			// from the cost of running the loop and doing the lookups
			// Average it over the number of runs
			double averageTime = ((midpointTime - startTime) - (stopTime - midpointTime)) / (double) timesToLoop;
			//averageTime /= 100;

			System.out.println(averageTime);
		}

	}
}
