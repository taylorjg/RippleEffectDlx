using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RippleEffectDlxWpf.Model;
using RippleEffectDlxWpf.ViewModel;

namespace RippleEffectDlxWpf.View
{
    public partial class BoardControl : IBoardControl
    {
        private const int GridLineThickness = 4;
        private const int GridLineHalfThickness = GridLineThickness / 2;
        private double _sw;
        private double _sh;
        private int _numRows;
        private readonly IDictionary<Tuple<Coords, int>, FrameworkElement> _placedDigits = new Dictionary<Tuple<Coords, int>,FrameworkElement>();

        private enum TagType
        {
            Room,
            InitialValue,
            Digit
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        public void InitialiseGrid(int numRows, int numCols)
        {
            _numRows = numRows;
            _sw = (ActualWidth - GridLineThickness) / numCols;
            _sh = (ActualHeight - GridLineThickness) / numRows;
        }

        public void AddRooms(IImmutableList<Room> rooms)
        {
            foreach (var room in rooms)
                AddRoom(room);
        }


        private void AddRoom(Room room)
        {
            var roomControl = new RoomControl(room, _numRows, _sw, _sh) {Tag = TagType.Room};
            Canvas.SetLeft(roomControl, GridLineHalfThickness);
            Canvas.SetTop(roomControl, GridLineHalfThickness);
            BoardCanvas.Children.Add(roomControl);
        }

        public void AddInitialValues(IImmutableList<InitialValue> initialValues)
        {
            foreach (var initialValue in initialValues)
                AddDigit(initialValue.Item1, initialValue.Item2, true);
        }

        public void AddDigit(Coords coords, int value)
        {
            AddDigit(coords, value, false);
        }

        public bool HasDigitAt(Coords coords, int value)
        {
            return _placedDigits.ContainsKey(Tuple.Create(coords, value));
        }

        public void RemoveDigitsOtherThan(IImmutableList<InternalRow> internalRows)
        {
            var placedDigitsToRemove = _placedDigits.Where(pd =>
            {
                return internalRows.FirstOrDefault(ir => ir.Item1.Equals(pd.Key.Item1) && ir.Item2 == pd.Key.Item2) == null;
            }).ToList();

            foreach (var placedDigitToRemove in placedDigitsToRemove)
            {
                BoardCanvas.Children.Remove(placedDigitToRemove.Value);
                _placedDigits.Remove(placedDigitToRemove);
            }
        }

        private void AddDigit(Coords coords, int value, bool isInitialValue)
        {
            // http://stackoverflow.com/questions/17828417/centering-text-vertically-and-horizontally-in-textblock-and-passwordbox-in-windo

            var textBlock = new TextBlock
            {
                Text = Convert.ToString(value),
                FontSize = 48,
                Foreground = new SolidColorBrush(Colors.Black),
                Opacity = isInitialValue ? 0.6 : 1.0,
                TextAlignment = TextAlignment.Center,
                Tag = isInitialValue ? TagType.InitialValue : TagType.Digit
            };

            var border = new Border
            {
                Width = _sw,
                Height = _sh,
                Child = textBlock
            };

            Canvas.SetLeft(border, coords.Col * _sw + GridLineHalfThickness);
            Canvas.SetTop(border, (_numRows - coords.Row - 1) * _sh + GridLineHalfThickness);
            BoardCanvas.Children.Add(border);

            if (!isInitialValue)
                _placedDigits[Tuple.Create(coords, value)] = border;
        }

        public void Reset()
        {
            RemoveChildrenWithTagType(
                TagType.Room,
                TagType.InitialValue,
                TagType.Digit);

            _placedDigits.Clear();
        }

        private void RemoveChildrenWithTagType(params TagType[] tagTypes)
        {
            BoardCanvas.Children
                .OfType<FrameworkElement>()
                .Where(fe => tagTypes.Contains((TagType)fe.Tag))
                .ToList()
                .ForEach(fe => BoardCanvas.Children.Remove(fe));
        }
    }
}
