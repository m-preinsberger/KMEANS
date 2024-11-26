namespace KMEANS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PictureIn = new PictureBox();
            PictureOut = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PictureIn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureOut).BeginInit();
            SuspendLayout();
            // 
            // PictureIn
            // 
            PictureIn.BackColor = SystemColors.ActiveCaption;
            PictureIn.Location = new Point(12, 12);
            PictureIn.Name = "PictureIn";
            PictureIn.Size = new Size(511, 538);
            PictureIn.TabIndex = 0;
            PictureIn.TabStop = false;
            PictureIn.Click += PictureIn_Click;
            // 
            // PictureOut
            // 
            PictureOut.BackColor = SystemColors.ControlLight;
            PictureOut.Location = new Point(539, 12);
            PictureOut.Name = "PictureOut";
            PictureOut.Size = new Size(511, 538);
            PictureOut.TabIndex = 1;
            PictureOut.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 562);
            Controls.Add(PictureOut);
            Controls.Add(PictureIn);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)PictureIn).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureOut).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox PictureIn;
        private PictureBox PictureOut;
    }
}
