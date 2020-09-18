/**
 * Chat room variables.
 */
const CHAT_HUB_ENDPOINT = "http://localhost:8080/chathub";
const RPC_JOIN_CHAT_ROOM = "JoinChatRoom";
const RPC_LEAVE_CHAT_ROOM = "LeaveChatRoom";
const RPC_SEND_MESSAGE_TO_CHAT_ROOM = "BroadcastMessageToChatRoom";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(CHAT_HUB_ENDPOINT)
    .build();

connection.on("Send", function (message) {
    var newChatMessageLi = document.createElement("li");
    newChatMessageLi.textContent = message;
    document.getElementById("chat-messages-list").appendChild(newChatMessageLi);
});

document.getElementById("chat-message-submit").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-message-input").value;
    var chatMessage = document.getElementById("chat-message-input").value;

    try {
        await connection.invoke(RPC_SEND_MESSAGE_TO_CHAT_ROOM, chatMessage, chatRoomName, "igp");
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});

document.getElementById("join-chat-room").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-room-name").value;

    try {
        await connection.invoke(RPC_JOIN_CHAT_ROOM, "igp", chatRoomName);
    }
    catch (e) {
        console.error(e.toString());
    }

    event.preventDefault();
});

document.getElementById("leave-chat-room").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-room-name").value;

    try {
        await connection.invoke(RPC_LEAVE_CHAT_ROOM, "igp", chatRoomName);
    }
    catch (e) {
        console.error(e.toString());
    }

    event.preventDefault();
});

// We need an async function in order to use await, but we want this code to run immediately,
// so we use an "immediately-executed async function"
(async () => {
    try {
        await connection.start();
    }
    catch (e) {
        console.error(e.toString());
    }
})();