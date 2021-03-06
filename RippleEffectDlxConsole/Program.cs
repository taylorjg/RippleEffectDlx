﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DlxLib;

namespace RippleEffectDlxConsole
{
    internal static class Program
    {
        private static void Main()
        {
            var samplePuzzle = SamplePuzzles.SamplePuzzle1;

            var initialGrid = SamplePuzzleToGrid(samplePuzzle);
            Console.WriteLine("Puzzle:");
            initialGrid.Draw();

            Console.WriteLine();

            var firstSolutionGrid = Solve(samplePuzzle);
            Console.WriteLine();

            if (firstSolutionGrid != null)
            {
                Console.WriteLine("Solution:");
                firstSolutionGrid.Draw();
            }
        }

        private static Grid SamplePuzzleToGrid(SamplePuzzle samplePuzzle)
        {
            var rooms = samplePuzzle.Rooms;
            var initialValues = samplePuzzle.InitialValues;

            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Row) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.Col) + 1;

            var rows = Enumerable.Range(0, numRows).Select(_ => new string(' ', numCols)).ToList();
            foreach (var initialValue in initialValues)
            {
                var row = initialValue.Item1.Row;
                var col = initialValue.Item1.Col;
                var value = initialValue.Item2;
                var s = rows[row];
                var cs = s.ToCharArray();
                cs[col] = Convert.ToChar('0' + value);
                rows[row] = new string(cs);
            }

            return new Grid(rows.ToImmutableList());
        }

        private static Grid Solve(SamplePuzzle samplePuzzle)
        {
            var rooms = samplePuzzle.Rooms;
            var initialValues = samplePuzzle.InitialValues;

            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Row) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.Col) + 1;
            var maxValue = rooms.Max(r => r.Cells.Count);

            var internalRows1 = rooms.SelectMany(room => BuildInternalRowsForRoom(rooms, initialValues, room));
            var internalRows2 = initialValues.Select(t => BuildInternalRow(rooms, t.Item1, t.Item2, true));
            var internalRows = internalRows1.Concat(internalRows2).ToImmutableList();

            var dlxRows = BuildDlxRows(rooms, numRows, numCols, maxValue, internalRows);

            var numRowColPrimaryColumns = numRows * numCols;
            var numCellWithinRoomPrimaryColumns = rooms.Sum(r => r.Cells.Count);
            var numPrimaryColumns = numRowColPrimaryColumns + numCellWithinRoomPrimaryColumns;

            var dlx = new Dlx();
            var solutions = dlx.Solve(dlxRows, d => d, r => r, numPrimaryColumns).ToList();
            Console.WriteLine($"Number of solutions found: {solutions.Count}");
            var firstSolution = solutions.FirstOrDefault();

            if (firstSolution == null) return null;

            var subsetOfInternalRows = firstSolution.RowIndexes.Select(idx => internalRows[idx]).ToImmutableList();
            var orderedSubsetOfInternalRows = subsetOfInternalRows.OrderBy(t => t.Item1.Row).ThenBy(t => t.Item1.Col);
            var rowStrings = Enumerable.Range(0, numRows).Select(row => string.Join("", orderedSubsetOfInternalRows.Skip(row * numCols).Take(numRows).Select(t => t.Item2)));
            var grid = new Grid(rowStrings.ToImmutableList());
            return grid;
        }

        private static IEnumerable<InternalRow> BuildInternalRowsForRoom(IReadOnlyList<Room> rooms, IImmutableList<InitialValue> initialValues, Room room)
        {
            var cellsWithInitialValues = initialValues.Select(initialValue => initialValue.Item1);
            var initialValuesInThisRoom = initialValues.Where(initialValue => room.Cells.Contains(initialValue.Item1)).Select(initialValue => initialValue.Item2);

            var cellsRemaining = room.Cells.Except(cellsWithInitialValues).ToImmutableList();
            var valuesRemaining = Enumerable.Range(1, room.Cells.Count).Except(initialValuesInThisRoom).ToImmutableList();

            return
                from cell in cellsRemaining
                from value in valuesRemaining
                select BuildInternalRow(rooms, cell, value, false);
        }

        private static InternalRow BuildInternalRow(IReadOnlyList<Room> rooms, Coords coords, int value, bool isInitialValue)
        {
            var roomIndex = GetRoomIndexForCoords(rooms, coords);
            return new InternalRow(coords, value, roomIndex, isInitialValue);
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
            IReadOnlyCollection<Room> rooms,
            int numRows,
            int numCols,
            int maxValue,
            InternalRow internalRow)
        {
            Func<Coords, bool> isValidRowCol = coords =>
                coords.Col >= 0 && coords.Col < numCols &&
                coords.Row >= 0 && coords.Row < numRows;

            var localIsValidRowCol = isValidRowCol;

            var row = internalRow.Item1.Row;
            var col = internalRow.Item1.Col;
            var value = internalRow.Item2;
            var roomIndex = internalRow.Item3;

            Func<IEnumerable<int[]>> buildSecondaryColumns = () =>
            {
                var allRippleSecondaryColumns = Enumerable.Range(0, maxValue * 4).Select(_ => new int[numRows * numCols]).ToList();

                var rippleUpCoords = Enumerable.Range(row, value + 1)
                    .Select(r => new Coords(r, col))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleDownCoords = Enumerable.Range(row - value, value + 1)
                    .Select(r => new Coords(r, col))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleLeftCoords = Enumerable.Range(col - value, value + 1)
                    .Select(c => new Coords(row, c))
                    .Where(localIsValidRowCol)
                    .ToList();

                var rippleRightCoords = Enumerable.Range(col, value + 1)
                    .Select(c => new Coords(row, c))
                    .Where(localIsValidRowCol)
                    .ToList();

                var baseIndex = (value - 1) * 4;

                var rippleUpSecondaryColumns = allRippleSecondaryColumns[baseIndex];
                rippleUpCoords.ForEach(coords => rippleUpSecondaryColumns[coords.Row * numCols + coords.Col] = 1);

                var rippleDownSecondaryColumns = allRippleSecondaryColumns[baseIndex + 1];
                rippleDownCoords.ForEach(coords => rippleDownSecondaryColumns[coords.Row * numCols + coords.Col] = 1);

                var rippleLeftSecondaryColumns = allRippleSecondaryColumns[baseIndex + 2];
                rippleLeftCoords.ForEach(coords => rippleLeftSecondaryColumns[coords.Row * numCols + coords.Col] = 1);

                var rippleRightSecondaryColumns = allRippleSecondaryColumns[baseIndex + 3];
                rippleRightCoords.ForEach(coords => rippleRightSecondaryColumns[coords.Row * numCols + coords.Col] = 1);

                return allRippleSecondaryColumns;
            };

            var numRowColPrimaryColumns = numRows*numCols;
            var rowColPrimaryColumns = new int[numRowColPrimaryColumns];
            var rowColPrimaryColumnsIndex = row*numCols + col;
            rowColPrimaryColumns[rowColPrimaryColumnsIndex] = 1;

            var numCellWithinRoomPrimaryColumns = rooms.Sum(r => r.Cells.Count);
            var cellWithinRoomPrimaryColumns = new int[numCellWithinRoomPrimaryColumns];
            var cellWithinRoomPrimaryColumnsIndex = rooms.Take(roomIndex).Sum(r => r.Cells.Count) + value - 1;
            cellWithinRoomPrimaryColumns[cellWithinRoomPrimaryColumnsIndex] = 1;

            var secondaryColumns = buildSecondaryColumns();

            var allColumns = new[] { rowColPrimaryColumns, cellWithinRoomPrimaryColumns }.Concat(secondaryColumns);

            return allColumns
                .SelectMany(columns => columns)
                .ToImmutableList();
        }
    }
}
