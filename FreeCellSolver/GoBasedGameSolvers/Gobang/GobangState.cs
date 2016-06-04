using GoBased;
using System.Collections.Generic;
using System;

namespace GoBasedGameSolvers.Gobang
{
    public class GobangState : GoSnapshot
    {
        public GobangState(int numRows, int numCols, int numToWin) : base(GetTotalBytes(numRows*numCols))
        {
            NumToWin = numToWin;
            NumCols = numCols;
            NumRows = numRows;
            InitPossibleMoves();
        }

        public PointStates NextPlayer { get; set; }

        public int NumCols { get; }
        public int NumRows { get; }

        public int NumToWin { get; }

        public int Count { get; private set; }

        public bool IsWin
        {
            get; private set;
        }

        public bool IsTie
        {
            get
            {
                // currently only check if it's full
                return Count == NumCols * NumRows;
            }
        }

        public PointStates this[int row, int col]
        {
            get
            {
                return GetPoint(row, col, NumCols);
            }
            set
            {
                SetPoint(row, col, NumCols, value);
                var move = new Move(row, col);
                if (value == PointStates.None)
                {
                    IsWin = false;
                    PossibleMoves.Add(move);
                    Count--;
                }
                else
                {
                    UpdateIsWin(row, col, value);
                    PossibleMoves.Remove(move);
                    Count++;
                }
            }
        }

        public HashSet<Move> PossibleMoves { get; } = new HashSet<Move>();

        private void UpdateIsWin(int row, int col, PointStates stone)
        {
            // horizontal
            int low, high;
            for (low = col - 1; low >= 0 && this[row, low] == stone; low--)
            {
            }
            for (high = col + 1; high < NumCols && this[row, high] == stone; high++)
            {
            }
            var count = high - low - 1;
            if (count >= NumToWin)
            {
                IsWin = true;
                return;
            }
            // vertical
            for (low = row - 1; low >= 0 && this[low, col] == stone; low--)
            {
            }
            for (high = row + 1; high < NumRows && this[high, col] == stone; high++)
            {
            }
            count = high - low - 1;
            if (count >= NumToWin)
            {
                IsWin = true;
                return;
            }

            // diagnoal
            for (low = 1; row - low >= 0 && col - low >= 0 && this[row - low, col - low] == stone; low++)
            {
            }
            for (high = 1; row + high < NumRows && col + high < NumCols
                && this[row + high, col + high] == stone; high++)
            {
            }
            count = high + low - 1;
            if (count >= NumToWin)
            {
                IsWin = true;
                return;
            }

            for (low = 1; row - low >= 0 && col + low < NumCols && this[row - low, col + low] == stone; low++)
            {
            }
            for (high = 1; row + high < NumRows && col - high >= 0
                && this[row + high, col - high] == stone; high++)
            {
            }
            count = high + low - 1;
            if (count >= NumToWin)
            {
                IsWin = true;
            }
        }
        
        private void InitPossibleMoves()
        {
            for (var i = 0; i < NumRows; i++)
            {
                for (var j = 0; j < NumCols; j++)
                {
                    PossibleMoves.Add(new Move(i, j));
                }
            }
        }
    }
}
