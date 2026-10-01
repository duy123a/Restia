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

// Logout button

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
