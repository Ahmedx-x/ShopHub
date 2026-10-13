// myShop — shared UI interactions (presentation only)
(function () {
    "use strict";

    function onReady(fn) {
        if (document.readyState !== "loading") {
            fn();
        } else {
            document.addEventListener("DOMContentLoaded", fn);
        }
    }

    onReady(function () {
        setupHeaderToggle();
        setupAccountMenu();
        setupImagePreviews();
        setupConfirmForms();
    });

    /* Mobile navigation drawer -------------------------------------------- */
    function setupHeaderToggle() {
        var toggle = document.querySelector("[data-nav-toggle]");
        var region = document.getElementById("site-nav-region");
        if (!toggle || !region) {
            return;
        }

        toggle.addEventListener("click", function () {
            var isOpen = region.classList.toggle("is-open");
            toggle.setAttribute("aria-expanded", String(isOpen));
        });
    }

    /* Account dropdown ---------------------------------------------------- */
    function setupAccountMenu() {
        var menu = document.querySelector("[data-account-menu]");
        if (!menu) {
            return;
        }

        var trigger = menu.querySelector("[data-account-menu-trigger]");
        var panel = menu.querySelector("[data-account-menu-panel]");
        if (!trigger || !panel) {
            return;
        }

        function close() {
            panel.classList.remove("is-open");
            trigger.setAttribute("aria-expanded", "false");
        }

        trigger.addEventListener("click", function (event) {
            event.stopPropagation();
            var isOpen = panel.classList.toggle("is-open");
            trigger.setAttribute("aria-expanded", String(isOpen));
        });

        document.addEventListener("click", function (event) {
            if (!menu.contains(event.target)) {
                close();
            }
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                close();
            }
        });
    }

    /* Image preview for admin file inputs -------------------------------- */
    function setupImagePreviews() {
        var inputs = document.querySelectorAll("[data-image-preview]");
        Array.prototype.forEach.call(inputs, function (input) {
            input.addEventListener("change", function () {
                var target = document.getElementById(input.getAttribute("data-image-preview"));
                if (!target || !input.files || !input.files[0]) {
                    return;
                }
                target.src = URL.createObjectURL(input.files[0]);
            });
        });
    }

    /* Confirmation for consequential admin actions ------------------------ */
    function setupConfirmForms() {
        var forms = document.querySelectorAll("form[data-confirm]");
        Array.prototype.forEach.call(forms, function (form) {
            form.addEventListener("submit", function (event) {
                if (form.dataset.confirmed === "true") {
                    return;
                }

                event.preventDefault();
                var message = form.getAttribute("data-confirm");
                var title = form.getAttribute("data-confirm-title") || "Are you sure?";
                var confirmText = form.getAttribute("data-confirm-button") || "Confirm";
                var proceed = function () {
                    form.dataset.confirmed = "true";
                    form.submit();
                };

                if (typeof Swal !== "undefined") {
                    Swal.fire({
                        title: title,
                        text: message,
                        icon: "warning",
                        showCancelButton: true,
                        confirmButtonText: confirmText,
                        confirmButtonColor: "#dc2626",
                        cancelButtonText: "Cancel"
                    }).then(function (result) {
                        if (result.isConfirmed) {
                            proceed();
                        }
                    });
                } else if (window.confirm(message)) {
                    proceed();
                }
            });
        });
    }
})();
