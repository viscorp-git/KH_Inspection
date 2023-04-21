namespace KH_Inspection
{
    partial class frm_Loader
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
            this.pgB_Loading = new System.Windows.Forms.ProgressBar();
            this.lb_Text = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgB_Loading
            // 
            this.pgB_Loading.Location = new System.Drawing.Point(12, 58);
            this.pgB_Loading.Name = "pgB_Loading";
            this.pgB_Loading.Size = new System.Drawing.Size(493, 29);
            this.pgB_Loading.TabIndex = 0;
            // 
            // lb_Text
            // 
            this.lb_Text.AutoSize = true;
            this.lb_Text.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_Text.ForeColor = System.Drawing.Color.White;
            this.lb_Text.Location = new System.Drawing.Point(12, 18);
            this.lb_Text.Name = "lb_Text";
            this.lb_Text.Size = new System.Drawing.Size(65, 25);
            this.lb_Text.TabIndex = 1;
            this.lb_Text.Text = "label1";
            // 
            // frm_Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(517, 99);
            this.Controls.Add(this.lb_Text);
            this.Controls.Add(this.pgB_Loading);
            this.Name = "frm_Loader";
            this.Text = "frm_Loader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgB_Loading;
        private System.Windows.Forms.Label lb_Text;
    }
}