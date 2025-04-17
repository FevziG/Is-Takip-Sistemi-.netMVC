// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#chatButton").click(function () {
        $("#chatContainer").toggle();
    });

    $("#closeChat").click(function () {
        $("#chatContainer").hide();
    });

    $("#sendChat").click(function () {
        sendMessage();
    });

    $("#chatInput").keypress(function (e) {
        if (e.which == 13) {  // Enter tuşu
            sendMessage();
        }
    });

    function sendMessage() {
        var message = $("#chatInput").val().trim();
        if (message === "") return;

        $("#chatMessages").append(`<div><strong>Admin:</strong> ${message}</div>`);
        $("#chatInput").val("");

        $.ajax({
            url: "/Chat/SendMessage",
            type: "POST",
            data: { message: message },
            success: function (response) {
                $("#chatMessages").append(`<div><strong>AI:</strong> ${response}</div>`);
            }
        });
    }
});
