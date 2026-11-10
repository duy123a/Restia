(() => {
    const TRANSLATIONS_REFRESH_COOLDOWN = 60 * 1000;
    let translationsRefreshPromise = null;
    let lastTranslationsRefresh = 0;

    async function loadTranslations(forceRefresh = false) {
        const configDiv = document.getElementById("layout-config");
        const culture = configDiv?.dataset.culture || "en";

        const cacheKey = `translations_${culture}`;
        const cacheTTL = 60 * 60 * 1000; // 1 hour (ms)

        if (forceRefresh) {
            localStorage.removeItem(cacheKey);
        } else {
            const cached = localStorage.getItem(cacheKey);
            if (cached) {
                try {
                    const parsed = JSON.parse(cached);
                    if (parsed && typeof parsed === "object" && Date.now() < parsed.expiry) {
                        window.translations = parsed.data;
                        return;
                    }
                } catch {
                    // Corrupted cache entry
                }
                localStorage.removeItem(cacheKey);
            }
        }

        const tenant = window.currentTenant || "public";
        const res = await fetch(`/${tenant}/api/notification/translations?culture=${culture}`);
        if (!res.ok) {
            throw new Error(`Translation API failed: ${res.status}`);
        }

        const data = await res.json();
        window.translations = data.translations;

        localStorage.setItem(cacheKey, JSON.stringify({
            data: data.translations,
            expiry: Date.now() + cacheTTL
        }));
    }

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

    window.t = function (key, paramsJson) {
        if (!window.translations) return key;
        const raw = window.translations[key];
        if (raw === undefined) {
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
                        if (typeof value === "string" && value.startsWith("@@") && value.endsWith("@@")) {
                            const innerKey = value.slice(2, -2).trim();
                            value = window.translations[innerKey] || value;
                        }
                        template = template.replaceAll(`{${i}}`, () => String(value ?? ""));
                    });
                }
            } catch {
                // Ignore invalid params JSON
            }
        }

        return template;
    };

    document.addEventListener("DOMContentLoaded", async () => {
        const configDiv = document.getElementById("layout-config");

        window.isAuthenticated =
            configDiv?.dataset.isAuthenticated === "true";

        window.currentCulture =
            configDiv?.dataset.culture || "en";

        window.currentTenant =
            configDiv?.dataset.tenant || "public";

        window.appTimeZone =
            configDiv?.dataset.timezone || "Asia/Ho_Chi_Minh";

        if (!window.isAuthenticated) {
            return;
        }

        try {
            await loadTranslations();
        } catch (e) {
            console.error("Failed to load translations:", e);
        }

        document.dispatchEvent(new Event("noticeReady"));
    });
})();
