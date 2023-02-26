
    # Author: Ethan Bowles
    # Class: CS3505
    # Assignment: 02
    # Date: Jan 31, 2022

    # Makefile used to construct all of the need dependencies to run spiralPDF


CC = g++
CFLAGS = -Wall
LIBHARU = ./libharu

spiralPDF: HaruPDF.o Spiral.o spiralPDF.o
	$(CC) $(CFLAGS) -I$(LIBHARU)/include -o spiralPDF HaruPDF.o Spiral.o spiralPDF.o -L$(LIBHARU)/src/ -lhpdfs

HaruPDF.o: HaruPDF.cpp HaruPDF.h 
	$(CC) $(CFLAGS) -I$(LIBHARU)/include -c HaruPDF.cpp

Spiral.o: Spiral.cpp Spiral.h
	$(CC) $(CFLAGS) -I$(LIBHARU)/include -c Spiral.cpp

spiralPDF.o: spiralPDF.cpp 
	$(CC) $(CFLAGS) -I$(LIBHARU)/include -c spiralPDF.cpp

clean:
	rm -f test test.pdf *.o 
