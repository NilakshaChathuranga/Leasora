using Microsoft.EntityFrameworkCore;

namespace Leasora.Api.Data;

public class LeasoraDbContext : DbContext
{
    public LeasoraDbContext(
        DbContextOptions<LeasoraDbContext> options)
        : base(options)
    {
    }
}