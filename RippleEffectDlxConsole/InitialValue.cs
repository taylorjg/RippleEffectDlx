using System;

namespace RippleEffectDlxConsole
{
    public class InitialValue : Tuple<Coords, int>
    {
        public InitialValue(Coords coords, int value) : base(coords, value)
        {
        }
    }
}
