namespace GoBased.Helpers
{
    public static class SnapshotHelper
    {
        public static GoSnapshot FlipHori(this GoSnapshot ss, int numrows, int numcols)
        {
            var rss = new GoSnapshot(ss.TotalBytes);
            for (var i = 0; i < numrows; i++)
            {
                var j2 = numcols - 1;
                for (var j = 0; j < numcols; j++, j2--)
                {
                    var v = ss.GetPoint(i, j2, numcols);
                    rss.SetPoint(i, j, numcols, v);
                }
            }
            return rss;
        }
        public static GoSnapshot FlipVert(this GoSnapshot ss, int numrows, int numcols)
        {
            var rss = new GoSnapshot(ss.TotalBytes);
            var i2 = numrows - 1;
            for (var i = 0; i < numrows; i++)
            {
                for (var j = 0; j < numcols; j++)
                {
                    var v = ss.GetPoint(i2, j, numcols);
                    rss.SetPoint(i, j, numcols, v);
                }
            }
            return rss;
        }
        public static GoSnapshot Transpose(this GoSnapshot ss, int numrowcols)
        {
            var rss = new GoSnapshot(ss.TotalBytes);
            for (var i = 0; i < numrowcols; i++)
            {
                for (var j = 0; j < numrowcols; j++)
                {
                    var v = ss.GetPoint(j, i, numrowcols);
                    rss.SetPoint(i, j, numrowcols, v);
                }
            }
            return rss;
        }

    }
}
