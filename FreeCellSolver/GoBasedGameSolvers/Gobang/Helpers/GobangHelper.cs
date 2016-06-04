using GoBased;
using System.Collections.Generic;

namespace GoBasedGameSolvers.Gobang.Helpers
{
    public static class GobangHelper
    {
        public static int PlayToWin(this GobangState state, Dictionary<GoSnapshot, SnapshotSolution> winmap)
        {
            var win = winmap[state];
            var c = win.MinWinChoice;
            var move = win.WinMoves[c];
            move.Redo(state);
            return win.MinWinLength;
        }
    }
}
