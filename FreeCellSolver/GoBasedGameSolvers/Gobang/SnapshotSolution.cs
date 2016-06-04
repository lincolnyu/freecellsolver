using System.Collections.Generic;

namespace GoBasedGameSolvers.Gobang
{
    public class SnapshotSolution
    {
        public enum StateTypes
        {
            GO = 0, // default
            GW,
            DL,
        }

        public StateTypes StateType { get; set; }

        /// <summary>
        ///  moves that lead to GW states (win)
        /// </summary>
        public List<Move> WinMoves { get; } = new List<Move>();

        /// <summary>
        ///  moves that lead to DL states (lose)
        /// </summary>
        public List<Move> LoseMoves { get; } = new List<Move>();
    }
}
