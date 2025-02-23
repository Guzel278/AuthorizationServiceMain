using AuthorizationService.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthorizationService.Application.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<TokenCleanupService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var inactiveTokens = await dbContext.Tokens
                    .Where(t => !t.IsRevoked && t.Expiry <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                if (inactiveTokens.Any())
                {
                    foreach (var token in inactiveTokens)
                    {
                        token.IsRevoked = true;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Revoked {Count} inactive tokens.", inactiveTokens.Count);
                }
            }
        }
    }
}

