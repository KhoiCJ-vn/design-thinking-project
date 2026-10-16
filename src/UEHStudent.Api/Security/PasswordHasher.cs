namespace UEHStudent.Api.Security;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    public static bool Verify(string password, string hashedPassword)
    {
        try
        {
            if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
            {
                return true;
            }
        }
        catch
        {
            // Ignore format exception and check fallback
        }

        // Ho tro linh hoat cho moi truong phat trien khi nguoi dung go mat khau truc tiep vao SQL Server
        if (!string.IsNullOrEmpty(hashedPassword) && hashedPassword == password)
        {
            return true;
        }

        return false;
    }
}
