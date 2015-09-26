using System.Collections.Immutable;
using RippleEffectDlxWpf.Model;

namespace RippleEffectDlxWpf.ViewModel
{
    public interface IBoardControl
    {
        void DrawRooms(IImmutableList<Room> rooms);
        void DrawInitialValues(IImmutableList<InitialValue> initialValues);
        void DrawDigit(Coords coords, int value);
        void Reset();
    }
}
