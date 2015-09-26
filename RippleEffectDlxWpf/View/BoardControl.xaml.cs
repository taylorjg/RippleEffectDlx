using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RippleEffectDlxWpf.Model;
using RippleEffectDlxWpf.ViewModel;

namespace RippleEffectDlxWpf.View
{
    public partial class BoardControl : IBoardControl
    {
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;
        private double _sw;
        private double _sh;
        private int _numRows;

        private enum TagType
        {
            Room,
            InitialValue,
            Digit
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        public void DrawGrid(int numRows, int numCols)
        {
            _numRows = numRows;
            _sw = (ActualWidth - GridLineThickness) / numCols;
            _sh = (ActualHeight - GridLineThickness) / numRows;
        }

        public void DrawRooms(IImmutableList<Room> rooms)
        {
            foreach (var room in rooms)
                DrawRoom(room);
        }

        private void DrawRoom(Room room)
        {
            var roomControl = new RoomControl(room, _numRows, _sw, _sh) {Tag = TagType.Room};
            Canvas.SetLeft(roomControl, GridLineHalfThickness);
            Canvas.SetTop(roomControl, GridLineHalfThickness);
            BoardCanvas.Children.Add(roomControl);
        }

        public void DrawInitialValues(IImmutableList<InitialValue> initialValues)
        {
            // Draw a slightly greyed out / semi-transparent number in the middle of the cell
        }

        public void DrawDigit(Coords coords, int value)
        {
            // Draw a black number in the middle of the cell
        }

        public void Reset()
        {
            RemoveChildrenWithTagType(
                TagType.Room,
                TagType.InitialValue,
                TagType.Digit);
        }

        private void RemoveChildrenWithTagType(params TagType[] tagTypes)
        {
            BoardCanvas.Children
                .OfType<FrameworkElement>()
                .Where(fe => tagTypes.Contains((TagType)fe.Tag))
                .ToList()
                .ForEach(fe => BoardCanvas.Children.Remove(fe));
        }
    }
}
