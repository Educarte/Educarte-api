using Core;
using Core.Enums;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data;

public class ApiDbContext : DbContext
{
    public DbSet<Address> Adresses { get; set; }
    public DbSet<ResetPasswordCode> ResetPasswordCodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<DiaryClassroom> DiaryClassrooms { get; set; }
    public DbSet<Diary> Diaries { get; set; }
    public DbSet<AccessControl> AccessControls { get; set; }

    public ApiDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        //mb.Entity<Address>(d =>
        //{
        //    d.HasKey(d => d.Id);
        //    d.HasOne(d => d.User).WithMany(d => d.Adresses).HasForeignKey(d => d.UserId);
        //});

        mb.Entity<AccessControl>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.Student).WithMany(d => d.AccessControls).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<Diary>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.Student).WithMany(d => d.Diaries).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<DiaryClassroom>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.Classroom).WithMany(d => d.DiaryClassrooms).HasForeignKey(d => d.ClassroomId);
        });

        mb.Entity<Classroom>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasMany(d => d.Employee).WithMany(d => d.Classrooms);
            d.HasMany(d => d.Students).WithOne(d => d.Classroom).HasForeignKey(d => d.ClassroomId);
        });

        mb.Entity<Menu>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.MenuPack).WithMany(d => d.Menu).HasForeignKey(d => d.MenuPackId);
        });

        mb.Entity<MenuPack>(d =>
        {
            d.HasKey(d => d.Id);
            //TODO: Estudar melhor forma para realizar cardápios restritivos e variações de cardápio (caso houver)
        });

        mb.Entity<Student>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.LegalGuardian).WithMany(d => d.Childs).HasForeignKey(d => d.LegalGuardianId);
            d.HasOne(d => d.Classroom).WithMany(d => d.Students).HasForeignKey(d => d.ClassroomId);
            d.HasMany(d => d.AccessControls).WithOne(d => d.Student).HasForeignKey(d => d.StudentId);
            d.HasOne(d => d.MenuPack).WithMany(d => d.Students).HasForeignKey(d => d.MenuPackId);
            d.HasMany(d => d.Diaries).WithOne(d => d.Student).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<ResetPasswordCode>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.User).WithMany(d => d.ResetPasswordCodes).HasForeignKey(d => d.UserId);
        });

        mb.Entity<User>(d =>
        {
            d.HasKey(d => d.Id);
            d.Property(d => d.Status).HasDefaultValue(Status.Active);
            //d.HasMany(d => d.Adresses).WithOne(d => d.User).HasForeignKey(d => d.UserId);
            d.HasMany(d => d.ResetPasswordCodes).WithOne(d => d.User).HasForeignKey(d => d.UserId);
            d.HasMany(d => d.Childs).WithOne(d => d.LegalGuardian).HasForeignKey(d => d.LegalGuardianId);
            d.HasMany(d => d.Classrooms).WithMany(d => d.Employee);
        });

        SetCreatedAtAndModifiedAtProperty(mb);
        base.OnModelCreating(mb);
    }


    #region Set CreatedAt and ModifiedAt Property

    /// <summary>
    /// Iterate on each IEntity and set the created and modified at behavior
    /// </summary>
    /// <param name="modelBuilder"></param>
    public void SetCreatedAtAndModifiedAtProperty(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;

            if (typeof(IEntity).IsAssignableFrom(entityClrType))
            {
                var method = SetCreatedAtAndModifiedAtPropertyOnAddMethodInfo.MakeGenericMethod(entityClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    /// <summary>
    /// Adds to the entity the behavior of filling date information on create or update
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    public void SetCreatedAtAndModifiedAtPropertyOnAdd<T>(ModelBuilder builder) where T : class, IEntity
    {
        builder.Entity<T>().Property(d => d.CreatedAt).HasDefaultValueSql("NOW(6)").ValueGeneratedOnAdd();
        builder.Entity<T>().Property(d => d.ModifiedAt).HasDefaultValueSql("NOW(6)").ValueGeneratedOnAddOrUpdate();
    }

    private static readonly MethodInfo SetCreatedAtAndModifiedAtPropertyOnAddMethodInfo = typeof(ApiDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(t => t.IsGenericMethod && t.Name == nameof(SetCreatedAtAndModifiedAtPropertyOnAdd));

    #endregion
}
