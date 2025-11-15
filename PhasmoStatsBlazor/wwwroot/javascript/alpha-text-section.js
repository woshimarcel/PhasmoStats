window.updateAlpha = (value) => {
    const number = Number(value);
    if (isNaN(number)) {
        return;
    }

    let alpha = Math.max(0, Math.min(100, number)) / 100;
    document.documentElement.style.setProperty('--text-section-alpha', String(alpha));
};