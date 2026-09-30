const statusEl = document.getElementById('status');
const connIdEl = document.getElementById('connId');
const logEl    = document.getElementById('log');
const input    = document.getElementById('msgInput');
const sendBtn  = document.getElementById('sendBtn');

const roomInput     = document.getElementById('roomInput');
const joinBtn       = document.getElementById('joinBtn');
const leaveBtn      = document.getElementById('leaveBtn');
const roomMsgInput  = document.getElementById('roomMsgInput');
const sendRoomBtn   = document.getElementById('sendRoomBtn');
const currentRoomEl = document.getElementById('currentRoom');

const onlineCountEl = document.getElementById('onlineCount');
const systemInput   = document.getElementById('systemInput');
const sendSystemBtn = document.getElementById('sendSystemBtn');

let currentRoom = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/activity')
    .build();

function log(text) {
    const div = document.createElement('div');
    const time = new Date().toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
    div.textContent = '[' + time + '] ' + text;
    logEl.appendChild(div);
    logEl.scrollTop = logEl.scrollHeight;
}

connection.on('UserConnected', function (connId) {
    log('Подключился: ' + connId);
});

connection.on('UserDisconnected', function (connId) {
    log('Отключился: ' + connId);
});

connection.on('ReceiveMessage', function (senderId, text) {
    log(senderId + ': ' + text);
});

connection.on('RoomJoined', function (roomName) {
    currentRoom = roomName;
    currentRoomEl.textContent = roomName;
    log('Вы вошли в комнату: ' + roomName);
});

connection.on('RoomLeft', function (roomName) {
    currentRoom = null;
    currentRoomEl.textContent = '—';
    log('Вы вышли из комнаты: ' + roomName);
});

connection.on('ReceiveRoomMessage', function (senderId, roomName, text) {
    log('[комната ' + roomName + '] ' + senderId + ': ' + text);
});

connection.on('OnlineCount', function (count) {
    onlineCountEl.textContent = count;
});

connection.on('SystemMessage', function (text) {
    log('*** СИСТЕМА: ' + text + ' ***');
});

async function start() {
    try {
        await connection.start();
        statusEl.textContent = 'подключено';
        connIdEl.textContent = connection.connectionId;
        sendBtn.disabled = false;
        joinBtn.disabled = false;
        leaveBtn.disabled = false;
        sendRoomBtn.disabled = false;
        log('Мы подключились. Наш ID: ' + connection.connectionId);
    } catch (err) {
        statusEl.textContent = 'ошибка: ' + err;
        setTimeout(start, 3000);
    }
}

function send() {
    const text = input.value;
    if (!text) return;
    connection.invoke('SendMessage', text);
    input.value = '';
    input.focus();
}

async function joinRoom() {
    const name = roomInput.value.trim();
    if (!name) return;

    if (currentRoom && currentRoom !== name) {
        await connection.invoke('LeaveRoom', currentRoom);
    }

    await connection.invoke('JoinRoom', name);
}

function leaveRoom() {
    const name = roomInput.value;
    if (!name) return;
    connection.invoke('LeaveRoom', name);
}

function sendToRoom() {
    const text = roomMsgInput.value;
    if (!text || !currentRoom) return;
    connection.invoke('SendToRoom', currentRoom, text);
    roomMsgInput.value = '';
    roomMsgInput.focus();
}

sendBtn.addEventListener('click', send);
input.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') send();
});

joinBtn.addEventListener('click', joinRoom);
leaveBtn.addEventListener('click', leaveRoom);
sendRoomBtn.addEventListener('click', sendToRoom);
roomMsgInput.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') sendToRoom();
});

async function sendSystemMessage() {
    const text = systemInput.value;
    if (!text) return;

    try {
        const resp = await fetch('/api/system/message', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ text: text })
        });
        if (!resp.ok) {
            log('Ошибка HTTP: ' + resp.status);
        }
    } catch (e) {
        log('Ошибка: ' + e);
    }

    systemInput.value = '';
}

sendSystemBtn.addEventListener('click', sendSystemMessage);

start();