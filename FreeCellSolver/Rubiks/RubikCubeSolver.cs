using QSharp.Classical.Algorithms;
using System.Collections.Generic;
using static Rubiks.RubikCube;

namespace Rubiks
{
    public class RubikCubeSolver : DepthFirstSolver
    {
        public RubikCubeSolver(RubikCube rubik, int maxDepth = int.MaxValue) : base(rubik, maxDepth)
        {
        }

        public RubikCube InitialRubik => (RubikCube)InitialState;

        public IList<Operation> SolveShortest(SolveShortestQuitPredicate<Operation> quit)
            => SolveShortest<Operation>(quit);

        public new IList<Operation> SolveFirst()
            => SolveFirst<Operation>();

        public new IList<Operation> SolveNext()
            => SolveNext<Operation>();
    }
}
