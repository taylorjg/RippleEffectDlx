using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RippleEffectDlxWpf.Model;
using RippleEffectDlxWpf.ViewModel;

namespace RippleEffectDlxWpf.View
{
    public partial class BoardControl : IBoardControl
    {
        private readonly Color _gridColour = Color.FromArgb(0x80, 0xCD, 0x85, 0x3F);
        private readonly Color _cellColour = Colors.White;
        private readonly Color _roomBorderColour = Colors.Black;
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;
        private const double BorderWidth = 8; // half of this width will be clipped away
        private double _sw;
        private double _sh;
        private int _numRows;

        private enum TagType
        {
            GridLine,
            Cell,
            RoomBorder,
            InitialValue,
            Digit
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        public void DrawRooms(IImmutableList<Room> rooms)
        {
            foreach (var room in rooms)
                DrawRoom(room);
        }

        private void DrawRoom(Room room)
        {
            // Draw a white square for each cell
            foreach (var cell in room.Cells)
            {
                var rect = new Rect(cell.Col * _sw + GridLineHalfThickness, (_numRows - cell.Row - 1) * _sh + GridLineHalfThickness, _sw, _sh);
                var rectangle = new Rectangle { Width = rect.Width, Height = rect.Height };
                Canvas.SetLeft(rectangle, rect.Left);
                Canvas.SetTop(rectangle, rect.Top);
                rectangle.Fill = new SolidColorBrush(_cellColour);
                rectangle.Tag = TagType.RoomBorder;
                BoardCanvas.Children.Add(rectangle);
            }

            // Draw a black border around the room
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
                TagType.Cell,
                TagType.RoomBorder,
                TagType.InitialValue,
                TagType.Digit);
        }

        public void DrawGrid(int numRows, int numCols)
        {
            _numRows = numRows;
            DrawGridLines(numRows, numCols);
        }

        private void DrawGridLines(int numRows, int numCols)
        {
            _sw = (ActualWidth - GridLineThickness) / numCols;
            _sh = (ActualHeight - GridLineThickness) / numRows;

            var gridLineBrush = new SolidColorBrush(_gridColour);

            // Horizontal grid lines
            for (var row = 0; row <= numRows; row++)
            {
                var line = new Line
                {
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    X1 = 0,
                    Y1 = row * _sh + GridLineHalfThickness,
                    X2 = ActualWidth,
                    Y2 = row * _sw + GridLineHalfThickness,
                    Tag = TagType.GridLine
                };
                BoardCanvas.Children.Add(line);
            }

            // Vertical grid lines
            for (var col = 0; col <= numCols; col++)
            {
                var line = new Line
                {
                    Stroke = gridLineBrush,
                    StrokeThickness = GridLineThickness,
                    X1 = col * _sw + GridLineHalfThickness,
                    Y1 = 0,
                    X2 = col * _sh + GridLineHalfThickness,
                    Y2 = ActualHeight,
                    Tag = TagType.GridLine
                };
                BoardCanvas.Children.Add(line);
            }
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
