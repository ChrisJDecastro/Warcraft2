namespace Demo
{
    partial class SoundOptionsMenu
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
            this.volMaster = new System.Windows.Forms.TrackBar();
            this.labMaster = new System.Windows.Forms.Label();
            this.labMusic = new System.Windows.Forms.Label();
            this.volBGM = new System.Windows.Forms.TrackBar();
            this.labEffects = new System.Windows.Forms.Label();
            this.volSFX = new System.Windows.Forms.TrackBar();
            this.numMaster = new System.Windows.Forms.Label();
            this.numBGM = new System.Windows.Forms.Label();
            this.numSFX = new System.Windows.Forms.Label();
            this.butEsc = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.volMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volBGM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volSFX)).BeginInit();
            this.SuspendLayout();
            // 
            // volMaster
            // 
            this.volMaster.Location = new System.Drawing.Point(144, 18);
            this.volMaster.Maximum = 100;
            this.volMaster.Name = "volMaster";
            this.volMaster.Size = new System.Drawing.Size(250, 45);
            this.volMaster.TabIndex = 0;
            this.volMaster.Value = 100;
            this.volMaster.Scroll += new System.EventHandler(this.volMaster_Scroll);
            // 
            // labMaster
            // 
            this.labMaster.AutoSize = true;
            this.labMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labMaster.Location = new System.Drawing.Point(12, 18);
            this.labMaster.Name = "labMaster";
            this.labMaster.Size = new System.Drawing.Size(114, 37);
            this.labMaster.TabIndex = 1;
            this.labMaster.Text = "Master";
            // 
            // labMusic
            // 
            this.labMusic.AutoSize = true;
            this.labMusic.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labMusic.Location = new System.Drawing.Point(12, 85);
            this.labMusic.Name = "labMusic";
            this.labMusic.Size = new System.Drawing.Size(100, 37);
            this.labMusic.TabIndex = 3;
            this.labMusic.Text = "Music";
            // 
            // volBGM
            // 
            this.volBGM.Location = new System.Drawing.Point(144, 85);
            this.volBGM.Maximum = 100;
            this.volBGM.Name = "volBGM";
            this.volBGM.Size = new System.Drawing.Size(250, 45);
            this.volBGM.TabIndex = 2;
            this.volBGM.Value = 100;
            this.volBGM.Scroll += new System.EventHandler(this.volBGM_Scroll);
            // 
            // labEffects
            // 
            this.labEffects.AutoSize = true;
            this.labEffects.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labEffects.Location = new System.Drawing.Point(12, 154);
            this.labEffects.Name = "labEffects";
            this.labEffects.Size = new System.Drawing.Size(114, 37);
            this.labEffects.TabIndex = 5;
            this.labEffects.Text = "Effects";
            // 
            // volSFX
            // 
            this.volSFX.Location = new System.Drawing.Point(144, 154);
            this.volSFX.Maximum = 100;
            this.volSFX.Name = "volSFX";
            this.volSFX.Size = new System.Drawing.Size(250, 45);
            this.volSFX.TabIndex = 4;
            this.volSFX.Value = 100;
            this.volSFX.Scroll += new System.EventHandler(this.volSFX_Scroll);
            // 
            // numMaster
            // 
            this.numMaster.AutoSize = true;
            this.numMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numMaster.Location = new System.Drawing.Point(406, 18);
            this.numMaster.Name = "numMaster";
            this.numMaster.Size = new System.Drawing.Size(69, 37);
            this.numMaster.TabIndex = 6;
            this.numMaster.Text = "100";
            // 
            // numBGM
            // 
            this.numBGM.AutoSize = true;
            this.numBGM.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numBGM.Location = new System.Drawing.Point(406, 85);
            this.numBGM.Name = "numBGM";
            this.numBGM.Size = new System.Drawing.Size(69, 37);
            this.numBGM.TabIndex = 7;
            this.numBGM.Text = "100";
            // 
            // numSFX
            // 
            this.numSFX.AutoSize = true;
            this.numSFX.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSFX.Location = new System.Drawing.Point(406, 154);
            this.numSFX.Name = "numSFX";
            this.numSFX.Size = new System.Drawing.Size(69, 37);
            this.numSFX.TabIndex = 8;
            this.numSFX.Text = "100";
            // 
            // butEsc
            // 
            this.butEsc.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butEsc.Location = new System.Drawing.Point(144, 368);
            this.butEsc.Name = "butEsc";
            this.butEsc.Size = new System.Drawing.Size(165, 43);
            this.butEsc.TabIndex = 9;
            this.butEsc.Text = "ESCAPE";
            this.butEsc.UseVisualStyleBackColor = true;
            this.butEsc.Click += new System.EventHandler(this.butEsc_Click);
            // 
            // SoundOptionsMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 450);
            this.Controls.Add(this.butEsc);
            this.Controls.Add(this.numSFX);
            this.Controls.Add(this.numBGM);
            this.Controls.Add(this.numMaster);
            this.Controls.Add(this.labEffects);
            this.Controls.Add(this.volSFX);
            this.Controls.Add(this.labMusic);
            this.Controls.Add(this.volBGM);
            this.Controls.Add(this.labMaster);
            this.Controls.Add(this.volMaster);
            this.Name = "SoundOptionsMenu";
            this.Text = "SoundOptionsMenu";
            ((System.ComponentModel.ISupportInitialize)(this.volMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volBGM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volSFX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar volMaster;
        private System.Windows.Forms.Label labMaster;
        private System.Windows.Forms.Label labMusic;
        private System.Windows.Forms.TrackBar volBGM;
        private System.Windows.Forms.Label labEffects;
        private System.Windows.Forms.TrackBar volSFX;
        private System.Windows.Forms.Label numMaster;
        private System.Windows.Forms.Label numBGM;
        private System.Windows.Forms.Label numSFX;
        private System.Windows.Forms.Button butEsc;
    }
}