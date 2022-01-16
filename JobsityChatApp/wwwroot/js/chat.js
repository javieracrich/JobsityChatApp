class Message {
    constructor(username, text) {
        this.userName = username;
        this.text = text;
    }
}

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Home/Index')
    .configureLogging("trace")
    .build();

connection.on('receiveMessage', onReceiveMessage);


connection.start()
    .then(() => console.info("connection started"))
    .catch((err) => console.error('Error while establishing signalr connection: ' + err))


const chat = document.getElementById('chat');
var text = document.getElementById('messageText');
const messagesQueue = [];

document.getElementById('submitButton').addEventListener('click', () => {
    debugger;
    var msg = new Message(userName, text.value);
    connection.invoke('sendMessage', msg);
    text.value = "";
});

function onReceiveMessage(message) {
    debugger;
    let isCurrentUserMessage = message.userName === userName;

    let row = document.createElement('div');
    let container = document.createElement('div');
    let subContainer = document.createElement('div');
    container.className = isCurrentUserMessage ? "col-md-6 offset-md-6" : "col-md-6";
    subContainer.className = isCurrentUserMessage ? "container darker" : "container ";
    row.className = "row"
    row.appendChild(container);
    container.appendChild(subContainer);

    let header = document.createElement('p');
    header.className = "sender";
    header.innerHTML = `${message.userName} - ${message.created}`;

    let text = document.createElement('p');
    text.innerHTML = message.text;
    subContainer.appendChild(header);
    subContainer.appendChild(text);
    chat.prepend(row);
}



