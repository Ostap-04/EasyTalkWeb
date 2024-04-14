﻿"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
console.log(connection);
//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;
//const username = document.getElementById("user");
connection.on("ReceiveMessage", function (user, message) {
    cosnole.log('received message ', message, 'from ', user);
    const li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});