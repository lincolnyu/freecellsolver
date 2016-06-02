using System;

namespace CardGames
{
    public class Card : IEquatable<Card>
    {
        public enum Types
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

        public Types CardType { get; set; }

        public Numbers Number { get; set; }

        public bool IsRed => CardType == Types.Heart || CardType == Types.Diamond;

        public bool IsBlack => CardType == Types.Spade || CardType == Types.Club;

        public bool Equals(Card other)
        {
            return CardType == other.CardType && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Card;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)CardType * 13 + (int)Number;
        }
    }
}
