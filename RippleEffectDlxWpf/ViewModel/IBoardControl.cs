using System.Collections.Immutable;
using RippleEffectDlxWpf.Model;

namespace RippleEffectDlxWpf.ViewModel
{
    public interface IBoardControl
    {
        void AddRooms(IImmutableList<Room> rooms);
        void AddInitialValues(IImmutableList<InitialValue> initialValues);
        void AddDigit(Coords coords, int value);
        bool HasDigitAt(Coords coords, int value);
        void RemoveDigitsOtherThan(IImmutableList<InternalRow> internalRows);
        void Reset();
    }
}
