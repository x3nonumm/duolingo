using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Duolingo
{
    public class RoundedPanel : Panel
    {
        private int borderRadius = 0;

        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value;
                this.Invalidate();
            }
        }

        public RoundedPanel()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (borderRadius > 0)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = borderRadius * 2;

                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    this.Region = new Region(path);

                    // Рисуем границу
                    using (Pen pen = new Pen(this.BackColor, 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }
    }
}