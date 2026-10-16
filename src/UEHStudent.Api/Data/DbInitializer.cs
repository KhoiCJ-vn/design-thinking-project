using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.Security;

namespace UEHStudent.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UEHStudentDbContext>();

        try
        {
            var placeholderUsers = await context.AppUsers
                .Where(u => u.PasswordHash.StartsWith("PLACEHOLDER_"))
                .ToListAsync();

            if (placeholderUsers.Any())
            {
                logger.LogInformation("Found {Count} users with placeholder password hashes. Updating to secure BCrypt hashes...", placeholderUsers.Count);
                foreach (var user in placeholderUsers)
                {
                    if (user.RoleName == "Admin")
                    {
                        user.PasswordHash = PasswordHasher.Hash("Admin@123");
                    }
                    else
                    {
                        user.PasswordHash = PasswordHasher.Hash("Student@123");
                    }
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Demo user password hashes initialized successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while initializing demo password hashes.");
        }
    }
}
