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
using Microsoft.Win32;

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
            //vm.FileName = @"C:\Users\BartCocquyt\OneDrive - Portima\Documents\Inkscape\WallDrawTests\output_0012.ngc";
            //vm.FileName = @"C:\Users\BartCocquyt\OneDrive - Portima\Documents\Inkscape\WallDrawTests\output_0013.ngc";

            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                openFileDialog.InitialDirectory = @"C:\Users\BartCocquyt\OneDrive - Portima\Documents\Inkscape\WallDrawTests";
                openFileDialog.Filter = "nc files (*.nc)|*.nc|ngc files (*.ngc)|*.ngc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog().Value)
                {
                    //Get the path of specified file
                    vm.FileName = openFileDialog.FileName;
                }
            }
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

        private void btnNext10_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadMultipleLines(10);
        }

        private void btnNext5_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadMultipleLines(5);
        }

        private void btnRunnTillLine_Click(object sender, RoutedEventArgs e)
        {
            btnLoadFile_Click(sender, e);
            vm.ReadMultipleLines(int.Parse(tbLines.Text));
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (vm.CurrentLineNumber > 0)
            {
                int gotoLine = vm.CurrentLineNumber - 1;
                string oldFileName = vm.FileName;
                vm.FileName = oldFileName;
                vm.ReadMultipleLines(gotoLine);
            }
        }
    }
}
