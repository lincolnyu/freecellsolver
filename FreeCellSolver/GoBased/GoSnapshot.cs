namespace GoBased
{
    public class GoSnapshot
    {
        public enum PointStates
        {
            None = 0,
            Black,
            White
        }

        public readonly byte[] EncodedBoard;

        public GoSnapshot(int totalBytes)
        {
            EncodedBoard = new byte[totalBytes];
        }

        public static int GetTotalBytes(int totalPoints)
        {
            var totalBytes = (totalPoints + 3) / 4;
            return totalBytes;
        }

        public PointStates GetPoint(int row, int col, int numcols)
        {
            var offset = row * numcols + col;
            var bytep = offset / 4;
            var bitp = offset % 4;
            var b = EncodedBoard[bytep];
            b >>= bitp * 2;
            b &= 0x3;
            return (PointStates)b;
        }

        public void SetPoint(int row, int col, int numcols, PointStates val)
        {
            var offset = row * numcols + col;
            var bytep = offset / 4;
            var bitp = offset % 4;
            var b = EncodedBoard[bytep];
            var d = (byte)((byte)val << (bitp * 2));
            var m = (byte)~(0x3 << (bitp * 2));
            b &= m;
            b |= d;
            EncodedBoard[bytep] = b;
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as GoSnapshot;
            if (other == null) return false;
            if (EncodedBoard.Length != other.EncodedBoard.Length) return false;
            for (var i = 0; i < EncodedBoard.Length; i++)
            {
                if (EncodedBoard[i] != other.EncodedBoard[i]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hc = 23;
            for (var i = 0; i < EncodedBoard.Length; i++)
            {
                hc *= 17;
                hc += EncodedBoard[i];
            }
            return hc;
        }

        public GoSnapshot Clone()
        {
            var clone = new GoSnapshot(EncodedBoard.Length);
            for (var i = 0; i < EncodedBoard.Length; i++)
            {
                clone.EncodedBoard[i] = EncodedBoard[i];
            }
            return clone;
        }
    }
}
