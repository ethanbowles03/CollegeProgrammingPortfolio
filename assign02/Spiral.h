/*
    Author: Ethan Bowles
    Class: CS3505
    Assignment: 02
    Date: Jan 31, 2022

    Header for the Spiral class. Outlines the structure of the class along with
    all of the intance variabless.
*/

#ifndef SPIRAL_H
#define SPIRAL_H

class Spiral{
private: //Instance Variables
    double centerX, 
            centerY, 
            angle, 
            loopScalingFactor, 
            radius;

    
public:
    //Used to construct a new Spiral with a set location, starting angle and scaling factor
    Spiral(double centerX, double centerY, double startingAngle, double loopScalingFactor);

    //Used to add a certain increase in agnle to the spiral angle
    Spiral& operator+=(int incAngle);
    //Used to get the x point on the spiral
    double getSpiralX();
    //Used to get the y point on the spiral
    double getSpiralY();
    //Used to get the rotation of the current spiral angle
    double getSpiralAngle();
private:
    //Used to convert degree to radians
    double degToRadWithOffset(double deg);
};

#endif