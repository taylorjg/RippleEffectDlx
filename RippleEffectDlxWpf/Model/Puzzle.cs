using System.Collections.Immutable;

namespace RippleEffectDlxWpf.Model
{
    public class Puzzle
    {
        public Puzzle(IImmutableList<Room> rooms, IImmutableList<InitialValue> initialValues)
        {
            Rooms = rooms;
            InitialValues = initialValues;
        }

        public IImmutableList<Room> Rooms { get; }
        public IImmutableList<InitialValue> InitialValues { get; }
    }
}
