const inputs = document.querySelectorAll('input');

inputs.forEach((input) => {
    const placeh = input.placeholder;
    input.addEventListener('focus', (e) => {
        e.target.placeholder = '';
    });
    input.addEventListener('blur', (e) => {
        e.target.placeholder = placeh;
    });
});
