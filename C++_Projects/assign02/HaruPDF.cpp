/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 02
    Date: Jan 31, 2022
*/

#include <math.h>
#include "hpdf.h"
#include "HaruPDF.h"

/*
    Constructor that sets up a new PDF object with a given file name
    and a font of size 30 in Courier-Bold
*/
HaruPDF::HaruPDF(char *fName) : fileName{fName}{
    //Makes the output file .pdf and makes new pdf and page obj
    pdf = HPDF_New (NULL, NULL);
    page = HPDF_AddPage (pdf);

    //Sets up properies of page
    HPDF_Page_SetSize (page, HPDF_PAGE_SIZE_A5, HPDF_PAGE_PORTRAIT);
    HPDF_Page_BeginText (page);
    HPDF_Font font = HPDF_GetFont (pdf, "Courier-Bold", NULL); // Set the font
    HPDF_Page_SetTextLeading (page, 20);
    HPDF_Page_SetGrayStroke (page, 0);
    HPDF_Page_SetFontAndSize (page, font, 30);
}

/*
    Places a character on the page at a certain location and a given orientation
    Uses a matrix to rotate the character on the screen
*/
void HaruPDF::placeCharOnPage(char text, float letterAngle, float xLoc, float yLoc){
    HPDF_Page_SetTextMatrix(page,
                            cos(letterAngle), sin(letterAngle), -sin(letterAngle), cos(letterAngle),
                            xLoc, yLoc);

    // C-style strings are null-terminated. The last character must a 0.
    char buf[2]{text, 0};
    HPDF_Page_ShowText (page, buf);
}

/*
    Saves the PDF with a certain name to files and frees the memory
*/
void HaruPDF::savePDF(){
    //Ends the page, saves to file and frees space
    HPDF_Page_EndText (page);
    HPDF_SaveToFile (pdf, fileName);
    HPDF_Free (pdf);

}

