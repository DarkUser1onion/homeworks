(function () {
    const input = document.getElementById('searchInput');
    const grid = document.getElementById('catalogGrid');

    if (!input || !grid) return;

    const saved = sessionStorage.getItem('catalogSearch');
    if (saved && saved.trim().length >= 2) {
        input.value = saved;
        searchProducts(saved.trim());
    }

    let timeoutId;

    input.addEventListener('input', function () {
        sessionStorage.setItem('catalogSearch', input.value);
        clearTimeout(timeoutId);

        const query = input.value.trim();

    if (query.length < 2) {
        timeoutId = setTimeout(function () {
            loadAllProducts();
        }, 300);
        return;
    }

        timeoutId = setTimeout(function () {
            searchProducts(query);
        }, 300);
    });

    async function searchProducts(query) {
        grid.innerHTML =
            '<div class="text-center py-5 w-100">' +
            '  <div class="spinner-border text-primary" role="status">' +
            '    <span class="visually-hidden">Загрузка...</span>' +
            '  </div>' +
            '</div>';

        try {
            const url = '/Catalog/Search?query=' + encodeURIComponent(query);
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error('HTTP ' + response.status);
            }

            const html = await response.text();

            if (!html.trim()) {
                grid.innerHTML =
                    '<div class="alert alert-info w-100">Ничего не найдено</div>';
            } else {
                grid.innerHTML = html;
            }
        } catch (error) {
            grid.innerHTML =
                '<div class="alert alert-danger w-100">Ошибка поиска. Попробуйте ещё раз.</div>';
            console.error('Search error:', error);
        }
    }

    async function loadAllProducts() {
        grid.innerHTML =
            '<div class="text-center py-5 w-100">' +
            '  <div class="spinner-border text-primary" role="status"></div>' +
            '</div>';

        try {
            const response = await fetch('/Catalog/Search');
            if (!response.ok) throw new Error('HTTP ' + response.status);

            const html = await response.text();
            grid.innerHTML = html.trim()
                ? html
                : '<div class="alert alert-info w-100">Ничего не найдено</div>';
        } catch (error) {
            grid.innerHTML =
                '<div class="alert alert-danger w-100">Ошибка загрузки. Попробуйте ещё раз.</div>';
            console.error(error);
        }
    }
})();