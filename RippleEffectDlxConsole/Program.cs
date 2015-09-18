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
            var rooms = ImmutableList.Create(
                new Room(
                    new Coords(0, 0),
                    new Coords(1, 0)),
                new Room(
                    new Coords(2, 1),
                    new Coords(2, 0),
                    new Coords(3, 0),
                    new Coords(4, 0)),
                new Room(
                    new Coords(5, 0),
                    new Coords(6, 0),
                    new Coords(7, 0),
                    new Coords(7, 1)),
                new Room(
                    new Coords(0, 1),
                    new Coords(0, 2)),
                new Room(
                    new Coords(1, 1)),
                new Room(
                    new Coords(3, 2)),
                new Room(
                    new Coords(3, 1),
                    new Coords(4, 1),
                    new Coords(4, 2),
                    new Coords(4, 3),
                    new Coords(3, 3)),
                new Room(
                    new Coords(5, 1),
                    new Coords(6, 1),
                    new Coords(6, 2),
                    new Coords(7, 2)),
                new Room(
                    new Coords(0, 3),
                    new Coords(1, 3),
                    new Coords(1, 2),
                    new Coords(1, 4),
                    new Coords(2, 4)),
                new Room(
                    new Coords(2, 2),
                    new Coords(2, 3)),
                new Room(
                    new Coords(3, 4),
                    new Coords(3, 5),
                    new Coords(3, 6),
                    new Coords(4, 5)),
                new Room(
                    new Coords(4, 4),
                    new Coords(5, 4),
                    new Coords(5, 3),
                    new Coords(5, 2),
                    new Coords(6, 3)),
                new Room(
                    new Coords(6, 4),
                    new Coords(7, 5),
                    new Coords(7, 4),
                    new Coords(7, 3)),
                new Room(
                    new Coords(0, 7)),
                new Room(
                    new Coords(1, 7),
                    new Coords(1, 6)),
                new Room(
                    new Coords(4, 6),
                    new Coords(5, 6)),
                new Room(
                    new Coords(5, 5),
                    new Coords(6, 5)),
                new Room(
                    new Coords(2, 7),
                    new Coords(3, 7),
                    new Coords(4, 7)),
                new Room(
                    new Coords(7, 6)),
                new Room(
                    new Coords(0, 4),
                    new Coords(0, 5),
                    new Coords(0, 6),
                    new Coords(1, 5)),
                new Room(
                    new Coords(2, 6),
                    new Coords(2, 5)),
                new Room(
                    new Coords(5, 7),
                    new Coords(6, 7),
                    new Coords(7, 7),
                    new Coords(6, 6)));

            var initialValues = ImmutableList.Create(
                Tuple.Create(new Coords(4, 0), 2),
                Tuple.Create(new Coords(1, 2), 3),
                Tuple.Create(new Coords(2, 3), 1),
                Tuple.Create(new Coords(4, 3), 5),
                Tuple.Create(new Coords(3, 4), 1),
                Tuple.Create(new Coords(5, 4), 3),
                Tuple.Create(new Coords(6, 5), 2),
                Tuple.Create(new Coords(3, 7), 3));

            var initialGrid = InitialGrid(rooms, initialValues);
            Console.WriteLine("Puzzle:");
            initialGrid.Draw();

            Console.WriteLine();

            //Console.WriteLine("Solution:");
            //var solutionGrid = Solve(rooms, initialValues);
            //solutionGrid.Draw();
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

            // Build internal rows
            // - lots of rows for each room being careful to take account of initial values
            // for each room
            //   BuildInternalRowsForRoom =>
            //   get list of cells that do not have initial values
            //   get list of remaining numbers that need to appear in this room
            //   - i.e. 1 to room size then remove any initial values in this room
            //   should now have n cells and n remaining numbers
            //   for each of the cells
            //     for each of the remaining numbers
            //       generate an internal row with isFixed = false
            // end for

            // - one row for each initial value
            var internalRows1 = Enumerable.Empty<Tuple<Coords, int, bool>>();
            var internalRows2 = initialValues.Select(t => BuildInternalRow(t.Item1, t.Item2, true));
            var internalRows = internalRows1.Concat(internalRows2).ToImmutableList();

            var dlxRows = BuildDlxRows(internalRows);

            var dlx = new Dlx();
            var firstSolution = dlx.Solve(dlxRows, d => d, r => r).First();

            var v1 = firstSolution.RowIndexes.Select(idx => internalRows[idx]);
            var v2 = v1.OrderBy(t => t.Item1.Y).ThenBy(t => t.Item1.X);

            var rowStrings = Enumerable.Range(0, numRows)
                .Select(row => string.Join("", v2.Skip(row * numCols).Take(numRows).Select(t => t.Item3)));

            return new Grid(rowStrings.ToImmutableList());
        }

        private static IImmutableList<IImmutableList<int>> BuildDlxRows(IImmutableList<Tuple<Coords, int, bool>> internalRows)
        {
            // Need primary and secondary columns
            // - primary for each (row, col) coords (total num bits: num rows x num cols)
            //  - all bits 0 except the:
            //   - bit representing the row / col
            // - secondary for each cell in the row (total num bits: num cols)
            //  - all bits 0 except for:
            //   - the bit representing col within the row
            //   - bits representing the n cols either side (L/R) that are valid locations
            // - secondary for each cell in the col (total num bits: num rows)
            //  - all bits 0 except for:
            //   - the bit representing row within the col
            //   - bits representing the n rows either side (U/D) that are valid locations

            return null;
        }

        private static Tuple<Coords, int, bool> BuildInternalRow(Coords coords, int value, bool isFixed)
        {
            return Tuple.Create(coords, value, isFixed);
        }
    }
}
