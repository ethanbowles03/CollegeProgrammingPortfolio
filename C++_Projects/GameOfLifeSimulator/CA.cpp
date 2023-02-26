/*
    Author: Ethan Bowles
    Date: Jan. 16, 2023
    Version: 1.0
*/

#include<iostream>
 
//Method Declarations
void convertRuleSetNumberToRuleSetArray(int ruleSetNumber, int ruleSet[8]);
void displayGeneration(int binaryArr[], int arrLen);
void computeNextGeneration(int currentGeneration[], int nextGeneration[], int generationArrLen, int ruleSet[8]);
int convertNeighborhoodToIndex(int left, int center, int right);
 
int main(){
   //Variables
   int ruleSetNumber;
   const int GENERATIONSIZE = 64;
   int ruleSet[8]{};
   int currentGeneration[GENERATIONSIZE]{};
   int nextGeneration[GENERATIONSIZE]{};
 
   //Get a number between 0 and 255 from the user
   std::cout << "Please Enter a Rule Set Number Between 0 and 255: " << std::endl;
   if(std::cin >> ruleSetNumber){
       if(ruleSetNumber < 0 || ruleSetNumber > 255){ //Bounds check
           std::cout << "The value entered is not within the specified range." << std::endl;
           return 0;
       }else{ //Convert their number to a rule set and store it in a binary array
           convertRuleSetNumberToRuleSetArray(ruleSetNumber, ruleSet);
       }
   }else{ //Non-int was entered
       std::cout << "The value entered is not within the specified range." << std::endl;
       return 0;
   }
 
   //Make a starting generation length of 64 length with all values set to 0 except index 32
   currentGeneration[GENERATIONSIZE/2] = 1;
   displayGeneration(currentGeneration, GENERATIONSIZE);
 
   //Display 49 more generations after
   for(int gen = 0; gen < 49; gen++){
       computeNextGeneration(currentGeneration, nextGeneration, GENERATIONSIZE, ruleSet);
       for(int i = 0; i < GENERATIONSIZE; i++){
           currentGeneration[i] = nextGeneration[i];
       }
       displayGeneration(currentGeneration, GENERATIONSIZE);
   }
 
   return 0;
}
 
/*
   Derives a rule set by converting a base-10 number into a base-2 number reversed
*/
void convertRuleSetNumberToRuleSetArray(int ruleSetNumber, int ruleSet[8]){
   //Starts at 128, checking if the value is greater. 
   //If greater subtract from value. Each iteration divides the p value by 2
   for(int p = 128, ruleSetPosition = 7; p > 0; p /= 2, ruleSetPosition--){ 
       if(p <= ruleSetNumber){
           ruleSetNumber -= p;
           ruleSet[ruleSetPosition] = 1;
       }else{
           ruleSet[ruleSetPosition] = 0;
       }
   }
}
 
/*
   Given a binary sequence prints '#' for 1 and ' ' for 0
*/
void displayGeneration(int binaryArr[], int arrLen){
   for(int i = 0; i < arrLen; i++){
       if(binaryArr[i] == 0){
           std::cout << " ";
       }else{
           std::cout << "#";
       }
   }
   std::cout << std::endl;
}
 
/*
   Computes the next generation of cellular automata using the given rule set at the location give
   by the convertNeighborhoodToIndex method. The first and last index stay the same each time
*/
void computeNextGeneration(int currentGeneration[], int nextGeneration[], int generationArrLen, int ruleSet[8]){
   //Keep first and last
   nextGeneration[0] = currentGeneration[0];
   nextGeneration[generationArrLen-1] = currentGeneration[generationArrLen-1];
 
   //Loops through the rest of the values and calculates the next generation
   for(int i = 1; i < generationArrLen - 1; i++){
       nextGeneration[i] = ruleSet[convertNeighborhoodToIndex(currentGeneration[i-1],currentGeneration[i],currentGeneration[i+1])];
   }
}
 
/*
   Converts a three digit binary sequence into a base ten number
*/
int convertNeighborhoodToIndex(int left, int center, int right){
    //4|2|1
    return (left * 4) + (center * 2) + (right * 1);
}