namespace KH_Inspection
{
    partial class Base_frm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_Auto = new System.Windows.Forms.ToolStripButton();
            this.btn_AQ_ON = new System.Windows.Forms.ToolStripButton();
            this.btn_Setting = new System.Windows.Forms.ToolStripButton();
            this.btn_Camera = new System.Windows.Forms.ToolStripButton();
            this.btn_Help = new System.Windows.Forms.ToolStripButton();
            this.btn_Log = new System.Windows.Forms.ToolStripButton();
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.VS2015DarkTheme = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dockPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(950, 563);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Auto,
            this.btn_AQ_ON,
            this.btn_Setting,
            this.btn_Camera,
            this.btn_Help,
            this.btn_Log});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(950, 50);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_Auto
            // 
            this.btn_Auto.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Auto.Image = global::KH_Inspection.Properties.Resources.play;
            this.btn_Auto.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_Auto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Auto.Name = "btn_Auto";
            this.btn_Auto.Size = new System.Drawing.Size(101, 47);
            this.btn_Auto.Text = "AUTO";
            this.btn_Auto.Click += new System.EventHandler(this.btn_Auto_Click);
            // 
            // btn_AQ_ON
            // 
            this.btn_AQ_ON.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_AQ_ON.Image = global::KH_Inspection.Properties.Resources.AQ_ON_2;
            this.btn_AQ_ON.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_AQ_ON.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_AQ_ON.Name = "btn_AQ_ON";
            this.btn_AQ_ON.Size = new System.Drawing.Size(121, 47);
            this.btn_AQ_ON.Text = "A/Q ON";
            this.btn_AQ_ON.Click += new System.EventHandler(this.btn_AQ_ON_Click);
            // 
            // btn_Setting
            // 
            this.btn_Setting.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Setting.Image = global::KH_Inspection.Properties.Resources.Setting;
            this.btn_Setting.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_Setting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Setting.Name = "btn_Setting";
            this.btn_Setting.Size = new System.Drawing.Size(126, 47);
            this.btn_Setting.Text = "SETTING";
            this.btn_Setting.Click += new System.EventHandler(this.btn_Setting_Click);
            // 
            // btn_Camera
            // 
            this.btn_Camera.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Camera.Image = global::KH_Inspection.Properties.Resources.Camera;
            this.btn_Camera.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_Camera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Camera.Name = "btn_Camera";
            this.btn_Camera.Size = new System.Drawing.Size(126, 47);
            this.btn_Camera.Text = "CAMERA";
            this.btn_Camera.Click += new System.EventHandler(this.btn_Camera_Click);
            // 
            // btn_Help
            // 
            this.btn_Help.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Help.Image = global::KH_Inspection.Properties.Resources.Help;
            this.btn_Help.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_Help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Help.Name = "btn_Help";
            this.btn_Help.Size = new System.Drawing.Size(95, 47);
            this.btn_Help.Text = "HELP";
            // 
            // btn_Log
            // 
            this.btn_Log.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Log.Image = global::KH_Inspection.Properties.Resources.Log;
            this.btn_Log.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_Log.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Log.Name = "btn_Log";
            this.btn_Log.Size = new System.Drawing.Size(87, 47);
            this.btn_Log.Text = "LOG";
            this.btn_Log.Click += new System.EventHandler(this.btn_Log_Click);
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.DockBackColor = System.Drawing.Color.Transparent;
            this.dockPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dockPanel1.Location = new System.Drawing.Point(3, 53);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel1.ShowAutoHideContentOnHover = false;
            this.dockPanel1.Size = new System.Drawing.Size(944, 507);
            this.dockPanel1.TabIndex = 2;
            this.dockPanel1.Theme = this.VS2015DarkTheme;
            // 
            // Base_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 563);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Base_frm";
            this.Text = "KH Inspection";
            this.Load += new System.EventHandler(this.Base_frm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_Auto;
        private System.Windows.Forms.ToolStripButton btn_AQ_ON;
        private System.Windows.Forms.ToolStripButton btn_Setting;
        private System.Windows.Forms.ToolStripButton btn_Camera;
        private System.Windows.Forms.ToolStripButton btn_Help;
        private System.Windows.Forms.ToolStripButton btn_Log;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme VS2015DarkTheme;
    }
}

