(function () {
    document.addEventListener('click', async function (e) {
        const button = e.target.closest('.add-to-cart');
        if (!button) return;

        const productId = button.dataset.productId;
        await addToCart(productId, button);
    });

    async function addToCart(productId, button) {
        const originalText = button.textContent;

        button.disabled = true;
        button.innerHTML =
            '<span class="spinner-border spinner-border-sm me-1"></span>Добавление...';

        try {
            const response = await fetch('/Catalog/AddToCart', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'id=' + encodeURIComponent(productId)
            });

            if (!response.ok) {
                throw new Error('HTTP ' + response.status);
            }

            const data = await response.json();

            if (data.success) {
                const badge = document.getElementById('cartBadge');
                if (badge) badge.textContent = data.cartCount;

                showCartToast('Товар «' + data.productName + '» добавлен в корзину');

                button.innerHTML = '✓ Добавлено';
                button.classList.remove('btn-primary');
                button.classList.add('btn-success');

                setTimeout(function () {
                    button.textContent = originalText;
                    button.classList.remove('btn-success');
                    button.classList.add('btn-primary');
                    button.disabled = false;
                }, 1000);
            } else {
                alert('Ошибка: ' + data.message);
                button.textContent = originalText;
                button.disabled = false;
            }
        } catch (error) {
            console.error(error);
            button.textContent = 'Ошибка';
            button.disabled = false;

            setTimeout(function () {
                button.textContent = originalText;
            }, 2000);
        }
    }
    function showCartToast(message) {
        const toastEl = document.getElementById('cartToast');
        const toastText = document.getElementById('cartToastText');
        if (!toastEl || !toastText) return;

        toastText.textContent = message;

        const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
        toast.show();
    }
})();