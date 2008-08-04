// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace MattEverson.SourceAnalysis
{
	partial class AnalysisProjectOptions : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.enableCheckBox = new System.Windows.Forms.CheckBox();
			this.modifyStyleCopSettingsButton = new System.Windows.Forms.Button();
			this.browseButton = new System.Windows.Forms.Button();
			this.settingsFileTextBox = new System.Windows.Forms.TextBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// enableCheckBox
			// 
			this.enableCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.enableCheckBox.Location = new System.Drawing.Point(15, 4);
			this.enableCheckBox.Name = "enableCheckBox";
			this.enableCheckBox.Size = new System.Drawing.Size(376, 24);
			this.enableCheckBox.TabIndex = 0;
			this.enableCheckBox.Text = "Run source analysis when compiling";
			this.enableCheckBox.UseVisualStyleBackColor = true;
			// 
			// modifyStyleCopSettingsButton
			// 
			this.modifyStyleCopSettingsButton.Location = new System.Drawing.Point(15, 60);
			this.modifyStyleCopSettingsButton.Name = "modifyStyleCopSettingsButton";
			this.modifyStyleCopSettingsButton.Size = new System.Drawing.Size(150, 23);
			this.modifyStyleCopSettingsButton.TabIndex = 2;
			this.modifyStyleCopSettingsButton.Text = "Modify StyleCop Settings";
			this.modifyStyleCopSettingsButton.UseVisualStyleBackColor = true;
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(349, 31);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(42, 23);
			this.browseButton.TabIndex = 3;
			this.browseButton.Text = "...";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.BrowseButtonClick);
			// 
			// settingsFileTextBox
			// 
			this.settingsFileTextBox.Location = new System.Drawing.Point(15, 34);
			this.settingsFileTextBox.Name = "settingsFileTextBox";
			this.settingsFileTextBox.Size = new System.Drawing.Size(328, 20);
			this.settingsFileTextBox.TabIndex = 4;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "SourceAnalysis";
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "SourceAnalysis files|*.SourceAnalysis|All files|*.*";
			this.openFileDialog1.Title = "Select settings file";
			// 
			// AnalysisProjectOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.settingsFileTextBox);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.modifyStyleCopSettingsButton);
			this.Controls.Add(this.enableCheckBox);
			this.Name = "AnalysisProjectOptions";
			this.Size = new System.Drawing.Size(395, 244);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button modifyStyleCopSettingsButton;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.TextBox settingsFileTextBox;
		private System.Windows.Forms.CheckBox enableCheckBox;
	}
}
