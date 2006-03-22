// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace ICSharpCode.CodeAnalysis
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
			System.Windows.Forms.SplitContainer splitContainer1;
			System.Windows.Forms.Panel panel1;
			this.ruleLabel = new System.Windows.Forms.Label();
			this.warningOrErrorLabel = new System.Windows.Forms.Label();
			this.enableCheckBox = new System.Windows.Forms.CheckBox();
			this.ruleTreeView = new System.Windows.Forms.TreeView();
			this.changeRuleAssembliesButton = new System.Windows.Forms.Button();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			panel1 = new System.Windows.Forms.Panel();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.BackColor = System.Drawing.SystemColors.ControlDark;
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			splitContainer1.Location = new System.Drawing.Point(1, 1);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(this.ruleLabel);
			splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(this.warningOrErrorLabel);
			splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			splitContainer1.Size = new System.Drawing.Size(375, 17);
			splitContainer1.SplitterDistance = 214;
			splitContainer1.SplitterWidth = 2;
			splitContainer1.TabIndex = 1;
			// 
			// ruleLabel
			// 
			this.ruleLabel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ruleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ruleLabel.Location = new System.Drawing.Point(0, 0);
			this.ruleLabel.Name = "ruleLabel";
			this.ruleLabel.Size = new System.Drawing.Size(214, 17);
			this.ruleLabel.TabIndex = 0;
			this.ruleLabel.Text = "${res:ICSharpCode.CodeAnalysis.Rule}";
			// 
			// warningOrErrorLabel
			// 
			this.warningOrErrorLabel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.warningOrErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.warningOrErrorLabel.Location = new System.Drawing.Point(0, 0);
			this.warningOrErrorLabel.Name = "warningOrErrorLabel";
			this.warningOrErrorLabel.Size = new System.Drawing.Size(159, 17);
			this.warningOrErrorLabel.TabIndex = 0;
			this.warningOrErrorLabel.Text = "${res:ICSharpCode.CodeAnalysis.ProjectOptions.WarningOrError}";
			// 
			// panel1
			// 
			panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			panel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
			panel1.Controls.Add(splitContainer1);
			panel1.Location = new System.Drawing.Point(15, 57);
			panel1.Name = "panel1";
			panel1.Padding = new System.Windows.Forms.Padding(1);
			panel1.Size = new System.Drawing.Size(377, 19);
			panel1.TabIndex = 3;
			// 
			// enableCheckBox
			// 
			this.enableCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.enableCheckBox.Location = new System.Drawing.Point(15, 4);
			this.enableCheckBox.Name = "enableCheckBox";
			this.enableCheckBox.Size = new System.Drawing.Size(376, 24);
			this.enableCheckBox.TabIndex = 0;
			this.enableCheckBox.Text = "${res:ICSharpCode.CodeAnalysis.ProjectOptions.RunOnBuild}";
			this.enableCheckBox.UseVisualStyleBackColor = true;
			// 
			// ruleTreeView
			// 
			this.ruleTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.ruleTreeView.CheckBoxes = true;
			this.ruleTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.ruleTreeView.Location = new System.Drawing.Point(15, 75);
			this.ruleTreeView.Name = "ruleTreeView";
			this.ruleTreeView.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ruleTreeView.Size = new System.Drawing.Size(377, 166);
			this.ruleTreeView.TabIndex = 2;
			this.ruleTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.RuleTreeViewAfterCheck);
			this.ruleTreeView.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.RuleTreeViewDrawNode);
			this.ruleTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RuleTreeViewMouseDown);
			// 
			// changeRuleAssembliesButton
			// 
			this.changeRuleAssembliesButton.AutoSize = true;
			this.changeRuleAssembliesButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.changeRuleAssembliesButton.Location = new System.Drawing.Point(15, 28);
			this.changeRuleAssembliesButton.Name = "changeRuleAssembliesButton";
			this.changeRuleAssembliesButton.Size = new System.Drawing.Size(387, 23);
			this.changeRuleAssembliesButton.TabIndex = 4;
			this.changeRuleAssembliesButton.Text = "${res:ICSharpCode.CodeAnalysis.ProjectOptions.AddRemoveRuleAssembly}";
			this.changeRuleAssembliesButton.UseVisualStyleBackColor = true;
			this.changeRuleAssembliesButton.Click += new System.EventHandler(this.ChangeRuleAssembliesButtonClick);
			// 
			// AnalysisProjectOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.changeRuleAssembliesButton);
			this.Controls.Add(this.ruleTreeView);
			this.Controls.Add(this.enableCheckBox);
			this.Controls.Add(panel1);
			this.Name = "AnalysisProjectOptions";
			this.Size = new System.Drawing.Size(395, 244);
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.ResumeLayout(false);
			panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button changeRuleAssembliesButton;
		private System.Windows.Forms.Label warningOrErrorLabel;
		private System.Windows.Forms.TreeView ruleTreeView;
		private System.Windows.Forms.Label ruleLabel;
		private System.Windows.Forms.CheckBox enableCheckBox;
	}
}
