﻿"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
const chatEvents = {
    receiveMessage: "ReceiveMessage",
    sendMessage: "SendMessage",
}
const messageBlock = (sender, msg, incoming = '') => `<div class="message ${incoming}"><div class="message-sender">${sender}</div><div class="message-content">${msg}</div></div>`
const sendMsgButton = document.getElementById("sendMessageBtn");
const msgList = document.querySelector(".message-list");
sendMsgButton.disabled = true;
const messageInput = document.getElementById("messageInput");
const msgPlaceholder = "write your message...";
messageInput.placeholder = msgPlaceholder;
const username = document.getElementById("user-name")?.innerText;

connection.on(chatEvents.receiveMessage, function (user, message) {
    console.log('received message ', message, 'from ', user);
    const msg = messageBlock(user, message, "incoming");
    msgList.insertAdjacentHTML("beforeend", msg);
});

connection.start().then(function () {
    sendMsgButton.disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

sendMsgButton.addEventListener("click", function (event) {
    const receiver = [...document
        .querySelectorAll("input[name=selectedUser]")]
        .find(input => input.checked)?.value;

    if (receiver) {
        connection.invoke(chatEvents.sendMessage, username, receiver, messageInput.value).catch(function (err) {
            return console.error(err.toString());
        });
        const msg = messageBlock(username, messageInput.value);
        msgList.insertAdjacentHTML("beforeend", msg);
        messageInput.value = '';
        messageInput.placeholder = msgPlaceholder;
    }
    event.preventDefault();
});