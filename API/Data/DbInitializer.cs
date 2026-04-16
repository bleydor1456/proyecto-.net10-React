using CrudNet10.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrudNet10.Data;

public static class DbInitializer
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        var adminEmail = "admin@correo.com";

        var adminExiste = await context.Usuarios
            .AnyAsync(u => u.Email == adminEmail);

        if (adminExiste)
            return;

        var admin = new Usuario
        {
            Nombre = "Administrador",
            Email = adminEmail,
            Role = "Admin"
        };

        var hasher = new PasswordHasher<Usuario>();
        admin.PasswordHash = hasher.HashPassword(admin, "149522");

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}