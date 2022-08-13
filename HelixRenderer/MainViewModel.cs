using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Render3DLib;

namespace HelixRenderer
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private Model3D model;
        private bool directionArrows;
        private string fileName;
        private int filePos;
        public List<Point3D> points = new List<Point3D>();
        public string[] FileContents;
        private string currentLine;
        private string currentPosition;
        private GCodeParser parse;

        public int CurrentLineNumber { get { return filePos; } }

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

        public void AddPoints(IEnumerable<Point3D> newPoints)
        {
            {
                this.points.AddRange(newPoints);
                RaisePropertyChanged("Points");
                UpdateModel();
            }
        }

        public void ReadNextLine()
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                if (filePos < FileContents.Length-1)
                {
                    filePos++;
                    this.CurrentLine = FileContents[filePos];
                    parser.GCode_Command = this.CurrentLine;
                    this.AddPoints(parser.Process_Parsed_Command());
                    this.CurrentPosition = $"Position: {filePos}/{FileContents.Length}";
                }
            }
        }

        public void ReadMultipleLines(int numberOfLines)
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                if (filePos < FileContents.Length - 1)
                {
                    List<Point3D> newPoints = new List<Point3D>();
                    for (int i = 0; i < numberOfLines; i++)
                    {
                        if (filePos > FileContents.Length - 2) break;
                        filePos++;
                        this.CurrentLine = FileContents[filePos];
                        parser.GCode_Command = this.CurrentLine;
                        newPoints.AddRange(parser.Process_Parsed_Command());
                    }
                    this.AddPoints(newPoints);
                    this.CurrentPosition = $"Position: {filePos}/{FileContents.Length}";
                }
            }
        }

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                filePos = -1;
                FileContents = System.IO.File.ReadAllLines(this.FileName);
                RaisePropertyChanged("FileName");
                ResetPoints(new Point3D(0, 0, 0));
                List<Point3D> newPoints = new List<Point3D>();
                Stepper.stepper_init(newPoints);
                newPoints.AddRange(newPoints);
                ReadNextLine();
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
            var gm2 = new MeshBuilder();

            foreach (Point3D p in path)
            {
                gm2.AddSphere(p, 0.4);
            }
            m.Children.Add(new GeometryModel3D(gm2.ToMesh(), Materials.Red));

            if (path.Count > 1)
            {
                gm.AddTube(path, 0.4, 10, false);
                //if (directionArrows)
                //{
                //    // sphere at the initial point
                //    gm.AddSphere(path[0], 1);
                //    // arrow heads every 100 point
                //    for (int i = 100; i + 1 < path.Count; i += 100)
                //    {
                //        gm.AddArrow(path[i], path[i + 1], 0.8);
                //    }
                //    // arrow head at the end
                //    Point3D p0 = path[path.Count - 2];
                //    Point3D p1 = path[path.Count - 1];
                //    var d = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                //    d.Normalize();
                //    Point3D p2 = p1 + d * 2;
                //    gm.AddArrow(p1, p2, 0.8);
                //}

                m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));
            }

            Model = m;
        }

        public String Points 
        { 
            get 
            {
                string pointString = string.Empty;
                foreach (Point3D p in points)
                {
                    pointString += p.ToString() + Environment.NewLine; 
                }
                return pointString; 
            }
            set
            {
                Console.WriteLine(value);
            }
        }
    }
}
