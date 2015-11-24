namespace EndClothing
{
    partial class Form1
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
            this.bn_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bn_start
            // 
            this.bn_start.Location = new System.Drawing.Point(445, 118);
            this.bn_start.Name = "bn_start";
            this.bn_start.Size = new System.Drawing.Size(422, 205);
            this.bn_start.TabIndex = 0;
            this.bn_start.Text = "START";
            this.bn_start.UseVisualStyleBackColor = true;
            this.bn_start.Click += new System.EventHandler(this.bn_start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 477);
            this.Controls.Add(this.bn_start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bn_start;
    }
}

