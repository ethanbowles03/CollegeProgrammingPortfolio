/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 02
    Date: Jan 31, 2022

    Class used to represent a spiral object with a location on a spiral along with
    the radius and the angle of the location
*/

#include <math.h>
#include "Spiral.h"

/*
    Constructs a new spiral object with a starting center point and orientation. 
    Furthermore, gives a loop scaling factor that controls the size of the growth of
    the spiral.

    If the given starting angle is negative or 0 that starting angle is set to 360
*/
Spiral::Spiral(double centerX, double centerY, double startingAngle, double loopScalingFactor):
centerX{centerX}, centerY{centerY}, loopScalingFactor{loopScalingFactor}{
    startingAngle <= 0 ? angle = 360 : angle = startingAngle;
    radius = angle * loopScalingFactor;
}

/*
    Increments the angle of the spiral moving it forward
    Does so by overloading the += operator
*/
Spiral& Spiral::operator+=(int incAngle){
    angle += incAngle;
    radius = angle * loopScalingFactor;
    return (*this);
}

/*
    Getter method for the current x location on the spiral
*/
double Spiral::getSpiralX(){
    return centerX + sin(degToRadWithOffset(angle)) * radius;
}

/*
    Getter method for the current y location on the spiral
*/
double Spiral::getSpiralY(){
    return centerY + cos(degToRadWithOffset(angle)) * radius;
}

/*
    Getter method for the current angle of the spiral
*/
double Spiral::getSpiralAngle(){
    return angle;
}

/*
    Method that converts a degree to a radians
    Used for location on the spiral
*/
double Spiral::degToRadWithOffset(double deg){
    return deg / 180 * 3.141592;
}
