using CardGames;
using System.Linq;
using System.Text;

namespace CardGameSolvers.FreeCell.Helpers
{
    public static class StateHelper
    {
        public static string ConvertToDisplayString(this FreeCellState state)
        {
            var sb = new StringBuilder();
            foreach (var s in state.Shelved)
            {
                if (s == null)
                {
                    sb.Append(" [   ]");
                }
                else
                {
                    sb.AppendFormat(" [{0}]", s.ConvertToString());
                }
            }
            for (var i = 0; i < state.Decked.Length; i++)
            {
                var top = state.GetTopDecked(i);
                if (top == null)
                {
                    sb.Append(" <   >");
                }
                else
                {
                    sb.AppendFormat(" <{0}>", top.ConvertToString());
                }
            }
            sb.AppendLine();
            var max = state.Active.Max(x => x.Count);
            for (var j = 0; j < max; j++)
            {
                for (var i = 0; i < state.Active.Length; i++)
                {
                    var col = state.Active[i];
                    if (j < col.Count)
                    {
                        var c = col[j];
                        sb.AppendFormat("  {0} ", c.ConvertToString());
                    }
                    else
                    {
                        sb.Append("      ");
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static void ConvertFromString(this FreeCellState state, string stateString)
        {
            var scsplit = stateString.Split(';');
            var shelve = scsplit[0];
            var deck = scsplit[1];
            var col = 0;
            var shelvesplit = shelve.Split(',');
            foreach (var s in shelvesplit)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var c = new Card();
                    c.ConvertFromString(s);
                    state.Shelved[col] = c;
                }
                else
                {
                    state.Shelved[col] = null;
                }
                col++;
            }

            var decksplit = deck.Split(',');
            foreach (var s in decksplit)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var c = new Card();
                    c.ConvertFromString(s);
                    var cb = new Card();
                    cb.CardType = c.CardType;
                    do
                    {
                        state.PushDeck(cb);
                        cb.Number = Card.Numbers.Ace;
                        cb = cb.GetSuccessor();
                    } while (cb != c);
                    state.PushDeck(c);
                }
            }
            for (var i = 2; i < scsplit.Length; i++)
            {
                var active = scsplit[i];
                var activesplit = active.Split(',');
                foreach (var s in activesplit)
                {
                    var c = new Card();
                    c.ConvertFromString(s);
                    state.Active[i - 2].Add(c);
                }
            }
        }

        public static bool CheckSanity(this FreeCellState state)
        {
            var shelvedCount = state.Shelved.Count(x => x != null);
            var activeCount = state.Active.Sum(x => x.Count);
            var deckedCount = state.Decked.Sum(x => x.Count);
            if (shelvedCount + activeCount + deckedCount != 52)
            {
                return false;
            }
            for (var i = 0; i< state.Decked.Length; i++)
            {
                var d = state.Decked[i];
                for (var j = 0;j < d.Count; j++)
                {
                    var c = d[j];
                    var index = c.GetDeckIndex();
                    if (index != i)
                        return false;
                    if ((int)c.Number != j + 1)
                        return false;
                }
            }
            return true;
        }
    }
}
