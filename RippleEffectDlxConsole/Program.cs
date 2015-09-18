using System;
using System.Collections.Immutable;
using System.Linq;

namespace RippleEffectDlxConsole
{
    internal static class Program
    {
        private static void Main()
        {
            var rooms = ImmutableList.Create(
                new Room(
                    new Coords(1, 0),
                    new Coords(0, 0),
                    new Coords(0, 1),
                    new Coords(0, 2)),
                new Room(
                    new Coords(2, 0),
                    new Coords(3, 0),
                    new Coords(3, 1),
                    new Coords(4, 1),
                    new Coords(4, 2)),
                new Room(
                    new Coords(4, 0),
                    new Coords(5, 0),
                    new Coords(6, 0),
                    new Coords(7, 0),
                    new Coords(8, 0),
                    new Coords(8, 1)),
                new Room(
                    new Coords(1, 1),
                    new Coords(1, 2),
                    new Coords(2, 2),
                    new Coords(3, 2),
                    new Coords(3, 3)),
                new Room(
                    new Coords(2, 1)),
                new Room(
                    new Coords(5, 1),
                    new Coords(5, 2)),
                new Room(
                    new Coords(6, 1),
                    new Coords(7, 1),
                    new Coords(6, 2),
                    new Coords(7, 2),
                    new Coords(8, 2),
                    new Coords(7, 3)),
                new Room(
                    new Coords(0, 3),
                    new Coords(1, 3),
                    new Coords(2, 3)),
                new Room(),
                new Room(),
                new Room(new Coords(8, 8)));

            var initialValues = ImmutableList.Create(
                Tuple.Create(new Coords(8, 0), 2),
                Tuple.Create(new Coords(4, 4), 5),
                Tuple.Create(new Coords(0, 8), 4));

            var initialGrid = InitialGrid(rooms, initialValues);
            Console.WriteLine("Puzzle:");
            initialGrid.Draw();

            //Console.WriteLine();

            //Console.WriteLine("Solution:");
            //var solutionGrid = Solve(rooms, initialValues);
            //solutionGrid.Draw();
        }

        private static Grid InitialGrid(IImmutableList<Room> rooms, IImmutableList<Tuple<Coords, int>> initialValues)
        {
            var numRows = rooms.SelectMany(r => r.Cells).Max(c => c.Y) + 1;
            var numCols = rooms.SelectMany(r => r.Cells).Max(c => c.X) + 1;

            var rows = Enumerable.Range(0, numRows).Select(_ => new string(' ', numCols)).ToList();
            foreach (var initialValue in initialValues)
            {
                var row = initialValue.Item1.X;
                var col = initialValue.Item1.Y;
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
            // build internal rows
            // - one row for each initial value
            // - lots of rows for each room being careful to take account of initial values

            // build dlx matrix from internal rows

            // use dlxlib to find first solution

            // use solution row indexes to build solution grid

            return null;
        }
    }
}
