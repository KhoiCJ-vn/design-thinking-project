/* ========================================================
   UEH Schedule Module — Tra cứu thời khóa biểu theo tuần
   Reproduces the authentic grid layout from reference image 1
   ======================================================== */

const ScheduleModule = {
    currentDate: null,
    data: null,

    async load(date) {
        this.currentDate = date || App.currentDate;
        const container = document.getElementById('schedule-content');
        if (!container) return;

        container.innerHTML = `
            <div style="text-align: center; padding: 40px;">
                <div class="spinner"></div>
                <p style="margin-top: 8px; color: var(--ueh-text-muted);">Đang tải thời khóa biểu...</p>
            </div>
        `;

        try {
            this.data = await ApiClient.getSchedule(this.currentDate);
            this.render();
        } catch (error) {
            container.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">⚠️</div>
                    <h3>Không thể tải thời khóa biểu</h3>
                    <p>${error.message || 'Lỗi kết nối máy chủ.'}</p>
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

    render() {
        const container = document.getElementById('schedule-content');
        if (!container || !this.data) return;

        const d = this.data;
        const [sy, sm, sd] = d.weekStart.split('-');
        const [ey, em, ed] = d.weekEnd.split('-');
        const formattedRange = `${sd}/${sm}/${sy} đến ngày ${ed}/${em}/${ey}`;

        container.innerHTML = `
            <div class="section-title-bar">
                <h2>
                    <span style="color: var(--ueh-teal);">🗓️</span> Thời khóa biểu sinh viên
                </h2>
                <span style="font-size: 13px; color: var(--ueh-text-muted);">
                    Năm học: 2026-2027 | Học kỳ: Học kỳ cuối
                </span>
            </div>

            <!-- Filter Controls matching reference layout -->
            <div class="week-selector-bar">
                <div style="display: flex; align-items: center; gap: 12px; flex-wrap: wrap;">
                    <div style="display: flex; align-items: center; gap: 6px;">
                        <label style="font-weight: 600; font-size: 12px;">Năm học:</label>
                        <select class="form-control" style="width: 120px; font-size: 12px; padding: 4px 8px;">
                            <option>2026-2027</option>
                        </select>
                    </div>

                    <div style="display: flex; align-items: center; gap: 6px;">
                        <label style="font-weight: 600; font-size: 12px;">Học kỳ:</label>
                        <select class="form-control" style="width: 130px; font-size: 12px; padding: 4px 8px;">
                            <option>Học kỳ cuối</option>
                        </select>
                    </div>

                    <div style="display: flex; align-items: center; gap: 6px;">
                        <label style="font-weight: 600; font-size: 12px;">Chọn ngày bất kỳ:</label>
                        <input type="date" class="form-control" style="width: 140px; font-size: 12px; padding: 4px 8px;"
                               value="${d.selectedDate}"
                               onchange="App.onDateChanged(this.value)">
                    </div>
                </div>

                <div class="week-selector-controls">
                    <button class="btn btn-outline btn-sm" onclick="ScheduleModule.changeWeek(-7)">◀ Tuần trước</button>
                    <button class="btn btn-outline btn-sm" onclick="App.onDateChanged('2026-10-16')">Tuần hiện tại (16/10)</button>
                    <button class="btn btn-outline btn-sm" onclick="ScheduleModule.changeWeek(7)">Tuần sau ▶</button>
                </div>
            </div>

            <!-- Week banner matching reference -->
            <div style="background: #fff8f5; border: 1px solid var(--ueh-orange-border); padding: 8px 14px; border-radius: var(--radius-sm); margin-bottom: 14px; font-weight: 700; color: var(--ueh-orange); display: flex; justify-content: space-between; align-items: center;">
                <span>📅 Thời khóa biểu từ ngày ${formattedRange}</span>
                <span style="font-size: 12px; font-weight: normal; color: var(--ueh-text-muted);">Tổng số: ${d.items.length} buổi học</span>
            </div>

            <!-- Timetable Grid Table (Periods 1 to 13 x Days) -->
            <div class="table-responsive">
                <table class="timetable-grid">
                    <thead>
                        <tr>
                            <th class="period-col">Tiết</th>
                            <th>Thứ hai</th>
                            <th>Thứ ba</th>
                            <th>Thứ tư</th>
                            <th>Thứ năm</th>
                            <th>Thứ sáu</th>
                            <th>Thứ bảy</th>
                            <th>Chủ nhật</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${this.renderTimetableRows(d.items)}
                    </tbody>
                </table>
            </div>

            <!-- List View Alternative -->
            <div style="margin-top: 24px;">
                <h3 style="font-size: 14px; font-weight: 700; color: var(--ueh-teal); margin-bottom: 8px;">
                    Chi tiết các lớp học phần trong tuần:
                </h3>
                <table class="ueh-table">
                    <thead>
                        <tr>
                            <th>Mã LHP</th>
                            <th>Tên học phần</th>
                            <th>Thứ / Ngày</th>
                            <th>Tiết / Giờ học</th>
                            <th>Phòng học</th>
                            <th>Giảng viên</th>
                            <th>Số tín chỉ</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${d.items.length === 0 ? `
                            <tr><td colspan="7" style="text-align: center; color: var(--ueh-text-muted); padding: 16px;">Không có buổi học nào trong tuần này.</td></tr>
                        ` : d.items.map(it => `
                            <tr>
                                <td style="font-weight: 700; color: var(--ueh-teal);">${it.classSectionCode}</td>
                                <td style="font-weight: 600;">${it.courseName}</td>
                                <td><span class="badge-tag blue">${it.dayOfWeekName}</span> (${it.date})</td>
                                <td>${it.periodDescription} (${it.startTime} - ${it.endTime})</td>
                                <td style="font-weight: 700; color: var(--ueh-orange);">${it.room || 'Chưa xếp'}</td>
                                <td>${it.instructor || 'Chưa phân công'}</td>
                                <td style="text-align: center;">${it.credits}</td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderTimetableRows(items) {
        // Map items by dayOfWeek (1=Mon, ..., 7=Sun) and period
        // Periods 1 to 13
        let rowsHtml = '';

        for (let period = 1; period <= 13; period++) {
            rowsHtml += `<tr>`;
            rowsHtml += `<td class="period-label">${period}</td>`;

            for (let day = 1; day <= 7; day++) {
                // Find if an item falls on this day and covers this period
                const matchedItem = items.find(it => {
                    if (it.dayOfWeek !== day) return false;
                    const startH = parseInt(it.startTime.split(':')[0]);
                    // Map hours to periods approximately
                    // Morning: 07:30 is period 2; evening/afternoon: 12:30 is period 8
                    if (startH < 12) {
                        return period === 2; // Render at period 2
                    } else {
                        return period === 8; // Render at period 8
                    }
                });

                if (matchedItem) {
                    rowsHtml += `
                        <td rowspan="4" style="background: #ffffff; padding: 2px;">
                            <div class="class-block" style="border: 1px dashed var(--ueh-orange); background: #fffdf9;">
                                <div class="class-block-code">${matchedItem.room || 'Phòng: Chưa xếp'}</div>
                                <div class="class-block-title" style="color: #b91c1c;">${matchedItem.courseName}</div>
                                <div class="class-block-details">LHP: ${matchedItem.classSectionCode}</div>
                                <div class="class-block-details" style="font-weight: 600;">Giờ học: ${matchedItem.startTime} ➔ ${matchedItem.endTime}</div>
                                <div class="class-block-details" style="color: #0369a1;">GV: ${matchedItem.instructor}</div>
                                <div class="class-block-details" style="color: var(--ueh-teal); font-weight: 700;">Học tại: TẬP TRUNG</div>
                            </div>
                        </td>
                    `;
                } else {
                    // Check if previous period in this cell had a rowspan
                    const prevMorning = items.find(it => it.dayOfWeek === day && parseInt(it.startTime.split(':')[0]) < 12);
                    const prevAfternoon = items.find(it => it.dayOfWeek === day && parseInt(it.startTime.split(':')[0]) >= 12);

                    const coveredByMorning = prevMorning && period >= 3 && period <= 5;
                    const coveredByAfternoon = prevAfternoon && period >= 9 && period <= 12;

                    if (!coveredByMorning && !coveredByAfternoon) {
                        rowsHtml += `<td></td>`;
                    }
                }
            }

            rowsHtml += `</tr>`;
        }

        return rowsHtml;
    }
};

window.ScheduleModule = ScheduleModule;
