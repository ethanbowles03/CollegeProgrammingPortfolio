
    # Author: Ethan Bowles
    # Class: CS3505
    # Assignment: 03
    # Date: Feb 15, 2023

    # Makefile used to construct all of the need dependencies to run TrieTest.cpp
CC = g++
CFLAGS = -Wall -fsanitize=undefined -fsanitize=address

TrieTest: Trie.o TrieTest.o
	$(CC) $(CFLAGS) -o TrieTest Trie.o TrieTest.o

Trie.o: Trie.cpp Trie.h
	$(CC) $(CFLAGS) -g -c Trie.cpp

TrieTest.o: TrieTest.cpp
	$(CC) $(CFLAGS) -g -c TrieTest.cpp

clean:
	rm -f TrieTest *.o

test: TrieTest
	./TrieTest words.txt queries.txt
