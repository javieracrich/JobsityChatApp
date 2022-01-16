class Message {
    constructor(username, text, room) {
        this.userName = username;
        this.text = text;
        this.roomId = room;
    }
}

const chat = document.getElementById('chat');
var text = document.getElementById('messageText');
var submit = document.getElementById('submitButton');
const messagesQueue = [];

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Home/Index')
    .configureLogging("trace")
    .build();

connection.onclose(error => {
    submit.disabled = true;
})

connection.on('receiveMessage', onReceiveMessage);
connection.on('userJoined', () => console.log(message));

connection.start()
    .then(() => {
        console.info("connection started");
        connection.invoke('AddToGroup', room);
        refreshMessges(room);
    })
    .catch((err) => {
        console.error('Error while establishing signalr connection: ' + err);
        submit.disabled = true;
    });

submit.addEventListener('click', () => {
    var msg = new Message(userName, text.value, parseInt(room));
    connection.invoke('sendMessage', msg);
    text.value = "";
});

function refreshMessges(roomId) {
    fetch(`/api/messages/${roomId}`)
        .then(response => {
            response.text().then(res => {
                chat.replaceChildren([]);
                let list = JSON.parse(res);
                for (let m of list) {
                    onReceiveMessage(m);
                }
            })
        })
        .catch(error => {
            console.error(error);

        });
}

function onReceiveMessage(message) {
    let isCurrentRoom = message.roomId == room;
    if (!isCurrentRoom) {
        return;
    }
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
    header.innerHTML = `${message.userName} - ${new Date(message.created).toISOString()}`;

    let text = document.createElement('p');
    text.innerHTML = message.text;
    subContainer.appendChild(header);
    subContainer.appendChild(text);
    chat.prepend(row);
}

function onRoomChanged() {
    room = document.getElementById("rooms").value;
    connection.invoke('AddToGroup', room);
    refreshMessges(room);
}

