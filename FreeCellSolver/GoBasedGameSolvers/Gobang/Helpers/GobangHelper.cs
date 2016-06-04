using GoBased;
using System.Collections.Generic;
using System.Linq;

namespace GoBasedGameSolvers.Gobang.Helpers
{
    public static class GobangHelper
    {
        public static int PlayToWin(this GobangState state, Dictionary<GoSnapshot, SnapshotSolution> winmap)
        {
            var win = winmap[state];
            var winminlen = -1;
            Move move;
            if (win.StateType == SnapshotSolution.StateTypes.DL)
            {
                var c = win.MinWinChoice;
                move = win.WinMoves[c];
                winminlen = win.MinWinLength;
            }
            else
            {
                move = state.PossibleMoves.FirstOrDefault(x => !win.LoseMoves.Contains(x));
                if (move == null)
                {
                    var c = win.MinWinChoice;
                    move = win.LoseMoves[c];
                }
            }
            move.Redo(state);
            return winminlen;
        }
    }
}
