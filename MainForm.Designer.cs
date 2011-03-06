namespace PvPTracker
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.updateTracking = new System.Windows.Forms.Timer(this.components);
			this.OutLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// updateTracking
			// 
			this.updateTracking.Interval = 500;
			this.updateTracking.Tick += new System.EventHandler(this.updateTracking_Tick);
			// 
			// OutLabel
			// 
			this.OutLabel.AutoSize = true;
			this.OutLabel.BackColor = System.Drawing.Color.Black;
			this.OutLabel.Font = new System.Drawing.Font("Arial Black", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.OutLabel.ForeColor = System.Drawing.Color.Red;
			this.OutLabel.Location = new System.Drawing.Point(0, 0);
			this.OutLabel.Name = "OutLabel";
			this.OutLabel.Size = new System.Drawing.Size(41, 45);
			this.OutLabel.TabIndex = 0;
			this.OutLabel.Text = "0";
			this.OutLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(64, 45);
			this.Controls.Add(this.OutLabel);
			this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::PvPTracker.Properties.Settings.Default, "WindowLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Location = global::PvPTracker.Properties.Settings.Default.WindowLocation;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "PvPTracker";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.Black;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer updateTracking;
		private System.Windows.Forms.Label OutLabel;
	}
}

