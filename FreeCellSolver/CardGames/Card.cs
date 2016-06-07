using System;

namespace CardGames
{
    public class Card : IEquatable<Card>
    {
        public enum Suits
        {
            JokerMajor = -2,
            JokerMinor = -1,
            Spade = 0,
            Heart = 1,
            Diamond = 2,
            Club = 3,
        }

        public enum Numbers
        {
            None = 0,
            Ace = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13
        }

        public Suits Suit { get; set; }

        public Numbers Number { get; set; }

        public bool IsRed => Suit == Suits.Heart || Suit == Suits.Diamond;

        public bool IsBlack => Suit == Suits.Spade || Suit == Suits.Club;

        public bool Equals(Card other)
        {
            return Suit == other.Suit && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Card;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)Suit * 13 + (int)Number;
        }
    }
}
