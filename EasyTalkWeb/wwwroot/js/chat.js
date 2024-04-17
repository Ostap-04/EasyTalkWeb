"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
const chatEvents = {
    receiveMessage: "ReceiveMessage",
    sendMessage: "SendMessage",
}

document.getElementById("sendButton").disabled = true;
const username = document.getElementById("user-name")?.innerText;
console.log(username)
connection.on(chatEvents.receiveMessage, function (user, message) {
    console.log('received message ', message, 'from ', user);
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
    const receiver = [...document
        .querySelectorAll("input[name=selectedUser]")]
        .find(input => input.checked)?.value;

    const message = document.getElementById("messageInput").value;
    if (receiver) {
        connection.invoke(chatEvents.sendMessage, username, receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});