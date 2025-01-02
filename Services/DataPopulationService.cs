using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Models;

namespace dotatryhard.Services
{
    public class DataPopulationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Task? _backgroundTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public DataPopulationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start the background task
            _backgroundTask = Task.Run(
                () => RunBackgroundTask(_cancellationTokenSource.Token),
                cancellationToken
            );

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

                // Mark the account as processed
                processedAccounts.Add(accountId);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var matchHistoryService =
                        scope.ServiceProvider.GetRequiredService<IMatchHistoryService>();
                    var playerProfileService =
                        scope.ServiceProvider.GetRequiredService<ISteamUserService>();
                    var matchDetailService =
                        scope.ServiceProvider.GetRequiredService<PlayersMatchesService>();

                    var dbContext =
                        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (accountQueue.Count <= 1)
                    {
                        var AllPlayersMatchesAverages =
                            await matchDetailService.GetAllWithAveragesAsync();
                        // if (AllPlayersMatchesAverages != null)
                        // {
                        //     // accountQueue.Enqueue(player);
                        //     var PlayersMatches = AllPlayersMatchesAverages.playersMatches.Select(
                        //         group =>
                        //         {
                        //             group.
                        //             foreach (var player in group.)
                        //             {
                        //                 accountQueue.Enqueue(player.account_id);
                        //             }
                        //         }
                        //     );
                        //     // first 2000 players
                        // }
                        // else
                        // {
                        //     accountQueue.Enqueue(87683422);
                        // }
                    }

                    try
                    {
                        Console.WriteLine($"-----------------------------");
                        Console.WriteLine($"Processing account ID: {accountId}");

                        // Fetch match history for the account
                        var matchHistory = await matchHistoryService.GetMatchHistoryAsync(
                            accountId
                        );

                        if (matchHistory?.Players != null)
                        {
                            Console.WriteLine($"-----------------------------");
                            var playersToCheck = matchHistory.Value.Players;
                            Console.WriteLine($"Players to check: {playersToCheck.Count}");
                            var existingPlayers = dbContext
                                .Players.Where(p => playersToCheck.Contains(p.account_id))
                                .Select(p => p.account_id)
                                .ToHashSet();
                            Console.WriteLine(
                                $"Existing players in database: {existingPlayers.Count}"
                            );
                            var newPlayers = playersToCheck
                                .Where(p => !existingPlayers.Contains(p))
                                .ToList();
                            Console.WriteLine($"newPlayers to check: {newPlayers.Count}");
                            Console.WriteLine($"-----------------------------");
                            foreach (var player in newPlayers)
                            {
                                // Add delay before fetching player profile
                                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                                // Fetch and save player profile
                                var playerProfile = await playerProfileService.FetchProfilesAsync(
                                    player
                                );
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

                        if (matchHistory?.Matches != null)
                        {
                            var matchesToCheck = matchHistory.Value.Matches;
                            Console.WriteLine($"Matches to check: {matchesToCheck.Count}");
                            var matchIdsToCheck = matchesToCheck.Select(m => m.id);
                            var existingMatches = dbContext
                                .Matches.Where(m => matchIdsToCheck.Contains(m.match_id))
                                .Select(m => m.match_id)
                                .ToHashSet();
                            Console.WriteLine(
                                $"Existing matches in database: {existingMatches.Count}"
                            );
                            var newMatches = matchesToCheck
                                .Where(m => !existingMatches.Contains(m.id))
                                .ToList();
                            Console.WriteLine($"New matches to process: {newMatches.Count}");
                            Console.WriteLine($"-----------------------------");

                            foreach (MatchInfo matchId in matchHistory.Value.Matches)
                            {
                                // Add delay before fetching match details
                                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);

                                // Fetch match details
                                MatchDetailResponse? matchDetails =
                                    await matchDetailService.GetMatchDetailsAsync(matchId.seq_num);
                                if (matchDetails != null)
                                {
                                    SaveMatchToDatabase(dbContext, matchDetails.Match);
                                    SavePlayersMatchesToDatabase(dbContext, matchDetails);
                                    SavePlayersMatchesAveragesToDatabase(dbContext, matchDetails);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex} - Error processing account ID: {accountId}");
                    }
                }

                // Delay to avoid overloading the API (additional delay if needed)
                // Add player account IDs to the queue for further processing


                await Task.Delay(1000, cancellationToken);
            }

            Console.WriteLine("DataPopulationService background task completed.");
        }

        private void SaveMatchToDatabase(ApplicationDbContext dbContext, Match match)
        {
            try
            {
                if (dbContext.Matches.Any(m => m.match_id == match.match_id))
                {
                    return;
                }
                dbContext.Matches.Add(match);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{ex} - Error saving match details for match ID: {match.match_id}"
                );
            }
        }

        private void SavePlayersMatchesToDatabase(
            ApplicationDbContext dbContext,
            MatchDetailResponse matchDetails
        )
        {
            try
            {
                if (
                    matchDetails == null
                    || matchDetails.PlayersMatches == null
                    || !matchDetails.PlayersMatches.Any()
                )
                {
                    return;
                }

                var playersMatchesToCheck = matchDetails.PlayersMatches;

                // Efficiently check for existing matches using a single query
                var existingMatchIds = dbContext
                    .PlayersMatches.Where(pm =>
                        playersMatchesToCheck.Select(p => p.match_id).Contains(pm.match_id)
                        && playersMatchesToCheck.Select(p => p.account_id).Contains(pm.account_id)
                    )
                    .Select(pm => new { pm.match_id, pm.account_id })
                    .ToList();

                // Filter out existing PlayersMatches
                var newData = playersMatchesToCheck
                    .Where(pm =>
                        !existingMatchIds.Any(em =>
                            em.match_id == pm.match_id && em.account_id == pm.account_id
                        )
                    )
                    .ToList();

                if (!newData.Any())
                {
                    return;
                }

                dbContext.PlayersMatches.AddRange(newData);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{ex} - Error saving match details for match ID: {matchDetails?.Match?.match_id ?? -1}" // Handle null Match
                );
            }
        }

        private void SavePlayersMatchesAveragesToDatabase(
            ApplicationDbContext dbContext,
            MatchDetailResponse matchDetails
        )
        {
            try
            {
                var playersMatchesAverageToCheck = matchDetails.playersMatchesAverages;
                foreach (var avg in playersMatchesAverageToCheck)
                {
                    // Check if the record already exists based on account_id
                    var existingAvg = dbContext.PlayersMatchesAverages.FirstOrDefault(e =>
                        e.account_id == avg.account_id
                    );
                    if (existingAvg == null)
                    {
                        dbContext.PlayersMatchesAverages.Add(avg);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        existingAvg.assists = (avg.assists + existingAvg.assists) / 2;
                        existingAvg.deaths = (avg.deaths + existingAvg.deaths) / 2;
                        existingAvg.denies = (avg.denies + existingAvg.denies) / 2;
                        existingAvg.gold_per_min =
                            (avg.gold_per_min + existingAvg.gold_per_min) / 2;
                        existingAvg.hero_damage = (avg.hero_damage + existingAvg.hero_damage) / 2;
                        existingAvg.hero_healing =
                            (avg.hero_healing + existingAvg.hero_healing) / 2;
                        existingAvg.kills = (avg.kills + existingAvg.kills) / 2;
                        existingAvg.last_hits = (avg.last_hits + existingAvg.last_hits) / 2;
                        existingAvg.net_worth = (avg.net_worth + existingAvg.net_worth) / 2;
                        existingAvg.tower_damage =
                            (avg.tower_damage + existingAvg.tower_damage) / 2;
                        existingAvg.xp_per_min = (avg.xp_per_min + existingAvg.xp_per_min) / 2;
                        existingAvg.hero_level = (avg.hero_level + existingAvg.hero_level) / 2;
                        existingAvg.score = (avg.score + existingAvg.score) / 2;

                        existingAvg.region = RegionMapper.GetRegion(
                            matchDetails?.Match?.cluster ?? 0
                        );

                        existingAvg.win += avg.win;
                        existingAvg.aghanims_scepter += avg.aghanims_scepter;
                        existingAvg.aghanims_shard += avg.aghanims_shard;
                        existingAvg.leaver_status += avg.leaver_status;
                        existingAvg.moonshard += avg.moonshard;
                        existingAvg.match_count += 1;

                        dbContext.PlayersMatchesAverages.Update(existingAvg);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{ex} - Error saving match details for match ID: {matchDetails.Match.match_id}"
                );
            }
        }

        private void SavePlayerProfileToDatabase(
            ApplicationDbContext dbContext,
            Player playerProfile
        )
        {
            try
            {
                if (!dbContext.Players.Any(p => p.account_id == playerProfile.account_id))
                {
                    dbContext.Players.Add(playerProfile);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $" {ex} - Error saving player profile with account ID: {playerProfile.account_id}"
                );
            }
        }

        private void UpdatePlayerProfileToDatabase(
            ApplicationDbContext dbContext,
            Player playerProfile
        )
        {
            try
            {
                if (dbContext.Players.Any(p => p.account_id == playerProfile.account_id))
                {
                    dbContext.Players.Update(playerProfile);
                }
                else
                {
                    dbContext.Players.Add(playerProfile);
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{ex} - Error saving player profile with account ID: {playerProfile.account_id}"
                );
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("DataPopulationService is stopping.");

            // Cancel the background task
            _cancellationTokenSource.Cancel();

            if (_backgroundTask != null)
            {
                await Task.WhenAny(
                    _backgroundTask,
                    Task.Delay(Timeout.Infinite, cancellationToken)
                );
            }

            Console.WriteLine("DataPopulationService stopped.");
        }
    }
}
