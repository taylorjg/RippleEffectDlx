using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
using DlxLib;

namespace RippleEffectDlxConsole
{
    internal static class Program
    {
        private static void Main()
        {
            //var rooms = ImmutableList.Create(
            //    new Room(
            //        new Coords(0, 0),
            //        new Coords(1, 0)),
            //    new Room(
            //        new Coords(2, 1),
            //        new Coords(2, 0),
            //        new Coords(3, 0),
            //        new Coords(4, 0)),
            //    new Room(
            //        new Coords(5, 0),
            //        new Coords(6, 0),
            //        new Coords(7, 0),
            //        new Coords(7, 1)),
            //    new Room(
            //        new Coords(0, 1),
            //        new Coords(0, 2)),
            //    new Room(
            //        new Coords(1, 1)),
            //    new Room(
            //        new Coords(3, 2)),
            //    new Room(
            //        new Coords(3, 1),
            //        new Coords(4, 1),
            //        new Coords(4, 2),
            //        new Coords(4, 3),
            //        new Coords(3, 3)),
            //    new Room(
            //        new Coords(5, 1),
            //        new Coords(6, 1),
            //        new Coords(6, 2),
            //        new Coords(7, 2)),
            //    new Room(
            //        new Coords(0, 3),
            //        new Coords(1, 3),
            //        new Coords(1, 2),
            //        new Coords(1, 4),
            //        new Coords(2, 4)),
            //    new Room(
            //        new Coords(2, 2),
            //        new Coords(2, 3)),
            //    new Room(
            //        new Coords(3, 4),
            //        new Coords(3, 5),
            //        new Coords(3, 6),
            //        new Coords(4, 5)),
            //    new Room(
            //        new Coords(4, 4),
            //        new Coords(5, 4),
            //        new Coords(5, 3),
            //        new Coords(5, 2),
            //        new Coords(6, 3)),
            //    new Room(
            //        new Coords(6, 4),
            //        new Coords(7, 5),
            //        new Coords(7, 4),
            //        new Coords(7, 3)),
            //    new Room(
            //        new Coords(0, 7)),
            //    new Room(
            //        new Coords(1, 7),
            //        new Coords(1, 6)),
            //    new Room(
            //        new Coords(4, 6),
            //        new Coords(5, 6)),
            //    new Room(
            //        new Coords(5, 5),
            //        new Coords(6, 5)),
            //    new Room(
            //        new Coords(2, 7),
            //        new Coords(3, 7),
            //        new Coords(4, 7)),
            //    new Room(
            //        new Coords(7, 6)),
            //    new Room(
            //        new Coords(0, 4),
            //        new Coords(0, 5),
            //        new Coords(0, 6),
            //        new Coords(1, 5)),
            //    new Room(
            //        new Coords(2, 6),
            //        new Coords(2, 5)),
            //    new Room(
            //        new Coords(5, 7),
            //        new Coords(6, 7),
            //        new Coords(7, 7),
            //        new Coords(6, 6)));

            //var initialValues = ImmutableList.Create(
            //    Tuple.Create(new Coords(4, 0), 2),
            //    Tuple.Create(new Coords(1, 2), 3),
            //    Tuple.Create(new Coords(2, 3), 1),
            //    Tuple.Create(new Coords(4, 3), 5),
            //    Tuple.Create(new Coords(3, 4), 1),
            //    Tuple.Create(new Coords(5, 4), 3),
            //    Tuple.Create(new Coords(6, 5), 2),
            //    Tuple.Create(new Coords(3, 7), 3));

            // http://www.sachsentext.de/gif/ripple_effect1.gif
            var rooms = ImmutableList.Create(
                new Room(
                    new Coords(0, 3),
                    new Coords(0, 2),
                    new Coords(0, 1),
                    new Coords(0, 0),
                    new Coords(1, 0)),
                new Room(
                    new Coords(1, 1),
                    new Coords(2, 1),
                    new Coords(3, 1),
                    new Coords(2, 0)),
                new Room(
                    new Coords(3, 0),
                    new Coords(4, 0),
                    new Coords(4, 1),
                    new Coords(4, 2),
                    new Coords(4, 3)),
                new Room(
                    new Coords(0, 4),
                    new Coords(1, 4),
                    new Coords(1, 3),
                    new Coords(1, 2)),
                new Room(
                    new Coords(3, 2),
                    new Coords(3, 3),
                    new Coords(3, 4),
                    new Coords(4, 4)),
                new Room(
                    new Coords(2, 4),
                    new Coords(2, 3),
                    new Coords(2, 2)));

            var initialValues = ImmutableList.Create(
                Tuple.Create(new Coords(0, 0), 1),
                Tuple.Create(new Coords(4, 0), 4),
                Tuple.Create(new Coords(2, 2), 2),
                Tuple.Create(new Coords(0, 4), 1),
                Tuple.Create(new Coords(4, 4), 1));

            var initialGrid = InitialGrid(rooms, initialValues);
            Console.WriteLine("Puzzle:");
            initialGrid.Draw();

            Console.WriteLine();

            Console.WriteLine("Solution:");
            var solutionGrid = Solve(rooms, initialValues);
            solutionGrid?.Draw();
        }

        private static Grid InitialGrid(IImmutableList<Room> rooms, IEnumerable<Tuple<Coords, int>> initialValues)
        {
            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Y) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.X) + 1;

            var rows = Enumerable.Range(0, numRows).Select(_ => new string(' ', numCols)).ToList();
            foreach (var initialValue in initialValues)
            {
                var row = initialValue.Item1.Y;
                var col = initialValue.Item1.X;
                var value = initialValue.Item2;
                var s = rows[row];
                var cs = s.ToCharArray();
                cs[col] = Convert.ToChar('0' + value);
                rows[row] = new string(cs);
            }

            return new Grid(rows.ToImmutableList());
        }

        private static Grid Solve(IImmutableList<Room> rooms, IImmutableList<Tuple<Coords, int>> initialValues)
        {
            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Y) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.X) + 1;
            var maxValue = rooms.Max(r => r.Cells.Count);

            var internalRows1 = rooms.SelectMany(room => BuildInternalRowsForRoom(initialValues, room));
            var internalRows2 = initialValues.Select(t => BuildInternalRow(t.Item1, t.Item2, true));
            var internalRows = internalRows1.Concat(internalRows2).ToImmutableList();

            var dlxRows = BuildDlxRows(numRows, numCols, maxValue, internalRows);

            DumpRows(numRows, numCols, internalRows, dlxRows);

            var dlx = new Dlx();
            var firstSolution = dlx.Solve(dlxRows, d => d, r => r, numRows * numCols).FirstOrDefault();

            if (firstSolution == null) return null;

            var v1 = firstSolution.RowIndexes.Select(idx => internalRows[idx]);
            var v2 = v1.OrderBy(t => t.Item1.Y).ThenBy(t => t.Item1.X);

            var rowStrings = Enumerable.Range(0, numRows)
                .Select(row => string.Join("", v2.Skip(row * numCols).Take(numRows).Select(t => t.Item2)));

            return new Grid(rowStrings.ToImmutableList());
        }

        private static IEnumerable<Tuple<Coords, int, bool>> BuildInternalRowsForRoom(
            IImmutableList<Tuple<Coords, int>> initialValues,
            Room room)
        {
            var ivCoords = initialValues.Select(iv => iv.Item1);
            var ivValuesInThisRoom = initialValues.Where(t => room.Cells.Contains(t.Item1)).Select(iv => iv.Item2);

            var cellsRemaining = room.Cells.Except(ivCoords).ToImmutableList();
            var valuesRemaining = Enumerable.Range(1, room.Cells.Count).Except(ivValuesInThisRoom).ToImmutableList();

            return
                from cell in cellsRemaining
                from value in valuesRemaining
                select BuildInternalRow(cell, value, false);
        }

        private static Tuple<Coords, int, bool> BuildInternalRow(Coords coords, int value, bool isFixed)
        {
            return Tuple.Create(coords, value, isFixed);
        }

        private static IImmutableList<IImmutableList<int>> BuildDlxRows(
            int numRows,
            int numCols,
            int maxValue,
            IEnumerable<Tuple<Coords, int, bool>> internalRows)
        {
            return internalRows
                .Select(internalRow => BuildDlxRow(numRows, numCols, maxValue, internalRow))
                .ToImmutableList();
        }

        private static IImmutableList<int> BuildDlxRow(
            int numRows,
            int numCols,
            int maxValue,
            Tuple<Coords, int, bool> internalRow)
        {
            Func<Coords, bool> isValidRowCol = coords =>
                coords.X >= 0 && coords.X < numCols &&
                coords.Y >= 0 && coords.Y < numRows;

            var row = internalRow.Item1.Y;
            var col = internalRow.Item1.X;
            var value = internalRow.Item2;

            Func<IEnumerable<int[]>> buildSecondaryColumns = () =>
            {
                var result = Enumerable.Range(0, maxValue).Select(_ => new int[numRows * numCols]).ToList();

                var rippleUpDownCoords = Enumerable.Range(row - value, value * 2 + 1)
                    .Select(r => new Coords(col, r))
                    .Where(isValidRowCol)
                    .ToList();

                var rippleLeftRightCoords = Enumerable.Range(col - value, value * 2 + 1)
                    .Select(c => new Coords(c, row))
                    .Where(isValidRowCol)
                    .ToList();

                var secondaryColumns = result[value - 1];

                rippleUpDownCoords.ForEach(coords => secondaryColumns[coords.Y * numCols + coords.X] = 1);
                rippleLeftRightCoords.ForEach(coords => secondaryColumns[coords.Y * numCols + coords.X] = 1);

                return result;
            };

            var primaryColumns = new int[numRows * numCols];
            primaryColumns[row * numCols + col] = 1;

            var scs = buildSecondaryColumns();

            var allColumns = new[] {primaryColumns}.Concat(scs);

            return allColumns
                .SelectMany(columns => columns)
                .ToImmutableList();
        }

        private static void DumpRows(
            int numRows,
            int numCols,
            IReadOnlyList<Tuple<Coords, int, bool>> internalRows,
            IReadOnlyList<IImmutableList<int>> dlxRows)
        {
            for (var index = 0; index < internalRows.Count; index++)
            {
                var internalRow = internalRows[index];
                var dlxRow = dlxRows[index];
                DumpRow(numRows, numCols, internalRow, dlxRow);
            }
        }

        private static void DumpRow(
            int numRows,
            int numCols,
            Tuple<Coords, int, bool> internalRow,
            IImmutableList<int> dlxRow)
        {
            Console.WriteLine($"Coords: {internalRow.Item1}; Value: {internalRow.Item2}; DlxRow: {DlxRowToString(numRows, numCols, dlxRow)}");
        }

        private static string DlxRowToString(
            int numRows,
            int numCols,
            IImmutableList<int> dlxRow)
        {
            //var part1 = string.Join("", dlxRow.Take(numRows * numCols).Select(n => Convert.ToString(n)));
            //var part2 = string.Join("", dlxRow.Skip(numRows * numCols).Select(n => Convert.ToString(n)));
            //return string.Join(" ", part1, part2);

            var part1 = string.Join("", dlxRow.Take(numRows * numCols).Select(n => Convert.ToString(n)));
            return part1;
        }
    }
}
