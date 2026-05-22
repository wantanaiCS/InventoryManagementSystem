// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-password-toggle]").forEach((button) => {
        const targetId = button.getAttribute("aria-controls");
        const input = targetId ? document.getElementById(targetId) : null;

        if (!input) {
            return;
        }

        button.addEventListener("click", () => {
            const showPassword = input.type === "password";
            const icon = button.querySelector("i");

            input.type = showPassword ? "text" : "password";
            button.setAttribute("aria-label", showPassword ? "Hide password" : "Show password");

            if (icon) {
                icon.classList.toggle("fa-eye", !showPassword);
                icon.classList.toggle("fa-eye-slash", showPassword);
            }
        });
    });
});
