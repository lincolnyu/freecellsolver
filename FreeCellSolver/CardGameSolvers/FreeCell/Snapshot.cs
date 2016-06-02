using CardGames;
using CardGameSolvers.FreeCell.Helpers;
using System;
using System.Collections.Generic;

namespace CardGameSolvers.FreeCell
{
    class Snapshot : IEquatable<Snapshot>
    {
        private int _hashCode;

        public List<List<Card>> Active { get; }
        public List<Card> Shelved { get; }

        public Snapshot(FreeCellState state)
        {
            Active = new List<List<Card>>();
            for (var i = 0; i < state.Active.Length; i++)
            {
                var a = state.Active[i];
                if (a == null) continue;
                var mya = new List<Card>();
                for (var j = 0; j < state.Active[i].Count; j++)
                {
                    mya.Add(state.Active[i][j]);
                }
                mya.Sort(CardHelper.Compare);
                Active.Add(mya);
            }
            Active.Sort(CardHelper.Compare);
            Shelved = new List<Card>();
            for (var i = 0; i < state.Shelved.Length; i++)
            {
                var s = state.Shelved[i];
                if (s != null) Shelved.Add(s);
            }
            Shelved.Sort(CardHelper.Compare);
            UpdateHashCode();
        }

        private void UpdateHashCode()
        {
            var hash = 23;
            for (var i = 0; i < Active.Count; i++)
            {
                for (var j = 0; j < Active[i].Count; j++)
                {
                    hash *= 31;
                    hash += (int)Active[i][j].Number;
                }
            }
            for (var i = 0; i < Shelved.Count; i++)
            {
                if (Shelved[i] == null) continue;
                hash *= 31;
                hash += (int)Shelved[i].Number;
            }
            _hashCode = hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as FreeCellState;
            if (other == null) return false;
            return Equals(other);
        }

        public bool Equals(Snapshot other)
        {
            if (Shelved.Count != other.Shelved.Count) return false;
            for (var i = 0; i < Shelved.Count; i++)
            {
                if (Shelved[i] != other.Shelved[i]) return false;
            }
            if (Active.Count != other.Active.Count) return false;
            for (var i = 0; i < Active.Count; i++)
            {
                if (Active[i].Count != other.Active[i].Count) return false;
            }
            for (var i = 0; i < Active.Count; i++)
            {
                for (var j = Active[i].Count - 1; j >= 0; j--)
                {
                    if (Active[i][j] != other.Active[i][j]) return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
