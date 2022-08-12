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
            return gcode_out;
        }
    }
}
