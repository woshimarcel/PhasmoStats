window.updateGradient = (value) => {
    console.log("updateGradient:", value);
    const el = document.querySelector(".page");
    if (!el) {
        console.warn("updateGradient: .page element not found");
        return;
    }

    if (Number(value) === 101) {
        ensureFlashOverlay();
        showFlashOverlay(3000);
        return;
    }

    const v = Math.max(0, Math.min(100, Number(value)));

    if (v >= 100) {
        el.style.setProperty('background', '#ffffff', 'important');
        el.style.setProperty('background-color', '#ffffff', 'important');
        return;
    }

    const alpha = v / 100;
    const coreSize = Math.max(2, v);
    const fadeEnd = Math.min(100, coreSize + 50);

    const coreColor = `rgba(255,255,255,${alpha})`;
    const coreFade = `rgba(255,255,255,0)`;

    const gradient = 
        `radial-gradient(circle at 40% 25%, ${coreColor} 0%, ${coreColor} ${coreSize}%, ${coreFade} ${fadeEnd}%), ` +
        `radial-gradient(circle at 80% bottom, #9e00ff 0%, transparent 22%)`;

    el.style.setProperty('background', gradient, 'important');
    el.style.setProperty('background-color', '#0F0F0F', 'important');
};

function ensureFlashOverlay() {
    if (document.getElementById('flash-overlay')) return;

    const overlay = document.createElement('div');
    overlay.id = 'flash-overlay';
    overlay.style.position = 'fixed';
    overlay.style.inset = '0';
    overlay.style.background = '#ffffff';
    overlay.style.zIndex = '1000';
    overlay.style.pointerEvents = 'none';
    overlay.style.opacity = '0';
    overlay.style.transition = 'opacity 150ms linear';
    document.body.appendChild(overlay);
}

let _flashTimeout = null;
function showFlashOverlay(durationMs = 600) {
    const overlay = document.getElementById('flash-overlay');
    if (!overlay) return;

    const protectedEls = document.querySelectorAll('.flash-protected');
    protectedEls.forEach(e => {
        if (getComputedStyle(e).position === 'static') e.style.position = 'relative';
        e.style.zIndex = '1001';
    });

    overlay.style.opacity = '1';

    if (_flashTimeout) {
        clearTimeout(_flashTimeout);
        _flashTimeout = null;
    }

    _flashTimeout = setTimeout(() => {
        overlay.style.opacity = '0';
        _flashTimeout = null;
    }, durationMs);
}
