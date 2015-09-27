using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using RippleEffectDlxWpf.Model;
using RippleEffectDlxWpf.ViewModel;

namespace RippleEffectDlxWpf.View
{
    public partial class MainWindow
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Queue<IImmutableList<InternalRow>> _searchSteps = new Queue<IImmutableList<InternalRow>>();

        public MainWindow()
        {
            InitializeComponent();

            _timer.Tick += (_, __) => OnTick();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Start();

            ContentRendered += (_, __) =>
            {
                BoardControl.InitialiseGrid(8, 8);
                BoardControl.AddRooms(SamplePuzzles.SamplePuzzle1.Rooms);
                BoardControl.AddInitialValues(SamplePuzzles.SamplePuzzle1.InitialValues);

                var puzzleSolver = new PuzzleSolver(
                    SamplePuzzles.SamplePuzzle1,
                    OnSolutionFound,
                    OnSearchStep,
                    SynchronizationContext.Current,
                    CancellationToken.None);

                puzzleSolver.SolvePuzzle();
            };
        }

        private void OnTick()
        {
            if (!_searchSteps.Any()) return;
            var internalRows = _searchSteps.Dequeue();
            foreach (var internalRow in internalRows.Where(ir => !ir.Item4))
            {
                if (BoardControl.HasDigitAt(internalRow.Item1, internalRow.Item2)) continue;
                BoardControl.AddDigit(internalRow.Item1, internalRow.Item2);
            }
            BoardControl.RemoveDigitsOtherThan(internalRows);
        }

        private void OnSolutionFound(IImmutableList<InternalRow> internalRows)
        {
            foreach (var internalRow in internalRows.Where(ir => !ir.Item4))
                BoardControl.AddDigit(internalRow.Item1, internalRow.Item2);
        }

        private void OnSearchStep(IImmutableList<InternalRow> internalRows)
        {
            _searchSteps.Enqueue(internalRows);
        }
    }
}
