using dotatryhard.Interfaces;
using dotatryhard.Data;
using dotatryhard.Models;

namespace dotatryhard.Services
{
    public class DataPopulationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataPopulationService> _logger;
        private Task? _backgroundTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public DataPopulationService(
            IServiceProvider serviceProvider,
            ILogger<DataPopulationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataPopulationService is starting...");

            // Start the background task
            _backgroundTask = Task.Run(() => RunBackgroundTask(_cancellationTokenSource.Token), cancellationToken);

            return Task.CompletedTask; // Don't block the application startup
        }

        private async Task RunBackgroundTask(CancellationToken cancellationToken)
        {
            var processedAccounts = new HashSet<long>();
            var accountQueue = new Queue<long>();
            var delay = 1;

            // Seed with an initial account ID
            accountQueue.Enqueue(87683422);

            while (accountQueue.Any() && !cancellationToken.IsCancellationRequested)
            {
                var accountId = accountQueue.Dequeue();

                if (processedAccounts.Contains(accountId))
                    continue;

                _logger.LogInformation($"Processing account ID: {accountId}");

                // Mark the account as processed
                processedAccounts.Add(accountId);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var matchHistoryService = scope.ServiceProvider.GetRequiredService<IMatchHistoryService>();
                    var playerProfileService = scope.ServiceProvider.GetRequiredService<ISteamUserService>();
                    var matchDetailService = scope.ServiceProvider.GetRequiredService<PlayersMatchesService>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    try
                    {
                        _logger.LogInformation($"Processing account ID: {accountId}");

                        // Fetch match history for the account
                        var matchHistory = await matchHistoryService.GetMatchHistoryAsync(accountId);

                        if (matchHistory?.Matches != null)
                        {
                            foreach (MatchInfo matchId in matchHistory.Value.Matches)
                            {
                                // Add delay before fetching match details
                                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                                // Fetch match details
                                MatchDetailResponse? matchDetails = await matchDetailService.GetMatchDetailsAsync(matchId.seq_num);
                                if (matchDetails != null)
                                {
                                    SaveMatchDetailsToDatabase(dbContext, matchDetails);
                                }
                            }
                        }

                        if (matchHistory?.Players != null)
                        {
                            foreach (var player in matchHistory.Value.Players)
                            {
                                // Add player account IDs to the queue for further processing
                                if (!processedAccounts.Contains(player))
                                {
                                    accountQueue.Enqueue(player);
                                }

                                // Add delay before fetching player profile
                                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                                // Fetch and save player profile
                                var playerProfile = await playerProfileService.FetchProfilesAsync(player);
                                if (playerProfile != null)
                                {
                                    if (playerProfile.account_id == accountId)
                                    {
                                        UpdatePlayerProfileToDatabase(dbContext, playerProfile);
                                    }
                                    else
                                    {
                                        SavePlayerProfileToDatabase(dbContext, playerProfile);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing account ID: {accountId}");
                    }
                }

                // Delay to avoid overloading the API (additional delay if needed)
                await Task.Delay(1000, cancellationToken);
            }

            _logger.LogInformation("DataPopulationService background task completed.");
        }

        private void SaveMatchDetailsToDatabase(ApplicationDbContext dbContext, MatchDetailResponse matchDetails)
        {
            if (dbContext.Matches.Any(m => m.match_id == matchDetails.Match.match_id))
                return;

            var match = matchDetails.Match;
            dbContext.Matches.Add(match);

            foreach (var playersMatch in matchDetails.PlayersMatches)
            {
                var playerExists = dbContext.Players.Any(p => p.account_id == playersMatch.account_id);
                if (!playerExists)
                {
                    _logger.LogWarning($"Player with account ID {playersMatch.account_id} does not exist. Skipping PlayersMatches record.");
                    continue;
                }

                dbContext.PlayersMatches.Add(playersMatch);
            }

            dbContext.SaveChanges();
        }

        private void SavePlayerProfileToDatabase(ApplicationDbContext dbContext, Player playerProfile)
        {
            try
            {
                if (dbContext.Players.Any(p => p.account_id == playerProfile.account_id))
                {
                    _logger.LogInformation($"Player: {playerProfile.account_id} exists");
                }
                else
                {
                    dbContext.Players.Add(playerProfile);
                    _logger.LogInformation($"Added new Player: {playerProfile.account_id}");
                }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving player profile with account ID: {playerProfile.account_id}");
            }
        }

        private void UpdatePlayerProfileToDatabase(ApplicationDbContext dbContext, Player playerProfile)
        {
            try
            {
                if (dbContext.Players.Any(p => p.account_id == playerProfile.account_id))
                {
                    dbContext.Players.Update(playerProfile);
                    _logger.LogInformation($"Updated Player: {playerProfile.account_id}");
                }
                else
                {
                    dbContext.Players.Add(playerProfile);
                    _logger.LogInformation($"Added new Player: {playerProfile.account_id}");
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving player profile with account ID: {playerProfile.account_id}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataPopulationService is stopping.");

            // Cancel the background task
            _cancellationTokenSource.Cancel();

            if (_backgroundTask != null)
            {
                await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

            _logger.LogInformation("DataPopulationService stopped.");
        }
    }
}
