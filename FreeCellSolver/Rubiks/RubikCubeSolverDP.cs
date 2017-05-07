using QSharp.Classical.Algorithms;
using System.Collections.Generic;
using static Rubiks.RubikCube;

namespace Rubiks
{
    public class RubikCubeSolverDP : DepthFirstSolverDP
    {
        public RubikCubeSolverDP(IState initialState) : base(initialState)
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
