const statusEl      = document.getElementById('status');
const connIdEl      = document.getElementById('connId');
const onlineCountEl = document.getElementById('onlineCount');
const logEl         = document.getElementById('log');
const input         = document.getElementById('msgInput');
const sendBtn       = document.getElementById('sendBtn');

const roomInput     = document.getElementById('roomInput');
const joinBtn       = document.getElementById('joinBtn');
const leaveBtn      = document.getElementById('leaveBtn');
const roomMsgInput  = document.getElementById('roomMsgInput');
const sendRoomBtn   = document.getElementById('sendRoomBtn');
const currentRoomEl = document.getElementById('currentRoom');

const systemInput   = document.getElementById('systemInput');
const sendSystemBtn = document.getElementById('sendSystemBtn');

const privateTargetInput    = document.getElementById('privateTargetInput');
const privateTextInput      = document.getElementById('privateTextInput');
const sendPrivateBtn        = document.getElementById('sendPrivateBtn');

const externalRoomInput     = document.getElementById('externalRoomInput');
const externalRoomTextInput = document.getElementById('externalRoomTextInput');
const sendExternalRoomBtn   = document.getElementById('sendExternalRoomBtn');

const typingIndicatorEl = document.getElementById('typingIndicator');

let currentRoom = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/activity')
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .build();

function log(text) {
    const div = document.createElement('div');
    const time = new Date().toLocaleTimeString('ru-RU', {
        hour: '2-digit', minute: '2-digit', second: '2-digit'
    });
    div.textContent = '[' + time + '] ' + text;
    logEl.appendChild(div);
    logEl.scrollTop = logEl.scrollHeight;
}

function setStatus(text) {
    statusEl.textContent = text;
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

connection.on('PrivateMessage', function (senderId, text) {
    log('[ПРИВАТНО от ' + senderId + '] ' + text);
});

connection.on('PrivateMessageSent', function (targetId, text) {
    log('[ПРИВАТНО → ' + targetId + '] ' + text);
});

connection.on('ExternalRoomMessage', function (roomName, text) {
    log('[внешнее в комнату ' + roomName + '] ' + text);
});

connection.onreconnecting(function (error) {
    setStatus('Переподключение...');
    log('Соединение потеряно. Переподключаемся...');
});

let typingTimeoutId = null;

connection.on('UserTyping', function (senderId, roomName) {
    typingIndicatorEl.textContent = 'Печатает (' + roomName + '): ' + senderId;
    typingIndicatorEl.style.display = 'block';

    clearTimeout(typingTimeoutId);
    typingTimeoutId = setTimeout(function () {
        typingIndicatorEl.style.display = 'none';
    }, 2000);
});

connection.onreconnected(async function (newConnectionId) {
    setStatus('подключено');
    connIdEl.textContent = newConnectionId;
    log('Переподключено. Новый ID: ' + newConnectionId);

    if (currentRoom) {
        try {
            await connection.invoke('JoinRoom', currentRoom);
            log('Автоматически вернулись в комнату: ' + currentRoom);
        } catch (err) {
            log('Не удалось вернуться в комнату: ' + err);
        }
    }
});

connection.onclose(function (error) {
    setStatus('Соединение потеряно');
    log('Соединение закрыто окончательно');
    sendBtn.disabled = true;
    joinBtn.disabled = true;
    leaveBtn.disabled = true;
    sendRoomBtn.disabled = true;
});

async function start() {
    try {
        await connection.start();
        setStatus('подключено');
        connIdEl.textContent = connection.connectionId;
        sendBtn.disabled = false;
        joinBtn.disabled = false;
        leaveBtn.disabled = false;
        sendRoomBtn.disabled = false;
        sendPrivateBtn.disabled = false;
        log('Мы подключились. Наш ID: ' + connection.connectionId);
    } catch (err) {
        setStatus('ошибка: ' + err);
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

    if (currentRoom === name) {
        log('Вы уже в комнате ' + name);
        return;
    }
    if (currentRoom) {
        await connection.invoke('LeaveRoom', currentRoom);
    }
    await connection.invoke('JoinRoom', name);
}

function leaveRoom() {
    const name = roomInput.value.trim();
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

async function sendSystemMessage() {
    const text = systemInput.value;
    if (!text) return;

    try {
        const resp = await fetch('/api/system/message', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ text: text })
        });
        if (!resp.ok) log('HTTP ошибка: ' + resp.status);
    } catch (e) {
        log('Ошибка: ' + e);
    }
    systemInput.value = '';
}

sendBtn.addEventListener('click', send);
input.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') send();
});

joinBtn.addEventListener('click', joinRoom);
leaveBtn.addEventListener('click', leaveRoom);
sendRoomBtn.addEventListener('click', sendToRoom);
let typingSendTimeoutId = null;
roomMsgInput.addEventListener('input', function () {
    if (!currentRoom) return;
    clearTimeout(typingSendTimeoutId);
    typingSendTimeoutId = setTimeout(function () {
        connection.invoke('Typing', currentRoom);
    }, 500);
});
roomMsgInput.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') sendToRoom();
});

sendSystemBtn.addEventListener('click', sendSystemMessage);

function sendPrivate() {
    const target = privateTargetInput.value.trim();
    const text = privateTextInput.value;
    if (!target || !text) return;

    connection.invoke('SendPrivate', target, text);
    privateTextInput.value = '';
    privateTextInput.focus();
}

async function sendExternalRoom() {
    const room = externalRoomInput.value.trim();
    const text = externalRoomTextInput.value;
    if (!room || !text) return;

    try {
        const resp = await fetch('/api/system/room', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ roomName: room, text: text })
        });
        if (!resp.ok) log('HTTP ошибка: ' + resp.status);
        else log('Внешнее уведомление отправлено в комнату ' + room);
    } catch (e) {
        log('Ошибка: ' + e);
    }

    externalRoomTextInput.value = '';
}

sendPrivateBtn.addEventListener('click', sendPrivate);
privateTextInput.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') sendPrivate();
});

sendExternalRoomBtn.addEventListener('click', sendExternalRoom);
externalRoomTextInput.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') sendExternalRoom();
});

start();