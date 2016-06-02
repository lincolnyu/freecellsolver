using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGameSolvers.FreeCell.Instructions;
using CardGameSolvers.FreeCell.Helpers;

namespace CardGameSolvers.FreeCell
{
    public class Solver
    {
        public Logger Log { get; set; }

        public IEnumerable<IList<Instruction>> Solve(FreeCellState state)
        {
            if (state.IsWon())
            {
                yield return new Instruction[] { };
                yield break;
            }
            var steps = new Stack<Step>();
            var sss = new HashSet<Snapshot>();
            var moves = state.PlanPossibleMoves().ToList();
            if (moves.Count > 0)
            {
                var step = new Step
                {
                    PossibleMoves = moves,
                    SelectedInstruction = -1
                };
                steps.Push(step);
            }
            while (steps.Count > 0)
            {
                var pop = steps.Pop();
                if (pop.SelectedInstruction >= 0)
                {
                    // undo it first
                    var i = pop.PossibleMoves[pop.SelectedInstruction];
                    // leave the snapshot in the set
                    i.Undo(state);
                }
                pop.SelectedInstruction++;
                if (pop.SelectedInstruction < pop.PossibleMoves.Count)
                {
                    var i = pop.PossibleMoves[pop.SelectedInstruction];
                    i.Redo(state);
                    var ss = new Snapshot(state);
                    steps.Push(pop);
                    // already added to state set by trimming
                    if (state.IsWon())
                    {
                        yield return steps.Reverse().Select(x => x.PossibleMoves[x.SelectedInstruction]).ToList();
                    }
                    else
                    {
                        moves = state.PlanPossibleMoves().ToList();
                        if (moves.Count > 0)
                        {
                            if (moves.OfType<Move>().Where(x=>x.MoveCount>1).Any())
                            {
                                ShowMoves(moves);
                                Log?.Log("\n");
                            }
                            TrimMoves(moves, state, sss);
                            var step = new Step
                            {
                                PossibleMoves = moves,
                                SelectedInstruction = -1
                            };
                            steps.Push(step);
                        }
                    }
                }
            }
        }

        private void TrimMoves(List<Instruction> moves, FreeCellState state, HashSet<Snapshot> sss)
        {
            var orig = moves.Count;
            for (var i = moves.Count - 1; i >= 0; i--)
            {
                var m = moves[i];
                m.Redo(state);
                var s = new Snapshot(state);
                if (sss.Contains(s))
                {
                    moves.RemoveAt(i);
                }
                else
                {
                    sss.Add(s);
                }
                m.Undo(state);
            }
        }

        private void ShowStack(Stack<Step> stack)
        {
            if (Log == null) return;
            var s = stack.ToList();
            var sb = new StringBuilder();
            s.Reverse();
            foreach (var i in s)
            {
                sb.Append(i.SelectedInstruction);
                sb.Append("/");
                sb.Append(i.PossibleMoves.Count);
                sb.Append(",");
            }
            var str = sb.ToString();
            str.TrimEnd(',');
            Log.Log(str);
        }

        private void ShowState(FreeCellState state)
        {
            if (Log == null) return;
            Log.Log(state.ConvertToDisplayString());
        }

        private void ShowMoves(List<Instruction> moves)
        {
            if (Log == null) return;
            foreach (var move in moves)
            {
                Log.Log(move.ToString());
            }
        }
    }
}
