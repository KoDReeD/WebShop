
document.addEventListener("DOMContentLoaded", function () {
    var textareas = document.querySelectorAll("textarea");

    textareas.forEach(function (textarea) {
        textarea.style.height = "auto";
        textarea.style.height = (textarea.scrollHeight) + "px";
    });
});
