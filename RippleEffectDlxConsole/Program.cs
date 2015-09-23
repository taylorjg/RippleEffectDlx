using System;
using System.Collections.Generic;
using System.Collections.Immutable;
//using System.Diagnostics;
using System.Linq;
using DlxLib;

namespace RippleEffectDlxConsole
{
    using InternalRow = Tuple<Coords, int, int, bool>;
    using InitialValue = Tuple<Coords, int>;

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

            var firstSolutionGrid = Solve(rooms, initialValues);
            Console.WriteLine();

            if (firstSolutionGrid != null)
            {
                Console.WriteLine("Solution:");
                firstSolutionGrid.Draw();
            }
        }

        private static Grid InitialGrid(IImmutableList<Room> rooms, IEnumerable<InitialValue> initialValues)
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

        private static Grid Solve(IReadOnlyList<Room> rooms, IImmutableList<InitialValue> initialValues)
        {
            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Y) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.X) + 1;
            var maxValue = rooms.Max(r => r.Cells.Count);

            var internalRows1 = rooms.SelectMany(room => BuildInternalRowsForRoom(rooms, initialValues, room));
            var internalRows2 = initialValues.Select(t => BuildInternalRow(rooms, t.Item1, t.Item2, true));
            var internalRows = internalRows1.Concat(internalRows2).ToImmutableList();

            var dlxRows = BuildDlxRows(rooms, numRows, numCols, maxValue, internalRows);

            //DumpRows(numRows, numCols, internalRows, dlxRows);

            var dlx = new Dlx();
            var solutions = dlx.Solve(dlxRows, d => d, r => r, numRows*numCols).ToList();
            Console.WriteLine($"Number of solutions found: {solutions.Count}");
            var firstSolution = solutions.FirstOrDefault();

            if (firstSolution == null) return null;

            var subsetOfInternalRows = firstSolution.RowIndexes.Select(idx => internalRows[idx]).ToImmutableList();
            //var subsetOfDlxRows = solution.RowIndexes.Select(idx => dlxRows[idx]).ToImmutableList();
            //DumpRows(numRows, numCols, subsetOfInternalRows, subsetOfDlxRows);
            var orderedSubsetOfInternalRows = subsetOfInternalRows.OrderBy(t => t.Item1.Y).ThenBy(t => t.Item1.X);
            var rowStrings = Enumerable.Range(0, numRows).Select(row => string.Join("", orderedSubsetOfInternalRows.Skip(row * numCols).Take(numRows).Select(t => t.Item2)));
            var grid = new Grid(rowStrings.ToImmutableList());
            return grid;
        }

        private static IEnumerable<InternalRow> BuildInternalRowsForRoom(IReadOnlyList<Room> rooms, IImmutableList<InitialValue> initialValues, Room room)
        {
            var ivCoords = initialValues.Select(iv => iv.Item1);
            var ivValuesInThisRoom = initialValues.Where(t => room.Cells.Contains(t.Item1)).Select(iv => iv.Item2);

            var cellsRemaining = room.Cells.Except(ivCoords).ToImmutableList();
            var valuesRemaining = Enumerable.Range(1, room.Cells.Count).Except(ivValuesInThisRoom).ToImmutableList();

            return
                from cell in cellsRemaining
                from value in valuesRemaining
                select BuildInternalRow(rooms, cell, value, false);
        }

        private static InternalRow BuildInternalRow(IReadOnlyList<Room> rooms, Coords coords, int value, bool isFixed)
        {
            var roomIndex = GetRoomIndexForCoords(rooms, coords);
            return Tuple.Create(coords, value, roomIndex, isFixed);
        }

        private static int GetRoomIndexForCoords(IReadOnlyList<Room> rooms, Coords coords)
        {
            for (var roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
            {
                var room = rooms[roomIndex];
                if (room.Cells.Contains(coords)) return roomIndex;
            }

            throw new InvalidOperationException($"Failed to find coords {coords} in any room!");
        }

        private static IImmutableList<IImmutableList<int>> BuildDlxRows(IReadOnlyList<Room> rooms, int numRows, int numCols, int maxValue, IEnumerable<InternalRow> internalRows)
        {
            return internalRows
                .Select(internalRow => BuildDlxRow(rooms, numRows, numCols, maxValue, internalRow))
                .ToImmutableList();
        }

        private static IImmutableList<int> BuildDlxRow(
            IReadOnlyList<Room> rooms,
            int numRows,
            int numCols,
            int maxValue,
            InternalRow internalRow)
        {
            Func<Coords, bool> isValidRowCol = coords =>
                coords.X >= 0 && coords.X < numCols &&
                coords.Y >= 0 && coords.Y < numRows;

            var localIsValidRowCol = isValidRowCol;

            var row = internalRow.Item1.Y;
            var col = internalRow.Item1.X;
            var value = internalRow.Item2;
            var roomIndex = internalRow.Item3;

            Func<IEnumerable<int[]>> buildRippleCoordsSecondaryColumns = () =>
            {
                var allRippleSecondaryColumns = Enumerable.Range(0, maxValue * 4).Select(_ => new int[numRows * numCols]).ToList();

                var rippleUpCoords = Enumerable.Range(row, value + 1)
                    .Select(r => new Coords(col, r))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleDownCoords = Enumerable.Range(row - value, value + 1)
                    .Select(r => new Coords(col, r))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleLeftCoords = Enumerable.Range(col - value, value + 1)
                    .Select(c => new Coords(c, row))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleRightCoords = Enumerable.Range(col, value + 1)
                    .Select(c => new Coords(c, row))
                    .Where(localIsValidRowCol)
                    .ToList();

                var baseIndex = (value - 1) * 4;

                var rippleUpSecondaryColumns = allRippleSecondaryColumns[baseIndex];
                rippleUpCoords.ForEach(coords => rippleUpSecondaryColumns[coords.Y * numCols + coords.X] = 1);

                var rippleDownSecondaryColumns = allRippleSecondaryColumns[baseIndex + 1];
                rippleDownCoords.ForEach(coords => rippleDownSecondaryColumns[coords.Y * numCols + coords.X] = 1);

                var rippleLeftSecondaryColumns = allRippleSecondaryColumns[baseIndex + 2];
                rippleLeftCoords.ForEach(coords => rippleLeftSecondaryColumns[coords.Y * numCols + coords.X] = 1);

                var rippleRightSecondaryColumns = allRippleSecondaryColumns[baseIndex + 3];
                rippleRightCoords.ForEach(coords => rippleRightSecondaryColumns[coords.Y * numCols + coords.X] = 1);

                return allRippleSecondaryColumns;
            };

            var primaryColumns = new int[numRows * numCols];
            primaryColumns[row * numCols + col] = 1;

            var rippleCoordsSecondaryColumns = buildRippleCoordsSecondaryColumns();

            var numRooms = rooms.Count;
            var roomsSecondaryColumns = new int[maxValue * numRooms];
            roomsSecondaryColumns[roomIndex * maxValue + value - 1] = 1;

            var allColumns = new[] {primaryColumns}.Concat(rippleCoordsSecondaryColumns).Concat(new[] {roomsSecondaryColumns});

            return allColumns
                .SelectMany(columns => columns)
                .ToImmutableList();
        }

        private static void DumpRows(
            int numRows,
            int numCols,
            IReadOnlyList<InternalRow> internalRows,
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
            InternalRow internalRow,
            IReadOnlyCollection<int> dlxRow)
        {
            Console.WriteLine($"Coords: {internalRow.Item1}; Value: {internalRow.Item2}; DlxRow: {DlxRowToString(numRows, numCols, dlxRow)}");
        }

        private static string DlxRowToString(
            int numRows,
            int numCols,
            IReadOnlyCollection<int> dlxRow)
        {
            //var totalNumBits = dlxRow.Count;
            //var chunkSize = numRows*numCols;
            //Debug.Assert(totalNumBits % chunkSize == 0);
            //var numChunks = totalNumBits/chunkSize;
            //var parts = new List<string>();
            //for (var index = 0; index < numChunks; index++)
            //{
            //    var part = string.Join("", dlxRow.Skip(index * chunkSize).Take(chunkSize).Select(n => Convert.ToString(n)));
            //    parts.Add(part);
            //}
            //return string.Join(" ", parts);
            return string.Empty;
        }
    }
}
