
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.on("InternalMessage", function (message) {
    debugger
    document.getElementById('mesage-count-order').innerHTML = message;
});
