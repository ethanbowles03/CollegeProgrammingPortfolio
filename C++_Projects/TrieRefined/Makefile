# Name: Conner Fisk & Ethan Bowles
# Class: CS3505
# Date: Feb 20, 2023
# Assignment: 04

CC = g++ -std=c++2a
CFLAGS = -g -Wall -fsanitize=address
GOOGLETEST ?= ./googletest

TrieTests: Trie.o TrieTests.o
	$(CC) $(CFLAGS) -I$(GOOGLETEST)/googletest/include -o TrieTests Trie.o TrieTests.o -L$(GOOGLETEST)/lib -lgtest

Trie.o: Trie.h Trie.cpp
	$(CC) $(CFLAGS) -c Trie.cpp 

TrieTests.o: TrieTests.cpp
	$(CC) $(CFLAGS) -I$(GOOGLETEST)/googletest/include -c TrieTests.cpp

clean:
	rm -f TrieTests *.o




