/* ========================================================
   UEH My Week — Central Feature Module
   ======================================================== */

const MyWeekModule = {
    data: null,
    currentDate: null,

    async load(date) {
        this.currentDate = date || App.currentDate;
        const container = document.getElementById('my-week-content');
        if (!container) return;

        container.innerHTML = `
            <div style="text-align: center; padding: 40px;">
                <div class="spinner"></div>
                <p style="margin-top: 8px; color: var(--ueh-text-muted);">Đang tải dữ liệu UEH My Week...</p>
            </div>
        `;

        try {
            this.data = await ApiClient.getMyWeek(this.currentDate);
            this.render();
        } catch (error) {
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">⚠️</div>
                    <h3>Không thể tải UEH My Week</h3>
                    <p>${error.message || 'Lỗi kết nối máy chủ.'}</p>
                    <button class="btn btn-outline" style="margin-top: 12px;" onclick="MyWeekModule.load('${this.currentDate}')">Thử lại</button>
                </div>
            `;
        }
    },

    changeWeek(offsetDays) {
        if (!this.currentDate) return;
        const [y, m, d] = this.currentDate.split('-').map(Number);
        const dt = new Date(y, m - 1, d);
        dt.setDate(dt.getDate() + offsetDays);

        const newDateStr = dt.toISOString().split('T')[0];
        App.onDateChanged(newDateStr);
    },

    jumpToToday() {
        App.onDateChanged('2026-10-16');
    },

    render() {
        const container = document.getElementById('my-week-content');
        if (!container || !this.data) return;

        const d = this.data;

        container.innerHTML = `
            <!-- Header & Week Selector -->
            <div class="section-title-bar">
                <h2>
                    <span style="color: var(--ueh-orange);">📅</span> UEH My Week — Lịch tuần tổng hợp
                </h2>
                <span style="font-size: 13px; color: var(--ueh-text-muted);">
                    Tự động đồng bộ Lịch học, Lịch thi, Sự kiện & Công việc
                </span>
            </div>

            <!-- Week Controls -->
            <div class="week-selector-bar">
                <div class="week-selector-controls">
                    <button class="btn btn-outline btn-sm" onclick="MyWeekModule.changeWeek(-7)">◀ Tuần trước</button>
                    <button class="btn btn-outline btn-sm" onclick="MyWeekModule.jumpToToday()">Tuần hiện tại (16/10)</button>
                    <button class="btn btn-outline btn-sm" onclick="MyWeekModule.changeWeek(7)">Tuần sau ▶</button>
                </div>

                <div class="week-display-badge">
                    🗓️ Tuần: ${d.formattedWeekRange}
                </div>

                <div style="display: flex; align-items: center; gap: 8px;">
                    <label style="font-size: 12px; font-weight: 600; color: var(--ueh-text-muted);">Chọn ngày bất kỳ:</label>
                    <input type="date" class="form-control" style="width: 140px; padding: 4px 8px; font-size: 12px;" 
                           value="${d.selectedDate}" 
                           onchange="App.onDateChanged(this.value)">
                </div>
            </div>

            <!-- Summary Ribbon -->
            <div class="summary-ribbon">
                <div class="summary-card">
                    <div class="summary-icon blue">📚</div>
                    <div class="summary-info">
                        <div class="num">${d.summary.totalClasses}</div>
                        <div class="label">Buổi học trong tuần</div>
                    </div>
                </div>

                <div class="summary-card">
                    <div class="summary-icon red">📝</div>
                    <div class="summary-info">
                        <div class="num">${d.summary.totalExams}</div>
                        <div class="label">Kỳ thi trong tuần</div>
                    </div>
                </div>

                <div class="summary-card">
                    <div class="summary-icon purple">🎯</div>
                    <div class="summary-info">
                        <div class="num">${d.summary.totalRegisteredEvents}</div>
                        <div class="label">Sự kiện đã đăng ký</div>
                    </div>
                </div>

                <div class="summary-card">
                    <div class="summary-icon orange">✅</div>
                    <div class="summary-info">
                        <div class="num">${d.summary.pendingTasksCount}</div>
                        <div class="label">Việc cá nhân cần làm</div>
                    </div>
                </div>

                <div class="summary-card">
                    <div class="summary-icon teal">🔔</div>
                    <div class="summary-info">
                        <div class="num">${d.summary.importantAnnouncementsCount}</div>
                        <div class="label">Thông báo cần chú ý</div>
                    </div>
                </div>
            </div>

            <!-- Main Layout: 2 Columns -->
            <div class="my-week-layout">
                <!-- Left Column: Weekly Timeline -->
                <div class="timeline-column">
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;">
                        <h3 style="font-size: 15px; font-weight: 700; color: var(--ueh-teal);">
                            Lịch trình cá nhân tuần này
                        </h3>
                        <button class="btn btn-orange btn-sm" onclick="MyWeekModule.openNewTaskModal()">
                            + Thêm việc cá nhân
                        </button>
                    </div>

                    <div class="timeline-container">
                        ${this.renderDaysTimeline(d.days)}
                    </div>
                </div>

                <!-- Right Column: Events & Notices & Deadlines -->
                <div class="side-column">
                    <!-- Deadline Radar -->
                    <div class="side-panel">
                        <div class="side-panel-header">
                            <span>⏳ Hạn chót & Deadline tuần này</span>
                            <span class="badge-tag ${d.upcomingDeadlines.length > 0 ? 'red' : 'gray'}">${d.upcomingDeadlines.length}</span>
                        </div>
                        <div class="side-panel-body">
                            ${this.renderDeadlines(d.upcomingDeadlines)}
                        </div>
                    </div>

                    <!-- UEH Events -->
                    <div class="side-panel">
                        <div class="side-panel-header">
                            <span>🎪 Hoạt động UEH trong tuần</span>
                            <span class="badge-tag purple">${d.uehEvents.length}</span>
                        </div>
                        <div class="side-panel-body">
                            ${this.renderEvents(d.uehEvents)}
                        </div>
                    </div>

                    <!-- Important Notices -->
                    <div class="side-panel">
                        <div class="side-panel-header">
                            <span>📢 Thông báo cần chú ý</span>
                            <span class="badge-tag teal">${d.relevantAnnouncements.length}</span>
                        </div>
                        <div class="side-panel-body">
                            ${this.renderNotices(d.relevantAnnouncements)}
                        </div>
                    </div>
                </div>
            </div>
        `;
    },

    renderDaysTimeline(days) {
        if (!days || days.length === 0) {
            return `
                <div class="empty-state">
                    <div class="empty-state-icon">📅</div>
                    <p>Không có lịch trình trong tuần này.</p>
                </div>
            `;
        }

        return days.map(day => {
            const [y, m, d] = day.date.split('-');
            const formattedDate = `${d}/${m}/${y}`;
            const isTodayClass = day.isToday ? 'is-today' : '';
            const todayBadge = day.isToday ? '<span class="badge-tag orange" style="margin-left: 6px;">Hôm nay</span>' : '';

            let itemsHtml = '';
            if (day.items && day.items.length > 0) {
                itemsHtml = day.items.map(item => `
                    <div class="timeline-item tag-${item.tagColor}">
                        <div class="timeline-item-time">${item.time || 'Cả ngày'}</div>
                        <div class="timeline-item-content">
                            <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 8px;">
                                <div class="timeline-item-title">${item.title}</div>
                                <span class="badge-tag ${item.tagColor}">${item.tag}</span>
                            </div>
                            ${item.subtitle ? `<div class="timeline-item-sub">${item.subtitle}</div>` : ''}
                            ${item.location ? `<div class="timeline-item-sub" style="color: var(--ueh-teal); font-weight: 500;">📍 ${item.location}</div>` : ''}
                        </div>
                        ${item.itemType === 'Task' ? `
                            <button class="btn btn-outline btn-sm" style="padding: 2px 6px;" title="Xóa task" onclick="MyWeekModule.deleteTask(${item.rawData.taskId})">✕</button>
                        ` : ''}
                    </div>
                `).join('');
            } else {
                itemsHtml = `<div style="font-size: 12px; color: var(--ueh-text-muted); font-style: italic; padding: 4px 0;">Không có lịch hoặc công việc nào</div>`;
            }

            return `
                <div class="day-card ${isTodayClass}">
                    <div class="day-card-header">
                        <div>
                            <span>${day.dayOfWeekName}</span>, ${formattedDate}
                            ${todayBadge}
                        </div>
                        <span style="font-size: 11px; color: var(--ueh-text-muted); font-weight: normal;">
                            ${day.items ? day.items.length : 0} mục
                        </span>
                    </div>
                    <div class="day-items-list">
                        ${itemsHtml}
                    </div>
                </div>
            `;
        }).join('');
    },

    renderDeadlines(deadlines) {
        if (!deadlines || deadlines.length === 0) {
            const weekStr = this.data && this.data.formattedWeekRange ? `trong tuần ${this.data.formattedWeekRange}` : 'trong tuần này';
            return `<div style="font-size: 12px; color: var(--ueh-text-muted); text-align: center; padding: 16px 8px; font-style: italic;">Không có hạn chót hoặc deadline nào ${weekStr}</div>`;
        }

        return deadlines.map(item => `
            <div style="border-left: 3px solid ${item.isUrgent ? 'var(--ueh-red)' : 'var(--ueh-orange)'}; padding: 8px 10px; background: #fafafa; border-radius: var(--radius-sm); margin-bottom: 8px; transition: background 0.15s ease;">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 6px;">
                    <div style="font-weight: 600; font-size: 12.5px; color: var(--ueh-text-dark); flex: 1;">${item.title}</div>
                    <span class="badge-tag ${item.tagColor || (item.isUrgent ? 'red' : 'orange')}">${item.type}</span>
                </div>
                ${item.subtitle ? `<div style="font-size: 11.5px; color: var(--ueh-text-muted); margin-top: 3px;">${item.subtitle}</div>` : ''}
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 11px; margin-top: 5px;">
                    <span style="font-weight: 700; color: ${item.isUrgent ? 'var(--ueh-red)' : 'var(--ueh-orange)'};">
                        ⏰ ${item.formattedDeadline}
                    </span>
                    ${item.isUrgent ? '<span style="font-size: 10px; color: var(--ueh-red); font-weight: 700;">GẤP</span>' : ''}
                </div>
            </div>
        `).join('');
    },

    renderEvents(events) {
        if (!events || events.length === 0) {
            return `<div style="font-size: 12px; color: var(--ueh-text-muted); text-align: center; padding: 12px 0;">Không có hoạt động nào trong tuần này</div>`;
        }

        return events.map(evt => {
            const startDt = new Date(evt.startDateTime);
            const timeStr = `${startDt.getHours().toString().padStart(2, '0')}:${startDt.getMinutes().toString().padStart(2, '0')} - ${startDt.toLocaleDateString('vi-VN')}`;

            return `
                <div class="event-card">
                    <div class="event-card-title">${evt.title}</div>
                    <div class="event-card-meta">
                        <div>🕒 ${timeStr}</div>
                        <div>📍 ${evt.location || 'UEH'}</div>
                    </div>
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-top: 4px;">
                        <span class="badge-tag purple">${evt.category}</span>
                        ${evt.isRegistered ? `
                            <div style="display: flex; align-items: center; gap: 6px;">
                                <span class="badge-tag green">✓ Đã tham gia</span>
                                <button class="btn btn-outline-danger btn-sm" onclick="MyWeekModule.cancelEvent(${evt.eventId})">Hủy</button>
                            </div>
                        ` : `
                            <button class="btn btn-primary btn-sm" ${!evt.canRegister ? 'disabled' : ''} onclick="MyWeekModule.registerEvent(${evt.eventId})">
                                ${evt.canRegister ? 'Đăng ký' : 'Hết hạn'}
                            </button>
                        `}
                    </div>
                </div>
            `;
        }).join('');
    },

    renderNotices(notices) {
        if (!notices || notices.length === 0) {
            return `<div style="font-size: 12px; color: var(--ueh-text-muted); text-align: center; padding: 12px 0;">Không có thông báo quan trọng</div>`;
        }

        return notices.map(notice => `
            <div class="notice-card ${notice.priority === 'Urgent' ? 'urgent' : ''}">
                <div class="notice-card-title">${notice.title}</div>
                <div style="font-size: 12px; color: var(--ueh-text-muted); margin-bottom: 6px;">
                    ${notice.summary || ''}
                </div>
                <div class="notice-card-date">
                    <span>${notice.issuer}</span>
                    ${notice.deadlineAt ? `<span style="color: var(--ueh-red); font-weight: 600;">Hạn: ${new Date(notice.deadlineAt).toLocaleDateString('vi-VN')}</span>` : ''}
                </div>
                <div style="margin-top: 8px; display: flex; justify-content: flex-end; gap: 6px;">
                    ${notice.isSaved ? `
                        <span class="badge-tag green">✓ Đã đưa vào lịch</span>
                        <button class="btn btn-outline btn-sm" onclick="MyWeekModule.removeSavedNotice(${notice.announcementId})">Bỏ lưu</button>
                    ` : `
                        <button class="btn btn-outline btn-sm" onclick="MyWeekModule.openSaveNoticeModal(${notice.announcementId}, '${notice.title.replace(/'/g, "\\'")}')">
                            + Thêm vào lịch
                        </button>
                    `}
                </div>
            </div>
        `).join('');
    },

    // Interactions
    async registerEvent(eventId) {
        try {
            await ApiClient.registerEvent(eventId);
            App.showToast('Đăng ký sự kiện thành công! Đã tự động cập nhật vào lịch tuần.', 'success');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    async cancelEvent(eventId) {
        if (!confirm('Bạn có chắc chắn muốn hủy đăng ký tham gia sự kiện này không?')) return;
        try {
            await ApiClient.cancelEventRegistration(eventId);
            App.showToast('Đã hủy đăng ký sự kiện.', 'info');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    openNewTaskModal() {
        document.getElementById('task-planned-date').value = this.currentDate;
        document.getElementById('task-title').value = '';
        document.getElementById('task-desc').value = '';
        document.getElementById('task-time').value = '08:00';
        document.getElementById('task-due-date').value = '';
        App.openModal('modal-task');
    },

    async saveNewTask() {
        const title = document.getElementById('task-title').value.trim();
        const plannedDate = document.getElementById('task-planned-date').value;
        const plannedTime = document.getElementById('task-time').value;
        const dueDate = document.getElementById('task-due-date').value;
        const desc = document.getElementById('task-desc').value.trim();

        if (!title) {
            App.showToast('Vui lòng nhập tiêu đề công việc.', 'error');
            return;
        }

        try {
            await ApiClient.createTask({
                title,
                description: desc || null,
                plannedDate,
                plannedTime: plannedTime || null,
                dueDate: dueDate || null
            });
            App.closeModal('modal-task');
            App.showToast('Đã thêm việc cá nhân vào lịch tuần.', 'success');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    async deleteTask(taskId) {
        if (!confirm('Bạn có chắc chắn muốn xóa việc cá nhân này không?')) return;
        try {
            await ApiClient.deleteTask(taskId);
            App.showToast('Đã xóa việc cá nhân.', 'info');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    openSaveNoticeModal(announcementId, title) {
        document.getElementById('save-notice-id').value = announcementId;
        document.getElementById('save-notice-title').textContent = title;
        document.getElementById('save-notice-date').value = this.currentDate;
        document.getElementById('save-notice-note').value = '';
        App.openModal('modal-save-notice');
    },

    async confirmSaveNotice() {
        const announcementId = parseInt(document.getElementById('save-notice-id').value);
        const plannedAt = document.getElementById('save-notice-date').value;
        const personalNote = document.getElementById('save-notice-note').value.trim();

        if (!plannedAt) {
            App.showToast('Vui lòng chọn ngày muốn nhắc nhở trên lịch.', 'error');
            return;
        }

        try {
            await ApiClient.saveAnnouncement(announcementId, plannedAt, personalNote || null);
            App.closeModal('modal-save-notice');
            App.showToast('Đã thêm thông báo vào lịch cá nhân.', 'success');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    async removeSavedNotice(announcementId) {
        if (!confirm('Bạn có chắc muốn bỏ lưu thông báo này khỏi lịch cá nhân không?')) return;
        try {
            await ApiClient.removeSavedAnnouncement(announcementId);
            App.showToast('Đã gỡ thông báo khỏi lịch cá nhân.', 'info');
            await this.load(this.currentDate);
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    }
};

window.MyWeekModule = MyWeekModule;
