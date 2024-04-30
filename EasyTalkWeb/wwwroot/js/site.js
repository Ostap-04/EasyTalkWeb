const scroll = document.querySelector(".more");
scroll?.addEventListener("click", () => {
    window.scrollTo({
        top: window.innerHeight,
        behavior: "smooth",
    });
});
