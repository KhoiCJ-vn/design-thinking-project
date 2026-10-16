/* ========================================================
   UEH Exams Module — Tra cứu lịch thi sinh viên
   Reproduces the table layout from reference image 2
   ======================================================== */

const ExamsModule = {
    data: null,

    async load() {
        const container = document.getElementById('exams-content');
        if (!container) return;

        container.innerHTML = `
            <div style="text-align: center; padding: 40px;">
                <div class="spinner"></div>
                <p style="margin-top: 8px; color: var(--ueh-text-muted);">Đang tải lịch thi...</p>
            </div>
        `;

        try {
            this.data = await ApiClient.getExams();
            this.render();
        } catch (error) {
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">⚠️</div>
                    <h3>Không thể tải lịch thi</h3>
                    <p>${error.message || 'Lỗi kết nối máy chủ.'}</p>
                </div>
            `;
        }
    },

    render() {
        const container = document.getElementById('exams-content');
        if (!container || !this.data) return;

        const d = this.data;

        container.innerHTML = `
            <div class="section-title-bar">
                <h2>
                    <span style="color: var(--ueh-teal);">📝</span> Lịch thi sinh viên
                </h2>
                <span style="font-size: 13px; color: var(--ueh-text-muted);">
                    Năm học: ${d.academicYear} | Học kỳ: ${d.semester}
                </span>
            </div>

            <!-- Notes Banner matching reference image 2 -->
            <div style="background: #fffdf5; border: 1px solid #fef08a; padding: 12px 16px; border-radius: var(--radius-sm); margin-bottom: 16px; font-size: 12.5px; line-height: 1.6;">
                <div style="font-weight: 700; color: #854d0e; margin-bottom: 4px;">Ghi chú quy chế phòng thi:</div>
                <ul style="padding-left: 20px; color: #713f12;">
                    <li><strong>Thi tập trung tại phòng thi:</strong> Người học có mặt tại phòng thi trước giờ thi 15 phút, xuất trình thẻ sinh viên hoặc CCCD.</li>
                    <li><strong>Thi trực tuyến (online):</strong> Đăng nhập hệ thống MS Teams trước 15 phút, chuẩn bị thiết bị có camera và micro.</li>
                </ul>
            </div>

            <!-- Filters -->
            <div class="week-selector-bar">
                <div style="display: flex; align-items: center; gap: 12px;">
                    <div style="display: flex; align-items: center; gap: 6px;">
                        <label style="font-weight: 600; font-size: 12px;">Năm học:</label>
                        <select class="form-control" style="width: 120px; font-size: 12px; padding: 4px 8px;">
                            <option>${d.academicYear}</option>
                        </select>
                    </div>

                    <div style="display: flex; align-items: center; gap: 6px;">
                        <label style="font-weight: 600; font-size: 12px;">Học kỳ:</label>
                        <select class="form-control" style="width: 130px; font-size: 12px; padding: 4px 8px;">
                            <option>${d.semester}</option>
                        </select>
                    </div>
                </div>

                <div>
                    <span style="font-weight: 700; color: var(--ueh-teal);">Tổng số môn thi: ${d.items.length}</span>
                </div>
            </div>

            <!-- Exam Table matching reference 2 -->
            <div class="table-responsive">
                <table class="ueh-table">
                    <thead>
                        <tr>
                            <th>Mã LHP<br/>Tên HP</th>
                            <th style="text-align: center;">Lần thi</th>
                            <th>Ngày thi</th>
                            <th>Giờ thi</th>
                            <th>Phòng thi</th>
                            <th>Địa điểm</th>
                            <th>Hình thức thi</th>
                            <th>Loại thi</th>
                            <th>Ghi chú</th>
                            <th style="text-align: center;">Tình trạng</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${d.items.length === 0 ? `
                            <tr>
                                <td colspan="10" style="text-align: center; color: var(--ueh-text-muted); padding: 24px;">
                                    Hiện chưa có lịch thi trong học kỳ này.
                                </td>
                            </tr>
                        ` : d.items.map(ex => {
                            const [y, m, day] = ex.examDate.split('-');
                            const formattedDate = `${day}/${m}/${y}`;
                            return `
                                <tr>
                                    <td>
                                        <div style="font-weight: 700; color: var(--ueh-teal); font-size: 12px;">${ex.classSectionCode}</div>
                                        <div style="font-weight: 600; font-size: 13px; color: var(--ueh-text-dark);">${ex.courseName}</div>
                                    </td>
                                    <td style="text-align: center; font-weight: 600;">1</td>
                                    <td style="font-weight: 700; color: var(--ueh-red);">${formattedDate}</td>
                                    <td style="font-weight: 600;">${ex.startTime}${ex.endTime ? ' - ' + ex.endTime : ''}</td>
                                    <td style="font-weight: 700; color: var(--ueh-teal);">${ex.room || 'Chưa xếp'}</td>
                                    <td>${ex.location || 'UEH Nguyễn Văn Linh'}</td>
                                    <td><span class="badge-tag blue">${ex.examForm || 'Tự luận'}</span></td>
                                    <td>${ex.examType}</td>
                                    <td style="font-size: 12px; color: var(--ueh-text-muted);">${ex.notes || '—'}</td>
                                    <td style="text-align: center;">
                                        <span class="badge-tag green">Đủ điều kiện</span>
                                    </td>
                                </tr>
                            `;
                        }).join('')}
                    </tbody>
                </table>
            </div>
        `;
    }
};

window.ExamsModule = ExamsModule;
