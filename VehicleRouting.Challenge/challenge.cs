using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VehicleRouting.Challenge
{
    public class MismatchedAlgorithm : Exception
    {
        public MismatchedAlgorithm(string message) : base(message) { }
    }
    public class Difficulty
    {
        public int NumNodes { get; init; }
        public int MinPercentShorter { get; init; }
        public int MinNodeDemand => 10;
        public int MaxNodeDemand => 25;
        public int VehicleCapacity => 100;

        public Difficulty(int numNodes, int minPercentShorter)
        {
            if (numNodes < 1)
                throw new ArgumentException("Value for VehicleRouting.Difficulty.NumNodes must be >= 1");
            if (minPercentShorter < 0)
                throw new ArgumentException("Value for VehicleRouting.Difficulty.MinPercentShorter must be >= 0");
            NumNodes = numNodes;
            MinPercentShorter = minPercentShorter;
        }

        public static Difficulty FromEncoding(string[] fields)
        {
            return new Difficulty(
                    numNodes: int.Parse(fields[0]),
                    minPercentShorter: int.Parse(fields[1])
                );
        }

        public string[] Fields()
        {
            return new[] {
                NumNodes.ToString(),
                MinPercentShorter.ToString(),
            };
        }

        public Challenge GenerateChallenge(int seed)
        {
            return new Challenge(this, seed);
        }
    }

    public partial class Solution
    {
        public List<List<int>> Routes { get; init; }
        public byte[] StateUpdates { get; init; }

        public Solution(List<List<int>> routes, byte[] stateUpdates)
        {
            Routes = routes;
            StateUpdates = stateUpdates;
        }

        protected void Write(BinaryWriter writer)
        {
            writer.Write(Routes.Count);
            foreach (var route in Routes)
            {
                writer.Write(route.Count);
                foreach (var n in route)
                    writer.Write(n);
            }
            writer.Write(StateUpdates.Length);
            writer.Write(StateUpdates);
        }

        public bool VerifySolutionOnly(Challenge challenge, double maxSecondsTaken = double.MaxValue)
        {
            return challenge.AreRoutesChallengeSolution(Routes);
        }

        public bool VerifyMethodOnly(Challenge challenge, Type algorithm, double maxSecondsTaken = double.MaxValue)
        {
            DateTime start = DateTime.Now;

            using (Stream updatesStream = new MemoryStream(StateUpdates))
            using (BinaryReader reader = new BinaryReader(updatesStream))
            {
                var checkAlgoIdentifier = delegate (int update)
                {
                    if (Utils.CalcRemainingMaxSecondsTaken(start, maxSecondsTaken) <= 0)
                        throw new TimeoutException($"Solving challenge exceeded maxSecondsTaken '{maxSecondsTaken}'");
                    if (reader.ReadInt32() != update)
                        throw new MismatchedAlgorithm("Mismatched algo");
                };

                try
                {
                    var solver = (Algorithms.Abstract)Activator.CreateInstance(algorithm, challenge.Seed, checkAlgoIdentifier, (object)challenge.AreRoutesChallengeSolution, (object)challenge.EvaluateRoutesTotalDistance, false);

                    solver.Solve(
                        demands: challenge.Demands.Clone() as int[],
                        distanceMatrix: challenge.DistanceMatrix.Clone() as int[,],
                        vehicleCapacity: challenge.VehicleCapacity,
                        maxDistance: challenge.MaxTotalDistance
                    );

                    return updatesStream.Position == updatesStream.Length;
                }
                catch (MismatchedAlgorithm)
                {
                    return false;
                }
            }
        }

        public static Solution Read(BinaryReader reader)
        {
            List<List<int>> routes = new();
            var nRoutes = reader.ReadInt32();
            for (int i = 0; i < nRoutes; i++)
            {
                var nNodes = reader.ReadInt32();
                var route = new List<int>();
                for (int j = 0; j < nNodes; j++)
                    route.Add(reader.ReadInt32());
                routes.Add(route);
            }
            return new Solution(
                routes: routes,
                stateUpdates: reader.ReadBytes(reader.ReadInt32())
            );
        }
    }
    public class Challenge
    {
        public int Seed { get; init; }
        public Difficulty Difficulty { get; init; }
        public int NumNodes => Difficulty.NumNodes;
        public int VehicleCapacity => Difficulty.VehicleCapacity;
        public int[] Demands { get; init; }
        public int[,] DistanceMatrix { get; init; }
        public int MaxTotalDistance { get; init; }

        public Challenge(Difficulty difficulty, int seed)
        {
            Difficulty = difficulty;
            Seed = seed;
            Random random = new Random(seed + 1337);

            Demands = new int[Difficulty.NumNodes];
            Vector2[] cities = new Vector2[Difficulty.NumNodes];

            cities[0] = new Vector2(250, 250);
            for (int i = 1; i < Difficulty.NumNodes; i++)
            {
                Demands[i] = random.Next(Difficulty.MinNodeDemand, Difficulty.MaxNodeDemand);
                cities[i] = new Vector2(random.Next(0, 501), random.Next(0, 501));
            }

            DistanceMatrix = new int[difficulty.NumNodes, difficulty.NumNodes];
            for (int i = 0; i < difficulty.NumNodes; i++)
                for (int j = i + 1; j < difficulty.NumNodes; j++)
                {
                    int distance = (int)(cities[i] - cities[j]).Length();
                    DistanceMatrix[i, j] = distance;
                    DistanceMatrix[j, i] = distance;
                }

            var baselineRoutes = ComputeBaselineRoutes();
            var baselineDistance = EvaluateRoutesTotalDistance(baselineRoutes);
            MaxTotalDistance = (int)((1 - Difficulty.MinPercentShorter / 1000.0) * baselineDistance);
        }

        public SolveResult Solve(Type algorithm, double maxSecondsTaken = double.MaxValue, bool debugMode = false)
        {
            DateTime start = DateTime.Now;

            using (MemoryStream updatesStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(updatesStream))
            {
                var writeAlgoIdentifier = delegate (int update)
                {
                    if (Utils.CalcRemainingMaxSecondsTaken(start, maxSecondsTaken) <= 0)
                        throw new TimeoutException($"Solving challenge exceeded maxSecondsTaken '{maxSecondsTaken}'");
                    writer.Write(update);
                };

                var solver = (Algorithms.Abstract)Activator.CreateInstance(algorithm, Seed, writeAlgoIdentifier, (object)AreRoutesChallengeSolution, (object)EvaluateRoutesTotalDistance, debugMode);

                List<List<int>> routes = solver.Solve(
                    demands: Demands.Clone() as int[],
                    distanceMatrix: DistanceMatrix.Clone() as int[,],
                    vehicleCapacity: VehicleCapacity,
                    maxDistance: MaxTotalDistance
                );

                return new SolveResult()
                {
                    IsSolution = AreRoutesChallengeSolution(routes),
                    Solution = new Solution(routes, updatesStream.ToArray())
                };
            }
        }
        public List<List<int>> ComputeBaselineRoutes()
        {
            List<List<int>> routes = new();
            List<int> notVisited = Enumerable.Range(1, Difficulty.NumNodes - 1).ToList();
            while (notVisited.Count > 0)
            {
                List<int> route = new();

                int currentNode = 0;
                int capacity = Difficulty.VehicleCapacity;

                while (capacity > 0 && notVisited.Count > 0)
                {
                    var eligibleNodes = notVisited
                        .Select((node, i) => new { Index = i, Node = node })
                        .Where(pair => Demands[pair.Node] <= capacity)
                        .ToList();

                    if (eligibleNodes.Count() > 0)
                    {
                        var closest = eligibleNodes.MinBy(pair => DistanceMatrix[currentNode, pair.Node]);
                        capacity -= Demands[closest.Node];
                        route.Add(closest.Node);
                        notVisited.RemoveAt(closest.Index);
                        currentNode = closest.Node;
                    }
                    else
                        break;
                }

                routes.Add(route);
            }

            return routes;
        }

        // utility methods
        public bool AreRoutesChallengeSolution(List<List<int>> routes)
        {
            int totalDistance = EvaluateRoutesTotalDistance(routes);
            return totalDistance != -1 && totalDistance <= MaxTotalDistance;
        }

        public int EvaluateRoutesTotalDistance(List<List<int>> routes)
        {
            if (routes.Count == 0 || routes.Any(r => r.Count == 0 || r.Any(n => n <= 0 || n >= Difficulty.NumNodes)))
                return -1;

            int distanceTravelled = 0;
            bool[] visited = new bool[Difficulty.NumNodes];
            visited[0] = true;

            foreach (var route in routes)
            {
                distanceTravelled += DistanceMatrix[0, route[0]];
                int currentCapacity = Difficulty.VehicleCapacity;
                for (int i = 0; i < route.Count - 1; i++)
                {
                    distanceTravelled += DistanceMatrix[route[i], route[i + 1]];
                    currentCapacity -= Demands[route[i]];

                    if (currentCapacity < 0 || visited[route[i]])
                        return -1;

                    visited[route[i]] = true;
                }
                visited[route.Last()] = true;
                distanceTravelled += DistanceMatrix[route.Last(), 0];
            }

            if (!visited.All(v => v))
                return -1;
            else
                return distanceTravelled;
        }
    }
}
