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
			this.groupBoxStepping = new System.Windows.Forms.GroupBox();
			this.stepOverFieldAccessProperties = new System.Windows.Forms.CheckBox();
			this.stepOverSingleLineProperties = new System.Windows.Forms.CheckBox();
			this.stepOverAllProperties = new System.Windows.Forms.CheckBox();
			this.stepOverDebuggerAttributes = new System.Windows.Forms.CheckBox();
			this.stepOverNoSymbols = new System.Windows.Forms.CheckBox();
			this.enableJustMyCode = new System.Windows.Forms.CheckBox();
			this.groupBoxStepping.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxStepping
			// 
			this.groupBoxStepping.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxStepping.Controls.Add(this.stepOverFieldAccessProperties);
			this.groupBoxStepping.Controls.Add(this.stepOverSingleLineProperties);
			this.groupBoxStepping.Controls.Add(this.stepOverAllProperties);
			this.groupBoxStepping.Controls.Add(this.stepOverDebuggerAttributes);
			this.groupBoxStepping.Controls.Add(this.stepOverNoSymbols);
			this.groupBoxStepping.Controls.Add(this.enableJustMyCode);
			this.groupBoxStepping.Location = new System.Drawing.Point(5, 9);
			this.groupBoxStepping.Name = "groupBoxStepping";
			this.groupBoxStepping.Size = new System.Drawing.Size(680, 204);
			this.groupBoxStepping.TabIndex = 0;
			this.groupBoxStepping.TabStop = false;
			this.groupBoxStepping.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping}";
			// 
			// stepOverFieldAccessProperties
			// 
			this.stepOverFieldAccessProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stepOverFieldAccessProperties.Location = new System.Drawing.Point(6, 175);
			this.stepOverFieldAccessProperties.Name = "stepOverFieldAccessProperties";
			this.stepOverFieldAccessProperties.Size = new System.Drawing.Size(668, 24);
			this.stepOverFieldAccessProperties.TabIndex = 0;
			this.stepOverFieldAccessProperties.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.StepOverFieldAccessProperties}" +
			"";
			this.stepOverFieldAccessProperties.UseVisualStyleBackColor = true;
			// 
			// stepOverSingleLineProperties
			// 
			this.stepOverSingleLineProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stepOverSingleLineProperties.Location = new System.Drawing.Point(6, 145);
			this.stepOverSingleLineProperties.Name = "stepOverSingleLineProperties";
			this.stepOverSingleLineProperties.Size = new System.Drawing.Size(668, 24);
			this.stepOverSingleLineProperties.TabIndex = 0;
			this.stepOverSingleLineProperties.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.StepOverSingleLineProperties}";
			this.stepOverSingleLineProperties.UseVisualStyleBackColor = true;
			// 
			// stepOverAllProperties
			// 
			this.stepOverAllProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stepOverAllProperties.Location = new System.Drawing.Point(6, 115);
			this.stepOverAllProperties.Name = "stepOverAllProperties";
			this.stepOverAllProperties.Size = new System.Drawing.Size(668, 24);
			this.stepOverAllProperties.TabIndex = 0;
			this.stepOverAllProperties.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.StepOverAllProperties}";
			this.stepOverAllProperties.UseVisualStyleBackColor = true;
			// 
			// stepOverDebuggerAttributes
			// 
			this.stepOverDebuggerAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stepOverDebuggerAttributes.Location = new System.Drawing.Point(6, 85);
			this.stepOverDebuggerAttributes.Name = "stepOverDebuggerAttributes";
			this.stepOverDebuggerAttributes.Size = new System.Drawing.Size(668, 24);
			this.stepOverDebuggerAttributes.TabIndex = 0;
			this.stepOverDebuggerAttributes.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.StepOverDebuggerAttributes}";
			this.stepOverDebuggerAttributes.UseVisualStyleBackColor = true;
			// 
			// stepOverNoSymbols
			// 
			this.stepOverNoSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stepOverNoSymbols.Location = new System.Drawing.Point(6, 55);
			this.stepOverNoSymbols.Name = "stepOverNoSymbols";
			this.stepOverNoSymbols.Size = new System.Drawing.Size(668, 24);
			this.stepOverNoSymbols.TabIndex = 0;
			this.stepOverNoSymbols.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.StepOverNoSymbols}";
			this.stepOverNoSymbols.UseVisualStyleBackColor = true;
			// 
			// enableJustMyCode
			// 
			this.enableJustMyCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.enableJustMyCode.Location = new System.Drawing.Point(6, 25);
			this.enableJustMyCode.Name = "enableJustMyCode";
			this.enableJustMyCode.Size = new System.Drawing.Size(668, 24);
			this.enableJustMyCode.TabIndex = 0;
			this.enableJustMyCode.Text = "${res:Dialog.Options.IDEOptions.Debugging.Stepping.EnableJustMyCode}";
			this.enableJustMyCode.UseVisualStyleBackColor = true;
			// 
			// DebuggingOptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBoxStepping);
			this.Name = "DebuggingOptionsPanel";
			this.Size = new System.Drawing.Size(688, 300);
			this.groupBoxStepping.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox enableJustMyCode;
		private System.Windows.Forms.CheckBox stepOverNoSymbols;
		private System.Windows.Forms.CheckBox stepOverDebuggerAttributes;
		private System.Windows.Forms.CheckBox stepOverAllProperties;
		private System.Windows.Forms.CheckBox stepOverSingleLineProperties;
		private System.Windows.Forms.CheckBox stepOverFieldAccessProperties;
		private System.Windows.Forms.GroupBox groupBoxStepping;
	}
}
