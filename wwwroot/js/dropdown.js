function dropdown() {
    document.querySelector(".page__dropdown").classList.toggle("page__dropdown-show");
    document.querySelector(".page__menu-background").classList.toggle("page__menu-background-show");

    document.body.style.overflowY = "hidden";
    clicked = true;
    if (clicked) document.body.style.overflowY = "";
}

function hideBackground() {
    document.querySelector(".page__dropdown").classList.remove("page__dropdown-show");
    document.querySelector(".page__menu-background").classList.remove("page__menu-background-show");

    document.body.style.overflowY = "";
}

