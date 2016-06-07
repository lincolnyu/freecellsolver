using GoBased;
using System.Collections.Generic;
using GoBasedGameSolvers.Gobang.Helpers;

namespace GoBasedGameSolvers.Gobang
{
    public class GobangState : GoSnapshot
    {
        public GobangState(int numRows, int numCols, int numToWin) : base(GetTotalBytes(numRows*numCols))
        {
            NumToWin = numToWin;
            NumCols = numCols;
            NumRows = numRows;
            ResetPossibleMoves();
            Liveness = new int[numRows, numCols, 4];
        }

        public PointStates NextPlayer { get; set; }

        public int NumCols { get; }
        public int NumRows { get; }

        public int NumToWin { get; }

        public int Count { get; private set; }

        public int[,,] Liveness { get; }
        public int AliveCount { get; set; }

        public bool IsWin
        {
            get; private set;
        }

        public bool IsTie
        {
            get; private set;
        }

        public bool IsTerminated => IsWin || IsTie;

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
                    var oldVal = GetPoint(row, col, NumCols);
                    // assuming the removed one is always the one that caused the one
                    // as we don't move any further once either of these terminal
                    // situations is reached
                    IsWin = false;
                    IsTie = false;
                    PossibleMoves.Add(move);
                    Count--;
                }
                else
                {
                    // assuming wasn't in win state
                    SetPoint(row, col, NumCols, value);
                    UpdateIsWin(row, col, value);
                    UpdateIsTie(row, col, PointStates.None, value);
                    PossibleMoves.Remove(move);
                    Count++;
                }
                // this is done at the end, at the moment there should be no issue
           
            }
        }

        public HashSet<Move> PossibleMoves { get; } = new HashSet<Move>();

        private void UpdateIsTie(int row, int col, PointStates oldValue, PointStates newValue)
        {
            //IsTie = Count == NumCols * NumRows;

            // TODO smarter tie
            if (newValue != PointStates.None)
            {
                this.UpdateLivenessForStone(row, col, newValue);
                this.UpdateNearbyForStone(row, col, newValue);
            }
            else
            {
                // this is not used anyway
                this.UpdateLivenessForRemoval(row, col);
                this.UpdateNearbyForRemoval(row, col, oldValue);
            }
        }


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
        
        private void ResetPossibleMoves()
        {
            PossibleMoves.Clear();
            for (var i = 0; i < NumRows; i++)
            {
                for (var j = 0; j < NumCols; j++)
                {
                    PossibleMoves.Add(new Move(i, j));
                }
            }
        }

        public void Reset()
        {
            IsWin = false;
            Count = 0;
            Clear();
            ResetPossibleMoves();
        }
    }
}
