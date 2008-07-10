// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="${USER} Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>
namespace TextEditorAnchors
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textEditorControl = new ICSharpCode.TextEditor.TextEditorControl();
			this.anchorListBox = new System.Windows.Forms.ListBox();
			this.addAnchorButton = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.textEditorControl);
			this.splitContainer1.Panel1MinSize = 100;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.anchorListBox);
			this.splitContainer1.Panel2.Controls.Add(this.addAnchorButton);
			this.splitContainer1.Panel2MinSize = 50;
			this.splitContainer1.Size = new System.Drawing.Size(556, 322);
			this.splitContainer1.SplitterDistance = 340;
			this.splitContainer1.TabIndex = 1;
			// 
			// textEditorControl
			// 
			this.textEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEditorControl.IsReadOnly = false;
			this.textEditorControl.Location = new System.Drawing.Point(0, 0);
			this.textEditorControl.Name = "textEditorControl";
			this.textEditorControl.Size = new System.Drawing.Size(340, 322);
			this.textEditorControl.TabIndex = 0;
			this.textEditorControl.Text = "Dies\r\nist ein\r\nmehrzeiliger\r\nText!";
			this.textEditorControl.TextChanged += new System.EventHandler(this.TextEditorControlTextChanged);
			// 
			// anchorListBox
			// 
			this.anchorListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.anchorListBox.FormattingEnabled = true;
			this.anchorListBox.IntegralHeight = false;
			this.anchorListBox.Location = new System.Drawing.Point(2, 32);
			this.anchorListBox.Name = "anchorListBox";
			this.anchorListBox.Size = new System.Drawing.Size(198, 278);
			this.anchorListBox.TabIndex = 2;
			this.anchorListBox.DoubleClick += new System.EventHandler(this.AnchorListBoxDoubleClick);
			this.anchorListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnchorListBoxKeyDown);
			// 
			// addAnchorButton
			// 
			this.addAnchorButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.addAnchorButton.Location = new System.Drawing.Point(2, 3);
			this.addAnchorButton.Name = "addAnchorButton";
			this.addAnchorButton.Size = new System.Drawing.Size(198, 23);
			this.addAnchorButton.TabIndex = 0;
			this.addAnchorButton.Text = "Add Anchor";
			this.addAnchorButton.UseVisualStyleBackColor = true;
			this.addAnchorButton.Click += new System.EventHandler(this.AddAnchorButtonClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(556, 322);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "TextEditorAnchors";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button addAnchorButton;
		private System.Windows.Forms.ListBox anchorListBox;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ICSharpCode.TextEditor.TextEditorControl textEditorControl;
	}
}
