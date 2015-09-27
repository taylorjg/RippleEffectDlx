using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using RippleEffectDlxWpf.Model;
using RippleEffectDlxWpf.ViewModel;

namespace RippleEffectDlxWpf.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ContentRendered += (_, __) =>
            {
                BoardControl.DrawGrid(8, 8);
                BoardControl.DrawRooms(SamplePuzzles.SamplePuzzle1.Rooms);
                BoardControl.DrawInitialValues(SamplePuzzles.SamplePuzzle1.InitialValues);

                var puzzleSolver = new PuzzleSolver(
                    SamplePuzzles.SamplePuzzle1,
                    OnSolutionFound, 
                    SynchronizationContext.Current,
                    CancellationToken.None);

                puzzleSolver.SolvePuzzle();
            };
        }

        private void OnSolutionFound(IImmutableList<InternalRow> internalRows)
        {
            foreach (var internalRow in internalRows.Where(ir => !ir.Item4))
                BoardControl.DrawDigit(internalRow.Item1, internalRow.Item2);
        }
    }
}
