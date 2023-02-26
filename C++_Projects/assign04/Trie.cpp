/*
    Author: Ethan Bowles & Conner Fisk
    Class: CS3505
    Assignment: 04
    Date: Feb 20, 2023
*/
#include "Trie.h"
#include<iostream>
#include<algorithm>

// Methods
/// @brief method that adds a word to the tree recusivly in the correct location
/// @param wordToAdd - the word to add
void Trie::addAWord(std::string wordToAdd)
{
    if (!wordToAdd.empty()) { // Continue if not at the end of the word
        char letter = wordToAdd[0];
        if (!alphabetMap.contains(letter)) { // If there is not a branch make one
            alphabetMap.insert(std::make_pair(letter, Trie()));
        }
        alphabetMap.at(letter).addAWord(wordToAdd.substr(1)); // Recurse down branch
    } else { // End of word reached
        endOfWord = true;
    }
}

/// @brief method that checks if the tree contains a word
/// @param wordToCheck - the word to check 
/// @return if the word is in the trie
bool Trie::isAWord(std::string wordToCheck)
{
    //Navigates to the end of the word and checks if nullptr, if not ret flag
    Trie* endOfWordNode = navigateToEndOfWord(wordToCheck);
    return !endOfWordNode ? false : endOfWordNode->endOfWord;
}

/// @brief method that returns all of the words beggining with a certain prefix
/// @param prefix - the prefix of the words you want to get
/// @return - a vector a words starting with the given prefix
std::vector<std::string> Trie::allWordsBeginningWithPrefix(std::string prefix)
{
    Trie* trieNode = navigateToEndOfWord(prefix); //Gets the node at the end of the prefix
    if(!trieNode){return {};} //Return if no node

    //Gets all of the words at this node and returns 
    std::vector<std::string> listOfWords;
    getAllInTrie(listOfWords, *trieNode, prefix);
    return listOfWords;
}

/// @brief private helper method that navigate the to the tree node at the end of the word given
/// @param word - the location you wish to travel to
/// @return - the trie node at the end of the given word
Trie* Trie::navigateToEndOfWord(std::string word){
    if(word.empty()){ //Returns current node in traverse if word exists
        return this;
    }
    
    char charValue = word[0];
    if (charValue < 'a' || charValue > 'z') { // Checks if character is valid
        return nullptr;
    }else if(!alphabetMap.contains(charValue)){ //End of branch check
        return nullptr;
    }else { //Keep traversing
        return alphabetMap.at(charValue).navigateToEndOfWord(word.substr(1));
    }
}

/// @brief Private helper method that gets all the words in the trie given
/// @param allWords - vector to be returned with all words
/// @param node - the starting point of the trie
/// @param prefix - the to add to the begging of the word
void Trie::getAllInTrie(std::vector<std::string>& allWords, Trie node, std::string prefix) {
    if(node.endOfWord){ //If end of word found push to vector
        allWords.push_back(prefix);     
    }
    for (auto& [letter, child] : node.alphabetMap) { //Loop through each branch recussivly calling at letter
        child.getAllInTrie(allWords, child, prefix + letter);
    }
}