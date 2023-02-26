/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 02
    Date: Jan 31, 2022

    Program that uses the Spiral.cpp and HaruPDF.cpp to make a PDF document with
    a spiral on the screen.

*/

#include "Spiral.h"
#include "HaruPDF.h"
#include "hpdf.h"
#include <iostream>
#include <string.h>


/*
    argc is the number of arguements, argv is the arguements provided from the console
*/
int main(int argc, char **argv){
    if(argc < 2){//If an arguement is not provided by the user report an error and close
        std::cout << "Error: No arguement was given" << std:: endl;
        return 0;
    }
    char *text = argv[1];

    //Constructs a new PDF with the name test.pdf and a new spiral object
    char fileName[] =  "spiralPDF.pdf";
    HaruPDF pdf(fileName);
    Spiral spiral(210, 300, 360, 0.25);
    
    //Loops through the text provided by the user and adds it to the document using the 
    //location given by the spiral class
    double textLength = strlen(text);
    for (int i = 0; i < textLength; i++) {
        pdf.placeCharOnPage(text[i],-(spiral.getSpiralAngle() / 180) * 3.141592,spiral.getSpiralX(),spiral.getSpiralY());
        spiral+= 10 * (4/(double)(i + 4)) + 5;
    }

    //Saves the PDF and closes the program
    pdf.savePDF();
    return 0;
}