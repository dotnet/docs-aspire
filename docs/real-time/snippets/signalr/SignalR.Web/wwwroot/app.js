window.app = {
    updateScroll: () => {
        const element = document.querySelector('html');
        element.scrollTop = element.scrollHeight;
    }
};

const getThemePreference = () => {
    const query = window.matchMedia('(prefers-color-scheme: dark)');
    query.addEventListener('change', applyTheme);
    applyTheme(query);
};

/**
 * @param {MediaQueryList} query
 * @returns
 */
const applyTheme = query => {
    if (!query) {
        return;
    }

    if (query.matches) {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'light');
    }
};

getThemePreference();
