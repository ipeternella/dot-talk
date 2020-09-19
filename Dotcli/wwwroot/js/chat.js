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

function writeServerMessageForSomeTime(message, isError = false, durationInMs = 1500) {
    let serverMessagesSection = document.getElementById("server-messages");
    const clearServerMessage = () => serverMessagesSection.innerHTML = "";

    serverMessagesSection.style.color = "LimeGreen";
    if (isError) serverMessagesSection.style.color = "IndianRed";

    serverMessagesSection.innerHTML = message;
    setTimeout(() => clearServerMessage(), durationInMs);  // clears after some time
}

function extractHubExceptionMessage(rawException) {
    const DEFAULT_EXCEPTION_MESSAGE = "Ops! Something went wrong in our servers... try again later!";
    const HUB_EXCEPTION_PREFIX = "HubException: ";
    let exceptionMessages = rawException.split(HUB_EXCEPTION_PREFIX);

    if (exceptionMessages.length > 1) return exceptionMessages[1];
    return DEFAULT_EXCEPTION_MESSAGE;
}

connection.on("ReceiveMessage", function (message) {
    var newChatMessageLi = document.createElement("li");

    newChatMessageLi.textContent = message;
    document.getElementById("chat-messages-list").appendChild(newChatMessageLi);
});

document.getElementById("chat-message-submit").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-message-input").value;
    var chatMessage = document.getElementById("chat-message-input").value;

    if (!userName) {
        console.log("User name has not been set!");
        return;
    }

    try {
        await connection.invoke(RPC_SEND_MESSAGE_TO_CHAT_ROOM, chatMessage, chatRoomName);
    }
    catch (e) {
        let hubException = extractHubExceptionMessage(e.toString());
        writeServerMessageForSomeTime(hubException, isError = true);
    }

    event.preventDefault();
});

document.getElementById("join-chat-room").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-room-name").value;
    userName = document.getElementById("user-name").value;

    if (!userName) {
        console.log("User name has not been set!");
        return;
    }

    try {
        await connection.invoke(RPC_JOIN_CHAT_ROOM, userName, chatRoomName);
        writeServerMessageForSomeTime(`Successfully connected to chat room ${chatRoomName}!`);
    }
    catch (e) {
        let hubException = extractHubExceptionMessage(e.toString());
        writeServerMessageForSomeTime(hubException, isError = true);
    }

    event.preventDefault();
});

document.getElementById("leave-chat-room").addEventListener("click", async (event) => {
    var chatRoomName = document.getElementById("chat-room-name").value;
    userName = document.getElementById("user-name").value;

    try {
        await connection.invoke(RPC_LEAVE_CHAT_ROOM, chatRoomName, userName);
        writeServerMessageForSomeTime(`Successfully left the chat room ${chatRoomName}`);
    }
    catch (e) {
        let hubException = extractHubExceptionMessage(e.toString());
        writeServerMessageForSomeTime(hubException, isError = true);
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