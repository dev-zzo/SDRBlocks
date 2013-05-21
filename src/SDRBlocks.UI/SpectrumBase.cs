using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.UI
{
    public class SpectrumBase : UserControl
    {
        public SpectrumBase()
        {
            this.AllocateBuffers();
        }

        #region Public properties

        public int AxisMargin { get; set; }

        /// <summary>
        /// How much of vertical range to display, in dB.
        /// </summary>
        public int DisplayRange
        {
            get { return this.displayRange; }
            set
            {
                // Rounded to 10.
                this.displayRange = (value / 10) * 10;
                this.InvalidateBackground();
            }
        }

        /// <summary>
        /// Where to start the vertical range, in dB.
        /// </summary>
        public int DisplayOffset
        {
            get { return this.displayOffset; }
            set
            {
                // Rounded to 10.
                this.displayOffset = (value / 10) * 10;
                this.InvalidateBackground();
            }
        }

        /// <summary>
        /// Which frequency is at the center on the display (disregarding zoom).
        /// </summary>
        public long CenterFrequency
        {
            get { return this.centerFreq; }
            set
            {
                this.centerFreq = value;
                this.InvalidateBackground();
            }
        }

        /// <summary>
        /// The frequency step for tuning.
        /// </summary>
        public int FrequencyStepSize 
        {
            get { return this.freqStepSize; }
            set
            {
                this.freqStepSize = value;
                this.InvalidateBackground();
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Update the control with the new spectrum data.
        /// </summary>
        /// <param name="powerSpectrum">Pointer to floating-point spectrum buffer</param>
        /// <param name="length">Number of samples in the buffer</param>
        public void Update(IntPtr powerSpectrum, int length)
        {
            // Update the data
            this.UpdateSpectrum(powerSpectrum, length);

            // Cause the foreground to be redrawn
            this.InvalidateForeground();
            this.RefreshBitmaps();
        }

        #endregion

        /// <summary>
        /// Front buffer, used to present the image on paint event.
        /// </summary>
        protected Bitmap foreBuffer;
        protected Bitmap backBuffer;
        protected Graphics backGraphics;

        #region UserControl overrides

        protected override void Dispose(bool disposing)
        {
            this.DisposeBuffers();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ConfigureGraphics(e.Graphics);
            e.Graphics.DrawImageUnscaled(this.foreBuffer, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Prevent default background painting
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                this.DisposeBuffers();
                this.AllocateBuffers();
                this.InvalidateBackground();
                this.RefreshBitmaps();
            }
        }

        #endregion

        #region Overridables

        protected virtual void AllocateBuffers()
        {
            this.foreBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);

            this.backBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            this.backGraphics = Graphics.FromImage(this.backBuffer);
            ConfigureGraphics(this.backGraphics);
        }

        protected virtual void DisposeBuffers()
        {
            this.foreBuffer.Dispose();
            this.backGraphics.Dispose();
            this.backBuffer.Dispose();
        }

        protected virtual void UpdateSpectrum(IntPtr powerSpectrum, int length)
        {
        }

        protected virtual void DrawBackground()
        {
        }

        protected virtual void DrawForeground()
        {
        }

        protected virtual void DrawCursor()
        {
        }

        #endregion

        protected static void ConfigureGraphics(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.High;
        }

        protected void InvalidateBackground()
        {
            this.backgroundInvalid = true;
        }

        protected void InvalidateForeground()
        {
            this.foregroundInvalid = true;
        }

        protected void RefreshBitmaps()
        {
            if (this.backgroundInvalid)
            {
                backGraphics.Clear(Color.Black);
                this.DrawBackground();
            }
            if (this.foregroundInvalid || this.backgroundInvalid)
            {
                this.CopyBackground();
                this.DrawForeground();
                this.DrawCursor();
                this.Invalidate();
            }
            this.foregroundInvalid = false;
            this.backgroundInvalid = false;
        }

        #region Implementation details

        private bool foregroundInvalid;
        private bool backgroundInvalid;
        private int displayRange;
        private int displayOffset;
        private long centerFreq;
        private int freqStepSize;

        private void CopyBackground()
        {
            BitmapData frontData = this.foreBuffer.LockBits(ClientRectangle, ImageLockMode.WriteOnly, this.foreBuffer.PixelFormat);
            BitmapData backData = this.backBuffer.LockBits(ClientRectangle, ImageLockMode.ReadOnly, this.backBuffer.PixelFormat);

            int size = Math.Abs(frontData.Stride) * frontData.Height;
            MemFuncs.MemCopy(frontData.Scan0, backData.Scan0, (UIntPtr)size);

            this.foreBuffer.UnlockBits(frontData);
            this.backBuffer.UnlockBits(backData);
        }

        #endregion
    }
}
