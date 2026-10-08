document.addEventListener('click', async function (e) {
    if (!e.target.classList.contains('btn-complete')) return;

    const btn = e.target;
    const taskId = btn.dataset.taskId;

    const meta = document.querySelector('meta[name="csrf-token"]');
    const token = meta ? meta.getAttribute('content') : '';

    btn.disabled = true;

    try {
        const response = await fetch('/Tasks/Index?handler=ToggleComplete&id=' + taskId, {
            method: 'POST',
            headers: {
                'X-CSRF-TOKEN': token
            }
        });

        if (!response.ok) throw new Error('HTTP ' + response.status);

        const data = await response.json();
        btn.textContent = data.isCompleted ? 'Снять отметку' : 'Отметить выполненной';
        btn.disabled = false;
    } catch (err) {
        console.error(err);
        btn.disabled = false;
        alert('Ошибка: ' + err.message);
    }
});