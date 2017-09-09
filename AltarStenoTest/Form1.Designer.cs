namespace AltarStenoTest {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.TestBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.TestBox)).BeginInit();
			this.SuspendLayout();
			// 
			// TestBox
			// 
			this.TestBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TestBox.Location = new System.Drawing.Point(0, 0);
			this.TestBox.Name = "TestBox";
			this.TestBox.Size = new System.Drawing.Size(557, 338);
			this.TestBox.TabIndex = 0;
			this.TestBox.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(557, 338);
			this.Controls.Add(this.TestBox);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.TestBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox TestBox;
	}
}

