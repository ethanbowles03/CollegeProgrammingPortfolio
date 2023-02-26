/*
    Author: Ethan Bowles & Conner Fisk
    Class: CS3505
    Assignment: 04
    Date: Feb 20, 2023
*/
#ifndef TRIE_H
#define TRIE_H

#include<string>
#include<vector>
#include<map>

/// @brief Represents a trie where there are 26 possible branches per node each representing a letter
//of the alphabet. Used to store and retrieve words by storing them through a collection of nodes
class Trie{
private: //Instance Variables
    std::map<char, Trie> alphabetMap;
    bool endOfWord = false;
    
public:
    //Base Constructor
    Trie(){};

    //Methods
    //Method to add word to the trie
    void addAWord(std::string wordToAdd);
    //Method to check if word is in trie
    bool isAWord(std::string wordToCheck);
    //Method to get all the words in the trie starting with the prefix
    std::vector<std::string> allWordsBeginningWithPrefix(std::string prefix);

private:
    //Helper method to navigate to trie node at the end of a word
    Trie* navigateToEndOfWord(std::string prefix);
    //Helper method to get all of the words starting a certain trie node
    void getAllInTrie(std::vector<std::string>& allWords, Trie trie, std::string prefix);
};

#endif