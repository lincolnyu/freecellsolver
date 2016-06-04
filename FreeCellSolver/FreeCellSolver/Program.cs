using CardGameSolvers;
using CardGameSolvers.FreeCell;
using CardGameSolvers.FreeCell.Helpers;
using System.Linq;
using System;
using GoBasedGameSolvers.Gobang;
using FreeCellSolver = CardGameSolvers.FreeCell.Solver;
using GobangSolver = GoBasedGameSolvers.Gobang.Solver;
using System.Collections.Generic;
using GoBased;
using GoBasedGameSolvers.Gobang.Helpers;

namespace FreeCellSolverConsole
{
    class Program
    {
        class MyLogger : Logger
        {
            public override void Log(string msg)
            {
                Console.WriteLine(msg);
                Console.ReadKey(true);
            }
        }

        static void SolveFreeCell()
        {
            var state = new FreeCellState();
            var str = ",,,;,,,;$7,*Q,*4,^10,@10,*7,@9;$8,@A,*J,@5,$9,$4,^7;^8,*2,$2,*9,$10,^2,@J;*K,*6,^Q,@Q,@2,@6,$3;*8,$5,*3,^6,^K,^9;$K,^5,*5,@3,*10,@7;^4,*A,^3,$6,@K,^J;$Q,@4,$J,@8,$A,^A";
            state.ConvertFromString(str);
            var display = state.ConvertToDisplayString();
            Console.WriteLine(display);

            var solver = new FreeCellSolver { Log = new MyLogger() };
            var sol = solver.Solve(state).First();
            foreach (var i in sol.Reverse())
            {
                i.Undo(state);
            }
            Console.WriteLine($"solution: {sol.Count} steps");
            foreach (var i in sol)
            {
                Console.WriteLine(i.ToString());
                Console.ReadKey(true);
                i.Redo(state);
                display = state.ConvertToDisplayString();
                Console.WriteLine(display);
            }
        }

        static void SolveGoBang3x3()
        {
            var state = new GobangState(4,4,3);
            state.NextPlayer = ((GoSnapshot)state).DeduceCurrentPlayer();
            var solver = new GobangSolver();
            var winmap = new Dictionary<GoSnapshot, SnapshotSolution>();
            solver.Solve(state, winmap);
            var win = winmap[state];
            Console.WriteLine($"type = {win.StateType}, winmoves = {win.WinMoves.Count}");
        }

        static void Main()
        {
            SolveGoBang3x3();
        }
    }
}
