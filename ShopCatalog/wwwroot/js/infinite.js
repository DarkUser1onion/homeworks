(function () {
    const grid = document.getElementById('catalogGrid');
    const sentinel = document.getElementById('sentinel');
    const spinner = document.getElementById('loadMoreSpinner');

    if (!grid || !sentinel) return;

    let currentPage = 1;
    let isLoading = false;
    let hasMore = true;

    const observer = new IntersectionObserver(async function (entries) {
        if (window.__infiniteDisabled) return;
        if (!entries[0].isIntersecting) return;
        if (isLoading || !hasMore) return;

        isLoading = true;
        if (spinner) spinner.style.display = 'block';

        currentPage++;

        try {
            const url = '/Catalog/LoadMore?page=' + currentPage;
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error('HTTP ' + response.status);
            }

            const html = await response.text();

            if (html.trim()) {
                grid.insertAdjacentHTML('beforeend', html);
                isLoading = false;
            } else {
                hasMore = false;
                observer.disconnect();
            }
        } catch (error) {
            console.error('LoadMore error:', error);
            isLoading = false;
        } finally {
            if (spinner) spinner.style.display = 'none';
        }
    }, {
        rootMargin: '200px'
    });

    observer.observe(sentinel);
})();