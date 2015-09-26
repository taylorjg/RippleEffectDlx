using System.Collections.Immutable;

namespace RippleEffectDlxWpf.Model
{
    public class Room
    {
        public Room(params Coords[] cells)
        {
            Cells = ImmutableList.Create(cells);
        }

        public IImmutableList<Coords> Cells { get; }
    }
}
