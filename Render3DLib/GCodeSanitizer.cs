using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render3DLib
{
    public static class GCodeSanitizer
    {
        public static string Sanitize(string gcode_in)
        {
            if (string.IsNullOrEmpty(gcode_in)) return string.Empty;
                //throw new ArgumentNullException("gcode_in");

            string gcode_out = System.Text.RegularExpressions.Regex.Replace(gcode_in, "%.*", "");
            gcode_out = System.Text.RegularExpressions.Regex.Replace(gcode_out, @"\(.*\)", "");
            gcode_out = gcode_out.Replace("G01", "G1");
            gcode_out = gcode_out.Replace("G02", "G2");
            gcode_out = gcode_out.Replace("G03", "G3");
            gcode_out = gcode_out.Replace("G04", "G4");
            return gcode_out;
        }
    }
}
