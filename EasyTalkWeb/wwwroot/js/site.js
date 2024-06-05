
﻿const scroll = document.querySelector(".more");
scroll?.addEventListener("click", () => {
    window.scrollTo({
        top: window.innerHeight,
        behavior: "smooth"
    });
  });

﻿const inputs = document.querySelectorAll('input');
inputs.forEach((input) => {
    const placeh = input.placeholder;
    input.addEventListener('focus', (e) => {
        e.target.placeholder = '';
    });
    input.addEventListener('blur', (e) => {
        e.target.placeholder = placeh;
    });
});
