using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<PointHistory> PointHistories { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<BorrowRecord> BorrowRecords { get; set; } = null!;
    public DbSet<Bookmark> Bookmarks { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<OrderLine> OrderLines { get; set; } = null!;
    public DbSet<OptionItem> OptionItems { get; set; } = null!;
    public DbSet<OrderLineOption> OrderLineOptions { get; set; } = null!;
    public DbSet<Stock> Stocks { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Students
        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(s => s.StudentCode);
            e.Property(s => s.StudentCode).HasMaxLength(16).IsRequired();
            e.Property(s => s.Name).HasMaxLength(10).IsRequired();
            e.HasIndex(s => s.StudentCode).IsUnique();
        });

        // Users
        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(x => x.Id);
            u.HasOne(x => x.Student)
                .WithOne(s => s.User)
                .HasForeignKey<User>(x => x.StudentCode)
                .OnDelete(DeleteBehavior.Restrict);
            u.HasIndex(x => x.StudentCode).IsUnique();
        });

        // Books
        modelBuilder.Entity<Book>(b =>
        {
            b.HasKey(x => x.BookCode);
            b.HasIndex(x => x.Isbn).IsUnique(false);
        });

        // BorrowRecords
        modelBuilder.Entity<BorrowRecord>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.User)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Book)
                .WithMany(bk => bk.BorrowRecords)
                .HasForeignKey(x => x.BookCode)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PointHistory
        modelBuilder.Entity<PointHistory>(p =>
        {
            p.HasKey(x => x.Id);
            p.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentCode)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Bookmark
        modelBuilder.Entity<Bookmark>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.BookCode }).IsUnique();
            b.HasOne(x => x.User)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Book)
                .WithMany(bk => bk.Bookmarks)
                .HasForeignKey(x => x.BookCode)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Orders
        modelBuilder.Entity<Order>(o =>
        {
            o.HasKey(x => x.OrderId);
            o.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentCode)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MenuItems
        modelBuilder.Entity<MenuItem>(m =>
        {
            m.HasKey(x => x.MenuId);
            m.HasIndex(x => x.Name).IsUnique();
        });

        // OrderLines
        modelBuilder.Entity<OrderLine>(l =>
        {
            l.HasKey(x => new { x.OrderId, x.LineNo });
            l.HasOne(x => x.Order)
                .WithMany(o => o.Lines)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            l.HasOne(x => x.MenuItem)
                .WithMany(m => m.Lines)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OptionItems
        modelBuilder.Entity<OptionItem>(o =>
        {
            o.HasKey(x => x.OptionId);
        });

        // OrderLineOptions
        modelBuilder.Entity<OrderLineOption>(o =>
        {
            o.HasKey(x => new { x.OrderId, x.LineNo, x.OptionId });
            o.HasOne(x => x.OrderLine)
                .WithMany(l => l.Options)
                .HasForeignKey(x => new { x.OrderId, x.LineNo })
                .OnDelete(DeleteBehavior.Cascade);
            o.HasOne(x => x.OptionItem)
                .WithMany(oi => oi.OrderLineOptions)
                .HasForeignKey(x => x.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Stock
        modelBuilder.Entity<Stock>(s =>
        {
            s.HasKey(x => new { x.MenuId, x.Date });
            s.HasOne(x => x.MenuItem)
                .WithMany(m => m.Stocks)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}