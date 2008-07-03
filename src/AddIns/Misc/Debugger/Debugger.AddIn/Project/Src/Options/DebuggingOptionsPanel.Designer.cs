// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class DebuggingOptionsPanel
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
			this.jmc = new System.Windows.Forms.CheckBox();
			this.obeyDebuggerAttributes = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// jmc
			// 
			this.jmc.AutoSize = true;
			this.jmc.Location = new System.Drawing.Point(13, 15);
			this.jmc.Name = "jmc";
			this.jmc.Size = new System.Drawing.Size(491, 24);
			this.jmc.TabIndex = 0;
			this.jmc.Text = "${res:Dialog.Options.IDEOptions.Debugging.EnableJustMyCode}";
			this.jmc.UseVisualStyleBackColor = true;
			// 
			// obeyDebuggerAttributes
			// 
			this.obeyDebuggerAttributes.AutoSize = true;
			this.obeyDebuggerAttributes.Location = new System.Drawing.Point(37, 45);
			this.obeyDebuggerAttributes.Name = "obeyDebuggerAttributes";
			this.obeyDebuggerAttributes.Size = new System.Drawing.Size(530, 24);
			this.obeyDebuggerAttributes.TabIndex = 1;
			this.obeyDebuggerAttributes.Text = "${res:Dialog.Options.IDEOptions.Debugging.ObeyDebuggerAttributes}";
			this.obeyDebuggerAttributes.UseVisualStyleBackColor = true;
			// 
			// DebuggingOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.obeyDebuggerAttributes);
			this.Controls.Add(this.jmc);
			this.Name = "DebuggingOptionsPanel";
			this.Size = new System.Drawing.Size(626, 300);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox obeyDebuggerAttributes;
		private System.Windows.Forms.CheckBox jmc;
	}
}
