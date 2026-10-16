using System.Globalization;

namespace UEHStudent.Api.Services;

public static class DateTimeHelper
{
    public static (DateOnly WeekStart, DateOnly WeekEnd) GetWeekRange(DateOnly date)
    {
        // DayOfWeek: Sunday = 0, Monday = 1, ..., Saturday = 6
        int diff = date.DayOfWeek switch
        {
            DayOfWeek.Sunday => 6,
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            _ => 0
        };

        var monday = date.AddDays(-diff);
        var sunday = monday.AddDays(6);
        return (monday, sunday);
    }

    public static string GetVietnameseDayOfWeekName(byte dayOfWeek)
    {
        return dayOfWeek switch
        {
            1 => "Thứ Hai",
            2 => "Thứ Ba",
            3 => "Thứ Tư",
            4 => "Thứ Năm",
            5 => "Thứ Sáu",
            6 => "Thứ Bảy",
            7 => "Chủ Nhật",
            _ => "N/A"
        };
    }

    public static string GetVietnameseDayOfWeekName(DayOfWeek dow)
    {
        return dow switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba",
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ Nhật",
            _ => "N/A"
        };
    }

    public static byte DayOfWeekToByte(DayOfWeek dow)
    {
        return dow switch
        {
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 3,
            DayOfWeek.Thursday => 4,
            DayOfWeek.Friday => 5,
            DayOfWeek.Saturday => 6,
            DayOfWeek.Sunday => 7,
            _ => 1
        };
    }

    public static string FormatDateVietnamese(DateOnly date)
    {
        return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    public static string FormatDateVietnamese(DateTime dt)
    {
        return dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }
}
