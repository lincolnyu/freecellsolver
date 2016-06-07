using CardGames;
using System.Collections.Generic;

namespace CardGameSolvers.FreeCell.Helpers
{
    public static class CardHelper
    {
        public enum CardColors
        {
            Red,
            Black
        }

        public static int[] RedSlots =
        {
            (int)Card.Suits.Heart,
            (int)Card.Suits.Diamond
        };

        public static int[] BlackSlots =
        {
            (int)Card.Suits.Spade,
            (int)Card.Suits.Club
        };

        public static int GetDeckIndex(this Card card)
        {
            return (int)card.Suit;
        }

        public static CardColors GetColor(this Card card)
        {
            return card.IsRed ? CardColors.Red : CardColors.Black;
        }

        public static bool IsLowerOrFirst(this Card lower, Card upper)
        {
            if (upper == null) return true;
            if (lower.GetColor() == upper.GetColor()) return false;
            return upper.IsSuccessor(lower);
        }
        
        public static int Compare(Card a, Card b)
        {
            var c = a.Suit.CompareTo(b.Suit);
            if (c != 0)
            {
                return c;
            }
            return a.Number.CompareTo(b.Number);
        }

        public static int Compare(List<Card> a, List<Card> b)
        {
            var i = 0;
            var countComp = a.Count.CompareTo(b.Count);
            var minCount = countComp < 0 ? a.Count : b.Count;
            for (; i < minCount; i++)
            {
                var c = Compare(a[i], b[i]);
                if (c != 0) return c;
            }
            return countComp;
        }

        /// <summary>
        ///  Check if <paramref name="card"/> is the successor of <paramref name="other"/>. Note it doesn't check if the card types match
        /// </summary>
        /// <param name="card">The card to check if is the successor of the second</param>
        /// <param name="other">The card to check against</param>
        /// <returns>true if it's successor</returns>
        public static bool IsSuccessor(this Card card, Card other)
        {
            return card.Number == Card.Numbers.Ace && other == null
                || other != null && (int)card.Number == (int)other.Number + 1;
        }

        public static Card GetPredecessor(this Card card)
        {
            if (card.Number == Card.Numbers.Ace)
            {
                return null;
            }
            else
            {
                var p = new Card
                {
                    Suit = card.Suit,
                    Number = (Card.Numbers)((int)card.Number - 1)
                };
                return p;
            }
        }

        public static Card GetSuccessor(this Card card)
        {
            if (card.Number == Card.Numbers.King)
            {
                return null;
            }
            else
            {
                var p = new Card
                {
                    Suit = card.Suit,
                    Number = (Card.Numbers)((int)card.Number + 1)
                };
                return p;
            }
        }

        public static void ConvertFromString(this Card card, string str)
        {
            var c = str[0];
            switch (c)
            {
                case '^':
                    card.Suit = Card.Suits.Diamond;
                    break;
                case '@':
                    card.Suit = Card.Suits.Heart;
                    break;
                case '*':
                    card.Suit = Card.Suits.Club;
                    break;
                case '$':
                    card.Suit = Card.Suits.Spade;
                    break;
            }
            var s = str.Substring(1).Trim().ToUpper();
            int i;
            if (int.TryParse(s, out i))
            {
                card.Number = (Card.Numbers)i;
            }
            else
            {
                switch (s)
                {
                    case "A":
                        card.Number = Card.Numbers.Ace;
                        break;
                    case "J":
                        card.Number = Card.Numbers.Jack;
                        break;
                    case "Q":
                        card.Number = Card.Numbers.Queen;
                        break;
                    case "K":
                        card.Number = Card.Numbers.King;
                        break;
                }
            }
        }

        public static string ConvertToString(this Card card)
        {
            var res = "";
            switch (card.Suit)
            {
                case Card.Suits.Diamond:
                    res = "^";
                    break;
                case Card.Suits.Heart:
                    res = "@";
                    break;
                case Card.Suits.Club:
                    res = "*";
                    break;
                case Card.Suits.Spade:
                    res = "$";
                    break;
            }
            switch (card.Number)
            {
                case Card.Numbers.Ace:
                    res += " A";
                    break;
                case Card.Numbers.Jack:
                    res += " J";
                    break;
                case Card.Numbers.Queen:
                    res += " Q";
                    break;
                case Card.Numbers.King:
                    res += " K";
                    break;
                default:
                    res += string.Format("{0,2}", (int)card.Number);
                    break;
            }
            return res;
        }
    }
}
