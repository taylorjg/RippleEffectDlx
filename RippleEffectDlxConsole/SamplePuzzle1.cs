using System;
using System.Collections.Immutable;

namespace RippleEffectDlxConsole
{
    using InitialValue = Tuple<Coords, int>;

    public static partial class SamplePuzzles
    {
        private static readonly IImmutableList<Room> SamplePuzzle1Rooms = ImmutableList.Create(
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

        private static readonly IImmutableList<InitialValue> SamplePuzzle1InitialValues = ImmutableList.Create(
            new InitialValue(new Coords(4, 0), 2),
            new InitialValue(new Coords(1, 2), 3),
            new InitialValue(new Coords(2, 3), 1),
            new InitialValue(new Coords(4, 3), 5),
            new InitialValue(new Coords(3, 4), 1),
            new InitialValue(new Coords(5, 4), 3),
            new InitialValue(new Coords(6, 5), 2),
            new InitialValue(new Coords(3, 7), 3));
    }
}
