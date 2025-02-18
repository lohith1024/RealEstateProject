using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Common.Resilience
{
    public abstract class ResilientRepository<TContext> where TContext : DbContext
    {
        protected readonly TContext _context;

        protected ResilientRepository(TContext context)
        {
            _context = context;
        }

        protected async Task ExecuteWithRetryAsync(Func<Task> operation)
        {
            await ResiliencePolicies.GetDatabaseRetryPolicy()
                .ExecuteAsync(async () => await operation());
        }

        protected async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            return await ResiliencePolicies.GetDatabaseRetryPolicy()
                .ExecuteAsync(async () => await operation());
        }

        protected async Task SaveChangesWithRetryAsync()
        {
            await ExecuteWithRetryAsync(async () => await _context.SaveChangesAsync());
        }
    }
}