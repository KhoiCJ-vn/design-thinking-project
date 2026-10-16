/* ========================================================
   UEH Admin Module — Giao diện quản trị hệ thống
   Role: Admin only
   ======================================================== */

const AdminModule = {
    currentTab: 'schedules',

    init() {
        const user = ApiClient.getUser();
        if (!user || user.role !== 'Admin') {
            alert('Bạn không có quyền truy cập trang quản trị này. Vui lòng đăng nhập bằng tài khoản Quản trị viên (admin).');
            window.location.href = 'index.html';
            return;
        }

        const badge = document.getElementById('admin-user-badge');
        if (badge) badge.textContent = `${user.username} [Quản trị viên]`;

        this.switchAdminTab('schedules');
    },

    switchAdminTab(tabName) {
        this.currentTab = tabName;

        document.querySelectorAll('.admin-nav-item').forEach(el => {
            if (el.getAttribute('data-admin-tab') === tabName) {
                el.classList.add('active');
            } else {
                el.classList.remove('active');
            }
        });

        document.querySelectorAll('.admin-tab-pane').forEach(pane => {
            pane.style.display = 'none';
        });

        const activePane = document.getElementById(`admin-tab-${tabName}`);
        if (activePane) activePane.style.display = 'block';

        if (tabName === 'schedules') this.loadSchedules();
        else if (tabName === 'exams') this.loadExams();
        else if (tabName === 'announcements') this.loadAnnouncements();
        else if (tabName === 'events') this.loadEvents();
    },

    // 1. Schedules Management
    async loadSchedules() {
        const tbody = document.getElementById('admin-schedules-tbody');
        if (!tbody) return;
        tbody.innerHTML = `<tr><td colspan="7" style="text-align: center; padding: 20px;"><div class="spinner"></div></td></tr>`;

        try {
            const rules = await ApiClient.adminGetSchedules();
            if (rules.length === 0) {
                tbody.innerHTML = `<tr><td colspan="7" style="text-align: center; color: var(--ueh-text-muted); padding: 16px;">Chưa có quy tắc lịch học nào.</td></tr>`;
                return;
            }

            tbody.innerHTML = rules.map(r => `
                <tr>
                    <td style="font-weight: 700; color: var(--ueh-teal);">${r.classSectionCode}</td>
                    <td>${r.courseName}</td>
                    <td><span class="badge-tag blue">${r.dayOfWeekName}</span></td>
                    <td>${r.startTime} - ${r.endTime}</td>
                    <td style="font-weight: 600; color: var(--ueh-orange);">${r.room || 'Chưa xếp'}</td>
                    <td>${r.instructor || 'Chưa phân công'}</td>
                    <td style="text-align: center;">
                        <button class="btn btn-outline-danger btn-sm" onclick="AdminModule.deleteSchedule(${r.scheduleRuleId})">Xóa</button>
                    </td>
                </tr>
            `).join('');
        } catch (error) {
            tbody.innerHTML = `<tr><td colspan="7" style="text-align: center; color: var(--ueh-red);">${error.message}</td></tr>`;
        }
    },

    async createSchedule(event) {
        event.preventDefault();
        const rule = {
            classSectionId: parseInt(document.getElementById('sched-class-id').value),
            dayOfWeek: parseInt(document.getElementById('sched-day').value),
            startTime: document.getElementById('sched-start-time').value,
            endTime: document.getElementById('sched-end-time').value,
            room: document.getElementById('sched-room').value.trim() || null,
            instructor: document.getElementById('sched-instructor').value.trim() || null,
            effectiveStartDate: document.getElementById('sched-start-date').value,
            effectiveEndDate: document.getElementById('sched-end-date').value
        };

        try {
            await ApiClient.adminCreateSchedule(rule);
            alert('Thêm quy tắc lịch học thành công! Sinh viên thuộc lớp này sẽ thấy cập nhật ngay trong My Week.');
            document.getElementById('form-create-sched').reset();
            this.loadSchedules();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    async deleteSchedule(id) {
        if (!confirm('Bạn có chắc chắn muốn xóa quy tắc lịch học này không?')) return;
        try {
            await ApiClient.adminDeleteSchedule(id);
            alert('Đã xóa quy tắc lịch học.');
            this.loadSchedules();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    // 2. Exams Management
    async loadExams() {
        const tbody = document.getElementById('admin-exams-tbody');
        if (!tbody) return;
        tbody.innerHTML = `<tr><td colspan="8" style="text-align: center; padding: 20px;"><div class="spinner"></div></td></tr>`;

        try {
            const exams = await ApiClient.adminGetExams();
            tbody.innerHTML = exams.map(e => `
                <tr>
                    <td style="font-weight: 700; color: var(--ueh-teal);">${e.classSectionCode}</td>
                    <td>${e.courseName}</td>
                    <td style="font-weight: 700; color: var(--ueh-red);">${e.examDate}</td>
                    <td>${e.startTime}${e.endTime ? ' - ' + e.endTime : ''}</td>
                    <td style="font-weight: 600;">${e.room || 'Chưa xếp'}</td>
                    <td>${e.location || 'UEH'}</td>
                    <td><span class="badge-tag blue">${e.examForm || 'Tự luận'}</span></td>
                    <td style="text-align: center;">
                        <button class="btn btn-outline-danger btn-sm" onclick="AdminModule.deleteExam(${e.examScheduleId})">Xóa</button>
                    </td>
                </tr>
            `).join('');
        } catch (error) {
            tbody.innerHTML = `<tr><td colspan="8" style="text-align: center; color: var(--ueh-red);">${error.message}</td></tr>`;
        }
    },

    async createExam(event) {
        event.preventDefault();
        const exam = {
            classSectionId: parseInt(document.getElementById('exam-class-id').value),
            examDate: document.getElementById('exam-date').value,
            startTime: document.getElementById('exam-start-time').value,
            endTime: document.getElementById('exam-end-time').value || null,
            room: document.getElementById('exam-room').value.trim() || null,
            location: document.getElementById('exam-location').value.trim() || 'UEH Nguyễn Văn Linh',
            examType: 'Thi cuối học phần',
            examForm: document.getElementById('exam-form').value.trim() || 'Tự luận',
            notes: document.getElementById('exam-notes').value.trim() || null
        };

        try {
            await ApiClient.adminCreateExam(exam);
            alert('Thêm lịch thi thành công! Sinh viên thuộc lớp sẽ thấy ngay trên trang Lịch thi và My Week.');
            document.getElementById('form-create-exam').reset();
            this.loadExams();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    async deleteExam(id) {
        if (!confirm('Bạn có chắc chắn muốn xóa lịch thi này không?')) return;
        try {
            await ApiClient.adminDeleteExam(id);
            alert('Đã xóa lịch thi.');
            this.loadExams();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    // 3. Announcements Management
    async loadAnnouncements() {
        const tbody = document.getElementById('admin-ann-tbody');
        if (!tbody) return;
        tbody.innerHTML = `<tr><td colspan="6" style="text-align: center; padding: 20px;"><div class="spinner"></div></td></tr>`;

        try {
            const list = await ApiClient.adminGetAnnouncements();
            tbody.innerHTML = list.map(a => `
                <tr>
                    <td style="font-weight: 600;">${a.title}</td>
                    <td><span class="badge-tag teal">${a.category}</span></td>
                    <td>${a.issuer}</td>
                    <td><span class="badge-tag ${a.priority === 'Urgent' ? 'red' : 'blue'}">${a.priority}</span></td>
                    <td>${a.deadlineAt ? new Date(a.deadlineAt).toLocaleDateString('vi-VN') : '—'}</td>
                    <td style="text-align: center;">
                        <button class="btn btn-outline-danger btn-sm" onclick="AdminModule.deleteAnnouncement(${a.announcementId})">Xóa</button>
                    </td>
                </tr>
            `).join('');
        } catch (error) {
            tbody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: var(--ueh-red);">${error.message}</td></tr>`;
        }
    },

    async createAnnouncement(event) {
        event.preventDefault();
        const ann = {
            title: document.getElementById('ann-title').value.trim(),
            summary: document.getElementById('ann-summary').value.trim() || null,
            content: document.getElementById('ann-content').value.trim() || null,
            issuer: document.getElementById('ann-issuer').value.trim(),
            category: document.getElementById('ann-category').value,
            priority: document.getElementById('ann-priority').value,
            deadlineAt: document.getElementById('ann-deadline').value ? new Date(document.getElementById('ann-deadline').value).toISOString() : null,
            targetType: 'All'
        };

        try {
            await ApiClient.adminCreateAnnouncement(ann);
            alert('Đăng thông báo thành công!');
            document.getElementById('form-create-ann').reset();
            this.loadAnnouncements();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    async deleteAnnouncement(id) {
        if (!confirm('Bạn có chắc chắn muốn xóa thông báo này không?')) return;
        try {
            const res = await ApiClient.adminDeleteAnnouncement(id);
            alert(res.message || 'Xóa thông báo thành công.');
            this.loadAnnouncements();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    // 4. Events Management
    async loadEvents() {
        const tbody = document.getElementById('admin-events-tbody');
        if (!tbody) return;
        tbody.innerHTML = `<tr><td colspan="7" style="text-align: center; padding: 20px;"><div class="spinner"></div></td></tr>`;

        try {
            const events = await ApiClient.adminGetEvents();
            tbody.innerHTML = events.map(e => `
                <tr>
                    <td style="font-weight: 600;">${e.title}</td>
                    <td><span class="badge-tag purple">${e.category}</span></td>
                    <td>${e.startDateTime}</td>
                    <td>${e.location || 'UEH'}</td>
                    <td style="text-align: center; font-weight: 700; color: var(--ueh-teal);">${e.registeredCount} / ${e.maxParticipants || '∞'}</td>
                    <td>${e.registrationDeadline || '—'}</td>
                    <td style="text-align: center;">
                        <button class="btn btn-outline-danger btn-sm" onclick="AdminModule.deleteEvent(${e.eventId})">Xóa</button>
                    </td>
                </tr>
            `).join('');
        } catch (error) {
            tbody.innerHTML = `<tr><td colspan="7" style="text-align: center; color: var(--ueh-red);">${error.message}</td></tr>`;
        }
    },

    async createEvent(event) {
        event.preventDefault();
        const start = document.getElementById('evt-start').value;
        const end = document.getElementById('evt-end').value;
        const deadline = document.getElementById('evt-deadline').value;

        const evt = {
            title: document.getElementById('evt-title').value.trim(),
            description: document.getElementById('evt-desc').value.trim() || null,
            category: document.getElementById('evt-category').value,
            startDateTime: new Date(start).toISOString(),
            endDateTime: new Date(end).toISOString(),
            location: document.getElementById('evt-location').value.trim() || 'Cơ sở Nguyễn Văn Linh',
            maxParticipants: document.getElementById('evt-max').value ? parseInt(document.getElementById('evt-max').value) : null,
            registrationDeadline: deadline ? new Date(deadline).toISOString() : null,
            targetAudience: 'Toàn trường'
        };

        try {
            await ApiClient.adminCreateEvent(evt);
            alert('Tạo sự kiện thành công! Sinh viên sẽ thấy sự kiện trên UEH Events.');
            document.getElementById('form-create-event').reset();
            this.loadEvents();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    },

    async deleteEvent(id) {
        if (!confirm('Bạn có chắc chắn muốn xóa sự kiện này không?')) return;
        try {
            const res = await ApiClient.adminDeleteEvent(id);
            alert(res.message || 'Xóa sự kiện thành công.');
            this.loadEvents();
        } catch (error) {
            alert('Lỗi: ' + error.message);
        }
    }
};

window.AdminModule = AdminModule;
