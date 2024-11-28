using System.Threading;
using System.Threading.Tasks;
using EQS.KMS.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace EQS.KMS.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<CryptoSession> CryptoSessions { get; }
        DbSet<KeySet> KeySets { get; }
        DbSet<User> Users { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}