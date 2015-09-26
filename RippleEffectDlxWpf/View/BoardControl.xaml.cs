using System.Windows.Media;
using System.Windows.Shapes;

namespace RippleEffectDlxWpf.View
{
    public partial class BoardControl
    {
        private readonly Color _gridColour = Color.FromArgb(0x80, 0xCD, 0x85, 0x3F);
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;
        private double _sw;
        private double _sh;

        private enum TagType
        {
            GridLine,
            RoomBorder,
            Digit
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        public void DrawGrid(int numRows, int numCols)
        {
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
    }
}
