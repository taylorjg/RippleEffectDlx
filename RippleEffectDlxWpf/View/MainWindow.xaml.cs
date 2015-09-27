using RippleEffectDlxWpf.Model;

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
            };
        }
    }
}
