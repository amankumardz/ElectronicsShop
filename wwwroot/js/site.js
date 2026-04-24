document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-loading-form]').forEach((form) => {
        form.addEventListener('submit', () => {
            const button = form.querySelector('[data-loading-btn]');
            if (!button) return;

            const defaultText = button.querySelector('.default-text');
            const loadingText = button.querySelector('.loading-text');

            button.disabled = true;
            defaultText?.classList.add('d-none');
            loadingText?.classList.remove('d-none');
        });
    });
});
