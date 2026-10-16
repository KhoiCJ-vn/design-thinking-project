/* ========================================================
   UEH Announcements Module — Thông báo chung
   Reproduces the table listing from reference image 3
   ======================================================== */

const AnnouncementsModule = {
    items: [],
    selectedCategory: 'Tất cả',
    searchKeyword: '',

    async load() {
        const container = document.getElementById('announcements-content');
        if (!container) return;

        container.innerHTML = `
            <div style="text-align: center; padding: 40px;">
                <div class="spinner"></div>
                <p style="margin-top: 8px; color: var(--ueh-text-muted);">Đang tải danh sách thông báo...</p>
            </div>
        `;

        try {
            this.items = await ApiClient.getAnnouncements(this.selectedCategory, this.searchKeyword);
            this.render();
        } catch (error) {
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">⚠️</div>
                    <h3>Không thể tải thông báo</h3>
                    <p>${error.message || 'Lỗi kết nối máy chủ.'}</p>
                </div>
            `;
        }
    },

    filterCategory(category) {
        this.selectedCategory = category;
        this.load();
    },

    search(keyword) {
        this.searchKeyword = keyword;
        this.load();
    },

    render() {
        const container = document.getElementById('announcements-content');
        if (!container) return;

        const categories = [
            'Tất cả',
            'Đăng ký học phần',
            'Lịch học',
            'Lịch thi',
            'Nộp chứng chỉ',
            'Học vụ và thủ tục',
            'Hoạt động sinh viên',
            'Thông báo chung'
        ];

        container.innerHTML = `
            <div class="section-title-bar">
                <h2>
                    <span style="color: var(--ueh-teal);">📢</span> Thông báo chung toàn trường
                </h2>
                <span style="font-size: 13px; color: var(--ueh-text-muted);">
                    Tìm thấy ${this.items.length} thông báo
                </span>
            </div>

            <!-- Filter Categories & Search Bar -->
            <div style="display: flex; flex-direction: column; gap: 12px; margin-bottom: 16px;">
                <div style="display: flex; gap: 8px; flex-wrap: wrap;">
                    ${categories.map(c => `
                        <button class="btn btn-sm ${this.selectedCategory === c ? 'btn-primary' : 'btn-outline'}" 
                                onclick="AnnouncementsModule.filterCategory('${c}')">
                            ${c}
                        </button>
                    `).join('')}
                </div>

                <div style="display: flex; gap: 8px;">
                    <input type="text" class="form-control" placeholder="Tìm kiếm theo tiêu đề, người gửi, nội dung..." 
                           value="${this.searchKeyword}" 
                           id="ann-search-input"
                           onkeyup="if(event.key==='Enter') AnnouncementsModule.search(this.value)">
                    <button class="btn btn-primary" onclick="AnnouncementsModule.search(document.getElementById('ann-search-input').value)">
                        🔍 Tìm
                    </button>
                    ${this.searchKeyword ? `
                        <button class="btn btn-outline" onclick="AnnouncementsModule.search('')">Đặt lại</button>
                    ` : ''}
                </div>
            </div>

            <!-- Announcements Table matching reference image 3 -->
            <div class="table-responsive">
                <table class="ueh-table">
                    <thead>
                        <tr>
                            <th style="width: 60%;">Tiêu đề</th>
                            <th style="width: 20%;">Người gửi</th>
                            <th style="width: 20%; text-align: center;">Thời gian gửi</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${this.items.length === 0 ? `
                            <tr>
                                <td colspan="3" style="text-align: center; color: var(--ueh-text-muted); padding: 24px;">
                                    Không tìm thấy thông báo nào phù hợp với bộ lọc.
                                </td>
                            </tr>
                        ` : this.items.map(a => {
                            const pubDate = new Date(a.publishedAt);
                            const formattedDate = `${pubDate.getDate().toString().padStart(2, '0')}/${(pubDate.getMonth() + 1).toString().padStart(2, '0')}/${pubDate.getFullYear()}`;
                            const isUrgent = a.priority === 'Urgent';

                            return `
                                <tr style="cursor: pointer;" onclick="AnnouncementsModule.viewDetail(${a.announcementId})">
                                    <td>
                                        <div style="font-weight: 600; color: ${isUrgent ? 'var(--ueh-red)' : 'var(--ueh-text-dark)'};">
                                            ${isUrgent ? '🔴 ' : ''}${a.title}
                                        </div>
                                        ${a.summary ? `
                                            <div style="font-size: 11.5px; color: var(--ueh-text-muted); margin-top: 2px;">
                                                ${a.summary}
                                            </div>
                                        ` : ''}
                                        <div style="margin-top: 4px; display: flex; gap: 6px; align-items: center;">
                                            <span class="badge-tag ${isUrgent ? 'red' : 'teal'}">${a.category}</span>
                                            ${a.deadlineAt ? `<span class="badge-tag red">Hạn: ${new Date(a.deadlineAt).toLocaleDateString('vi-VN')}</span>` : ''}
                                            ${a.isSaved ? `<span class="badge-tag green">✓ Đã trong lịch cá nhân</span>` : ''}
                                        </div>
                                    </td>
                                    <td style="color: var(--ueh-teal); font-weight: 500;">${a.issuer}</td>
                                    <td style="text-align: center; color: var(--ueh-text-muted);">${formattedDate}</td>
                                </tr>
                            `;
                        }).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    async viewDetail(id) {
        try {
            const ann = await ApiClient.getAnnouncement(id);
            const pubDate = new Date(ann.publishedAt).toLocaleDateString('vi-VN');
            const deadline = ann.deadlineAt ? new Date(ann.deadlineAt).toLocaleString('vi-VN') : 'Không có';

            document.getElementById('modal-ann-title').textContent = ann.title;
            document.getElementById('modal-ann-body').innerHTML = `
                <div style="display: flex; gap: 8px; margin-bottom: 12px; flex-wrap: wrap;">
                    <span class="badge-tag teal">${ann.category}</span>
                    <span class="badge-tag ${ann.priority === 'Urgent' ? 'red' : 'blue'}">Mức độ: ${ann.priority}</span>
                    ${ann.deadlineAt ? `<span class="badge-tag red">⏰ Hạn chót: ${deadline}</span>` : ''}
                </div>

                <div style="font-size: 12px; color: var(--ueh-text-muted); margin-bottom: 14px; border-bottom: 1px solid var(--ueh-border-light); padding-bottom: 8px;">
                    <div><strong>Người gửi:</strong> ${ann.issuer}</div>
                    <div><strong>Ngày đăng:</strong> ${pubDate}</div>
                </div>

                <div style="font-size: 14px; line-height: 1.6; color: var(--ueh-text-dark); margin-bottom: 18px;">
                    ${ann.content || ann.summary || 'Không có nội dung chi tiết.'}
                </div>

                <div style="background: #f8fafc; border: 1px solid var(--ueh-border); padding: 12px; border-radius: var(--radius-sm);">
                    <div style="font-weight: 700; color: var(--ueh-teal); margin-bottom: 4px;">📅 Thêm vào lịch cá nhân (My Week):</div>
                    <p style="font-size: 12px; color: var(--ueh-text-muted); margin-bottom: 10px;">
                        Đặt ngày nhắc nhở để thông báo này xuất hiện trong tuần tương ứng trên My Week của bạn.
                    </p>
                    ${ann.isSaved ? `
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <span class="badge-tag green">✓ Đang theo dõi vào ngày: ${ann.plannedAt}</span>
                            <button class="btn btn-outline-danger btn-sm" onclick="AnnouncementsModule.removeSaved(${ann.announcementId})">Bỏ theo dõi</button>
                        </div>
                    ` : `
                        <div style="display: flex; gap: 8px; align-items: center;">
                            <input type="date" id="ann-modal-plan-date" class="form-control" style="width: 150px;" value="${App.currentDate}">
                            <input type="text" id="ann-modal-plan-note" class="form-control" placeholder="Ghi chú cá nhân (tùy chọn)...">
                            <button class="btn btn-orange btn-sm" onclick="AnnouncementsModule.saveToCalendar(${ann.announcementId})">Thêm vào lịch</button>
                        </div>
                    `}
                </div>
            `;

            App.openModal('modal-ann-detail');
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    async saveToCalendar(announcementId) {
        const plannedAt = document.getElementById('ann-modal-plan-date').value;
        const personalNote = document.getElementById('ann-modal-plan-note').value.trim();

        if (!plannedAt) {
            App.showToast('Vui lòng chọn ngày nhắc nhở.', 'error');
            return;
        }

        try {
            await ApiClient.saveAnnouncement(announcementId, plannedAt, personalNote || null);
            App.closeModal('modal-ann-detail');
            App.showToast('Đã thêm thông báo vào lịch cá nhân My Week.', 'success');
            await this.load();
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    },

    async removeSaved(announcementId) {
        if (!confirm('Bạn có chắc muốn bỏ theo dõi thông báo này trong lịch cá nhân không?')) return;
        try {
            await ApiClient.removeSavedAnnouncement(announcementId);
            App.closeModal('modal-ann-detail');
            App.showToast('Đã gỡ thông báo khỏi lịch cá nhân.', 'info');
            await this.load();
        } catch (error) {
            App.showToast(error.message, 'error');
        }
    }
};

window.AnnouncementsModule = AnnouncementsModule;
