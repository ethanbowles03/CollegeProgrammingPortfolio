function setupvars(){
  this.navbar = document.querySelector(".navbar");
  this.menu = document.querySelector(".menu");
  this.menuBtn = document.querySelector(".menuButton");
  this.cancelBtn = document.querySelector(".canButton");
}

function menuBtnClicked(){
  setupvars();
  menu.classList.add("active");
  menuBtn.classList.add("hide");
  cancelBtn.classList.add("show");
}

function cancelBtnClicked(){
  setupvars();
  menu.classList.remove("active");
  menuBtn.classList.remove("hide");
  cancelBtn.classList.remove("show");
}

window.onscroll = function() {scrollFunction()};
function scrollFunction() {
  setupvars();
  this.scrollY > 20 ? navbar.classList.add("sticky") : navbar.classList.remove("sticky");
  this.scrollY > window.innerHeight ? navbar.style.background = "#F0EAD6" : navbar.style.background = "transparent" ;
}

function welcomeText(text) {
  var index = 0;
  var speed = 100;
  const elem = document.getElementById("welcome-text");
  var printNextLetter = function() {
    if (index < text.length) {
      var ch = text[index];
      if(ch == '*'){
        elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + '<br>' + '|';
      }else{
        if(index == text.length - 1){
          elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + ch;
        }else{
          elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + ch + '|';
        }

      }
      index++;
      setTimeout(printNextLetter, speed);
    }
  }
  setTimeout(printNextLetter, 1000);
}
