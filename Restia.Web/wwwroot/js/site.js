(() => {
    // Sidebar and top nav bar
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarClose = document.getElementById('sidebarClose');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const topNavbar = document.querySelector('.top-navbar');

    function closeSidebar() {
        if (sidebar) {
            sidebar.classList.remove('show');
        }
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            if (sidebar) {
                if (window.innerWidth >= 992) {
                    sidebar.classList.toggle('collapsed');
                    if (topNavbar) {
                        topNavbar.classList.toggle('sidebar-collapsed');
                    }
                } else {
                    sidebar.classList.toggle('show');
                }
            }
        });
    }

    if (sidebarClose) {
        sidebarClose.addEventListener('click', closeSidebar);
    }

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', closeSidebar);
    }

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            closeSidebar();
        }
    });

    // User dropdown button
    const userDropdown = document.getElementById('userDropdown');
    if (userDropdown) {
        userDropdown.addEventListener('show.bs.dropdown', function () {
            this.classList.add('arrow-up');
        });
        userDropdown.addEventListener('hidden.bs.dropdown', function () {
            this.classList.remove('arrow-up');
        });
    }

    // Logout modal
    const logoutOpenBtn = document.getElementById('logoutOpenBtn');
    const logoutConfirmBtn = document.getElementById('logoutConfirmBtn');
    const logoutForm = document.getElementById('logoutForm');
    const logoutConfirmModalEl = document.getElementById('logoutConfirmModal');

    if (logoutOpenBtn && logoutConfirmModalEl && window.bootstrap?.Modal) {
        const logoutModal = window.bootstrap.Modal.getOrCreateInstance(logoutConfirmModalEl);
        logoutOpenBtn.addEventListener('click', () => logoutModal.show());
    }

    if (logoutConfirmBtn && logoutForm) {
        logoutConfirmBtn.addEventListener('click', () => logoutForm.submit());
    }

    // Bulk column selector for permission matrix
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-select-column]');
        if (!btn) return;
        const col = btn.getAttribute('data-select-column');
        if (!col) return;
        const radios = document.querySelectorAll(`input[type="radio"][data-column="${col}"]:not(:disabled)`);
        radios.forEach(r => { r.checked = true; });
    });

    // Table delete confirmation modal
    const deleteConfirmModalEl = document.getElementById('deleteConfirmModal');
    const deleteConfirmBtn = document.getElementById('deleteConfirmBtn');
    let pendingDeleteForm = null;

    document.addEventListener('click', function (e) {
        const trigger = e.target.closest('.table-delete-trigger');
        if (!trigger) return;
        const form = trigger.closest('form');
        if (!form) return;

        if (deleteConfirmModalEl && window.bootstrap?.Modal) {
            pendingDeleteForm = form;
            window.bootstrap.Modal.getOrCreateInstance(deleteConfirmModalEl).show();
        }
    });

    if (deleteConfirmBtn) {
        deleteConfirmBtn.addEventListener('click', function () {
            if (pendingDeleteForm) {
                pendingDeleteForm.submit();
            }
        });
    }
})();
