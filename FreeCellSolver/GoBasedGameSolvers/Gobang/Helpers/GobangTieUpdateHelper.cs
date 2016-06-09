using System;
using static GoBased.GoSnapshot;

namespace GoBasedGameSolvers.Gobang.Helpers
{
    public static class GobangTieUpdateHelper
    {
        private delegate int UpdateLiveness(GobangState state, int row, int col, PointStates stone);

        private static UpdateLiveness[] UpdateLivenessList = new UpdateLiveness[]
            {
                UpdateLivenessForStoneHori,
                UpdateLivenessForStoneVert,
                UpdateLivenessForStoneDiag1,
                UpdateLivenessForStoneDiag2
            };

        public static void ForceLiveness(this GobangState state, int row, int col, int index, int liveness)
        {
            state.RemoveLiveCount(state.Liveness[row, col, index]);

            state.Liveness[row, col, index] = liveness;

            state.AddLiveCount(liveness);
        }

        public static void UpdateLivenessForRemoval(this GobangState state, int row, int col)
        {
            for (var i = 0; i < 4; i++)
            {
                state.ForceLiveness(row, col, i, 0);
            }
        }

        public static void UpdateNearbyForRemoval(this GobangState state, int row, int col, PointStates oldRef)
        {
            var d = new int[8];
            var r = new int[8];
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var x1 = col - i;
                var x2 = col + i;
                var y1 = row - i;
                var y2 = row + i;
                var bx1 = x1 >= 0;
                var bx2 = x2 < state.NumCols;
                var by1 = y1 >= 0;
                var by2 = y2 < state.NumRows;

                if (bx1 && d[0] < 2)
                {
                    state.UpdateNearbySingleDir(row, x1, oldRef, 0, ref d[0], ref r[0]);
                }
                if (bx2 && d[1] < 2)
                {
                    state.UpdateNearbySingleDir(row, x2, oldRef, 0, ref d[1], ref r[1]);
                }
                if (by1 && d[2] < 2)
                {
                    state.UpdateNearbySingleDir(y1, col, oldRef, 1, ref d[2], ref r[2]);
                }
                if (by2 && d[3] < 2)
                {
                    state.UpdateNearbySingleDir(y2, col, oldRef, 1, ref d[3], ref r[3]);
                }
                if (bx1 && by1 && d[4] < 2)
                {
                    state.UpdateNearbySingleDir(y1, x1, oldRef, 2, ref d[4], ref r[4]);
                }
                if (bx2 && by2 && d[5] < 2)
                {
                    state.UpdateNearbySingleDir(y2, x2, oldRef, 2, ref d[5], ref r[5]);
                }
                if (bx2 && by1 && d[6] < 2)
                {
                    state.UpdateNearbySingleDir(y1, x2, oldRef, 3, ref d[6], ref r[6]);
                }
                if (bx1 && by2 && d[7] < 2)
                {
                    state.UpdateNearbySingleDir(y2, x1, oldRef, 3, ref d[7], ref r[7]);
                }
            }
        }

        public static int UpdateLivenessForStoneHori(this GobangState state, int row, int col, PointStates stone)
        {
            int d1 = 0, d2 = 0;
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var x1 = col - i;
                var x2 = col + i;
                var bx1 = x1 >= 0;
                var bx2 = x2 < state.NumCols;

                if (d1 == 0)
                {
                    if (bx1)
                    {
                        var c = state[row, x1];
                        if (c != stone && c != PointStates.None)
                        {
                            d1 = i;
                        }
                    }
                    else
                    {
                        d1 = i;
                    }
                }
                if (d2 == 0)
                {
                    if (bx2)
                    {
                        var c = state[row, x2];
                        if (c != stone && c != PointStates.None)
                        {
                            d2 = i;
                        }
                    }
                    else if (d2 == 0)
                    {
                        d2 = i;
                    }
                }
            }

            var l = Math.Min(state.NumToWin,
                d1 > 0 && d2 > 0 ? d2 + d1 - 1 : state.NumToWin);
            state.ForceLiveness(row, col, 0, l);
            return l;
        }

        public static int UpdateLivenessForStoneVert(this GobangState state, int row, int col, PointStates stone)
        {
            int d1 = 0, d2 = 0;
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var y1 = row - i;
                var y2 = row + i;
                var by1 = y1 >= 0;
                var by2 = y2 < state.NumRows;

                if (d1 == 0)
                {
                    if (by1)
                    {
                        var c = state[y1, col];
                        if (c != stone && c != PointStates.None)
                        {
                            d1 = i;
                        }
                    }
                    else if (d1 == 0)
                    {
                        d1 = i;
                    }
                }
                if (d2 == 0)
                {
                    if (by2)
                    {
                        var c = state[y2, col];
                        if (c != stone && c != PointStates.None)
                        {
                            d2 = i;
                        }
                    }
                    else
                    {
                        d2 = i;
                    }
                }
            }

            var l = Math.Min(state.NumToWin,
                d1 > 0 && d2 > 0 ? d2 + d1 - 1 : state.NumToWin);
            state.ForceLiveness(row, col, 1, l);

            return l;
        }

        public static int UpdateLivenessForStoneDiag1(this GobangState state, int row, int col, PointStates stone)
        {
            int d1 = 0, d2 = 0;
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var x1 = col - i;
                var x2 = col + i;
                var y1 = row - i;
                var y2 = row + i;
                var bx1 = x1 >= 0;
                var bx2 = x2 < state.NumCols;
                var by1 = y1 >= 0;
                var by2 = y2 < state.NumRows;

                if (d1 == 0)
                {
                    if (bx1 && by1)
                    {
                        var c = state[y1, x1];
                        if (c != stone && c != PointStates.None)
                        {
                            d1 = i;
                        }
                    }
                    else
                    {
                        d1 = i;
                    }
                }
                if (d2 == 0)
                {
                    if (bx2 && by2)
                    {
                        var c = state[y2, x2];
                        if (c != stone && c != PointStates.None)
                        {
                            d2 = i;
                        }
                    }
                    else
                    {
                        d2 = i;
                    }
                }
            }

            var l = Math.Min(state.NumToWin,
                d1 > 0 && d2 > 0 ? d2 + d1 - 1 : state.NumToWin);
            state.ForceLiveness(row, col, 2, l);
            return l;
        }

        private static int UpdateLivenessForStoneDiag2(this GobangState state, int row, int col, PointStates stone)
        {
            int d1 = 0, d2 = 0;
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var x1 = col - i;
                var x2 = col + i;
                var y1 = row - i;
                var y2 = row + i;
                var bx1 = x1 >= 0;
                var bx2 = x2 < state.NumCols;
                var by1 = y1 >= 0;
                var by2 = y2 < state.NumRows;

                if (d1 == 0)
                {
                    if (bx2 && by1)
                    {
                        var c = state[y1, x2];
                        if (c != stone && c != PointStates.None)
                        {
                            d1 = i;
                        }
                    }
                    else
                    {
                        d1 = i;
                    }
                }
                if (d2 == 0)
                {
                    if (bx1 && by2)
                    {
                        var c = state[y2, x1];
                        if (c != stone && c != PointStates.None)
                        {
                            d2 = i;
                        }
                    }
                    else
                    {
                        d2 = i;
                    }
                }
            }

            var l = Math.Min(state.NumToWin,
                d1 > 0 && d2 > 0 ? d2 + d1 - 1 : state.NumToWin);
            state.ForceLiveness(row, col, 3, l);
            return l;
        }

        public static void UpdateLivenessForStone(this GobangState state, int row, int col, PointStates stone)
        {
            state.UpdateLivenessForStoneHori(row, col, stone);
            state.UpdateLivenessForStoneVert(row, col, stone);
            state.UpdateLivenessForStoneDiag1(row, col, stone);
            state.UpdateLivenessForStoneDiag2(row, col, stone);
        }

        private static void RemoveLiveCount(this GobangState state, int l)
        {
            if (l == state.NumToWin)
            {
                state.AliveCount--;
                if (state.AliveCount == 0 && state.Count > 0)
                {
                    state.IsTie = true;
                }
            }
        }

        private static void AddLiveCount(this GobangState state, int l)
        {
            if (l == state.NumToWin)
            {
                state.AliveCount++;
                if (state.AliveCount > 0)
                {
                    state.IsTie = false;
                }
            }
        }

        public static void UpdateNearbyForStone(this GobangState state, int row, int col, PointStates stone)
        {
            var d = new int[8];
            var r = new int[8];
            for (var i = 1; i <= state.NumToWin; i++)
            {
                var x1 = col - i;
                var x2 = col + i;
                var y1 = row - i;
                var y2 = row + i;
                var bx1 = x1 >= 0;
                var bx2 = x2 < state.NumCols;
                var by1 = y1 >= 0;
                var by2 = y2 < state.NumRows;

                if (bx1 && d[0] < 2)
                {
                    state.UpdateNearbySingleDir(row, x1, stone, 0, ref d[0], ref r[0]);
                }
                if (bx2 && d[1] < 2)
                {
                    state.UpdateNearbySingleDir(row, x2, stone, 0, ref d[1], ref d[1]);
                }
                if (by1 && d[2] < 2)
                {
                    state.UpdateNearbySingleDir(y1, col, stone, 1, ref d[2], ref d[2]);
                }
                if (by2 && d[3] < 2)
                {
                    state.UpdateNearbySingleDir(y2, col, stone, 1, ref d[3], ref d[3]);
                }
                if (bx1 && by1 && d[4] < 2)
                {
                    state.UpdateNearbySingleDir(y1, x1, stone, 2, ref d[4], ref d[4]);
                }
                if (bx2 && by2 && d[5] < 2)
                {
                    state.UpdateNearbySingleDir(y2, x2, stone, 2, ref d[5], ref d[5]);
                }
                if (bx2 && by1 && d[6] < 2)
                {
                    state.UpdateNearbySingleDir(y1, x2, stone, 3, ref d[6], ref d[6]);
                }
                if (bx1 && by2 && d[7] < 2)
                {
                    state.UpdateNearbySingleDir(y2, x1, stone, 3, ref d[7], ref d[7]);
                }
            }
        }

        private static void UpdateNearbySingleDir(this GobangState state, int row, int col, PointStates stone, int index, ref int d, ref int r)
        {
            var c = state[row, col];
            if (c == stone)
            {
                // no more need to check
                d = 2;
            }
            else if (c != PointStates.None)
            {
                if (d == 0)
                {
                    // update c
                    var updateLiveness = UpdateLivenessList[index];
                    r = updateLiveness(state, row, col, state[row, col]);
                    d = 1;
                }
                else if (d == 1)
                {
                    // follow
                    state.ForceLiveness(row, col, index, r);
                }
            }
        }
    }
}
