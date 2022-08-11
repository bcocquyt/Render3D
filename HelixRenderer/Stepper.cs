using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HelixRenderer
{
    public class Stepper
    {
        public static readonly int PEN_UP_ANGLE = 5;
        public static readonly int PEN_DOWN_ANGLE = 30;
        public static readonly int TPD = 300;
        public static readonly int LINE_DELAY = 1;
        public static readonly int X_AXIS = 0;
        public static readonly int Y_AXIS = 1;
        public static readonly int Z_AXIS = 2;
        public static readonly int X_SEPARATION = 346;
        public static readonly double X_MIN_POS = X_SEPARATION * 0.5;
        public static readonly double X_MAX_POS = X_SEPARATION * -0.5;
        public static readonly int Y_SEPARATION = 570;
        public static readonly double Y_MIN_POS = Y_SEPARATION * 0.5;
        public static readonly double Y_MAX_POS = Y_SEPARATION * -0.5;
        public static readonly int STEPS_PER_TURN = 2048;
        public static readonly int SPOOL_DIAMETER = 35;
        public static readonly double SPOOL_CIRC = (SPOOL_DIAMETER * 3.1416);
        public static readonly double DEFAULT_XY_MM_PER_STEP = (SPOOL_CIRC / STEPS_PER_TURN);
        public static readonly int MM_PER_ARC_SEGMENT = 1;
        public static readonly int N_ARC_CORRECTION = 25;
        public const int XY = 2;
        public const int XYZ = 3;
        public static readonly int INVERT_M1_DIR = 1;
        public static readonly int INVERT_M2_DIR = 1;
        public static double[] current_position = { 0, 0, 0 };
        public static double[] destination = { 0, 0, 0 };
        public static int current_steps_M1;
        public static int current_steps_M2;

        // pen state 笔状态（抬笔，落笔）.
        static int ps;
        static Servo pen;

        static void delay(int waitFor)
        {
            Serial.print("Wait for ");
            Serial.print(waitFor);
            Serial.println(" ms.");
        }

        static void delayMicroseconds(int waitFor)
        {
            delay(waitFor);
        }

        static void pen_down()
        {
            if (ps == PEN_UP_ANGLE)
            {
                ps = PEN_DOWN_ANGLE;
                pen.write(ps);
                delay(TPD);
            }
        }

        public static void print_position()
        {
            //	Serial.print("G01 X"); Serial.print(destination[X_AXIS]);
            //	Serial.print("Y"); Serial.print(destination[Y_AXIS]);
            //	Serial.print("Z"); Serial.println(destination[Z_AXIS]);
            Serial.print("<Idle|MPos:");
            Serial.print(current_position[X_AXIS]);
            Serial.print(",");
            Serial.print(current_position[Y_AXIS]);
            Serial.print(",");
            Serial.print(current_position[Z_AXIS]);
            Serial.println("|FS:0,0|Ov:100,100,100>");
        }

        static void pen_up()
        {
            if (ps == PEN_DOWN_ANGLE)
            {
                ps = PEN_UP_ANGLE;
                pen.write(ps);
            }
        }

        static void IK(double x, double y, out int target_steps_m1, out int target_steps_m2)
        {
            double dy = y - Y_MIN_POS;
            double dx = x - X_MIN_POS;
            target_steps_m1 = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy) / DEFAULT_XY_MM_PER_STEP);
            dx = x - X_MAX_POS;
            target_steps_m2 = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy) / DEFAULT_XY_MM_PER_STEP);
        }

        static void stepper_init(List<Point3D> newPoints)
        {
            int target_steps_m1, target_steps_m2;
            IK(0, 0, out target_steps_m1, out target_steps_m2);
            current_steps_M1 = target_steps_m1;
            current_steps_M2 = target_steps_m2;

            //m1.connectToPins(10, 9, 8, 7); //M1 L步进电机   in1~4端口对应UNO  7 8 9 10
            //m2.connectToPins(6, 5, 3, 2);  //M2 R步进电机   in1~4端口对应UNO 2 3 5 6
            //m1.setSpeedInStepsPerSecond(10000);
            //m1.setAccelerationInStepsPerSecondPerSecond(100000);
            //m2.setSpeedInStepsPerSecond(10000);
            //m2.setAccelerationInStepsPerSecondPerSecond(100000);
            //舵机初始化
            //pen.attach(A0);

            current_position[Z_AXIS] = 0;
            current_position[X_AXIS] = 0;
            current_position[Y_AXIS] = 0;

            destination[Z_AXIS] = 1;
            destination[X_AXIS] = 0;
            destination[Y_AXIS] = 0;
            buffer_line_to_destination(newPoints);
            ps = PEN_UP_ANGLE;
            pen.write(ps);
        }

        //直接由当前位置移动到目标位置
        static void moveto(List<Point3D> newPoints, double target_X, double target_Y)
        {
            //newPoints.Add(new Point3D(target_X, target_Y, current_position[Z_AXIS]));

            int target_steps_m1, target_steps_m2;
            IK(target_X, target_Y, out target_steps_m1, out target_steps_m2);
            long dif_abs_steps_run_m1 = Math.Abs(target_steps_m1 - current_steps_M1);
            long dif_abs_steps_run_m2 = Math.Abs(target_steps_m2 - current_steps_M2);
            int dir1 = (target_steps_m1 - current_steps_M1) > 0 ? (-1 * INVERT_M1_DIR) : INVERT_M1_DIR;
            int dir2 = (target_steps_m2 - current_steps_M2) > 0 ? (-1 * INVERT_M2_DIR) : INVERT_M2_DIR;
            long over = 0;
            long i;
            if (dif_abs_steps_run_m1 > dif_abs_steps_run_m2)
            {
                for (i = 0; i < dif_abs_steps_run_m1; ++i)
                {
                    //m1.moveRelativeInSteps(dir1);
                    over += dif_abs_steps_run_m2;
                    if (over >= dif_abs_steps_run_m1)
                    {
                        over -= dif_abs_steps_run_m1;
                        //m2.moveRelativeInSteps(dir2);
                    }
                    delayMicroseconds(LINE_DELAY);
                }
            }
            else
            {
                for (i = 0; i < dif_abs_steps_run_m2; ++i)
                {
                    //m2.moveRelativeInSteps(dir2);
                    over += dif_abs_steps_run_m1;
                    if (over >= dif_abs_steps_run_m2)
                    {
                        over -= dif_abs_steps_run_m2;
                        //m1.moveRelativeInSteps(dir1);
                    }
                    delayMicroseconds(LINE_DELAY);
                }
            }
            current_steps_M1 = target_steps_m1;
            current_steps_M2 = target_steps_m2;

            current_position[X_AXIS] = target_X;
            current_position[Y_AXIS] = target_Y;
        }

        public static void buffer_line_to_destination(List<Point3D> newPoints)
        {
            newPoints.Add(new Point3D(destination[X_AXIS], destination[Y_AXIS], current_position[Z_AXIS]));
            double cartesian_mm = Math.Sqrt((current_position[X_AXIS] - destination[X_AXIS]) * (current_position[X_AXIS] - destination[X_AXIS])
                        + (current_position[Y_AXIS] - destination[Y_AXIS]) * (current_position[Y_AXIS] - destination[Y_AXIS]));

            if (destination[Z_AXIS] > 0)
            {
                if (current_position[Z_AXIS] <= 0)
                {
                    current_position[Z_AXIS] = destination[Z_AXIS];
                    pen_up();
                }
            }
            else
            {
                if (current_position[Z_AXIS] > 0)
                {
                    current_position[Z_AXIS] = destination[Z_AXIS];
                    pen_down();
                }
            }

            if (cartesian_mm <= DEFAULT_XY_MM_PER_STEP) { moveto(newPoints, destination[X_AXIS], destination[Y_AXIS]); return; }

            int steps = (int)Math.Floor(cartesian_mm / DEFAULT_XY_MM_PER_STEP);
            double init_X = current_position[X_AXIS];
            double init_Y = current_position[Y_AXIS];
            double scale;
            for (long s = 0; s <= steps; ++s)
            {
                scale = (float)s / (float)steps;
                moveto(newPoints, (destination[X_AXIS] - init_X) * scale + init_X,
                     (destination[Y_AXIS] - init_Y) * scale + init_Y);
            }
            moveto(newPoints, destination[X_AXIS], destination[Y_AXIS]);
        }

        public static double Hypot2(double x, double y)
        {
            return (x * x) + (y * y);
        }
        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(Hypot2(x, y));
        }

        public static double Radians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double sq(double x)
        {
            return x * x;
        }

        public static void buffer_arc_to_destination(List<Point3D> newPoints, double[] offset, bool clockwise)
        {
            double r_P = -offset[0], r_Q = -offset[1];
            int p_axis = X_AXIS, q_axis = Y_AXIS, l_axis = Z_AXIS;
            double radius = Hypot(r_P, r_Q),
                    center_P = current_position[p_axis] - r_P,
                    center_Q = current_position[q_axis] - r_Q,
                    rt_X = destination[p_axis] - center_P,
                    rt_Y = destination[q_axis] - center_Q;
            double angular_travel = Math.Atan2(r_P * rt_Y - r_Q * rt_X, r_P * rt_X + r_Q * rt_Y);
            Serial.println(angular_travel);

            if (angular_travel < 0) angular_travel += Radians(360);
            if (clockwise) angular_travel -= Radians(360);

            Serial.println(angular_travel);

            if (angular_travel == 0 && current_position[p_axis] == destination[p_axis] && current_position[q_axis] == destination[q_axis])
                angular_travel = Radians(360);

            double mm_of_travel = Hypot(angular_travel * radius, 0);  //第一个参数是弧长计算
            if (mm_of_travel < 0.001) return;
            Serial.println(mm_of_travel);

            int segments = (int)Math.Floor(mm_of_travel / (MM_PER_ARC_SEGMENT));
            if (segments < 1) segments = 1;

            double[] raw = new double[XY];
            double theta_per_segment = angular_travel / segments,
                        sin_T = theta_per_segment,
                        cos_T = 1 - 0.5 * sq(theta_per_segment);

            int arc_recalc_count = N_ARC_CORRECTION;

            for (int i = 1; i < segments; i++)
            {
                if (--arc_recalc_count > 0)
                {
                    double r_new_Y = r_P * sin_T + r_Q * cos_T;
                    r_P = r_P * cos_T - r_Q * sin_T;
                    r_Q = r_new_Y;
                }
                else
                {
                    arc_recalc_count = N_ARC_CORRECTION;
                    double cos_Ti = Math.Cos(i * theta_per_segment), sin_Ti = Math.Sin(i * theta_per_segment);
                    r_P = -offset[0] * cos_Ti + offset[1] * sin_Ti;
                    r_Q = -offset[0] * sin_Ti - offset[1] * cos_Ti;
                }

                raw[p_axis] = center_P + r_P;
                raw[q_axis] = center_Q + r_Q;

                moveto(newPoints, raw[p_axis], raw[q_axis]); //逆解执行函数

                Serial.print("G0 X");
                Serial.print(raw[p_axis]);
                Serial.print("Y");
                Serial.println(raw[q_axis]);
            }
        }

    }
}
