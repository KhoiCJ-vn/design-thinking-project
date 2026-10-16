/* ========================================================
   UEH Student & My Week — Main Application Router & UI Shell
   ======================================================== */

const App = {
    currentTab: 'my-week',
    currentDate: '2026-10-16', // Default demo anchor date

    init() {
        if (!ApiClient.isAuthenticated()) {
            window.location.href = 'login.html';
            return;
        }

        this.renderUserInfo();
        this.setupNavigation();
        this.setupDateSelectors();

        // Load initial tab
        this.switchTab('my-week');
    },

    renderUserInfo() {
        const user = ApiClient.getUser();
        const userBadge = document.getElementById('user-badge');
        const adminLink = document.getElementById('admin-link');

        if (user && userBadge) {
            const roleLabel = user.role === 'Admin' ? ' [Quản trị]' : '';
            userBadge.textContent = `${user.studentCode ? user.studentCode + ' | ' : ''}${user.fullName || user.username}${roleLabel}`;
        }

        if (adminLink) {
            adminLink.style.display = (user && user.role === 'Admin') ? 'inline-block' : 'none';
        }
    },

    setupNavigation() {
        const navLinks = document.querySelectorAll('[data-tab]');
        navLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const tab = link.getAttribute('data-tab');
                this.switchTab(tab);
            });
        });

        const logoutBtn = document.getElementById('btn-logout');
        if (logoutBtn) {
            logoutBtn.addEventListener('click', (e) => {
                e.preventDefault();
                if (confirm('Bạn có chắc chắn muốn đăng xuất không?')) {
                    ApiClient.logout();
                }
            });
        }
    },

    switchTab(tabName) {
        this.currentTab = tabName;

        // Update active class in top navigation & sidebar
        document.querySelectorAll('[data-tab]').forEach(el => {
            if (el.getAttribute('data-tab') === tabName) {
                el.classList.add('active');
                if (el.parentElement.classList.contains('nav-item')) {
                    el.parentElement.classList.add('active');
                }
                if (el.parentElement.classList.contains('sidebar-menu-item')) {
                    el.parentElement.classList.add('active');
                }
            } else {
                el.classList.remove('active');
                if (el.parentElement.classList.contains('nav-item')) {
                    el.parentElement.classList.remove('active');
                }
                if (el.parentElement.classList.contains('sidebar-menu-item')) {
                    el.parentElement.classList.remove('active');
                }
            }
        });

        // Hide all tab sections, show active tab section
        document.querySelectorAll('.tab-section').forEach(section => {
            section.style.display = 'none';
        });

        const activeSection = document.getElementById(`tab-${tabName}`);
        if (activeSection) {
            activeSection.style.display = 'block';
        }

        // Trigger module load
        if (tabName === 'my-week' && window.MyWeekModule) {
            window.MyWeekModule.load(this.currentDate);
        } else if (tabName === 'schedule' && window.ScheduleModule) {
            window.ScheduleModule.load(this.currentDate);
        } else if (tabName === 'exams' && window.ExamsModule) {
            window.ExamsModule.load();
        } else if (tabName === 'announcements' && window.AnnouncementsModule) {
            window.AnnouncementsModule.load();
        }
    },

    setupDateSelectors() {
        // Global date change handlers
        const datePickers = document.querySelectorAll('.date-picker-input');
        datePickers.forEach(input => {
            input.value = this.currentDate;
            input.addEventListener('change', (e) => {
                this.currentDate = e.target.value;
                this.onDateChanged(this.currentDate);
            });
        });
    },

    onDateChanged(newDate) {
        this.currentDate = newDate;
        // Synchronize all date pickers
        document.querySelectorAll('.date-picker-input').forEach(input => {
            input.value = newDate;
        });

        if (this.currentTab === 'my-week' && window.MyWeekModule) {
            window.MyWeekModule.load(newDate);
        } else if (this.currentTab === 'schedule' && window.ScheduleModule) {
            window.ScheduleModule.load(newDate);
        }
    },

    // UI Helpers: Toast
    showToast(message, type = 'info') {
        const container = document.getElementById('toast-container');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.innerHTML = `<span>${message}</span>`;
        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            setTimeout(() => toast.remove(), 250);
        }, 3500);
    },

    // UI Helpers: Modal
    openModal(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) modal.classList.add('active');
    },

    closeModal(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) modal.classList.remove('active');
    }
};

window.App = App;

document.addEventListener('DOMContentLoaded', () => {
    App.init();
});
