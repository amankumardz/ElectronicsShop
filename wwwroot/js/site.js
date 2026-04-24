// Global UI interactions
(() => {
    document.querySelectorAll('[data-loading-form]').forEach(form => {
        form.addEventListener('submit', () => {
            const button = form.querySelector('[data-loading-button]');
            if (!button) return;

            button.disabled = true;
            button.querySelector('.default-label')?.classList.add('d-none');
            button.querySelector('.loading-label')?.classList.remove('d-none');
        });
    });
})();
