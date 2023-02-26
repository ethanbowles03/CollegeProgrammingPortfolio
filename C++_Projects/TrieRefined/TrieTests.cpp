/*
    Author: Ethan Bowles & Conner Fisk
    Class: CS3505
    Assignment: 04
    Date: Feb 20, 2023
*/

#include "Trie.h"
#include <string>
#include <iostream>
#include <bits/stdc++.h>
#include "gtest/gtest.h"

// Test if AddAWord works.
TEST(BasicCases, AddAWord){
    Trie testTrie;
    testTrie.addAWord("hello");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("");

    std::vector<std::string> helloVec;
    helloVec.push_back("hello");
    
    for (unsigned int i = 0; i < testVec.size(); i++){
        ASSERT_EQ(testVec[i], helloVec[i]);
    }
}

// This make sure that calling allWordsBeginningWithPrefix with an empty string will
// return all the words in the Trie.
TEST(BasicCases, allWords){
    Trie testTrie;
    testTrie.addAWord("cat");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("");

    std::vector<std::string> animalVec;
    animalVec.push_back("cat");

    for (unsigned int i = 0; i < testVec.size(); i++){
        ASSERT_EQ(testVec[i], animalVec[i]);
    }
}

// This make sure that calling allWordsBeginningWithPrefix with a prefix returns the
// correct values.
TEST(BasicCases, prefixTest){
    Trie testTrie;
    testTrie.addAWord("sat");
    testTrie.addAWord("saturn");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("satu");

    std::vector<std::string> prefixVec;
    prefixVec.push_back("saturn");

    for (unsigned int i = 0; i < testVec.size(); i++){
        ASSERT_EQ(testVec[i], prefixVec[i]);
    }
}

// This makes sure that isAWord returns true when the word is in the Trie.
TEST(BasicCases, isAWordTrue){
    Trie testTrie;
    testTrie.addAWord("sat");
    testTrie.addAWord("saturn");

    EXPECT_TRUE(testTrie.isAWord("saturn"));
}

// This makes sure that isAWord returns false when the word is not in the Trie.
TEST(BasicCases, isAWordFalse){
    Trie testTrie;
    testTrie.addAWord("sat");
    testTrie.addAWord("saturn");

    EXPECT_FALSE(testTrie.isAWord("pluto"));
}

// This makes sure that isAWord returns false when the word is not in the Trie.
TEST(BasicCases, isAWordFalse2){
    Trie testTrie;
    testTrie.addAWord("sat");
    testTrie.addAWord("saturn");

    EXPECT_FALSE(testTrie.isAWord("saturns"));
}

// This make sure that calling allWordsBeginningWithPrefix with a prefix that is a word
// will return a word in the vector.
TEST(EdgeCases, selfPrefix){
    Trie testTrie;
    testTrie.addAWord("sat");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("sat");

    std::vector<std::string> prefixVec;
    prefixVec.push_back("sat");

    for (unsigned int i = 0; i < testVec.size(); i++){
        ASSERT_EQ(testVec[i], prefixVec[i]);
    }
}

// This make sure that calling allWordsBeginningWithPrefix with a invalid word (ie has a capital letter)
// an empty vector is returned.
TEST(EdgeCases, invalidLetter){
    Trie testTrie;
    testTrie.addAWord("sat");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("Sat");
    EXPECT_FALSE(testTrie.isAWord("Sat"));

    size_t emptyCount = 0;
    ASSERT_EQ(testVec.size(), emptyCount);
}

// This make sure that adding an empty word still gets added.
TEST(EdgeCases, addEmptyWord){
    Trie testTrie;
    testTrie.addAWord("");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("");

    size_t oneCount = 1;
    ASSERT_EQ(testVec.size(), oneCount);
}

// This make sure that calling allWordsBeginningWithPrefix with a number (which is invalid)
// an empty vector is returned.
TEST(EdgeCases, numberInvalid){
    Trie testTrie;
    testTrie.addAWord("sat");
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("7");

    size_t emptyCount = 0;
    ASSERT_EQ(testVec.size(), emptyCount);
}

// This makes sure that, just because there is a word before and after a group of letters,
// doesn't mean it is considered a word.
TEST(EdgeCases, notAWordInBetween){
    Trie testTrie;
    testTrie.addAWord("sat");
    testTrie.addAWord("saturn");

    EXPECT_FALSE(testTrie.isAWord("satu"));
}

// This makes sure that, just because a word is contained in the given word, does not
// mean that the given word is a word.
TEST(EdgeCases, tooLong){
    Trie testTrie;
    testTrie.addAWord("saturn");

    EXPECT_FALSE(testTrie.isAWord("saturns"));
}

// This makes sure that, just because there is a word with the same letters up to the last
// letter, doesn't mean that it is a word.
TEST(EdgeCases, notAWordDifferentLastLetters){
    Trie testTrie;
    testTrie.addAWord("saturn");

    EXPECT_FALSE(testTrie.isAWord("saturz"));
}

//Tests the default constructor to make sure that the trie is empty
TEST(RuleOfThreeCases, defaultConst){
    Trie testTrie;
    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("");
    size_t emptyCount = 0;
    ASSERT_EQ(testVec.size(), emptyCount);
}

//Tests the destructor to make sure that everything is destructed without a memory leak
TEST(RuleOfThreeCases, destructorTest){
    auto bulk = []() {
        Trie testTrie;
        std::string alpha = "abcdefghijklmnopqrstuvwxyz" ;
        std::string str ;
        for(int i = 0; i < 26; i++){
            str += alpha[i];
            for(int j = 0; j < 26; j++){
                str += alpha[j];
                for(int k = 0; k < 26; k++){
                    str += alpha[k];
                    testTrie.addAWord(str);
                }
                str = "";
                str += alpha[i];
            }
            str = "";
        }
    };
    bulk();
    ASSERT_TRUE(true);
}

//Tests the copy constructor in the rule of three and makes sure objects are independent from eachother
TEST(RuleOfThreeCases, copyConstructor){
    Trie testTrie;
    testTrie.addAWord("hello");
    testTrie.addAWord("world");

    Trie ruleOfThreeOpTestTrie(testTrie);
    ruleOfThreeOpTestTrie.addAWord("cplusplus");


    std::vector<std::string> testVecOriginal = testTrie.allWordsBeginningWithPrefix("");
    std::vector<std::string> testVecCopy = ruleOfThreeOpTestTrie.allWordsBeginningWithPrefix("");
    
    for (unsigned int i = 0; i < testVecOriginal.size(); i++){
        if(testVecOriginal[i].compare("cplusplus") == 0){
            ASSERT_TRUE(false);
        }   
    }
    
    
    ASSERT_EQ(testVecCopy.size(), (size_t)3);
    ASSERT_EQ(testVecOriginal.size(), (size_t)2);
}

// This makes sure that the assignment operator makes a SEPERATE Trie to the Trie on the right side 
// of the operation with the same values as it.
TEST(RuleOfThreeCases, assignmentConstructor){
    Trie testTrie;
    testTrie.addAWord("hello");

    Trie assignTrie;
    assignTrie = testTrie;

    std::vector<std::string> testVec = testTrie.allWordsBeginningWithPrefix("");
    std::vector<std::string> assignVec = assignTrie.allWordsBeginningWithPrefix("");
    
    for (unsigned int i = 0; i < testVec.size(); i++){
        ASSERT_EQ(assignVec[i], testVec[i]);
    }

    assignTrie.addAWord("world");
    assignVec = assignTrie.allWordsBeginningWithPrefix("");
    testVec = testTrie.allWordsBeginningWithPrefix("");
    
    ASSERT_NE(assignVec.size(), testVec.size());
}

int main (int argc, char* argv[]){
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}