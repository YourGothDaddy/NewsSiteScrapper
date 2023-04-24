$('.dropdown-submenu a.dropdown-toggle').on("click", function (e) {
    $('.dropdown-submenu .dropdown-menu').not($(this).next('.dropdown-menu')).removeClass('show');
    $(this).next('.dropdown-menu').toggleClass('show');
    e.stopPropagation();
});

const navbarToggler = document.querySelector('.navbar-toggler');
const wave = document.querySelector('.wave');

navbarToggler.addEventListener('click', function () {
    if (navbarToggler.classList.contains('collapsed')) {
        wave.style.transition = 'all 0.5s ease-out';
        wave.style.top = '0';
    } else {
        wave.style.transition = 'all 0.5s ease-out';
        wave.style.top = '90px';
    }
});

const waveElement = document.querySelector('.wave');

function checkViewportWidth() {
    const viewportWidth = window.innerWidth;
    if (viewportWidth > 576) {
        waveElement.style.top = '0';
    } else {
        waveElement.style.top = '90px';
    }
}

// Check viewport width on load and resize
window.addEventListener('load', checkViewportWidth);
window.addEventListener('resize', checkViewportWidth);