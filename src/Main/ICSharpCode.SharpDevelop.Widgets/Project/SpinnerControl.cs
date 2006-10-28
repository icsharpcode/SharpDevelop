using System;
using System.Windows.Forms;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public class SpinnerControl : UserControl
	{
		private int lines   = 8;
		private int current = 0;
        private Timer timer;

		public SpinnerControl()
		{
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}

		public int Lines
        {
			get
            {
                return lines;
            }
			set
            {
				this.lines = value;
			}
		}

		public void Start()
		{
			timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
		}

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.current >= this.lines - 1)
            {
                this.current = 0;
            }
            else
            {
                this.current++;
            }
            Invalidate();
        }

		public void Stop ()
		{
            timer.Stop();
		}

        protected override void OnPaint(PaintEventArgs e)
        {
            double x, y;
            double radius;
            double half;
            int i;

            x = Width / 2;
            y = Height / 2;
            radius = Math.Min(Width / 2, Height / 2) - 5;
            half = lines / 2;

            for (i = 0; i < lines; i++)
            {
                double inset = 0.7 * radius;
                double t = (double)((i + lines - current) % lines) / lines;

                Color c = Color.FromArgb((int)(t * 255), 0, 0, 0);
                Pen pen = new Pen(c);
                pen.Width = 2;

                PointF start = new PointF((float)(x + (radius - inset) * Math.Cos(i * Math.PI / half)),
                           (float)(y + (radius - inset) * Math.Sin(i * Math.PI / half)));

                PointF end = new PointF((float)(x + radius * Math.Cos(i * Math.PI / half)),
                           (float)(y + radius * Math.Sin(i * Math.PI / half)));

                e.Graphics.DrawLine(pen, start, end);
                pen.Dispose();
            }

            base.OnPaint(e);
        }
	}
}
