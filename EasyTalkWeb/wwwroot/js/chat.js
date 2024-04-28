const connection = new signalR.HubConnectionBuilder()
  .withUrl('/chatHub')
  .build();

const chatEvents = {
  receiveMessage: 'ReceiveMessage',
  sendMessage: 'SendMessage',
  joinChatRoom: 'JoinChatRoom',
  leaveChatRoom: 'LeaveChatRoom',
  typingActivity: 'TypingActivity',
};
const chatContainer = document.querySelector('.chat-container');
const chatButtons = chatContainer.querySelectorAll('.button-wrapper');
const sendMsgButton = chatContainer.querySelector('.sendMessageBtn');
const msgList = chatContainer.querySelector('.message-list');
const messageInput = chatContainer.querySelector('.messageInput');
const chatBody = chatContainer.querySelector('.chat-body');
const chatHead = chatContainer.querySelector('.chat-head');
const chatTitleEl = chatBody.querySelector('.chat-head__title');
const chatActivityEl = chatBody.querySelector('.chat-head__activity');
const chatActivityMemberEl = chatBody.querySelector('.chat-activity__name');
const noMessaggesLabel = chatBody.querySelector('.no-messages');

let currentRoomIdx;
let isConnected = false;
sendMsgButton.disabled = true;
const msgPlaceholder = messageInput.placeholder;
const currentChatData = Object.seal({
  currentRoomId: '',
  title: '',
  description: '',
  members: [],
  user: {},
  messages: [],
});

connection
  .start()
  .then(function () {
    isConnected = true;
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

connection.on(
  chatEvents.receiveMessage,
  function (senderId, senderName, message) {
    noMessaggesLabel.hidden = true;
    let msg = makeMessageBlock(
      senderName,
      message,
      currentChatData.user.Id === senderId
    );

    msgList.insertAdjacentHTML('beforeend', msg);
  }
);

let activityTimer;
const typingUsers = new Set();
connection.on(chatEvents.typingActivity, function (name) {
  console.log('typing', name);
  chatActivityEl.classList.add('show');
  typingUsers.add(name);
  chatActivityMemberEl.innerHTML = [...typingUsers.values()].toString();
  clearTimeout(activityTimer);
  activityTimer = setTimeout(() => {
    chatActivityEl.classList.remove('show');
    typingUsers.clear();
    activityUser.innerHTML = '';
  }, 2000);
});

messageInput.addEventListener('input', (e) => {
  connection
    .invoke(
      chatEvents.typingActivity,
      currentChatData.currentRoomId,
      `${currentChatData.user.FirstName} ${currentChatData.user.LastName}`
    )
    .catch((err) => console.error(err));
});

chatButtons.forEach((chatBtn, idx) => {
  chatBtn.addEventListener('click', async function () {
    if (isConnected) {
      const chatId = this.getAttribute('data-chat-id');
      if (
        !!currentChatData.currentRoomId &&
        currentChatData.currentRoomId !== chatId
      ) {
        await connection.invoke(
          chatEvents.leaveChatRoom,
          currentChatData.currentRoomId
        );
      }
      currentChatData.currentRoomId = chatId;
      if (currentRoomIdx !== undefined)
        chatButtons[currentRoomIdx].classList.remove('active');

      currentRoomIdx = idx;
      chatButtons[currentRoomIdx].classList.add('active');
      sendMsgButton.disabled = false;
      currentChatData.currentRoomId = chatId;
      connection
        .invoke(chatEvents.joinChatRoom, chatId)
        .then(async () => {
          const data = await fetchMessages(currentChatData.currentRoomId);
          const { userId, chatData } = data;
          const {
            Name: chatName,
            Description: descr,
            Members: members,
            Messages: messages,
            } = JSON.parse(chatData);
          currentChatData.title = chatName;
          currentChatData.description = descr;
          currentChatData.members.length = 0;
          currentChatData.members = members;
          currentChatData.messages = messages;
          console.log(currentChatData);
          if (messages.length === 0) noMessaggesLabel.hidden = false;
          currentChatData.user = members.find((member) => member.Id == userId);
          renderChatBody();
        })
        .catch((e) => {
          console.log('error when connecing to the room', e.message);
        });
    }
  });
});

//! TODO
chatHead.addEventListener('click', (e) => {});

sendMsgButton.addEventListener('click', function (event) {
  event.preventDefault();
  if (messageInput.value.trim().length) {
    connection
      .invoke(
        chatEvents.sendMessage,
        currentChatData.currentRoomId,
        currentChatData.user.Id,
        `${currentChatData.user.FirstName} ${currentChatData.user.LastName}`,
        messageInput.value
      )
      .catch(function (err) {
        return console.error(err.toString());
      });
    messageInput.value = '';
    messageInput.placeholder = msgPlaceholder;
  }
});

async function fetchMessages(id) {
  const response = await fetch(`/Chat/LoadChatData/${id}`, {
    method: 'GET',
    headers: {
      Accept: 'application/json; charset=utf-8',
      'Content-Type': 'application/json;charset=UTF-8',
    },
  });

  if (!response.ok) return null;

  const data = await response.json();
  return data;
}

function renderChatBody() {
  msgList.innerHTML = '';
  chatTitleEl.innerHTML = currentChatData.title;
  let messageHistory = currentChatData.messages
    .sort((m1, m2) => new Date(m1.Date) - new Date(m2.Date))
    .map((message) => {
      const { Text, SenderId, SenderName } = message;
      return makeMessageBlock(
        SenderName,
        Text,
        SenderId === currentChatData.user.Id
      );
    })
    .join('');

  msgList.insertAdjacentHTML('beforeend', messageHistory);
  chatBody.classList.add('active');
  sendMsgButton.disabled = false;
}

function makeMessageBlock(sender, msg, isOwnMessage) {
  if (isOwnMessage)
    return `<div class="message own"><div class="message-sender">You:</div><div class="message-content">${msg}</div></div>`;
  else
    return `<div class="message"><div class="message-sender">${sender}:</div><div class="message-content">${msg}</div></div>`;
}

function showChatDetails() {}
