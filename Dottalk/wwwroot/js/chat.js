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
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messagesList").appendChild(li);
});

document.getElementById("groupmsg").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    var groupMsg = document.getElementById("group-message-text").value;
    try {
        await connection.invoke(RPC_SEND_MESSAGE_TO_CHAT_ROOM, groupMsg, groupName, "igp");
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});

document.getElementById("join-group").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    try {
        await connection.invoke(RPC_JOIN_CHAT_ROOM, "igp", groupName);
    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
});

document.getElementById("leave-group").addEventListener("click", async (event) => {
    var groupName = document.getElementById("group-name").value;
    try {
        await connection.invoke(RPC_LEAVE_CHAT_ROOM, "igp", groupName);
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