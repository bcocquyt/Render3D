using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace HelixRenderer
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private Model3D model;
        private bool directionArrows;
        private string fileName;
        private int filePos;
        public List<Point3D> points = new List<Point3D>();
        private string[] fileContents;
        private string currentLine;
        private string currentPosition;
        private GCodeParser parse;

        public string CurrentLine
        {
            get { return this.currentLine; }
            set
            {
                currentLine = value;
                RaisePropertyChanged("CurrentLine");
            }
        }
        public string CurrentPosition
        {
            get { return this.currentPosition; }
            set
            {
                currentPosition = value;
                RaisePropertyChanged("CurrentPosition");
            }
        }
        private GCodeParser parser = new GCodeParser();
        public void AddPoint(Point3D p)
        {
            {
                this.points.Add(p);
                RaisePropertyChanged("Points");
                UpdateModel();
            }
        }

        public void ReadNextLine()
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                if (filePos < fileContents.Length-1)
                {
                    filePos++;
                    this.CurrentLine = fileContents[filePos];
                    parser.GCode_Command = this.CurrentLine;
                    parser.Process_Parsed_Command();
                    this.CurrentPosition = $"Position: {filePos}/{fileContents.Length}";
                }
            }
        }

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                filePos = 0;
                fileContents = System.IO.File.ReadAllLines(this.FileName);
                RaisePropertyChanged("FileName");
            }
        }
        public void ResetPoints(Point3D p)
        {
            {
                this.points.Clear();
                this.points.Add(p);
                RaisePropertyChanged("Points");
                UpdateModel();
            }
        }
        public MainViewModel()
        {
            parser = new GCodeParser();
            UpdateModel();
        }

        public bool DirectionArrows
        {
            get { return directionArrows; }
            set
            {
                if (directionArrows != value)
                {
                    directionArrows = value;
                    RaisePropertyChanged("DirectionArrows");
                    UpdateModel();
                }
            }
        }

        public Model3D Model
        {
            get { return model; }
            set
            {
                model = value;
                RaisePropertyChanged("Model");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private void UpdateModel()
        {
            List<Point3D> path = this.points;

            // create the WPF3D model
            var m = new Model3DGroup();
            var gm = new MeshBuilder();

            if (path.Count > 2)
            {
                gm.AddTube(path, 0.8, 10, false);
                if (directionArrows)
                {
                    // sphere at the initial point
                    gm.AddSphere(path[0], 1);
                    // arrow heads every 100 point
                    for (int i = 100; i + 1 < path.Count; i += 100)
                    {
                        gm.AddArrow(path[i], path[i + 1], 0.8);
                    }
                    // arrow head at the end
                    Point3D p0 = path[path.Count - 2];
                    Point3D p1 = path[path.Count - 1];
                    var d = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                    d.Normalize();
                    Point3D p2 = p1 + d * 2;
                    gm.AddArrow(p1, p2, 0.8);
                }

                m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));
            }

            Model = m;
        }
    }
}
