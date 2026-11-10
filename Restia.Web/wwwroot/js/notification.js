(() => {
    document.addEventListener("noticeReady", function () {
        const badge = document.getElementById("notificationBadge");
        const list = document.getElementById("notificationList");
        const showMoreBtn = document.getElementById("notificationShowMoreBtn");
        const showMoreLi = document.getElementById("notificationShowMoreLi");
        const readAllBtn = document.getElementById("notificationReadAllBtn");
        const dropdownToggle = document.getElementById("notificationToggle");

        if (!badge || !list || !showMoreBtn || !showMoreLi || !dropdownToggle) return;

        let notificationSkip = 0;
        const notificationTake = 10;

        const apiBase = `/${window.currentTenant || "public"}/api/notification`;

        async function loadUnreadCount() {
            try {
                const res = await fetch(`${apiBase}/unread-count`);
                if (!res.ok) return;

                const count = await res.json();
                if (count > 0) {
                    badge.classList.remove("d-none");
                    readAllBtn?.classList.remove("d-none");
                } else {
                    badge.classList.add("d-none");
                    readAllBtn?.classList.add("d-none");
                }
            } catch (e) {
                console.error("Failed to load unread count:", e);
            }
        }

        function createNotificationElement(n) {
            const a = document.createElement("a");
            const readClass = n.isRead ? "notification-read" : "notification-unread";
            const created = n.createdAt ? new Date(n.createdAt).toLocaleString(window.currentCulture, { timeZone: window.appTimeZone }) : "";

            a.href = n.url || "#";
            a.className = `dropdown-item text-wrap notification-item position-relative ${readClass}`;
            a.dataset.id = n.id;
            a.dataset.url = n.url || "";

            if (!n.isRead) {
                const dot = document.createElement("span");
                dot.className = "unread-dot";
                a.appendChild(dot);
            }

            const textContainer = document.createElement("div");
            textContainer.className = "notif-text-container";

            const titleEl = document.createElement("div");
            titleEl.className = "notif-title";
            titleEl.textContent = window.t(n.titleKey, n.titleParams);

            const messageEl = document.createElement("div");
            messageEl.className = "text-muted notif-message";
            messageEl.textContent = window.t(n.messageKey, n.messageParams);

            const timeEl = document.createElement("div");
            timeEl.className = "text-muted notif-time";
            timeEl.textContent = created;

            textContainer.append(titleEl, messageEl, timeEl);
            a.appendChild(textContainer);

            return a;
        }

        async function loadNotifications(skip = 0, take = 10, replace = false) {
            try {
                const res = await fetch(`${apiBase}/list?skip=${skip}&take=${take}`);
                if (!res.ok) return;

                const data = await res.json();

                if (replace) {
                    const noNotif = list.querySelector("#noNotificationLi");
                    list.replaceChildren();
                    if (noNotif) list.appendChild(noNotif);
                    notificationSkip = 0;
                }

                if (data.length === 0 && replace) {
                    const noNotif = list.querySelector("#noNotificationLi");
                    if (noNotif) noNotif.classList.remove("d-none");
                    showMoreLi.classList.add("d-none");
                    return;
                } else {
                    const noNotif = list.querySelector("#noNotificationLi");
                    if (noNotif) noNotif.classList.add("d-none");
                }

                const fragment = document.createDocumentFragment();
                data.forEach(n => {
                    fragment.appendChild(createNotificationElement(n));
                });

                list.appendChild(fragment);
                notificationSkip += data.length;

                if (data.length < take) {
                    showMoreLi.classList.add("d-none");
                } else {
                    showMoreLi.classList.remove("d-none");
                }

                if (!replace) {
                    const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
                    dropdown.show();
                }
            } catch (e) {
                console.error("Failed to load notifications:", e);
            }
        }

        list.addEventListener("click", async e => {
            const target = e.target;
            if (!(target instanceof Element)) return;

            const a = target.closest("a[data-id]");
            if (!a || !list.contains(a)) return;

            e.preventDefault();

            if (a.dataset.processing === "true") return;
            a.dataset.processing = "true";

            const id = a.dataset.id;
            const url = a.dataset.url;

            try {
                const res = await fetch(
                    `${apiBase}/mark-as-read/${id}`,
                    { method: "POST" }
                );

                if (!res.ok) {
                    throw new Error(`HTTP error: ${res.status}`);
                }

                a.classList.remove("notification-unread");
                a.classList.add("notification-read");

                const dot = a.querySelector(".unread-dot");
                if (dot) dot.remove();

                await loadUnreadCount();
            } catch (err) {
                console.error("Failed to mark read:", err);
            } finally {
                delete a.dataset.processing;
            }

            if (!url) {
                const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
                dropdown.show();
                return;
            }

            try {
                const urlObj = new URL(url, window.location.origin);
                const isExternal = urlObj.origin !== window.location.origin;

                if (isExternal) {
                    window.open(urlObj.href, "_blank", "noopener,noreferrer");
                } else {
                    window.location.href = urlObj.href;
                }
            } catch (err) {
                console.error("Invalid notification URL:", err);
            }
        });

        readAllBtn?.addEventListener("click", async (e) => {
            e.stopPropagation();
            if (readAllBtn.disabled) return;
            readAllBtn.disabled = true;

            try {
                const res = await fetch(`${apiBase}/mark-all-as-read`, { method: "POST" });
                if (!res.ok) throw new Error(`HTTP error: ${res.status}`);

                list.querySelectorAll(".notification-unread").forEach(el => {
                    el.classList.remove("notification-unread");
                    el.classList.add("notification-read");
                    el.querySelector(".unread-dot")?.remove();
                });

                badge.classList.add("d-none");
                readAllBtn.classList.add("d-none");
            } catch (err) {
                console.error("Failed to mark all as read:", err);
            } finally {
                readAllBtn.disabled = false;
            }
        });

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        // refresh UI when getting new notification
        connection.on("NewNotification", function () {
            if (window.isAuthenticated) {
                loadUnreadCount();
                loadNotifications(0, notificationTake, true);
            }
        });

        connection.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => console.error("SignalR error", err));

        showMoreBtn.addEventListener("click", (e) => {
            e.stopPropagation(); // prevent dropdown auto-close
            loadNotifications(notificationSkip, notificationTake, false);
        });

        if (window.isAuthenticated) {
            loadUnreadCount();
            loadNotifications(0, notificationTake, true);
        }

        document.addEventListener("translationsRefreshed", () => {
            if (window.isAuthenticated) {
                loadNotifications(0, notificationTake, true);
            }
        });
    });
})();
