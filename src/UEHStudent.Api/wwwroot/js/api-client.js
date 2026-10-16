/* ========================================================
   UEH Student & My Week — API Client Module
   Handles authentication state, JWT tokens, and fetch calls
   ======================================================== */

const API_BASE = '/api';

const ApiClient = {
    getToken() {
        return localStorage.getItem('ueh_auth_token');
    },

    setAuth(loginData) {
        localStorage.setItem('ueh_auth_token', loginData.token);
        localStorage.setItem('ueh_user_info', JSON.stringify(loginData));
    },

    getUser() {
        const data = localStorage.getItem('ueh_user_info');
        try {
            return data ? JSON.parse(data) : null;
        } catch {
            return null;
        }
    },

    clearAuth() {
        localStorage.removeItem('ueh_auth_token');
        localStorage.removeItem('ueh_user_info');
    },

    isAuthenticated() {
        return !!this.getToken();
    },

    async request(endpoint, options = {}) {
        const url = endpoint.startsWith('http') ? endpoint : `${API_BASE}${endpoint}`;
        const headers = options.headers || {};

        const token = this.getToken();
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        if (options.body && typeof options.body === 'object' && !(options.body instanceof FormData)) {
            headers['Content-Type'] = 'application/json';
            options.body = JSON.stringify(options.body);
        }

        try {
            const response = await fetch(url, { ...options, headers });

            if (response.status === 401) {
                this.clearAuth();
                if (!window.location.pathname.endsWith('login.html')) {
                    window.location.href = 'login.html';
                }
                throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
            }

            if (response.status === 403) {
                throw new Error('Bạn không có quyền truy cập chức năng này.');
            }

            const contentType = response.headers.get('content-type');
            let data = null;
            if (contentType && contentType.includes('application/json')) {
                data = await response.json();
            } else {
                data = await response.text();
            }

            if (!response.ok) {
                const message = (data && data.message) || response.statusText || 'Lỗi xử lý yêu cầu';
                throw new Error(message);
            }

            return data;
        } catch (error) {
            console.error(`API Error [${endpoint}]:`, error);
            throw error;
        }
    },

    // Auth methods
    async login(username, password) {
        const result = await this.request('/auth/login', {
            method: 'POST',
            body: { username, password }
        });
        this.setAuth(result);
        return result;
    },

    logout() {
        this.clearAuth();
        window.location.href = 'login.html';
    },

    async getProfile() {
        return await this.request('/auth/me');
    },

    // My Week
    async getMyWeek(date) {
        const q = date ? `?date=${encodeURIComponent(date)}` : '';
        return await this.request(`/my-week${q}`);
    },

    // Schedules
    async getSchedule(date) {
        const q = date ? `?date=${encodeURIComponent(date)}` : '';
        return await this.request(`/schedules${q}`);
    },

    // Exams
    async getExams(termId, from, to) {
        const params = new URLSearchParams();
        if (termId) params.append('termId', termId);
        if (from) params.append('from', from);
        if (to) params.append('to', to);
        const q = params.toString() ? `?${params.toString()}` : '';
        return await this.request(`/exams${q}`);
    },

    // Announcements
    async getAnnouncements(category, search, priority) {
        const params = new URLSearchParams();
        if (category && category !== 'Tất cả') params.append('category', category);
        if (search) params.append('search', search);
        if (priority && priority !== 'Tất cả') params.append('priority', priority);
        const q = params.toString() ? `?${params.toString()}` : '';
        return await this.request(`/announcements${q}`);
    },

    async getAnnouncement(id) {
        return await this.request(`/announcements/${id}`);
    },

    async saveAnnouncement(announcementId, plannedAt, personalNote) {
        return await this.request('/saved-announcements', {
            method: 'POST',
            body: { announcementId, plannedAt, personalNote }
        });
    },

    async removeSavedAnnouncement(announcementId) {
        return await this.request(`/saved-announcements/${announcementId}`, {
            method: 'DELETE'
        });
    },

    // Events
    async getEvents(from, to) {
        const params = new URLSearchParams();
        if (from) params.append('from', from);
        if (to) params.append('to', to);
        const q = params.toString() ? `?${params.toString()}` : '';
        return await this.request(`/events${q}`);
    },

    async registerEvent(eventId) {
        return await this.request(`/events/${eventId}/register`, {
            method: 'POST'
        });
    },

    async cancelEventRegistration(eventId) {
        return await this.request(`/events/${eventId}/registration`, {
            method: 'DELETE'
        });
    },

    // Personal Tasks
    async getTasks(from, to) {
        const params = new URLSearchParams();
        if (from) params.append('from', from);
        if (to) params.append('to', to);
        const q = params.toString() ? `?${params.toString()}` : '';
        return await this.request(`/personal-tasks${q}`);
    },

    async createTask(task) {
        return await this.request('/personal-tasks', {
            method: 'POST',
            body: task
        });
    },

    async updateTask(id, task) {
        return await this.request(`/personal-tasks/${id}`, {
            method: 'PUT',
            body: task
        });
    },

    async toggleTask(id) {
        return await this.request(`/personal-tasks/${id}/toggle`, {
            method: 'PATCH'
        });
    },

    async deleteTask(id) {
        return await this.request(`/personal-tasks/${id}`, {
            method: 'DELETE'
        });
    },

    // Admin endpoints
    async adminGetSchedules() {
        return await this.request('/admin/schedules');
    },

    async adminCreateSchedule(rule) {
        return await this.request('/admin/schedules', { method: 'POST', body: rule });
    },

    async adminDeleteSchedule(id) {
        return await this.request(`/admin/schedules/${id}`, { method: 'DELETE' });
    },

    async adminGetExams() {
        return await this.request('/admin/exams');
    },

    async adminCreateExam(exam) {
        return await this.request('/admin/exams', { method: 'POST', body: exam });
    },

    async adminDeleteExam(id) {
        return await this.request(`/admin/exams/${id}`, { method: 'DELETE' });
    },

    async adminGetAnnouncements() {
        return await this.request('/admin/announcements');
    },

    async adminCreateAnnouncement(ann) {
        return await this.request('/admin/announcements', { method: 'POST', body: ann });
    },

    async adminDeleteAnnouncement(id) {
        return await this.request(`/admin/announcements/${id}`, { method: 'DELETE' });
    },

    async adminGetEvents() {
        return await this.request('/admin/events');
    },

    async adminCreateEvent(evt) {
        return await this.request('/admin/events', { method: 'POST', body: evt });
    },

    async adminDeleteEvent(id) {
        return await this.request(`/admin/events/${id}`, { method: 'DELETE' });
    }
};

window.ApiClient = ApiClient;
