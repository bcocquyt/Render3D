using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SerialTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
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

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            vm.SendNextLine();
        }

        private void btnRemaining_Click(object sender, RoutedEventArgs e)
        {
            int remainginLineCount = vm.FileContents.Length - vm.CurrentLineNumber - 1;
            vm.SendMultipleLines(remainginLineCount);
        }

        private void btnConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (vm.Connected)
            {
                vm.Close();
            }
            else
            {
                vm.Open();
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            vm.Read();
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            vm.Write(tbMessage.Text);
        }
    }
}
