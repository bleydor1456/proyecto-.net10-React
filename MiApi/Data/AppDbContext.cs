//crreacion de base de datos

using Microsoft.EntityFrameworkCore;
using CrudNet10.Models;

namespace CrudNet10.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // tooma como referencia la clase "Productos" para inyectar la base de datos
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
}