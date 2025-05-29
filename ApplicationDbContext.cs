using Microsoft.EntityFrameworkCore;
//onde fica as configurações para banco de dados
//CLASSE CONFIGURADORA
public class ApplicationDbContext : DbContext
{
    //faz o mapeamento para fazer a class "Product" virar uma tabela
    // no braco de dados
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    //

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>()
        .Property(p => p.Code).HasMaxLength(500).IsRequired();

        builder.Entity<Product>()
        .Property(p => p.Name).HasMaxLength(500).IsRequired();

        builder.Entity<Product>()
        .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Entity<Category>()
        .ToTable("Categories");
    }



    // conectar no banco

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    // {
    // }
    
}