﻿namespace SDRBlocks
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spectrumAnalyzer1 = new SDRBlocks.UI.SpectrumAnalyzer();
            this.SuspendLayout();
            // 
            // spectrumAnalyzer1
            // 
            this.spectrumAnalyzer1.AxisMargin = 25;
            this.spectrumAnalyzer1.DisplayOffset = 0;
            this.spectrumAnalyzer1.DisplayRange = 80;
            this.spectrumAnalyzer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.spectrumAnalyzer1.Location = new System.Drawing.Point(0, 0);
            this.spectrumAnalyzer1.Name = "spectrumAnalyzer1";
            this.spectrumAnalyzer1.Size = new System.Drawing.Size(687, 169);
            this.spectrumAnalyzer1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 273);
            this.Controls.Add(this.spectrumAnalyzer1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UI.SpectrumAnalyzer spectrumAnalyzer1;
    }
}

