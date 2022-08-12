using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render3DLib
{
    public class Servo
    {
        public int Position { get; set; }
        public void write(int position)
        {
            this.Position = position;
        }
    }
}
