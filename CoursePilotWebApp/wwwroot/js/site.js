// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// const variables to store the body and the toggle button elements
const body = document.querySelector("body");
const darkModeToggle = document.getElementById("darkModeToggle");

// Used local storage to get the local storage dark mode setting
let getMode = localStorage.getItem("mode");

// Function to toggle light and dark mode
function toggle() {
    body.classList.toggle('dark');
    body.classList.toggle('light');
}

// Check the stored mode and apply it to the web page
if (getMode) {
    body.classList.add(getMode);

    // Updates the checkbox state based on the mode
    darkModeToggle.checked = getMode === "dark";
}

//event listener to check if the toggle element is clicked
darkModeToggle.addEventListener("click", () => {
    toggle();

    // Updates the mode in local storage 
    if (body.classList.contains("dark")) {
        localStorage.setItem("mode", "dark");
    } else {
        localStorage.setItem("mode", "light");
    }
});







