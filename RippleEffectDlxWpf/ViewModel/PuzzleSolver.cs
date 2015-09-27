using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DlxLib;
using RippleEffectDlxWpf.Extensions;
using RippleEffectDlxWpf.Model;

namespace RippleEffectDlxWpf.ViewModel
{
    public class PuzzleSolver
    {
        private readonly Puzzle _puzzle;
        private readonly Action<IImmutableList<InternalRow>> _onSolutionFound;
        private readonly Action<IImmutableList<InternalRow>> _onSearchStep;
        private readonly SynchronizationContext _synchronizationContext;
        private readonly CancellationToken _cancellationToken;

        public PuzzleSolver(
            Puzzle puzzle,
            Action<IImmutableList<InternalRow>> onSolutionFound,
            Action<IImmutableList<InternalRow>> onSearchStep,
            SynchronizationContext synchronizationContext,
            CancellationToken cancellationToken)
        {
            _puzzle = puzzle;
            _onSolutionFound = onSolutionFound;
            _onSearchStep = onSearchStep;
            _synchronizationContext = synchronizationContext;
            _cancellationToken = cancellationToken;
        }

        public void SolvePuzzle()
        {
            Task.Factory.StartNew(
                SolvePuzzleInBackground,
                _cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void SolvePuzzleInBackground()
        {
            var rooms = _puzzle.Rooms;
            var initialValues = _puzzle.InitialValues;

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

            var dlx = new Dlx(_cancellationToken);

            dlx.SearchStep += (_, searchStepEventArgs) =>
            {
                var subsetOfInternalRows = searchStepEventArgs.RowIndexes.Select(idx => internalRows[idx]).ToImmutableList();
                _synchronizationContext.Post(_onSearchStep, subsetOfInternalRows);
            };

            var firstSolution = dlx.Solve(dlxRows, d => d, r => r, numPrimaryColumns).FirstOrDefault();

            if (firstSolution != null)
            {
                var subsetOfInternalRows = firstSolution.RowIndexes.Select(idx => internalRows[idx]).ToImmutableList();
                _synchronizationContext.Post(_onSolutionFound, subsetOfInternalRows);
            }
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

            var numRowColPrimaryColumns = numRows * numCols;
            var rowColPrimaryColumns = new int[numRowColPrimaryColumns];
            var rowColPrimaryColumnsIndex = row * numCols + col;
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
