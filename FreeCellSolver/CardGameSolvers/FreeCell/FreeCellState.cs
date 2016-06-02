using CardGames;
using CardGameSolvers.FreeCell.Helpers;
using CardGameSolvers.FreeCell.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGameSolvers.FreeCell
{
    public class FreeCellState
    {
        public List<Card>[] Active = new List<Card>[8];
        public Card[] Shelved = new Card[4];
        public List<Card>[] Decked = new List<Card>[4];

        public FreeCellState()
        {
            for (var i = 0; i < Active.Length; i++)
            {
                Active[i] = new List<Card>();
            }
            for (var i = 0; i < Decked.Length; i++)
            {
                Decked[i] = new List<Card>();
            }
        }

        public bool IsWon()
        {
            for (var i = 0; i < Decked.Length; i++)
            {
                var top = GetTopDecked(i);
                if (top == null || top.Number != Card.Numbers.King) return false;
            }
            return true;
        }

        public IEnumerable<Instruction> GetPossibleMoves()
        {
            var a = GetSafeDeck();
            if (a != null)
            {
                return new[] { a };
            }

            // decking active
            var r = GetPossibleActiveDeckings();
            // decking shelved
            r = r.Concat(GetPossibleShelvedDeckings());
            // moving
            r = r.Concat(GetPossibleMovings().Cast<Move>().OrderByDescending(x => x.MoveCount));
            // shelving
            r = r.Concat(GetPossibleShelvings());
            // unshelving
            r = r.Concat(GetPossibleUnshelvings());
            return r;
        }

        public IEnumerable<Instruction> PlanPossibleMoves()
        {
            var a = GetSafeDeck();
            if (a != null)
            {
                return new[] { a };
            }

            // decking active
            var r = GetPossibleActiveDeckings();
            // decking shelved
            r = r.Concat(GetPossibleShelvedDeckings());
            // moving
            r = r.Concat(GetPossibleMovings().Cast<Move>().OrderByDescending(x=>x.MoveCount));
            if (Shelved.Count(x=>x!=null) > 2)
            {
                // unshelving then shelving
                r = r.Concat(GetPossibleUnshelvings());
                r = r.Concat(GetPossibleShelvings());
            }
            else
            {
                // shelving then unshelving
                r = r.Concat(GetPossibleShelvings());
                r = r.Concat(GetPossibleUnshelvings());
            }
            return r;
        }

        private Instruction GetSafeDeck()
        {
            for (var i = 0; i < Shelved.Length; i++)
            {
                var s = Shelved[i];
                if (s != null && CanDeck(s) && IsSafeDeck(s))
                {
                    return new DeckShelved(i);
                }
            }
            for (var i = 0; i < Active.Length; i++)
            {
                var top = GetTopActive(i);
                if (top != null && CanDeck(top) && IsSafeDeck(top))
                {
                    return new DeckActive(i);
                }
            }
            return null;
        }

        private bool IsSafeDeck(Card card)
        {
            if (card.IsRed)
            {
                var blacks = CardHelper.BlackSlots;
                var d = GetTopDecked(blacks[0]);
                var dn = d != null ? (int)d.Number : 0;
                if (dn < (int)(card.Number - 1)) return false;
                d = GetTopDecked(blacks[1]);
                dn = d != null ? (int)d.Number : 0;
                if (dn < (int)(card.Number - 1)) return false;
            }
            else
            {
                var reds = CardHelper.RedSlots;
                var d = GetTopDecked(reds[0]);
                var dn = d != null ? (int)d.Number : 0;
                if (dn < (int)(card.Number - 1)) return false;
                d = GetTopDecked(reds[1]);
                dn = d != null ? (int)d.Number : 0;
                if (dn < (int)(card.Number - 1)) return false;
            }
            return true;
        }

        private bool CanDeckActive(int colIdx)
        {
            var col = Active[colIdx];
            var active = col[col.Count-1];
            return CanDeck(active);
        }

        private bool CanDeck(Card card)
        {
            var deckIdx = card.GetDeckIndex();
            var decked = GetTopDecked(deckIdx);
            return card.IsSuccessor(decked);
        }

        private IEnumerable<Instruction> GetPossibleActiveDeckings()
        {
            for (var i = 0; i < Active.Length; i++)
            {
                var top = GetTopActive(i);
                if (top != null && CanDeck(top))
                {
                    yield return new DeckActive(i);
                }
            }
        }

        private IEnumerable<Instruction> GetPossibleShelvedDeckings()
        {
            for (var i = 0; i < Shelved.Length; i++)
            {
                if (Shelved[i] != null && CanDeck(Shelved[i]))
                {
                    yield return new DeckShelved(i);
                }
            }
        }

        private int GetFreeCells()
        {
            var s = Shelved.Count(x => x == null);
            s += Active.Count(x => x.Count == 0);
            return s + 1;
        }

        private IEnumerable<Instruction> GetPossibleMovings()
        {
            var maxMove = GetFreeCells();
            for (var i = 0; i < Active.Length - 1; i++)
            {
                var coli = Active[i];
                var topi = coli.Count > 0 ? coli[coli.Count - 1] : null;
                for (var j = i + 1; j < Active.Length; j++)
                {
                    var colj = Active[j];
                    var topj = colj.Count > 0 ? colj[colj.Count - 1] : null;
                    Card upper = null;

                    for (var k = coli.Count - 1; k >= 0; k--)
                    {
                        var move = coli.Count - k;
                        if (move > maxMove) break;
                        var curr = coli[k];
                        if (!curr.IsLowerOrFirst(upper)) break;
                        if (curr.IsLowerOrFirst(topj))
                        {
                            yield return new Move(i, move, j);
                        }
                        upper = curr;
                    }
                    upper = null;
                    for (var k = colj.Count - 1; k >= 0; k--)
                    {
                        var move = colj.Count - k;
                        if (move > maxMove) break;
                        var curr = colj[k];
                        if (!curr.IsLowerOrFirst(upper)) break;
                        if (curr.IsLowerOrFirst(topi))
                        {
                            yield return new Move(j, move, i);
                        }
                        upper = curr;
                    }
                }
            }
        }

        private IEnumerable<Instruction> GetPossibleShelvings()
        {
            int nextShelve = NextAvailableShelveCell();
            if (nextShelve < 0)
            {
                yield break;
            }
            for (var i = 0; i < Active.Length; i++)
            {
                var col = Active[i];
                if (col.Count == 0) continue;
                yield return new Shelve(i, nextShelve);
            }
        }

        private IEnumerable<Instruction> GetPossibleUnshelvings()
        {
            for (var i = 0; i < Shelved.Length; i++)
            {
                var s = Shelved[i];
                if (s == null) continue;
                for (var j = 0; j < Active.Length; j++)
                {
                    var top = GetTopActive(j);
                    if (s.IsLowerOrFirst(top))
                    {
                        yield return new Unshelve(j, i);
                    }
                }
            }
        }

        private int NextAvailableShelveCell()
        {
            for (var i = 0; i < Shelved.Length; i++)
            {
                if (Shelved[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private Card GetTopActive(int colIdx)
        {
            var col = Active[colIdx];
            var topIdx = col.Count - 1;
            if (topIdx < 0) return null;
            var top = col[topIdx];
            return top;
        }

        private Card PopActive(int colIdx)
        {
            var col = Active[colIdx];
            var topIdx = col.Count - 1;
            var top = col[topIdx];
            col.RemoveAt(topIdx);
            return top;
        }

        private void PushActive(int colIdx, Card card)
        {
            var col = Active[colIdx];
            col.Add(card);
        }

        public Card GetTopDecked(int deckIdx)
        {
            var d = Decked[deckIdx];
            var topIdx = d.Count - 1;
            if (topIdx < 0) return null;
            var top = d[topIdx];
            return top;
        }

        private Card PopDeck(int deckIdx)
        {
            var deck = Decked[deckIdx];
            var topIdx = deck.Count - 1;
            var top = deck[topIdx];
            deck.RemoveAt(topIdx);
            return top;
        }

        public int PushDeck(Card card)
        {
            var deckIdx = card.GetDeckIndex();
            var deck = Decked[deckIdx];
            deck.Add(card);
            return deckIdx;
        }

        public void Shelve(int colIdx, int shelveIdx)
        {
            Shelved[shelveIdx] = PopActive(colIdx);
        }

        public void Unshelve(int colIdx, int shelveIdx)
        {
            var unshelve = Shelved[shelveIdx];
            PushActive(colIdx, unshelve);
            Shelved[shelveIdx] = null;
        }

        public void Move(int srcColIdx, int tgtColIdx, int moveCount)
        {
            var srcCol = Active[srcColIdx];
            var tgtCol = Active[tgtColIdx];
            var start = srcCol.Count - moveCount;
            for (var i = start; i < srcCol.Count; i++)
            {
                tgtCol.Add(srcCol[i]);
            }
            srcCol.RemoveRange(start, moveCount);
        }

        public int DeckActive(int colIdx)
        {
            var card = PopActive(colIdx);
            var deckIdx = PushDeck(card);
            return deckIdx;
        }

        public int DeckShelved(int shelveIdx)
        {
            var card = Shelved[shelveIdx];
            var deckIdx = PushDeck(card);
            Shelved[shelveIdx] = null;
            return deckIdx;
        }

        public void UndeckActive(int colIdx, int deckIdx)
        {
            var deck = PopDeck(deckIdx);
            PushActive(colIdx, deck);
        }

        public void UndeckShelved(int shelveIdx, int deckIdx)
        {
            var deck = PopDeck(deckIdx);
            Shelved[shelveIdx] = deck;
        }
    }
}
