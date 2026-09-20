using Leasora.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Leasora.Api.Data;

public class LeasoraDbContext : IdentityDbContext<ApplicationUser>
{
    public LeasoraDbContext(
        DbContextOptions<LeasoraDbContext> options)
        : base(options)
    {
    }

    public DbSet<FinanceCompany> FinanceCompanies
        => Set<FinanceCompany>();

    public DbSet<FinanceOfficerProfile> FinanceOfficerProfiles
        => Set<FinanceOfficerProfile>();
    // Provides access to the provinces stored in the database.
    public DbSet<Province> Provinces => Set<Province>();

    // Provides access to districts, each belonging to a province.
    public DbSet<District> Districts => Set<District>();

    // Provides access to records linking officers to the districts they serve.
    public DbSet<OfficerServiceDistrict> OfficerServiceDistricts
        => Set<OfficerServiceDistrict>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FinanceOfficerProfile>()
            .HasOne(profile => profile.User)
            .WithOne()
            .HasForeignKey<FinanceOfficerProfile>(
                profile => profile.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Add this new reviewer relationship:
        builder.Entity<FinanceOfficerProfile>()
            .HasOne(profile => profile.ReviewedByUser)
            .WithMany()
            .HasForeignKey(profile => profile.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FinanceOfficerProfile>()
            .HasOne(profile => profile.FinanceCompany)
            .WithMany()
            .HasForeignKey(profile => profile.FinanceCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Each district belongs to one province.
        // A province can contain many districts.
        builder.Entity<District>()
            .HasOne(district => district.Province)
            .WithMany()
            .HasForeignKey(district => district.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents assigning the same district to the same officer twice.
        builder.Entity<OfficerServiceDistrict>()
            .HasKey(service => new
            {
                service.OfficerUserId,
                service.DistrictId
            });

        // One officer profile can have many service-district records.
        builder.Entity<OfficerServiceDistrict>()
            .HasOne(service => service.OfficerProfile)
            .WithMany()
            .HasForeignKey(service => service.OfficerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One district can be served by many officers.
        builder.Entity<OfficerServiceDistrict>()
            .HasOne(service => service.District)
            .WithMany()
            .HasForeignKey(service => service.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}