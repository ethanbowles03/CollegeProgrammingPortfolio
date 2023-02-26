/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 03
    Date: Feb 15, 2023
*/
#include<iostream>
#include<fstream>
#include"Trie.h"

/// @brief - Testing file for the Trie.cpp class. Goes through and makes sure all the
/// functionality of the Trie is working; adds, gets and rule of three
/// @param argc - number of arguements being passed into the program
/// @param argv - the arguements being passed into the program
/// @return -exits with 0 when the program is terminated
int main(int argc, char** argv){
    if(argc != 3){//Check if arguements are correct
        std::cout << "The number of arguements inputed was incorrect" << std::endl;
        return 0;
    }

    //Opens files with reading access
    char* wordsFileName = argv[1];
    std::ifstream wordsFile(wordsFileName);
    char* queriesFileName = argv[2];
    std::ifstream queriesFile(queriesFileName);

    //Read and store all words and queries into vector objects
    std::string fileLine;
    std::vector<std::string> wordsVec;
    std::vector<std::string> queriesVec;
    if (wordsFile.is_open()){
        while (getline(wordsFile,fileLine)){
            wordsVec.push_back(fileLine);
        }
        wordsFile.close();
    }else{ //Error if file doesnt read correctly
         std::cout << "Issue reading the words file" << std::endl;
        return 0;
    }
    if (queriesFile.is_open()){
        while (getline(queriesFile,fileLine)){
            queriesVec.push_back(fileLine);
        }
        queriesFile.close();
    }else{ //Error if file doesnt read correctly
        std::cout << "Issue reading the queries file" << std::endl;
        return 0;
    }

    Trie testTrie;
    
    //Adds words to trie
    for(std::string word : wordsVec){
        testTrie.addAWord(word);
    }
    
    //Tests all queries
    for(std::string query : queriesVec){
        std::cout << "Testing " << query << ":" << std::endl;
        testTrie.isAWord(query) ? std::cout << "Found" << std::endl : std::cout << "Not found" << std::endl;

        std::vector<std::string> wordsStartingWithGivenPrefix;
        wordsStartingWithGivenPrefix = testTrie.allWordsBeginningWithPrefix(query);
        for(std::string word : wordsStartingWithGivenPrefix){
            std::cout << word << " ";
        }
        std::cout << std::endl;
        std::cout << std::endl;
    }

    //Rule of three tests
    Trie ruleOfThreeTestTrie;
    ruleOfThreeTestTrie.addAWord("a");
    ruleOfThreeTestTrie.addAWord("apple");
    ruleOfThreeTestTrie.addAWord("pple");
    ruleOfThreeTestTrie.addAWord("hello");

    //Shows original trie
    std::cout << "Testing rule of three:" << std::endl;
    std::cout << "  Trie contents before assignment and copy" << std::endl;
    for(std::string word : ruleOfThreeTestTrie.allWordsBeginningWithPrefix("")){
        std::cout << word << " ";
    }
    std::cout << std::endl;
    std::cout << std::endl;

    //Make two more tries for test, one using assiging and the other coping
    Trie ruleOfThreeTestTrie2(ruleOfThreeTestTrie);
    Trie ruleOfThreeTestTrie3;
    ruleOfThreeTestTrie3 = ruleOfThreeTestTrie;

    ruleOfThreeTestTrie2.addAWord("world");
    ruleOfThreeTestTrie3.addAWord("cplusplus");

    //Shows the content of new trees
    std::cout << "Trie2(copy) and Trie3(assignment) after adding world and cplusplus" << std::endl;
    for(std::string word : ruleOfThreeTestTrie2.allWordsBeginningWithPrefix("")){
        std::cout << word << " ";
    }
    std::cout << std::endl;
    for(std::string word : ruleOfThreeTestTrie3.allWordsBeginningWithPrefix("")){
        std::cout << word << " ";
    }
    std::cout << std::endl;

    //Prints the contents of the original trie again
    std::cout << "Trie1 contents after assignment and copy additions" << std::endl;
    for(std::string word : ruleOfThreeTestTrie.allWordsBeginningWithPrefix("")){
        std::cout << word << " ";
    }

    std::cout << std::endl;
    std::cout << std::endl;

    //Checks if the old tree has changed at all
    bool contains = false;
    for(std::string word : ruleOfThreeTestTrie.allWordsBeginningWithPrefix("")){
        if(word.compare("world") == 0 || word.compare("cplusplus") == 0){
            contains = true;
        }
    }
    
    int trie1Size = ruleOfThreeTestTrie.allWordsBeginningWithPrefix("").size();
    int trie2Size = ruleOfThreeTestTrie2.allWordsBeginningWithPrefix("").size();
    int trie3Size = ruleOfThreeTestTrie3.allWordsBeginningWithPrefix("").size();
    if(!contains && trie1Size != trie2Size && trie1Size != trie3Size){
         std::cout << "Rule of three tests passed" << std::endl;
    }else{
        std::cout << "Rule of three tests failed" << std::endl;
    }

    return 0;
}