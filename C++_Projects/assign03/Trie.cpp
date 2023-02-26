/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 03
    Date: Feb 15, 2023
*/
#include "Trie.h"
#include<iostream>
#include<algorithm>

//Constructors
///Destuctor that deletes the contents of the backing array
Trie::~Trie()
{
    for (int i = 0; i < 26; i++) {
        delete alphabetArr[i];
    }   
}

/// @brief Copy constructor that copies the contents of the provided Trie's backing array to this obj
/// @param other - trie to be copied
Trie::Trie(const Trie &other)
{
    endOfWord = other.endOfWord;
    for (int i = 0; i < 26; i++) {
        alphabetArr[i] = nullptr;
        if(other.alphabetArr[i]){
            alphabetArr[i] = new Trie(*(other.alphabetArr[i]));
        }
    } 
}

// Operators
/// @brief Assignment overload to copy contents safely without messing up either trie
/// @param other - trie to be assigned to
Trie& Trie::operator=(Trie other)
{
    std::swap(endOfWord, other.endOfWord);
    for (int i = 0; i < 26; i++) {
        std::swap(alphabetArr[i], other.alphabetArr[i]);
    } 
    return *this;
}

// Methods
/// @brief method that adds a word to the tree recusivly in the correct location
/// @param wordToAdd - the word to add
void Trie::addAWord(std::string wordToAdd)
{
    if(wordToAdd.length() > 0){ // Go until end of word adding
        int charValue = wordToAdd.at(0) - 97;
        if(!alphabetArr[charValue]) {alphabetArr[charValue] = new Trie();}
        alphabetArr[charValue]->addAWord(wordToAdd.substr(1));
    }else{ //End reached
        endOfWord = true;
    }
}

/// @brief method that checks if the tree contains a word
/// @param wordToCheck - the word to check 
/// @return if the word is in the trie
bool Trie::isAWord(std::string wordToCheck)
{
    //Navigates to the end of the word and checks if nullptr, if not ret flag
    Trie* endOfWordTree =navigateToEndOfWord(wordToCheck);
    return !endOfWordTree ? false : endOfWordTree->endOfWord;
}

/// @brief method that returns all of the words beggining with a certain prefix
/// @param prefix - the prefix of the words you want to get
/// @return - a vector a words starting with the given prefix
std::vector<std::string> Trie::allWordsBeginningWithPrefix(std::string prefix)
{
    Trie* trie = navigateToEndOfWord(prefix);
    if(!trie){return {};}

    std::vector<std::string> listOfWords;
    getAllInTrie(listOfWords, trie, prefix);

    return listOfWords;
}

/// @brief private helper method that navigate the to the tree node at the end of the word given
/// @param word - the location you wish to travel to
/// @return - the trie node at the end of the given word
Trie* Trie::navigateToEndOfWord(std::string word){
    if(word.length() == 0){ //Returns if str is a word
        return this;
    }

    int charValue = word.at(0) - 97;
    if(charValue < 0 || charValue > 26){ //Valid Char Check
        return nullptr;
    }else if(!alphabetArr[charValue]){ //End of branch check
        return nullptr;
    }else { //Keep traversing
        return alphabetArr[charValue]->navigateToEndOfWord(word.substr(1));
    }
}

/// @brief Private helper method that gets all the words in the trie given
/// @param allWords - vector to be returned with all words
/// @param trie - the starting point of the trie
/// @param prefix - the to add to the begging of the word
void Trie::getAllInTrie(std::vector<std::string>& allWords, Trie* trie, std::string prefix) {
    if(trie->endOfWord){
        allWords.push_back(prefix);     
    }
    for(int i = 0; i < 26; i++){
        if (trie->alphabetArr[i]){
            trie->alphabetArr[i]->getAllInTrie(allWords, trie->alphabetArr[i], prefix + std::string (1, char(i+97)));
        }   
    }
}