(function () {
    const timerEl = document.getElementById('lockoutTimer');
    const remainingEl = document.getElementById('lockoutRemainingText');
    if (!timerEl || !remainingEl) return;

    const initialSeconds = parseInt(timerEl.dataset.remainingSeconds, 10);
    if (isNaN(initialSeconds) || initialSeconds <= 0) return;

    const targetTime = Date.now() + initialSeconds * 1000;
    let intervalId = null;

    function updateRemaining() {
        const diffSec = Math.max(0, Math.ceil((targetTime - Date.now()) / 1000));
        if (diffSec <= 0) {
            if (intervalId !== null) {
                clearInterval(intervalId);
            }
            window.location.reload();
            return;
        }
        const d = Math.floor(diffSec / 86400);
        const h = Math.floor((diffSec % 86400) / 3600);
        const m = Math.floor((diffSec % 3600) / 60);
        const s = diffSec % 60;
        const parts = [];
        if (d > 0) parts.push(`${d}d`);
        if (h > 0 || d > 0) parts.push(`${h}h`);
        if (m > 0 || h > 0 || d > 0) parts.push(`${m}m`);
        parts.push(`${s}s`);
        remainingEl.textContent = parts.join(' ');
    }

    updateRemaining();
    intervalId = setInterval(updateRemaining, 1000);
})();
