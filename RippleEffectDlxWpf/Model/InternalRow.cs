using System;

namespace RippleEffectDlxWpf.Model
{
    public class InternalRow : Tuple<Coords, int, int, bool>
    {
        public InternalRow(Coords coords, int value, int roomIndex, bool isInitialValue)
            : base(coords, value, roomIndex, isInitialValue)
        {
        }
    }
}
