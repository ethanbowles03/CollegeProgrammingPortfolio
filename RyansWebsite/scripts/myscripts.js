// Author: Ethan Bowles
// Date: March 1, 2023
// Version: 1.1

//Variables
const navLinks = document.querySelectorAll('nav ul li a');
const sections = document.getElementsByClassName('page');

//Add event listener to each link in the nav bar
for (let i = 0; i < navLinks.length; i++) {
    navLinks[i].addEventListener("click", function (event) {
        event.preventDefault(); // prevent default link behavior

        // hide all sections
        for (let j = 0; j < sections.length; j++) {
            sections[j].classList.remove("active");
        }

        const href = this.getAttribute("href");
        const section = document.querySelector(href);
        section.classList.add("active"); // show corresponding section
    });
}

//Set Starting Screen
const section = document.querySelector("#home");
section.classList.add("active"); // show corresponding section


//Images in the welcome menu
const carousel = document.querySelector('.image-carousel');
const images = carousel.querySelectorAll('img');
let currentImg = 0;

//Rotates images every 5 seconds
setInterval(() => {
    images[currentImg].classList.remove('active');
    currentImg = (currentImg + 1) % images.length;
    images[currentImg].classList.add('active');
}, 5000);