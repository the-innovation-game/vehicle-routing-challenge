using Xunit;
using VehicleRouting.Challenge;

namespace VehicleRouting.Tests
{
    [Collection("VehicleRouting")]
    public class Tests
    {

        [Theory]
        [InlineData("VehicleRouting,10:0,Heirloom-5,1,1957,68a127b5a3dde1775577b97bb9eb4981,RandomSolver,1382842976")]
        public void Solve_Works(string encoding)
        {
            var challengeParams = Params.FromEncoding(encoding);
            challengeParams.Solve(60);
        }

        [Theory]
        [InlineData("VehicleRouting,10:0,Heirloom-5,1,1957,68a127b5a3dde1775577b97bb9eb4981,RandomSolver,1382842976")]
        public void Solution_Encoding_Works(string encoding)
        {
            var _params = Params.FromEncoding(encoding);
            var solveResult = _params.Solve(60);
            var proof = solveResult.Solution.ToProof();
            var b = Solution.FromProof(proof).VerifySolutionAndMethod(_params, 30);
        }
    }
}
