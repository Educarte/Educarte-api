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
    public DbSet<ContractedHour> ContractedHours { get; set; }
    public DbSet<Diary> Diaries { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    public DbSet<AccessControl> AccessControls { get; set; }

    public ApiDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Address>(d =>
        {
            d.HasKey(d => d.Id);
        });

        mb.Entity<Menu>(d =>
        {
            d.HasKey(d => d.Id);
            d.Property(d => d.Status).HasDefaultValue(Status.Active);
        });

        mb.Entity<AccessControl>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.Student).WithMany(d => d.AccessControls).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<Diary>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasMany(d => d.Students).WithMany(d => d.Diaries);
            d.HasMany(d => d.Classrooms).WithMany(d => d.Diaries);
        });

        mb.Entity<Classroom>(d =>
        {
            d.HasKey(d => d.Id);
            d.Property(d => d.Status).HasDefaultValue(Status.Active);
            d.HasMany(d => d.Teachers).WithMany(d => d.Classrooms);
            d.HasMany(d => d.Diaries).WithMany(d => d.Classrooms);
            d.HasMany(d => d.Students).WithOne(d => d.Classroom).HasForeignKey(d => d.ClassroomId);
        });

        mb.Entity<EmergencyContact>(d =>
        {
            d.HasKey(d => d.Id);
            d.HasOne(d => d.Student).WithMany(d => d.EmergencyContacts).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<ContractedHour>(d =>
        {
            d.HasKey(d => d.Id);
            d.Property(d => d.Status).HasDefaultValue(Status.Active);
            d.HasOne(d => d.Student).WithMany(d => d.ContractedHours).HasForeignKey(d => d.StudentId);
        });

        mb.Entity<Student>(d =>
        {
            d.HasKey(d => d.Id);
            d.Property(d => d.Status).HasDefaultValue(Status.Active);
            d.HasMany(d => d.LegalGuardians).WithMany(d => d.Childs);
            d.HasOne(d => d.Classroom).WithMany(d => d.Students).HasForeignKey(d => d.ClassroomId);
            d.HasMany(d => d.AccessControls).WithOne(d => d.Student).HasForeignKey(d => d.StudentId);
            d.HasMany(d => d.ContractedHours).WithOne(d => d.Student).HasForeignKey(d => d.StudentId);
            d.HasMany(d => d.Diaries).WithMany(d => d.Students);
            d.HasMany(d => d.EmergencyContacts).WithOne(d => d.Student).HasForeignKey(d => d.StudentId);
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
            d.HasOne(d => d.Address).WithOne().HasForeignKey<Address>(d => d.Id);
            d.HasMany(d => d.ResetPasswordCodes).WithOne(d => d.User).HasForeignKey(d => d.UserId);
            d.HasMany(d => d.Childs).WithMany(d => d.LegalGuardians);
            d.HasMany(d => d.Classrooms).WithMany(d => d.Teachers);
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
