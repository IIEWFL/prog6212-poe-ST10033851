// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const body = document.querySelector("body");
const darkModeToggle = document.getElementById("darkModeToggle");

let getMode = localStorage.getItem("mode");

function toggle() {
    body.classList.toggle('dark');
    body.classList.toggle('light');
}

// Check the stored mode and apply it
if (getMode) {
    body.classList.add(getMode);

    // Update the checkbox state based on the mode
    darkModeToggle.checked = getMode === "dark";
}

darkModeToggle.addEventListener("click", () => {
    toggle();

    // Update the mode in local storage
    if (body.classList.contains("dark")) {
        localStorage.setItem("mode", "dark");
    } else {
        localStorage.setItem("mode", "light");
    }
});







