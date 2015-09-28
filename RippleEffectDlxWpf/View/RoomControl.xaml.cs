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
        private const double OuterCellLineWidth = 6;
        private const double InnerCellLineWidth = 1;
        private readonly Color _cellColour = Colors.White;
        private readonly Color _outerCellLineColour = Colors.Black;
        private readonly Color _innerCellLineColour = Colors.Black;

        public RoomControl(Room room, int numRows, double sw, double sh)
        {
            _numRows = numRows;
            _sw = sw;
            _sh = sh;

            InitializeComponent();

            var insideEdges = new List<Coords>();
            var outsideEdges = new List<Coords>();

            foreach (var cell in room.Cells)
            {
                var rect = new Rect(cell.Col * _sw, (_numRows - cell.Row - 1) * _sh, _sw, _sh);
                var rectangle = new Rectangle { Width = rect.Width, Height = rect.Height };
                Canvas.SetLeft(rectangle, rect.Left);
                Canvas.SetTop(rectangle, rect.Top);
                rectangle.Fill = new SolidColorBrush(_cellColour);
                RoomCanvas.Children.Add(rectangle);
                DetermineEdges(insideEdges, outsideEdges, room.Cells, cell.Col, cell.Row);
            }

            // TODO: we should probably de-dup insideEdges.
            // e.g. [(1, 1), (1, 0), (1, 0), (1, 1)] => [(1, 1), (1, 0)]
            var insideEdgeLinePoints = CalculateEdgeLinePoints(insideEdges);
            for (var i = 0; i < insideEdgeLinePoints.Count/2; i++)
            {
                var pt1 = insideEdgeLinePoints[i * 2];
                var pt2 = insideEdgeLinePoints[i * 2 + 1];
                var line = new Line
                {
                    X1 = pt1.X,
                    Y1 = pt1.Y,
                    X2 = pt2.X,
                    Y2 = pt2.Y,
                    Stroke = new SolidColorBrush(_innerCellLineColour),
                    StrokeThickness = InnerCellLineWidth
                };
                RoomCanvas.Children.Add(line);
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
                Stroke = new SolidColorBrush(_outerCellLineColour),
                StrokeThickness = OuterCellLineWidth,
                StrokeEndLineCap = PenLineCap.Square,
                Data = pathGeometry
            };
            RoomCanvas.Children.Add(path);
        }

        private enum Side
        {
            Top,
            Bottom,
            Left,
            Right
        };

        private static void DetermineEdges(
            ICollection<Coords> insideEdges,
            ICollection<Coords> outsideEdges,
            IImmutableList<Coords> cells,
            int x,
            int y)
        {
            var topEdge = cells.Max(c => c.Row) + 1;
            var bottomEdge = cells.Min(c => c.Row);
            var leftEdge = cells.Min(c => c.Col);
            var rightEdge = cells.Max(c => c.Col) + 1;

            Func<int, int, bool> roomHasCellAt = (col, row) => cells.Contains(new Coords(row, col));

            foreach (var side in Enum.GetValues(typeof(Side)).Cast<Side>())
            {
                ICollection<Coords> edges;
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
                        edges = isOutsideEdge ? outsideEdges : insideEdges;
                        edges.Add(new Coords(y + 1, x));
                        edges.Add(new Coords(y + 1, x + 1));
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
                        edges = isOutsideEdge ? outsideEdges : insideEdges;
                        edges.Add(new Coords(y, x + 1));
                        edges.Add(new Coords(y, x));
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
                        edges = isOutsideEdge ? outsideEdges : insideEdges;
                        edges.Add(new Coords(y, x));
                        edges.Add(new Coords(y + 1, x));
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
                        edges = isOutsideEdge ? outsideEdges : insideEdges;
                        edges.Add(new Coords(y + 1, x + 1));
                        edges.Add(new Coords(y, x + 1));
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

        private IList<Point> CalculateEdgeLinePoints(IEnumerable<Coords> edges)
        {
            return edges.Select(coords => new Point
            {
                X = coords.Col * _sw,
                Y = (_numRows - coords.Row) * _sh
            }).ToList();
        }
    }
}
