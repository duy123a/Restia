document.addEventListener("DOMContentLoaded", async () => {
    const configDiv = document.getElementById("layout-config");

    window.isAuthenticated =
        configDiv?.dataset.isAuthenticated === "true";

    window.currentCulture =
        configDiv?.dataset.culture || "en";

    try {
        await loadTranslations();
    } catch (e) {
        console.error("Failed to load translations:", e);
    }

    document.dispatchEvent(new Event("noticeReady"));
});

async function loadTranslations(forceRefresh = false) {
    const configDiv = document.getElementById("layout-config");
    const culture = configDiv?.dataset.culture || "en";

    const cacheKey = `translations_${culture}`;
    const cacheTTL = 60 * 60 * 1000; // 1 hour (ms)

    if (forceRefresh) {
        localStorage.removeItem(cacheKey);
    } else {
        // try to load from localStorage
        const cached = localStorage.getItem(cacheKey);
        if (cached) {
            const parsed = JSON.parse(cached);
            if (Date.now() < parsed.expiry) {
                // still valid
                window.translations = parsed.data;
                return;
            } else {
                // expired, remove from localStorage
                localStorage.removeItem(cacheKey);
            }
        }
    }

    // fetch from server
    const res = await fetch(`/api/notification/translations?culture=${culture}`);
    if (!res.ok) {
        throw new Error(`Translation API failed: ${res.status}`);
    }

    const data = await res.json();
    window.translations = data.translations;

    // save to localStorage with expiry
    localStorage.setItem(cacheKey, JSON.stringify({
        data: data.translations,
        expiry: Date.now() + cacheTTL
    }));
}

// One-shot refetch when a missing key is hit (e.g. after a deploy added new keys)
let translationsRefreshPromise = null;
let lastTranslationsRefresh = 0;

const TRANSLATIONS_REFRESH_COOLDOWN = 60 * 1000;

function scheduleTranslationsRefresh() {
    const now = Date.now();

    if (translationsRefreshPromise) {
        return translationsRefreshPromise;
    }

    if (now - lastTranslationsRefresh < TRANSLATIONS_REFRESH_COOLDOWN) {
        return Promise.resolve();
    }

    lastTranslationsRefresh = now;

    translationsRefreshPromise = loadTranslations(true)
        .then(() => {
            document.dispatchEvent(new Event("translationsRefreshed"));
        })
        .catch(error => {
            console.error("Failed to refresh translations:", error);
        })
        .finally(() => {
            translationsRefreshPromise = null;
        });

    return translationsRefreshPromise;
}

// Support multiple params via JSON array
window.t = function (key, paramsJson) {
    if (!window.translations) return key;
    const raw = window.translations[key];
    if (raw === undefined) {
        // Stale cache after deploy — refresh in background, return key for now
        scheduleTranslationsRefresh();
        return key;
    }
    let template = raw;

    if (paramsJson) {
        try {
            const params = JSON.parse(paramsJson);
            if (Array.isArray(params)) {
                params.forEach((p, i) => {
                    let value = p;
                    // check if param is in @@ KEY @@ format
                    if (typeof value === "string" && value.startsWith("@@") && value.endsWith("@@")) {
                        const innerKey = value.slice(2, -2).trim();
                        value = window.translations[innerKey] || value;
                    }
                    template = template.replaceAll(`{${i}}`, () => String(value ?? ""));
                });
            }
        } catch (e) {
        }
    }

    return template;
};
