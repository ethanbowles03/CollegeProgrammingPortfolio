/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 02
    Date: Jan 31, 2022

    Header for the HaruPDF class. Outlines the structure of the class along with
    all of the intance variabless.
*/

#ifndef HARUPDF_H
#define HARUPDF_H

#include "hpdf.h"

class HaruPDF{
private: //Instance Variables
    HPDF_Doc  pdf;
    HPDF_Page page;
    char *fileName;
    
public:
    // Use to construct a new pdf wiith a certain file name
    HaruPDF(char *fName);

    // Use to place a character on the page at a certain location and orientation
    void placeCharOnPage(char text, float rad, float x, float y);
    // Use to save PDF after edits
    void savePDF();
};

#endif