namespace Ascension_Calculator
{
    partial class OptionsDlg
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbRock = new System.Windows.Forms.PictureBox();
            this.pbParchment = new System.Windows.Forms.PictureBox();
            this.pbMarble = new System.Windows.Forms.PictureBox();
            this.radExportBackgroundRock = new System.Windows.Forms.RadioButton();
            this.radExportBackgroundParchment = new System.Windows.Forms.RadioButton();
            this.radExportBackgroundMarble = new System.Windows.Forms.RadioButton();
            this.pbExportGradientRight = new System.Windows.Forms.PictureBox();
            this.pbExportGradientLeft = new System.Windows.Forms.PictureBox();
            this.radExportGradient = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCheckForUpdates = new System.Windows.Forms.CheckBox();
            this.numLevelLimit = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radAbilitiesTalents = new System.Windows.Forms.RadioButton();
            this.radTalentsAbilities = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbRock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParchment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMarble)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportGradientRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportGradientLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLevelLimit)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(176, 175);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(257, 175);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pbRock
            // 
            this.pbRock.Image = global::Ascension_Calculator.Properties.Resources.tiled_rock;
            this.pbRock.Location = new System.Drawing.Point(104, 88);
            this.pbRock.Name = "pbRock";
            this.pbRock.Size = new System.Drawing.Size(62, 17);
            this.pbRock.TabIndex = 8;
            this.pbRock.TabStop = false;
            // 
            // pbParchment
            // 
            this.pbParchment.Image = global::Ascension_Calculator.Properties.Resources.tiled_parchment;
            this.pbParchment.Location = new System.Drawing.Point(104, 65);
            this.pbParchment.Name = "pbParchment";
            this.pbParchment.Size = new System.Drawing.Size(62, 17);
            this.pbParchment.TabIndex = 7;
            this.pbParchment.TabStop = false;
            // 
            // pbMarble
            // 
            this.pbMarble.Image = global::Ascension_Calculator.Properties.Resources.tiled_marble;
            this.pbMarble.Location = new System.Drawing.Point(104, 42);
            this.pbMarble.Name = "pbMarble";
            this.pbMarble.Size = new System.Drawing.Size(62, 17);
            this.pbMarble.TabIndex = 6;
            this.pbMarble.TabStop = false;
            // 
            // radExportBackgroundRock
            // 
            this.radExportBackgroundRock.AutoSize = true;
            this.radExportBackgroundRock.Location = new System.Drawing.Point(6, 88);
            this.radExportBackgroundRock.Name = "radExportBackgroundRock";
            this.radExportBackgroundRock.Size = new System.Drawing.Size(51, 17);
            this.radExportBackgroundRock.TabIndex = 3;
            this.radExportBackgroundRock.TabStop = true;
            this.radExportBackgroundRock.Text = "Rock";
            this.radExportBackgroundRock.UseVisualStyleBackColor = true;
            // 
            // radExportBackgroundParchment
            // 
            this.radExportBackgroundParchment.AutoSize = true;
            this.radExportBackgroundParchment.Location = new System.Drawing.Point(6, 65);
            this.radExportBackgroundParchment.Name = "radExportBackgroundParchment";
            this.radExportBackgroundParchment.Size = new System.Drawing.Size(76, 17);
            this.radExportBackgroundParchment.TabIndex = 2;
            this.radExportBackgroundParchment.TabStop = true;
            this.radExportBackgroundParchment.Text = "Parchment";
            this.radExportBackgroundParchment.UseVisualStyleBackColor = true;
            // 
            // radExportBackgroundMarble
            // 
            this.radExportBackgroundMarble.AutoSize = true;
            this.radExportBackgroundMarble.Location = new System.Drawing.Point(6, 42);
            this.radExportBackgroundMarble.Name = "radExportBackgroundMarble";
            this.radExportBackgroundMarble.Size = new System.Drawing.Size(57, 17);
            this.radExportBackgroundMarble.TabIndex = 1;
            this.radExportBackgroundMarble.TabStop = true;
            this.radExportBackgroundMarble.Text = "Marble";
            this.radExportBackgroundMarble.UseVisualStyleBackColor = true;
            // 
            // pbExportGradientRight
            // 
            this.pbExportGradientRight.BackColor = System.Drawing.Color.White;
            this.pbExportGradientRight.Location = new System.Drawing.Point(126, 20);
            this.pbExportGradientRight.Name = "pbExportGradientRight";
            this.pbExportGradientRight.Size = new System.Drawing.Size(16, 16);
            this.pbExportGradientRight.TabIndex = 2;
            this.pbExportGradientRight.TabStop = false;
            this.pbExportGradientRight.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExportGradient_MouseClick);
            // 
            // pbExportGradientLeft
            // 
            this.pbExportGradientLeft.BackColor = System.Drawing.Color.White;
            this.pbExportGradientLeft.Location = new System.Drawing.Point(104, 20);
            this.pbExportGradientLeft.Name = "pbExportGradientLeft";
            this.pbExportGradientLeft.Size = new System.Drawing.Size(16, 16);
            this.pbExportGradientLeft.TabIndex = 1;
            this.pbExportGradientLeft.TabStop = false;
            this.pbExportGradientLeft.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExportGradient_MouseClick);
            // 
            // radExportGradient
            // 
            this.radExportGradient.AutoSize = true;
            this.radExportGradient.Location = new System.Drawing.Point(6, 19);
            this.radExportGradient.Name = "radExportGradient";
            this.radExportGradient.Size = new System.Drawing.Size(92, 17);
            this.radExportGradient.TabIndex = 0;
            this.radExportGradient.TabStop = true;
            this.radExportGradient.Text = "Color Gradient";
            this.radExportGradient.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Level Limit:";
            // 
            // chkCheckForUpdates
            // 
            this.chkCheckForUpdates.AutoSize = true;
            this.chkCheckForUpdates.Location = new System.Drawing.Point(6, 27);
            this.chkCheckForUpdates.Name = "chkCheckForUpdates";
            this.chkCheckForUpdates.Size = new System.Drawing.Size(118, 17);
            this.chkCheckForUpdates.TabIndex = 1;
            this.chkCheckForUpdates.Text = "Check For Updates";
            this.chkCheckForUpdates.UseVisualStyleBackColor = true;
            // 
            // numLevelLimit
            // 
            this.numLevelLimit.Location = new System.Drawing.Point(69, 1);
            this.numLevelLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLevelLimit.Name = "numLevelLimit";
            this.numLevelLimit.Size = new System.Drawing.Size(57, 20);
            this.numLevelLimit.TabIndex = 0;
            this.numLevelLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numLevelLimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(320, 157);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.chkCheckForUpdates);
            this.tabPage1.Controls.Add(this.numLevelLimit);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(312, 131);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Application";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(312, 131);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Interface";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radAbilitiesTalents);
            this.groupBox2.Controls.Add(this.radTalentsAbilities);
            this.groupBox2.Location = new System.Drawing.Point(189, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(117, 119);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Skill Panel Layout";
            // 
            // radAbilitiesTalents
            // 
            this.radAbilitiesTalents.AutoSize = true;
            this.radAbilitiesTalents.Location = new System.Drawing.Point(6, 42);
            this.radAbilitiesTalents.Name = "radAbilitiesTalents";
            this.radAbilitiesTalents.Size = new System.Drawing.Size(106, 17);
            this.radAbilitiesTalents.TabIndex = 1;
            this.radAbilitiesTalents.TabStop = true;
            this.radAbilitiesTalents.Text = "Abilities / Talents";
            this.radAbilitiesTalents.UseVisualStyleBackColor = true;
            // 
            // radTalentsAbilities
            // 
            this.radTalentsAbilities.AutoSize = true;
            this.radTalentsAbilities.Location = new System.Drawing.Point(6, 19);
            this.radTalentsAbilities.Name = "radTalentsAbilities";
            this.radTalentsAbilities.Size = new System.Drawing.Size(106, 17);
            this.radTalentsAbilities.TabIndex = 0;
            this.radTalentsAbilities.TabStop = true;
            this.radTalentsAbilities.Text = "Talents / Abilities";
            this.radTalentsAbilities.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbRock);
            this.groupBox1.Controls.Add(this.radExportGradient);
            this.groupBox1.Controls.Add(this.radExportBackgroundParchment);
            this.groupBox1.Controls.Add(this.pbParchment);
            this.groupBox1.Controls.Add(this.radExportBackgroundMarble);
            this.groupBox1.Controls.Add(this.pbExportGradientLeft);
            this.groupBox1.Controls.Add(this.radExportBackgroundRock);
            this.groupBox1.Controls.Add(this.pbMarble);
            this.groupBox1.Controls.Add(this.pbExportGradientRight);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 119);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Share Image Background";
            // 
            // OptionsDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 205);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.pbRock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParchment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMarble)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportGradientRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbExportGradientLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLevelLimit)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pbRock;
        private System.Windows.Forms.PictureBox pbParchment;
        private System.Windows.Forms.PictureBox pbMarble;
        private System.Windows.Forms.RadioButton radExportBackgroundRock;
        private System.Windows.Forms.RadioButton radExportBackgroundParchment;
        private System.Windows.Forms.RadioButton radExportBackgroundMarble;
        private System.Windows.Forms.PictureBox pbExportGradientRight;
        private System.Windows.Forms.PictureBox pbExportGradientLeft;
        private System.Windows.Forms.RadioButton radExportGradient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCheckForUpdates;
        private System.Windows.Forms.NumericUpDown numLevelLimit;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.RadioButton radAbilitiesTalents;
        private System.Windows.Forms.RadioButton radTalentsAbilities;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}