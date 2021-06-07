window.onload = () => {
    let carousel = document.querySelector(".system__slider"),
        imageContainer = carousel.querySelector(".image__container"),
        image = imageContainer.querySelectorAll(".carousel-image"),
        prev = carousel.querySelector(".button__prev"),
        next = carousel.querySelector(".button__next");

    let currentIndex, currentOffset, slideWidth;

    init();
    window.addEventListener("resize", init);

    prev.addEventListener("click", () => {
        if (currentIndex > 0) {
            currentIndex--;
        }
        currentOffset = calcOffset(currentIndex, slideWidth);
        setOffset(currentOffset, imageContainer);
        console.log(currentIndex);
    });

    next.addEventListener("click", () => {
        if (currentIndex < image.length - 1) {
            currentIndex++;
        }
        currentOffset = calcOffset(currentIndex, slideWidth);
        setOffset(currentOffset, imageContainer);
        console.log(currentIndex);
    });

    function calcOffset(index, slideWidth) {
        return -(index * slideWidth - slideWidth);
    }

    function setOffset(offset, container) {
        container.querySelector("div").style.left = offset + "px";
    }

    function init() {
        slideWidth = parseInt(window.getComputedStyle(image[0], null).width) +
            parseInt(window.getComputedStyle(image[0], null).paddingLeft) * 2 +
            parseInt(window.getComputedStyle(image[0], null).marginLeft) * 2;

        currentIndex = 3;
        currentOffset = calcOffset(currentIndex, slideWidth);
        imageContainer.style.width = currentIndex * slideWidth + "px";

        setOffset(currentOffset, imageContainer);
    }
}