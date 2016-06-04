using GoBasedGameSolvers.Gobang.Helpers;

namespace GoBasedGameSolvers.Gobang
{
    public class Move
    {
        public ushort Row;
        public ushort Col;

        public Move(int row, int col)
        {
            Row = (ushort)row;
            Col = (ushort)col;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Move;
            if (other == null) return false;
            return Row == other.Row && Col == other.Col;
        }

        public override int GetHashCode()
        {
            return (23 + Row) * 17 + Col;
        }

        public void Redo(GobangState state)
        {
            var cp = state.NextPlayer;
            state[Row, Col] = cp;
            state.NextPlayer = state.NextPlayer.GetOpponent();
        }

        public void Undo(GobangState state)
        {
            state[Row, Col] = GoBased.GoSnapshot.PointStates.None;
            state.NextPlayer = state.NextPlayer.GetOpponent();
        }
    }
}
