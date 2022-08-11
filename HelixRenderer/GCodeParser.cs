using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixRenderer
{
    internal class GCodeParser
    {
        private string gcode_command;

        public string GCode_Command 
        { 
            get 
            { 
                return this.gcode_command; 
            } 
            set 
            { 
                this.gcode_command = value; 
            } 
        }

        public GCodeParser()
        {

        }

        public void Process_Parsed_Command()
        {
            gcode_command.ToUpper();
            if (gcode_command.IndexOf('G') > -1)
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
                    case 3: gcode_M3(); break;
                    case 4: gcode_M5(); break;
                }
            }
        }

        void gcode_G0_G1()
        {
            if (gcode_command.IndexOf('X') > -1)
            {
                if (gcode_command.IndexOf('Y') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Y') - gcode_command.IndexOf('X') - 1));
                else if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(
                            gcode_command.Substring(gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('X')));
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('X') - 1));
                else Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(
                            gcode_command.Substring(gcode_command.IndexOf('X') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('X') - 1));
            }
            if (gcode_command.IndexOf('Y') > -1)
            {
                if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('Y') - 1));
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Y') - 1));
                else Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Y') - 1));
            }

            if (gcode_command.IndexOf('Z') > -1)
            {
                if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.Z_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Z') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Z') - 1));
                else Stepper.destination[Stepper.Z_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Z') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Z') - 1));
            }

            Stepper.buffer_line_to_destination();
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
                            gcode_command.IndexOf('Y') - gcode_command.IndexOf('X') - 1));
                else if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('X') - 1));
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('X') - 1));
                else Stepper.destination[Stepper.X_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('X') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('X') - 1));
            }
            if (gcode_command.IndexOf('Y') > -1)
            {
                if (gcode_command.IndexOf('Z') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('Z') - gcode_command.IndexOf('Y') - 1));
                else if (gcode_command.IndexOf('S') > -1) 
                    Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.IndexOf('S') - gcode_command.IndexOf('Y') - 1));
                else Stepper.destination[Stepper.Y_AXIS] =
                        double.Parse(gcode_command.Substring(
                            gcode_command.IndexOf('Y') + 1, 
                            gcode_command.Length - gcode_command.IndexOf('Y') - 1));
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
                        gcode_command.IndexOf('S') - gcode_command.IndexOf('R') - 1));
                else r = double.Parse(gcode_command.Substring(
                    gcode_command.IndexOf('R') + 1, 
                    gcode_command.Length - gcode_command.IndexOf('R') - 1));

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
                                gcode_command.IndexOf('J') - gcode_command.IndexOf('I') - 1));
                    else if (gcode_command.IndexOf('S') > -1) 
                        arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1, 
                                gcode_command.IndexOf('S') - gcode_command.IndexOf('I') - 1));
                    else arc_offset[0] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('I') + 1, 
                                gcode_command.Length - gcode_command.IndexOf('I') - 1));
                }
                if (gcode_command.IndexOf('J') > -1)
                {
                    if (gcode_command.IndexOf('S') > -1) 
                        arc_offset[1] =
                            double.Parse(gcode_command.Substring(
                                gcode_command.IndexOf('J') + 1, 
                                gcode_command.IndexOf('S') - gcode_command.IndexOf('J') - 1));
                    else arc_offset[1] = double.Parse(gcode_command.Substring(
                        gcode_command.IndexOf('J') + 1, 
                        gcode_command.Length - gcode_command.IndexOf('J') - 1));
                }
            }
            Serial.print("G23 X"); Serial.print(Stepper.destination[Stepper.X_AXIS]);
            Serial.print("Y"); Serial.print(Stepper.destination[Stepper.Y_AXIS]);
            Serial.print("I"); Serial.print(arc_offset[0]);
            Serial.print("J"); Serial.println(arc_offset[1]);

            Stepper.buffer_arc_to_destination(arc_offset, clockwise);
        }

        public void gcode_G4()
        {
            Serial.println("G4");
        }

        public void gcode_M3()
        {
            Serial.println("M3");
        }

        public void gcode_M5()
        {
            Serial.println("M5");
        }

    }
}
