const statusEl = document.getElementById('status');
const connIdEl = document.getElementById('connId');
const logEl    = document.getElementById('log');
const input    = document.getElementById('msgInput');
const sendBtn  = document.getElementById('sendBtn');

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/activity')
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

function appendEntry(kind, text) {
    const div = document.createElement('div');
    div.className = 'entry ' + kind;
    const time = new Date().toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
    div.textContent = '[' + time + '] ' + text;
    logEl.appendChild(div);
    logEl.scrollTop = logEl.scrollHeight;
}

function setStatus(text, cssClass) {
    statusEl.textContent = text;
    statusEl.className = 'status ' + cssClass;
}

connection.on('UserConnected', (connId) => {
    appendEntry('system', 'Подключился пользователь' + connId);
});

connection.on('UserDisconnected', (connId) => {
    appendEntry('system', `Отключился пользователь <span class="who">${connId}</span>`);
});

connection.on('ReceiveMessage', (senderId, text) => {
    const isSelf = senderId === connection.connectionId;
    const kind = isSelf ? 'message self' : 'message';
    const label = isSelf ? 'Вы' : `<span class="who">${senderId}</span>`;
    appendEntry(kind, `${label}: ${escapeHtml(text)}`);
});

connection.onreconnecting(() => {
    setStatus('Переподключение…', 'connecting');
    appendEntry('system', 'Соединение потеряно, пытаемся переподключиться…');
});

connection.onreconnected((newId) => {
    setStatus('Подключено', 'connected');
    connIdEl.textContent = newId;
    appendEntry('system', `Переподключено. Новый ID: <span class="who">${newId}</span>`);
});

connection.onclose(() => {
    setStatus('Соединение потеряно', 'disconnected');
    appendEntry('system', 'Соединение закрыто окончательно');
    sendBtn.disabled = true;
});

async function start() {
    try {
        await connection.start();
        setStatus('Подключено', 'connected');
        connIdEl.textContent = connection.connectionId;
        sendBtn.disabled = false;
        appendEntry('system', 'Подключено. Ваш ID: ' + connection.connectionId);
    } catch (err) {
        setStatus('Ошибка подключения', 'disconnected');
        appendEntry('system', 'Не удалось подключиться: ' + err);
        setTimeout(start, 3000);
    }
}

async function send() {
    const text = input.value.trim();
    if (!text) return;
    try {
        await connection.invoke('SendMessage', text);
        input.value = '';
        input.focus();
    } catch (err) {
        appendEntry('system', 'Ошибка отправки: ' + err);
    }
}

sendBtn.addEventListener('click', send);
input.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') send();
});

function escapeHtml(s) {
    return String(s)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

start();