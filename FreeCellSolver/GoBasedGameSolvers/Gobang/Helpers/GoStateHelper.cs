using GoBased;
using System.Text;

namespace GoBasedGameSolvers.Gobang.Helpers
{
    public static class GoStateHelper
    {
        public static GoSnapshot.PointStates DeduceCurrentPlayer(this GoSnapshot state)
        {
            var blacks = 0;
            var whites = 0;
            foreach (var b in state.EncodedBoard)
            {
                var bb = b;
                var bit = bb & 0x3;
                if (bit == 2) whites++;
                else if (bit == 1) blacks++;

                bb >>= 2;
                bit = bb & 0x3;
                if (bit == 2) whites++;
                else if (bit == 1) blacks++;

                bb >>= 2;
                bit = bb & 0x3;
                if (bit == 2) whites++;
                else if (bit == 1) blacks++;

                bb >>= 2;
                bit = bb & 0x3;
                if (bit == 2) whites++;
                else if (bit == 1) blacks++;
            }

            return whites == blacks ? GoSnapshot.PointStates.Black : GoSnapshot.PointStates.White;
        }

        public static GoSnapshot.PointStates GetOpponent(this GoSnapshot.PointStates player)
        {
            return (GoSnapshot.PointStates)(3 - (int)player);
        }

        public static bool GameHasDefaultFirstMove(this GobangState state)
        {
            var dstm = state.DeduceCurrentPlayer();
            return dstm == state.NextPlayer;
        }

        public static void LoadSnapshot(this GobangState state, GoSnapshot snapshot, bool defaultFirstMove)
        {
            for (var i = 0; i < state.EncodedBoard.Length; i++)
            {
                state.EncodedBoard[i] = snapshot.EncodedBoard[i];
            }
            state.NextPlayer = state.DeduceCurrentPlayer();
            if (!defaultFirstMove )
            {
                state.NextPlayer = state.NextPlayer.GetOpponent();
            }
        }

        public static string ConvertToDisplayString(this GobangState state)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < state.NumRows; i++)
            {
                for (var j = 0; j < state.NumCols; j++)
                {
                    var s = state[i, j];
                    switch (s)
                    {
                        case GoSnapshot.PointStates.Black:
                            sb.Append('X');
                            break;
                        case GoSnapshot.PointStates.White:
                            sb.Append('O');
                            break;
                        default:
                            sb.Append(' ');
                            break;
                    }
                    if (j < state.NumCols - 1)
                    {
                        sb.Append('|');
                    }
                }
                if (i < state.NumRows-1)
                {
                    sb.AppendLine();
                    sb.AppendLine(new string('-', state.NumCols * 2 - 1));
                }
            }
            return sb.ToString();
        }
    }
}
