namespace GoBasedGameSolvers.Gobang.Helpers
{
    public static class SolutionHelper
    {
        public static Move FlipHori(this Move move, int numcols)
        {
            return new Move(move.Row, numcols - move.Col - 1);
        }

        public static Move FlipVert(this Move move, int numrows)
        {
            return new Move(numrows - move.Row - 1, move.Col);
        }

        public static Move Transpose(this Move move, int numrowcols)
        {
            return new Move(move.Col, move.Row);
        }

        private delegate Move MoveFlip(Move move, int n);
        
        private static SnapshotSolution Flip(this SnapshotSolution sol, int n, MoveFlip flip)
        {
            var f = new SnapshotSolution
            {
                StateType = sol.StateType,
                MinWinChoice = sol.MinWinChoice,
                MinWinLength = sol.MinWinLength
            };
            foreach (var m in sol.WinMoves)
            {
                var nm = flip(m, n);
                f.WinMoves.Add(nm);
            }
            foreach (var m in sol.LoseMoves)
            {
                var nm = flip(m, n);
                f.LoseMoves.Add(nm);
            }
            return f;
        }

        public static SnapshotSolution FlipHori(this SnapshotSolution sol, int numcols)
        {
            return Flip(sol, numcols, FlipHori);
        }

        public static SnapshotSolution FlipVert(this SnapshotSolution sol, int numcols)
        {
            return Flip(sol, numcols, FlipVert);
        }

        public static SnapshotSolution Transpose(this SnapshotSolution sol, int numcols)
        {
            return Flip(sol, numcols, Transpose);
        }
    }
}
