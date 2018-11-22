
namespace Graphviz4Net.Graphs
{
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class RankDirection
    {
        public static RankDirection[] AllDirections =
            new[] {LeftToRight, RightToLeft, TopToBottom, BottomToTop};

        public static RankDirection LeftToRight = new RankDirection("LR");

        public static RankDirection RightToLeft = new RankDirection("RL");

        public static RankDirection TopToBottom = new RankDirection("TB");

        public static RankDirection BottomToTop = new RankDirection("BT");

        private readonly string value;

        private RankDirection(string value)
        {
            this.value = value;
        }

        public static bool operator ==(string str, RankDirection value)
        {
            return str == value.value;
        }

        public static bool operator !=(string str, RankDirection value)
        {
            return (str == value) == false;
        }

        public static RankDirection FromString(string value)
        {
            Contract.Requires(AllDirections.Any(x => x.ToString() == value));
            return AllDirections.First(x => x.ToString() == value);
        }

        public override string ToString()
        {
            return this.value;
        }

        public bool Equals(RankDirection other)
        {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.value, this.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != typeof (RankDirection)) { return false; }
            return Equals((RankDirection) obj);
        }

        public override int GetHashCode()
        {
            return (this.value != null ? this.value.GetHashCode() : 0);
        }
    }
}
