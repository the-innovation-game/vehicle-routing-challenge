using VehicleRouting.Challenge;

namespace VehicleRouting.Runner
{
    public class Program
    {
        static Random Random = new Random(1337);
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] {
                    $"RandomSolver", // typeof(Algorithms.RandomSolver).Name
                    $"{false}",
                    $"{int.MaxValue}"
                };
            }

            Type algorithm = Utils.GetAlgorithm(args[0]);
            bool debug = bool.Parse(args[1]);
            int numRuns = int.Parse(args[2]);

            for (int i = 0; i < numRuns; i++)
            {
                Run(algorithm, debug);
            }
        }
        public static void Run(Type algorithm, bool debug = false)
        {
            var start = DateTime.Now;
            var difficulty = new Difficulty(
                // the number of nodes in the Vehicle Routing Problem
                numNodes: Random.Next(20, 50),

                // your algorithm must find a set of routes with total distance x% shorter than a greedy algorithm
                // minPercentShorter is a fixed point number with 1/1000 scaling. i.e. 12.5% is stored as 125
                minPercentShorter: Random.Next(100, 200)
            );
            int seed = Random.Next();
            var challenge = difficulty.GenerateChallenge(seed);
            var solveResult = challenge.Solve(algorithm, debugMode: debug);

            Console.WriteLine($"VehicleRouting, {algorithm.Name}, {seed}, {difficulty.NumNodes}, {difficulty.MinPercentShorter}, {solveResult.IsSolution}, {(DateTime.Now - start).TotalSeconds}");
        }
    }
}
