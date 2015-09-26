using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RippleEffectDlxWpf.Model;

namespace RippleEffectDlxWpf.View
{
    public partial class RoomControl
    {
        private readonly int _numRows;
        private readonly double _sw;
        private readonly double _sh;
        private const double BorderWidth = 4; // half of this width will be clipped away
        private readonly Color _cellColour = Colors.White;
        private readonly Color _roomBorderColour = Colors.Black;

        public RoomControl(Room room, int numRows, double sw, double sh)
        {
            _numRows = numRows;
            _sw = sw;
            _sh = sh;
            InitializeComponent();
            var clipGeometryGroup = new GeometryGroup();
            var outsideEdges = new List<Coords>();

            foreach (var cell in room.Cells)
            {
                var rect = new Rect(cell.Col * _sw, (_numRows - cell.Row - 1) * _sh, _sw, _sh);
                var rectangle = new Rectangle { Width = rect.Width, Height = rect.Height };
                Canvas.SetLeft(rectangle, rect.Left);
                Canvas.SetTop(rectangle, rect.Top);
                rectangle.Fill = new SolidColorBrush(_cellColour);
                RoomCanvas.Children.Add(rectangle);

                var clipRectangleGeometry = new RectangleGeometry(rect);
                clipGeometryGroup.Children.Add(clipRectangleGeometry);
                DetermineOutsideEdges(outsideEdges, room.Cells, cell.Col, cell.Row);
            }

            var combinedOutsideEdges = CombineOutsideEdges(outsideEdges);
            var outsideEdgeLinePoints = CalculateEdgeLinePoints(combinedOutsideEdges);

            var polyLineSegment = new PolyLineSegment(outsideEdgeLinePoints, true);
            var pathFigure = new PathFigure { StartPoint = outsideEdgeLinePoints.First() };
            pathFigure.Segments.Add(polyLineSegment);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            var path = new Path
            {
                Stroke = new SolidColorBrush(_roomBorderColour),
                StrokeThickness = BorderWidth,
                StrokeEndLineCap = PenLineCap.Square,
                Data = pathGeometry
            };
            RoomCanvas.Children.Add(path);
            RoomCanvas.Clip = clipGeometryGroup;
        }

        private enum Side
        {
            Top,
            Bottom,
            Left,
            Right
        };

        private void DetermineOutsideEdges(ICollection<Coords> outsideEdges, IImmutableList<Coords> cells, int x, int y)
        {
            var topEdge = cells.Max(c => c.Row) + 1;
            var bottomEdge = cells.Min(c => c.Row);
            var leftEdge = cells.Min(c => c.Col);
            var rightEdge = cells.Max(c => c.Col) + 1;

            Func<int, int, bool> roomHasCellAt = (col, row) => cells.Contains(new Coords(row, col));

            foreach (var side in Enum.GetValues(typeof(Side)).Cast<Side>())
            {
                var isOutsideEdge = false;

                switch (side)
                {
                    case Side.Top:
                        if (y + 1 >= topEdge)
                        {
                            isOutsideEdge = true;
                        }
                        else
                        {
                            if (!roomHasCellAt(x, y + 1))
                            {
                                isOutsideEdge = true;
                            }
                        }
                        if (isOutsideEdge)
                        {
                            outsideEdges.Add(new Coords(y + 1, x));
                            outsideEdges.Add(new Coords(y + 1, x + 1));
                        }
                        break;

                    case Side.Right:
                        if (x + 1 >= rightEdge)
                        {
                            isOutsideEdge = true;
                        }
                        else
                        {
                            if (!roomHasCellAt(x + 1, y))
                            {
                                isOutsideEdge = true;
                            }
                        }
                        if (isOutsideEdge)
                        {
                            outsideEdges.Add(new Coords(y + 1, x + 1));
                            outsideEdges.Add(new Coords(y, x + 1));
                        }
                        break;

                    case Side.Bottom:
                        if (y <= bottomEdge)
                        {
                            isOutsideEdge = true;
                        }
                        else
                        {
                            if (!roomHasCellAt(x, y - 1))
                            {
                                isOutsideEdge = true;
                            }
                        }
                        if (isOutsideEdge)
                        {
                            outsideEdges.Add(new Coords(y, x + 1));
                            outsideEdges.Add(new Coords(y, x));
                        }
                        break;

                    case Side.Left:
                        if (x <= leftEdge)
                        {
                            isOutsideEdge = true;
                        }
                        else
                        {
                            if (!roomHasCellAt(x - 1, y))
                            {
                                isOutsideEdge = true;
                            }
                        }
                        if (isOutsideEdge)
                        {
                            outsideEdges.Add(new Coords(y, x));
                            outsideEdges.Add(new Coords(y + 1, x));
                        }
                        break;
                }
            }
        }

        private static IEnumerable<Coords> CombineOutsideEdges(IList<Coords> outsideEdges)
        {
            var combinedOutsideEdges = new List<Coords>();

            var firstLineStartCoords = outsideEdges[0];
            var firstLineEndCoords = outsideEdges[1];

            combinedOutsideEdges.Add(firstLineStartCoords);

            var currentLineEndCoords = firstLineEndCoords;

            for (;;)
            {
                Coords nextLineStartCoords;
                Coords nextLineEndCoords;

                FindNextLine(outsideEdges, currentLineEndCoords, out nextLineStartCoords, out nextLineEndCoords);

                combinedOutsideEdges.Add(nextLineStartCoords);
                currentLineEndCoords = nextLineEndCoords;

                if (nextLineEndCoords.Equals(firstLineStartCoords))
                {
                    break;
                }
            }

            combinedOutsideEdges.Add(firstLineStartCoords);

            return combinedOutsideEdges;
        }

        private static void FindNextLine(IList<Coords> outsideEdges, Coords currentLineEndCoords, out Coords nextLineStartCoords, out Coords nextLineEndCoords)
        {
            var numLines = outsideEdges.Count / 2;

            for (var i = 0; i < numLines; i++)
            {
                var pt1 = outsideEdges[i * 2];
                var pt2 = outsideEdges[i * 2 + 1];

                if (pt1.Equals(currentLineEndCoords))
                {
                    nextLineStartCoords = pt1;
                    nextLineEndCoords = pt2;
                    return;
                }
            }

            throw new InvalidOperationException("FindNextLine failed to find the next line!");
        }

        private IList<Point> CalculateEdgeLinePoints(IEnumerable<Coords> combinedOutsideEdges)
        {
            return combinedOutsideEdges.Select(coords => new Point
            {
                X = coords.Col * _sw,
                Y = (_numRows - coords.Row) * _sh
            }).ToList();
        }
    }
}
