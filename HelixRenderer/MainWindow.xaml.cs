using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelixRenderer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// https://github.com/helix-toolkit/helix-toolkit
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Stopwatch watch = new Stopwatch();
        private Point3D position = new Point3D(0, 0, 0);
        private int StepSize = 10;
        private MainViewModel vm = new MainViewModel();

        public MainWindow()
        { 
            InitializeComponent();
            this.DataContext = vm;
            vm.DirectionArrows = true;
        }

        private void AddLineToNewPosition()
        {
            vm.AddPoint(position);
            //vm.DirectionArrows = !vm.DirectionArrows;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //vm.DirectionArrows = !vm.DirectionArrows;
            vm.ReadNextLine();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            this.position.Y += StepSize;
            AddLineToNewPosition();
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            this.position.X += -StepSize;
            AddLineToNewPosition();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            this.position.X += StepSize;
            AddLineToNewPosition();
        }

        private void btnBackward_Click(object sender, RoutedEventArgs e)
        {
            this.position.Y += -StepSize;
            AddLineToNewPosition();
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            //vm.FileName = @"C:\Users\BartCocquyt\source\repos\WalldrawBcocquyt\NC\Dakar A4竖.nc";
            vm.FileName = @"C:\Users\BartCocquyt\OneDrive - Portima\Documents\Inkscape\WallDrawTests\output_0012.ngc";
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            vm.ResetPoints(new Point3D(0, 0, 0));
        }

        private void btnNext100_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadMultipleLines(100);
        }

        private void btnNext1000_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadMultipleLines(1000);
        }

        private void btnRemaining_Click(object sender, RoutedEventArgs e)
        {
            int remainginLineCount = vm.FileContents.Length - vm.CurrentLineNumber - 1;
            vm.ReadMultipleLines(remainginLineCount);
        }
    }
}
