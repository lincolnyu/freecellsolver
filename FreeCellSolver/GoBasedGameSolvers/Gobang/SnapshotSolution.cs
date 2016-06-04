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

        /// <summary>
        ///  Max steps towards a win the state is GW or DL
        /// </summary>
        public int MinWinLength;

        /// <summary>
        ///  For DL it's the choice to take to ensure the above length
        ///  For GW it's the best choice to delay the opponent's victory
        ///   (which also complies with the above length)
        /// </summary>
        public int MinWinChoice;
    }
}
