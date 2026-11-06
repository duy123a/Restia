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

    async function loadUnreadCount() {
        try {
            const res = await fetch("/api/notification/unread-count");
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

    async function loadNotifications(skip = 0, take = 10, replace = false) {
        try {
            const res = await fetch(`/api/notification/list?skip=${skip}&take=${take}`);
            if (!res.ok) return;

            const data = await res.json();

            if (replace) {
                const noNotif = list.querySelector("#noNotificationLi");
                list.innerHTML = '';
                if (noNotif) list.appendChild(noNotif);
                notificationSkip = 0; // reset skip
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

            // append notifications
            const fragment = document.createDocumentFragment();
            data.forEach(n => {
                const a = document.createElement("a");
                const readClass = n.isRead ? "notification-read" : "notification-unread";
                const created = n.createdAt ? new Date(n.createdAt).toLocaleString(window.currentCulture) : "";

                a.href = n.url || '#';
                a.className = `dropdown-item text-wrap notification-item position-relative ${readClass}`;
                a.dataset.id = n.id;
                a.dataset.url = n.url || '';

                const unreadDot = !n.isRead ? `<span class="unread-dot"></span>` : '';

                a.innerHTML = `
                    ${unreadDot}
                    <div class="notif-text-container">
                        <div class="notif-title">${t(n.titleKey, n.titleParams)}</div>
                        <div class="text-muted notif-message">${t(n.messageKey, n.messageParams)}</div>
                        <div class="text-muted notif-time">${created}</div>
                    </div>
                `;
                fragment.appendChild(a);
            });

            // append notifications to list
            list.appendChild(fragment);

            notificationSkip += data.length;

            // Show/hide show more
            if (data.length < take) {
                showMoreLi.classList.add("d-none");
            } else {
                showMoreLi.classList.remove("d-none");
            }

            // keep dropdown open
            if (!replace) {
                const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
                dropdown.show();
            }

        } catch (e) {
            console.error("Failed to load notifications:", e);
        }
    }

    // Register click handler only once using event delegation
    if (list.dataset.clickHandlerAttached !== "true") {

        list.addEventListener("click", async e => {
            const target = e.target;

            // Ensure the click target is an HTML element
            if (!(target instanceof Element)) return;

            // Find the notification link containing the clicked element
            const a = target.closest("a[data-id]");

            // Ignore clicks outside notification links
            if (!a || !list.contains(a)) return;

            e.preventDefault();

            // Prevent multiple clicks while processing
            if (a.dataset.processing === "true") return;

            a.dataset.processing = "true";

            const id = a.dataset.id;
            const url = a.dataset.url;

            try {
                const res = await fetch(
                    `/api/notification/mark-as-read/${id}`,
                    { method: "POST" }
                );

                if (!res.ok) {
                    throw new Error(`HTTP error: ${res.status}`);
                }

                // Update notification UI
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

            // Handle notification without a URL
            if (!url) {
                const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
                dropdown.show();
                return;
            }

            // Handle notification with a URL
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

        list.dataset.clickHandlerAttached = "true";
    }

    // Mark all as read
    readAllBtn?.addEventListener("click", async (e) => {
        e.stopPropagation();
        if (readAllBtn.disabled) return;
        readAllBtn.disabled = true;

        try {
            const res = await fetch("/api/notification/mark-all-as-read", { method: "POST" });
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

    // SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    // refresh UI when getting new notification
    connection.on("NewNotification", function (notificationDto) {
        if (window.isAuthenticated) {
            loadUnreadCount();
            loadNotifications(0, notificationTake, true);
        }
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR error", err));

    // click Show More
    showMoreBtn.addEventListener("click", (e) => {
        e.stopPropagation(); // prevent dropdown auto-close
        loadNotifications(notificationSkip, notificationTake, false);
    });

    if (window.isAuthenticated) {
        loadUnreadCount();
        loadNotifications(0, notificationTake, true);
    }

    // Re-render notifications when translations have just been refreshed
    // (the t() helper triggers this when it encounters a key missing from the cache).
    document.addEventListener("translationsRefreshed", () => {
        if (window.isAuthenticated) {
            loadNotifications(0, notificationTake, true);
        }
    });
});
