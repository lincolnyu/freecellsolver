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

        static void InteractiveGobang(int numrows, int numcols, int numtowin, bool humanfirst)
        {
            var state = new GobangState(numrows, numcols, numtowin);
            state.NextPlayer = ((GoSnapshot)state).DeduceCurrentPlayer();
            var cpucolor = humanfirst ? state.NextPlayer.GetOpponent() : state.NextPlayer;
            var solver = new GobangSolver();
            var winmap = new Dictionary<GoSnapshot, SnapshotSolution>();
            Console.WriteLine("Computer solving ...");
            var t1 = DateTime.UtcNow;
            solver.Solve(state, winmap);
            var t2 = DateTime.UtcNow;
            Console.WriteLine("Solving completed, taking {0}.", t2-t1);
            var toContinue = true;
            while (toContinue)
            {
                while (!state.IsTerminated)
                {
                    if (state.NextPlayer == cpucolor)
                    {
                        Console.WriteLine("Computer moving ...");
                        var steps = state.PlayToWin(winmap);
                        if (steps >= 0)
                        {
                            Console.WriteLine($"Computer is bound to win in {steps} step(s)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Your turn ...");
                        while (true)
                        {
                            try
                            {
                                var line = Console.ReadLine();
                                var s = line.Split(',');
                                var row = int.Parse(s[0]) - 1;
                                var col = int.Parse(s[1]) - 1;
                                var m = new Move(row, col);
                                if (state[row, col] != GoSnapshot.PointStates.None)
                                {
                                    Console.WriteLine("Can't place the stone here");
                                    continue;
                                }
                                m.Redo(state);
                                break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Something wrong with your input, try again ...");
                            }
                        }
                    }
                    var board = state.ConvertToDisplayString();
                    Console.WriteLine(board);
                }
                if (state.IsWin)
                {
                    if (state.NextPlayer == cpucolor)
                        Console.WriteLine("You won.");
                    else
                        Console.WriteLine("Computer won.");
                }
                else
                {
                    Console.WriteLine("Tie.");
                }
                var validInput = false;
                while (!validInput)
                {
                    Console.Write("Continue playing? (Y/N)");
                    var g = (char)Console.ReadKey().Key;
                    if (g == 'N' || g == 'n')
                    {
                        toContinue = false;
                        validInput = true;
                    }
                    else if (g == 'Y' || g == 'y')
                    {
                        state.Reset();
                        state.NextPlayer = ((GoSnapshot)state).DeduceCurrentPlayer();
                        validInput = true;
                    }
                    Console.WriteLine();
                }
            }
        }

        static void SolveGobang()
        {
            var state = new GobangState(4,4,4);
            state.NextPlayer = ((GoSnapshot)state).DeduceCurrentPlayer();
            var solver = new GobangSolver();
            var winmap = new Dictionary<GoSnapshot, SnapshotSolution>();
            solver.Solve(state, winmap);
            var win = winmap[state];
            Console.WriteLine($"type = {win.StateType}, winmoves = {win.WinMoves.Count}");
        }

        static void Main()
        {
            InteractiveGobang(3,3,3,true);
            //SolveGobang();
        }
    }
}