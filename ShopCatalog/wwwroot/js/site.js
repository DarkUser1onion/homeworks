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