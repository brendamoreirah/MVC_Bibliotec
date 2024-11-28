function mostraMenu() {
    document.getElementById("navbar").classList.toggle("mostra-menu");
    document.getElementById("menu-hamburger").classList.toggle("menu-hamburger-rotate");
}

function recolheMenu(event) {
    const aside = event.target;
    aside.classList.toggle("mnu-arrow-left-min");
    document.getElementsByTagName("aside")[0].classList.toggle("aside-min");
    document.getElementsByTagName("nav")[0].classList.toggle("menu-lateral-min");
    document.querySelector(".aside-top > img").classList.toggle("logo-mobile-min");
    document.querySelector(".user-logout").classList.toggle("user-logout-min");

    if (window.innerWidth > 768) {
        document.getElementsByTagName("main")[0].classList.toggle("main-min");
    }

    if (window.innerWidth <= 768 && document.querySelector(".aside-min") == null) {
        document.querySelector(".sombra").classList.add("movimento-sombra");
    } else {
        document.querySelector(".sombra").classList.remove("movimento-sombra");
    }
}

window.onresize = posicaoResponsiva;
window.onload = posicaoResponsiva;

function posicaoResponsiva() {
    if (window.innerWidth <= 768) {
        document.getElementsByTagName("aside")[0].classList.add("aside-min");
        document.getElementsByTagName("nav")[0].classList.add("menu-lateral-min");
        document.querySelector(".aside-top > img").classList.add("logo-mobile-min");
        document.querySelector(".user-logout").classList.add("user-logout-min");
        document.querySelector(".mnu-arrow-left").classList.add("mnu-arrow-left-min");
    } else {
        document.getElementsByTagName("aside")[0].classList.remove("aside-min");
        document.getElementsByTagName("nav")[0].classList.remove("menu-lateral-min");
        document.querySelector(".aside-top > img").classList.remove("logo-mobile-min");
        document.querySelector(".user-logout").classList.remove("user-logout-min");
        document.querySelector(".mnu-arrow-left").classList.remove("mnu-arrow-left-min");
        document.querySelector(".sombra").classList.remove("movimento-sombra");
    }
}