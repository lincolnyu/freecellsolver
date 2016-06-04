using GoBased;
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
        private SnapshotSolution.StateTypes GetStateType(GobangState state, Dictionary<GoSnapshot, SnapshotSolution> winmap)
        {
            SnapshotSolution mls;
            if (winmap.TryGetValue(state, out mls))
            {
                return mls.StateType;
            }
            Solve(state, winmap);
            return winmap[state].StateType;
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
            var mls = new SnapshotSolution(); // default type being GO
            if (state.IsWin)
            {
                mls.StateType = SnapshotSolution.StateTypes.GW;
                winmap[state.Clone()] = mls;
                return;
            }
            if (state.IsTie)
            {
                mls.StateType = SnapshotSolution.StateTypes.GO;
                winmap[state.Clone()] = mls;
                return;
            }
            var moves = state.PossibleMoves.ToList();
            var isGW = true;
            // go through all possible moves of the next player
            foreach (var move in moves)
            {
                move.Redo(state);
                var st = GetStateType(state, winmap);
                switch (st)
                {
                    case SnapshotSolution.StateTypes.GW:
                        // this move leads to GW for the moving player
                        // this means it's a DL for the current
                        mls.StateType = SnapshotSolution.StateTypes.DL;
                        mls.WinMoves.Add(move);
                        isGW = false;
                        break;
                    case SnapshotSolution.StateTypes.DL:
                        // if all moves are like this, it means
                        // it's a GW for the moved
                        mls.LoseMoves.Add(move);
                        break;
                    default: // GO
                        isGW = false;
                        break;
                }
                move.Undo(state);
            }
            if (isGW)
            {
                mls.StateType = SnapshotSolution.StateTypes.GW;
            }
            winmap[state.Clone()] = mls;
        }
    }
}

