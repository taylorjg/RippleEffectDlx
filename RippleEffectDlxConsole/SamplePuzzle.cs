using System.Collections.Immutable;

namespace RippleEffectDlxConsole
{
    public class SamplePuzzle
    {
        public SamplePuzzle(IImmutableList<Room> rooms, IImmutableList<InitialValue> initialValues)
        {
            Rooms = rooms;
            InitialValues = initialValues;
        }

        public IImmutableList<Room> Rooms { get; }
        public IImmutableList<InitialValue> InitialValues { get; }
    }
}
