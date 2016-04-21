using JetBrains.Annotations;
using Styx;

namespace ImpMove.Core
{
    public class ImpPoint
    {

        public ImpPoint(string name, WoWPoint point)
        {
            Name = name;
            Point = point;
        }

        [UsedImplicitly]
        public ImpPoint()
        {
            Name = "неизвестно";
            Point = WoWPoint.Empty;
        }

        public string Name { get; set; }

        public WoWPoint Point { get; set; }

        public override string ToString()
        {
            return Name + "  " + Point.ToString();
        }


    }
}
