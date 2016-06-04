using GoBased;
using GoBased.Helpers;
using GoBasedGameSolvers.Gobang.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace GoBasedGameSolvers.Gobang
{
    public class Solver
    {
        /// <summary>
        ///  Get the type for the state by either consulting 
        ///  <paramref name="winmap"/> or recursively solving
        /// </summary>
        /// <param name="state">The state to check</param>
        /// <param name="winmap">The snapshot solution map</param>
        /// <returns>The state</returns>
        /// <remarks>
        ///  See Solve() for detail
        /// </remarks>
        private SnapshotSolution GetSolution(GobangState state, Dictionary<GoSnapshot, SnapshotSolution> winmap)
        {
            SnapshotSolution mls;
            if (winmap.TryGetValue(state, out mls))
            {
                return mls;
            }
            Solve(state, winmap);
            return winmap[state];
        }

        /// <summary>
        ///  start from <paramref name="state"/>finds all the possible states down the
        ///  track and their correspoinding player's win guaranteed paths
        /// </summary>
        /// <param name="state">go-state must not be in won state</param>
        /// <param name="winmap">all move info of states visited</param>
        /// <remarks>
        ///  A state is a moved state for a player if the player just moved
        ///  the last step, and the player is called the moved player of the
        ///  state. Otherwise if it's the player's turn, the state is a moving
        ///  state for her and the player is called the moving player or next
        ///  player.
        ///  a state for a moved player can be either of the following
        ///  1. Win/guaranteed win state, GW: if any of the possible moves
        ///     by the other leads to her GL state (all MLs). It's also called
        ///     a no-choice state for the next player.
        ///  2. Destined to a lose, DL: if there exists a next move for the
        ///     other player to be in GW state (exits MW for that player).
        ///     It's also called a smart-win state for the next player.
        ///  3. Other than above: GO.
        ///  This method returns all the MLs for the <paramref name="state"/>
        ///  (which should be a moving state) if any.
        /// </remarks>
        public void Solve(GobangState state, Dictionary<GoSnapshot, SnapshotSolution> winmap)
        {
            var ss = new SnapshotSolution(); // default type being GO
            if (state.IsWin)
            {
                ss.StateType = SnapshotSolution.StateTypes.GW;
                winmap[state.Clone()] = ss;
                ss.MinWinLength = 0;
                return;
            }
            if (state.IsTie)
            {
                ss.StateType = SnapshotSolution.StateTypes.GO;
                winmap[state.Clone()] = ss;
                return;
            }
            var moves = state.PossibleMoves.ToList();
            var isGW = true;
            var minWinLength = int.MaxValue;
            var minWinChoice = -1;
            var maxWinLengthForOppo = int.MinValue;
            // go through all possible moves of the next player
            foreach (var move in moves)
            {
                move.Redo(state);
                var sol = GetSolution(state, winmap);
                var st = sol.StateType;
                switch (st)
                {
                    case SnapshotSolution.StateTypes.GW:
                        if (sol.MinWinLength < minWinLength)
                        {
                            minWinLength = sol.MinWinLength;
                            minWinChoice = ss.WinMoves.Count;
                        }
                        // this move leads to GW for the moving player
                        // this means it's a DL for the moved
                        ss.StateType = SnapshotSolution.StateTypes.DL;
                        ss.WinMoves.Add(move);
                        isGW = false;
                        break;
                    case SnapshotSolution.StateTypes.DL:
                        // choose the longest for the opponent
                        if (sol.MinWinLength > maxWinLengthForOppo)
                        {
                            minWinChoice = ss.LoseMoves.Count;
                            maxWinLengthForOppo = sol.MinWinLength;
                        }
                        // if all moves are like this, it means
                        // it's a GW for the moved
                        ss.LoseMoves.Add(move);
                        break;
                    default: // GO
                        isGW = false;
                        break;
                }
                move.Undo(state);
            }
            if (isGW)
            {
                ss.StateType = SnapshotSolution.StateTypes.GW;
                ss.MinWinLength = maxWinLengthForOppo + 1;
                ss.MinWinChoice = minWinChoice;
            }
            else if (ss.StateType == SnapshotSolution.StateTypes.DL)
            {
                ss.MinWinLength = minWinLength + 1;
                ss.MinWinChoice = minWinChoice;
            }

            UpdateWinMap(winmap, state, ss);
        }

        private void UpdateWinMap(Dictionary<GoSnapshot, SnapshotSolution> winmap, GobangState gbs, SnapshotSolution ss)
        {
            var gc = gbs.Clone();
            winmap[gc] = ss;

            var h = gc.FlipHori(gbs.NumRows, gbs.NumCols);
            var hss = ss.FlipHori(gbs.NumCols);
            winmap[h] = hss;

            var v = gc.FlipVert(gbs.NumRows, gbs.NumCols);
            var vss = ss.FlipVert(gbs.NumCols);
            winmap[v] = vss;

            var hv = h.FlipVert(gbs.NumRows, gbs.NumCols);
            var hvss = hss.FlipVert(gbs.NumCols);
            winmap[hv] = hvss;

            if (gbs.NumRows == gbs.NumCols)
            {
                var t = gc.Transpose(gbs.NumRows);
                var tss = ss.Transpose(gbs.NumRows);
                winmap[t] = tss;
                
                var hvt = hv.Transpose(gbs.NumRows);
                var hvtss = hvss.Transpose(gbs.NumRows);
                winmap[hvt] = hvtss;

                var ht = h.Transpose(gbs.NumRows);
                var htss = hss.Transpose(gbs.NumRows);
                winmap[ht] = htss;

                var vt = v.Transpose(gbs.NumRows);
                var vtss = vss.Transpose(gbs.NumRows);
                winmap[vt] = vtss;
            }
        }
    }
}

