using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;

namespace SDRBlocks.UI
{
    /// <summary>
    /// Implements a line/bar-type power spectrum analyzer.
    /// </summary>
    public class SpectrumAnalyzer : SpectrumBase
    {
        public SpectrumAnalyzer()
        {
            this.AxisMargin = 25;
            this.DisplayRange = 80;
            this.DisplayOffset = 0;
            this.RefreshBitmaps();
        }

        #region SpectrumBase overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.disposed)
                {
                    this.disposed = true;
                }
            }
            base.Dispose(disposing);
        }

        protected override void AllocateBuffers()
        {
            base.AllocateBuffers();
        }

        protected override void DisposeBuffers()
        {
            base.DisposeBuffers();
        }

        protected override void UpdateSpectrum(IntPtr powerSpectrum, int length)
        {
        }

        protected override void DrawBackground()
        {
            // Draw the power grid
            using (Font labelFont = new Font("Arial", 8f))
            using (SolidBrush labelFontBrush = new SolidBrush(Color.Silver))
            using (Pen gridPen = new Pen(Color.FromArgb(80, 80, 80)))
            {
                gridPen.DashStyle = DashStyle.Dash;

                // Power axis lines
                int gridLineCount = this.DisplayRange / 10;
                float yIncrement = (ClientRectangle.Height - 2 * this.AxisMargin) / (float)gridLineCount;
                for (int i = 1; i <= gridLineCount; i++)
                {
                    int y = (int)(ClientRectangle.Height - this.AxisMargin - i * yIncrement);
                    this.backGraphics.DrawLine(gridPen,
                        this.AxisMargin, y,
                        ClientRectangle.Width - this.AxisMargin, y);
                }
                // Power axis labels
                for (var i = 0; i <= gridLineCount; i++)
                {
                    string dB = (this.DisplayOffset - (gridLineCount - i) * 10).ToString();
                    var sizeF = this.backGraphics.MeasureString(dB, labelFont);
                    var width = sizeF.Width;
                    var height = sizeF.Height;
                    this.backGraphics.DrawString(dB, labelFont, labelFontBrush,
                        AxisMargin - width - 3, ClientRectangle.Height - AxisMargin - i * yIncrement - height / 2f);
                }
            }

            // Draw horizontal and vertical axes
            using (var axisPen = new Pen(Color.DarkGray))
            {
                this.backGraphics.DrawLine(axisPen,
                    this.AxisMargin, this.AxisMargin,
                    this.AxisMargin, ClientRectangle.Height - AxisMargin);
                this.backGraphics.DrawLine(axisPen,
                    this.AxisMargin, ClientRectangle.Height - this.AxisMargin,
                    ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin);
            }
        }

        #endregion

        #region Implementation details

        private bool disposed;
        private byte[] spectrum;

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SpectrumAnalyzer
            // 
            this.Name = "SpectrumAnalyzer";
            this.Size = new System.Drawing.Size(212, 174);
            this.ResumeLayout(false);

        }
    }
}
