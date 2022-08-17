using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Threading;

namespace SerialTerminal
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private SerialPort _serialPort;
        private bool _continue;
        private Thread readThread;
        private int filePos;
        public string[] FileContents;
        private string currentLine;
        private string currentPosition;


        private string lastData;

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool connected;

        public bool Connected
        {
            get { return connected; }
            set { connected = value; RaisePropertyChanged("Connected"); }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set 
            { 
                this.fileName = value;
                filePos = -1;
                FileContents = System.IO.File.ReadAllLines(this.FileName);
                RaisePropertyChanged("FileName");
            }
        }
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

        public string LastData
        {
            get { return lastData; }
            set
            {
                lastData = value;
                RaisePropertyChanged("LastData");
            }
        }

        public MainViewModel()
        {
            _serialPort = new SerialPort();

            readThread = new Thread(Read);

            _serialPort.PortName = "COM5";
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;

            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _continue = false;
        }

        public void Open()
        {
            _serialPort.Open();

            this.Connected = true;

            _continue = true;

            readThread.Start();
        }

        public void Close()
        {
            _continue = false;
            readThread.Join();
            _serialPort.Close();

            this.Connected = false;

            _continue = false;
        }

        public void GetStatus()
        {
            _serialPort.WriteLine("$$");
        }

        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    this.LastData = message;
                }
                catch (TimeoutException) { }
            }
        }

        public void SendNextLine()
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                if (filePos < FileContents.Length - 1)
                {
                    filePos++;
                    this.CurrentLine = FileContents[filePos];
                    this.CurrentPosition = $"Position: {filePos}/{FileContents.Length}";
                    _serialPort.WriteLine(this.CurrentLine);
                }
            }
        }

        public void SendMultipleLines(int numberOfLines)
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                if (filePos < FileContents.Length - 1)
                {
                    for (int i = 0; i < numberOfLines; i++)
                    {
                        SendNextLine();
                        Thread.Sleep(1500);
                    }
                }
            }
        }

        public void Write(string message)
        {
            _serialPort.WriteLine(message);
        }

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
