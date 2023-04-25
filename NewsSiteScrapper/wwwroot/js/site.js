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

function updateWaveElementPosition() {
    const viewportWidth = window.innerWidth || document.documentElement.clientWidth;

    const navbarToggler = document.querySelector('.navbar-toggler');
    const waveElement = document.querySelector('.wave');

    if (navbarToggler.classList.contains('collapsed') && viewportWidth < 576) {
        waveElement.style.top = '0';
    } else if (viewportWidth < 576) {
        waveElement.style.top = '90px';
    } else {
        waveElement.style.top = '0';
    }
}

// Call the function once on page load
updateWaveElementPosition();

// Call the function on window resize
window.addEventListener('resize', updateWaveElementPosition);