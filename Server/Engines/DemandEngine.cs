using RebatesSimulator.Server.Models;

namespace RebatesSimulator.Server.Engines
{
    public static class DemandEngine
    {
        public static int GetPlayerForTruckToGoTo(ICollection<Player> players)
        {
            var playerDemands = players
                .Select(player =>
                {
                    var randomness = new Random();
                    var advertisingBonus = randomness.NextDouble() + 0.5;
                    var demandLevel = player.Rebate * player.Stock * advertisingBonus;
                    return new
                    {
                        player,
                        demandLevel
                    };
                })
                .ToArray();

            var demandSum = playerDemands.Select(o => o.demandLevel).DefaultIfEmpty(1).Sum();

            var truckProbabilities = playerDemands
                .Select(player => {
                    return new
                    {
                        player,
                        demandLevel = player.demandLevel / demandSum
                    };
                })
                .ToArray();

            var winningPlayer = truckProbabilities.OrderByDescending(p => p.demandLevel).First();
            return winningPlayer.player.player.Id;
        }
    }
}
