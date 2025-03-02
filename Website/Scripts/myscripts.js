function welcomeText(text) {
  var index = 0;
  var speed = 100;
  const elem = document.getElementById("welcome-text");
  var printNextLetter = function () {
    if (index < text.length) {
      var ch = text[index];
      if (ch == '*') {
        elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + '<br>' + '|';
      } else {
        if (index == text.length - 1) {
          elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + ch;
          if(window.pageYOffset <= 100){
            scrollToBottom(1000);
          }
        } else {
          elem.innerHTML = elem.innerHTML.substring(0, elem.innerHTML.length - 1) + ch + '|';
        }
      }
      index++;
      setTimeout(printNextLetter, speed);
    }
  }
  setTimeout(printNextLetter, 1000);
}

// Scroll to the bottom of the page
function scrollToBottom(duration) {
  const start = window.pageYOffset;
  const end = start + window.innerHeight;
  const change = end - start;
  let currentTime = 0;
  const increment = 20;

  const animateScroll = function() {
    currentTime += increment;
    const val = Math.easeInOutQuad(currentTime, start, change, duration);
    window.scrollTo(0, val);
    if (currentTime < duration) {
      setTimeout(animateScroll, increment);
    }
  };

  Math.easeInOutQuad = function(t, b, c, d) {
    t /= d/2;
    if (t < 1) return c/2*t*t + b;
    t--;
    return -c/2 * (t*(t-2) - 1) + b;
  };

  animateScroll();
}