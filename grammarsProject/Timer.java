package comprehensive;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Random;

import assign09.StudentGoodHash;

public class Timer {

	public static void main(String[] args) {
		// Do 10000 lookups and use the average running time
				int timesToLoop = 100000;

				// For each problem size n . . .
				for (int n = 1000; n <= 10000; n += 1000) {
					long startTime, midpointTime, stopTime;
					
					int[] list = new int[n];
					
					// First, spin computing stuff until one second has gone by
					// This allows this thread to stabilize
					startTime = System.nanoTime();
					while (System.nanoTime() - startTime < 1000000000) { // empty block
					}

					// Now, run the test
					startTime = System.nanoTime();

					for (int i = 0; i < timesToLoop; i++) {
						for(int j = 0; j < n; j++) {
							list[j] = j;
						}
						
						list = new int[n];
					}

					midpointTime = System.nanoTime();

					// Run a loop to capture the cost of running the "timesToLoop" loop
					for (int i = 0; i < timesToLoop; i++) {
						
					}

					stopTime = System.nanoTime();

					// Compute the time, subtract the cost of running the loop
					// from the cost of running the loop and doing the lookups
					// Average it over the number of runs
					double averageTime = ((midpointTime - startTime) - (stopTime - midpointTime)) / (double) timesToLoop;
					averageTime /= n;

					System.out.println(averageTime);
				}

	}

}
