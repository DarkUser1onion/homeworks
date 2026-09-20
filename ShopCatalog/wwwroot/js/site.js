(function () {
    const filterBox = document.getElementById('tagFilter');
    if (!filterBox) return;

    const pills = filterBox.querySelectorAll('[data-tag]');

    pills.forEach(pill => {
        pill.addEventListener('click', function () {
            pills.forEach(p => p.classList.remove('active'));
            this.classList.add('active');
        });
    });
})();

document.addEventListener('DOMContentLoaded', async function () {
    const badge = document.getElementById('cartBadge');
    if (!badge) return;

    try {
        const response = await fetch('/Catalog/GetCartCount');
        if (!response.ok) return;

        const data = await response.json();
        badge.textContent = data.count ?? 0;
    } catch (error) {
        console.error('Не удалось получить количество товаров:', error);
    }
});