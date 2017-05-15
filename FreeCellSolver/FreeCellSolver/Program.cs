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
using Rubiks;
using static Rubiks.RubikCube;
using static QSharp.Classical.Algorithms.DepthFirstSolverDP;
using static Rubiks.RubikCube.Operation;

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

        static string OpToString(Operation op)
        {
            var a = (int)op.Angle + op.DiscIndex * 3 + (int)op.DiscAxis * 6;
            return string.Format("{0,2}", a);
        }

        static void SolveRubik()
        {
            
            var rubikstr = "120";
            rubikstr += "   015";
            rubikstr += "   355";
            rubikstr += "221203032434";
            rubikstr += "541322131454";
            rubikstr += "004021411543";
            rubikstr += "   545";
            rubikstr += "   305";
            rubikstr += "   203";
            /*
            var rubikstr = "444";
            rubikstr += "   444";
            rubikstr += "   444";
            rubikstr += "222333000111";
            rubikstr += "333000111222";
            rubikstr += "333000111222";
            rubikstr += "   555";
            rubikstr += "   555";
            rubikstr += "   555";
            */
            var rubik = new RubikCube(3);
            rubik.FromString(rubikstr);
            var s = rubik.ToString();
            Console.WriteLine(s);
            var solver = new RubikCubeSolverDP(rubik);

            int count = 0;
            solver.SolveStep += (dfs, state, type) =>
            {
                if (type == SolveStepTypes.Regress /*|| type == SolveStepTypes.HitVisited*/)
                {
                    Console.WriteLine(type);
                }

                if (count++ % 100000 == 0)
                {
                  //  foreach (var op in dfs.OperationStack.Reverse())
                    //{
                    //    Console.Write(OpToString((Operation)op) + " ");
                   // }
                    Console.WriteLine("{0},{1}", dfs.OperationStack.Count, ((RubikCubeSolverDP)dfs).Visited.Count);
                }
                //Console.WriteLine("------------------------------");
                //Console.WriteLine(type);
                //Console.WriteLine(dfs.LastOperation);
                //Console.WriteLine(state);
            };
#if false
            solver.Stepped += (v, k, op) =>
            {
#if false
                Console.WriteLine("------------------------------");
                Console.WriteLine(op);
                Console.WriteLine(k);
                Console.ReadKey();
#else
                if (k == null)
                {
                    //Console.WriteLine("repeating");
                }
                /*
                if (v.VisitedCubeCount % 10000 == 0 && op != null)
                {
                    Console.Write(v.VisitedCubeCount);
                    Console.WriteLine("------------------------------");
                    Console.WriteLine(k);
                }*/
#endif
            };
#endif
            var r = solver.SolveFirst();
            if (r != null)
            {
                Console.WriteLine("Solved");
            }
            else
            {
                Console.WriteLine("Failed to solve");
            }
        }

        static void PlayRubik()
        {
            var rubik = new RubikCube(3);
            Console.WriteLine(rubik);
            while (true)
            {
                Console.Write(">");
                var opStr = Console.ReadLine();
                Operation[] ops = null;
                if (opStr == "reset")
                {
                    rubik.ResetToGood();
                    Console.WriteLine(rubik);
                    continue;
                }
                else if (opStr == "quit")
                {
                    break;
                }
                else if (string.IsNullOrWhiteSpace(opStr))
                {
                    continue;
                }
                else if (Enum.TryParse<Singmaster>(opStr, out var op))
                {
                    ops = new Operation[] { op };
                }
                else if (opStr == "twist")
                {
                    ops = new Operation[]
                    {
                        Singmaster.B,
                        Singmaster.Rp,
                        Singmaster.D2,
                        Singmaster.R,
                        Singmaster.Bp,
                        Singmaster.U2,
                        Singmaster.B,
                        Singmaster.Rp,
                        Singmaster.D2,
                        Singmaster.R,
                        Singmaster.Bp,
                        Singmaster.U2
                    };
                }
                else if (opStr == "flip")
                {
                    ops = new Operation[]
                    {
                        Singmaster.R,
                        Singmaster.U,
                        Singmaster.D,
                        Singmaster.B2,
                        Singmaster.U2,
                        Singmaster.Bp,
                        Singmaster.U,
                        Singmaster.B,
                        Singmaster.U,
                        Singmaster.B2,
                        Singmaster.Dp,
                        Singmaster.Rp,
                        Singmaster.Up,
                    };
                }
                else if (opStr == "3cycle")
                {
                    ops = new Operation[]
                    {
                        Singmaster.R2,
                        Singmaster.Up,
                        Singmaster.F,
                        Singmaster.Bp,
                        Singmaster.R2,
                        Singmaster.Fp,
                        Singmaster.B,
                        Singmaster.Up,
                        Singmaster.R2,
                    };
                }
                else
                {
                    Console.WriteLine("Unknown operation");
                    continue;
                }

                if (ops != null)
                {
                    Array.ForEach(ops, o =>
                    {
                        rubik.OperateSelf(o);
                        Console.WriteLine($"{o} ({o.ToSingmaster()})");
                        Console.WriteLine(rubik);
                    });
                }
            }
        }

        static void Main()
        {
            //InteractiveGobang(3,3,3,true);
            ////SolveGobang();

            //SolveRubik();
            PlayRubik();
        }
    }
}