using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Render3DLib
{
    public  class GCodeParser
    {
        private string gcode_command;
        private List<Point3D> newPoints;

        public string GCode_Command 
        { 
            get 
            { 
                return this.gcode_command; 
            } 
            set 
            { 
                this.gcode_command = GCodeSanitizer.Sanitize(value); 
            } 
        }

        public GCodeParser()
        {
            newPoints = new List<Point3D>();
        }

        public List<Point3D> Process_Parsed_Command()
        {
            newPoints.Clear();
            gcode_command.ToUpper();
            try
            {
                if (gcode_command.IndexOf("G21") > -1) gcode_G21();
                else if (gcode_command.IndexOf('G') > -1)
                {
                    switch (int.Parse(gcode_command.Substring(gcode_command.IndexOf('G') + 1, 1)))
                    {
                        case 0:
                        case 1: gcode_G0_G1(); break;
                        case 2: gcode_G2_G3(true); break;
                        case 3: gcode_G2_G3(false); break;
                        case 4: gcode_G4(); break;
                    }
                }
                else if (gcode_command.IndexOf('M') > -1)
                {
                    switch (int.Parse(gcode_command.Substring(gcode_command.IndexOf('M') + 1, 1)))
                    {
                        case 2: gcode_M2(); break;
                        case 3: gcode_M3(); break;
                        case 4: gcode_M4(); break;
                        case 5: gcode_M5(); break;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
            return newPoints;
        }

        void gcode_G0_G1()
        {
            if (gcode_command.IndexOf('X') > -1)
            {
                if (gcode_command.IndexOf('Y') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Y') - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(
                            gcode_command.Substring(gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('X')), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
                else Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(
                            gcode_command.Substring(gcode_command.IndexOf('X') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
            }
            if (gcode_command.IndexOf('Y') > -1)
            {
                if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('S') > -1)
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1,
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('F') > -1)
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1,
                            gcode_command.IndexOf('F') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
            }

            if (gcode_command.IndexOf('Z') > -1)
            {
                if (gcode_command.IndexOf('S') > -1)
                    Stepper.destination[Stepper.Z_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Z') + 1,
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Z') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('F') > -1)
                    Stepper.destination[Stepper.Z_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Z') + 1,
                            gcode_command.IndexOf('F') - gcode_command.IndexOf('Z') - 1), CultureInfo.InvariantCulture);
                else Stepper.destination[Stepper.Z_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Z') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Z') - 1), CultureInfo.InvariantCulture);
            }

            Stepper.buffer_line_to_destination(newPoints);
            Stepper.print_position();
        }

        void gcode_G2_G3(bool clockwise)
        {
            if (gcode_command.IndexOf('X') > -1)
            {
                if (gcode_command.IndexOf('Y') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(
                            gcode_command.Substring(gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Y') - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
                else Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('X') - 1), CultureInfo.InvariantCulture);
            }
            if (gcode_command.IndexOf('Y') > -1)
            {
                if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('S') > -1)
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1,
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('F') > -1)
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1,
                            gcode_command.IndexOf('F') - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
                else Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Y') - 1), CultureInfo.InvariantCulture);
            }

            double[] arc_offset = { 0.0, 0.0 };

            if (gcode_command.IndexOf('R') > -1)
            {
                double r = 0,
                      p1 = Stepper.current_position[Stepper.X_AXIS], q1 = Stepper.current_position[Stepper.Y_AXIS],
                      p2 = Stepper.destination[Stepper.X_AXIS], q2 = Stepper.destination[Stepper.Y_AXIS];
                if (gcode_command.IndexOf('S') > -1)
                    r = double.Parse(gcode_command.Substring(
                        gcode_command.IndexOf('R') + 1,
                        gcode_command.IndexOf('S') - gcode_command.IndexOf('R') - 1), CultureInfo.InvariantCulture);
                else if (gcode_command.IndexOf('F') > -1)
                    r = double.Parse(gcode_command.Substring(
                        gcode_command.IndexOf('R') + 1,
                        gcode_command.IndexOf('F') - gcode_command.IndexOf('R') - 1), CultureInfo.InvariantCulture);
                else r = double.Parse(gcode_command.Substring(
                    gcode_command.IndexOf('R') + 1, 
                    gcode_command.Length - gcode_command.IndexOf('R') - 1), CultureInfo.InvariantCulture);

                if ((r > 0) && (p2 != p1 || q2 != q1))
                {
                    double e = clockwise ^ (r < 0) ? -1 : 1,
                                dx = p2 - p1, dy = q2 - q1,
                                d = Math.Sqrt(Stepper.sq(dx) + Stepper.sq(dy)),
                                h = Math.Sqrt(Stepper.sq(r) - Stepper.sq(d * 0.5)),
                                mx = (p1 + p2) * 0.5, my = (q1 + q2) * 0.5,
                                sx = -dy / d, sy = dx / d,
                                cx = mx + e * h * sx, cy = my + e * h * sy;
                    arc_offset[0] = cx - p1;
                    arc_offset[1] = cy - q1;
                }
            }
            else
            {
                if (gcode_command.IndexOf('I') > -1)
                {
                    if (gcode_command.IndexOf('J') > -1) 
                        arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1, 
                                gcode_command.IndexOf('J') - gcode_command.IndexOf('I') - 1), CultureInfo.InvariantCulture);
                    else if (gcode_command.IndexOf('S') > -1)
                        arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1,
                                gcode_command.IndexOf('S') - gcode_command.IndexOf('I') - 1), CultureInfo.InvariantCulture);
                    else if (gcode_command.IndexOf('F') > -1)
                        arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1,
                                gcode_command.IndexOf('F') - gcode_command.IndexOf('I') - 1), CultureInfo.InvariantCulture);
                    else arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1, 
                                gcode_command.Length - gcode_command.IndexOf('I') - 1), CultureInfo.InvariantCulture);
                }
                if (gcode_command.IndexOf('J') > -1)
                {
                    if (gcode_command.IndexOf('S') > -1)
                        arc_offset[1] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('J') + 1,
                                gcode_command.IndexOf('S') - gcode_command.IndexOf('J') - 1), CultureInfo.InvariantCulture);
                    else if (gcode_command.IndexOf('F') > -1)
                        arc_offset[1] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('J') + 1,
                                gcode_command.IndexOf('F') - gcode_command.IndexOf('J') - 1), CultureInfo.InvariantCulture);
                    else arc_offset[1] = double.Parse(gcode_command.Substring(
                        gcode_command.IndexOf('J') + 1, 
                        gcode_command.Length - gcode_command.IndexOf('J') - 1), CultureInfo.InvariantCulture);
                }
            }
            Serial.print("G23 X"); Serial.print(Stepper.destination[Stepper.X_AXIS]);
            Serial.print("Y"); Serial.print(Stepper.destination[Stepper.Y_AXIS]);
            Serial.print("I"); Serial.print(arc_offset[0]);
            Serial.print("J"); Serial.println(arc_offset[1]);

            Stepper.buffer_arc_to_destination(newPoints, arc_offset, clockwise);
        }

        public void gcode_G21()
        {
            Serial.println("G21");
        }

        public void gcode_G4()
        {
            Serial.println("G4");
        }

        public void gcode_M2()
        {
            Serial.println("M2");
        }

        public void gcode_M3()
        {
            Serial.println("M3");
        }

        public void gcode_M4()
        {
            Serial.println("M4");
        }

        public void gcode_M5()
        {
            Serial.println("M5");
        }

    }
}
