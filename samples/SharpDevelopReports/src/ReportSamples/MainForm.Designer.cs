// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

namespace ReportSamples
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
			this.previewControl1 = new ICSharpCode.Reports.Core.ReportViewer.PreviewControl();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.formSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.jACToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pullModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.employeesHiredateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.missingConnectionStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mSDESqlExpressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SaleyByYearWithParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pushModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.employeesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.iListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contributorsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contributorsCustomizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contributorsSortedByLastnameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnPDF = new System.Windows.Forms.RadioButton();
			this.btnPrinter = new System.Windows.Forms.RadioButton();
			this.btnPreviewControl = new System.Windows.Forms.RadioButton();
			this.btnReportViewer = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.eventLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// previewControl1
			// 
			this.previewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.previewControl1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.previewControl1.Location = new System.Drawing.Point(34, 83);
			this.previewControl1.Messages = null;
			this.previewControl1.Name = "previewControl1";
			this.previewControl1.Padding = new System.Windows.Forms.Padding(5);
//			this.previewControl1.PageSettings = null;
			this.previewControl1.Size = new System.Drawing.Size(605, 294);
			this.previewControl1.TabIndex = 0;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.formSheetToolStripMenuItem,
									this.pullModelToolStripMenuItem,
									this.pushModelToolStripMenuItem,
									this.iListToolStripMenuItem,
									this.customizedToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menuStrip1.Size = new System.Drawing.Size(671, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// formSheetToolStripMenuItem
			// 
			this.formSheetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.jACToolStripMenuItem});
			this.formSheetToolStripMenuItem.Name = "formSheetToolStripMenuItem";
			this.formSheetToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
			this.formSheetToolStripMenuItem.Text = "FormSheet";
			// 
			// jACToolStripMenuItem
			// 
			this.jACToolStripMenuItem.Name = "jACToolStripMenuItem";
			this.jACToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
			this.jACToolStripMenuItem.Text = "JAC";
			this.jACToolStripMenuItem.Click += new System.EventHandler(this.FormSheetToolStripMenuItemClick);
			// 
			// pullModelToolStripMenuItem
			// 
			this.pullModelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.employeesHiredateToolStripMenuItem,
									this.missingConnectionStringToolStripMenuItem,
									this.mSDESqlExpressToolStripMenuItem});
			this.pullModelToolStripMenuItem.Name = "pullModelToolStripMenuItem";
			this.pullModelToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
			this.pullModelToolStripMenuItem.Text = "PullModel";
			// 
			// employeesHiredateToolStripMenuItem
			// 
			this.employeesHiredateToolStripMenuItem.Name = "employeesHiredateToolStripMenuItem";
			this.employeesHiredateToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.employeesHiredateToolStripMenuItem.Text = "StandardPullModel-Reports";
			this.employeesHiredateToolStripMenuItem.Click += new System.EventHandler(this.StandartPullModelClick);
			// 
			// missingConnectionStringToolStripMenuItem
			// 
			this.missingConnectionStringToolStripMenuItem.Name = "missingConnectionStringToolStripMenuItem";
			this.missingConnectionStringToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.missingConnectionStringToolStripMenuItem.Text = "ProviderIndependent";
			this.missingConnectionStringToolStripMenuItem.Click += new System.EventHandler(this.ProviderIndependentClick);
			// 
			// mSDESqlExpressToolStripMenuItem
			// 
			this.mSDESqlExpressToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.SaleyByYearWithParametersToolStripMenuItem});
			this.mSDESqlExpressToolStripMenuItem.Name = "mSDESqlExpressToolStripMenuItem";
			this.mSDESqlExpressToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.mSDESqlExpressToolStripMenuItem.Text = "MSDE_SqlExpress";
			// 
			// SaleyByYearWithParametersToolStripMenuItem
			// 
			this.SaleyByYearWithParametersToolStripMenuItem.Name = "SaleyByYearWithParametersToolStripMenuItem";
			this.SaleyByYearWithParametersToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.SaleyByYearWithParametersToolStripMenuItem.Text = "Saley by Year with Parameters";
			this.SaleyByYearWithParametersToolStripMenuItem.Click += new System.EventHandler(this.SaleyByYearWithParameters);
			// 
			// pushModelToolStripMenuItem
			// 
			this.pushModelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.employeesToolStripMenuItem});
			this.pushModelToolStripMenuItem.Name = "pushModelToolStripMenuItem";
			this.pushModelToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
			this.pushModelToolStripMenuItem.Text = "PushModel";
			// 
			// employeesToolStripMenuItem
			// 
			this.employeesToolStripMenuItem.Name = "employeesToolStripMenuItem";
			this.employeesToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.employeesToolStripMenuItem.Text = "Employees";
			this.employeesToolStripMenuItem.Click += new System.EventHandler(this.StandartPushModelClick);
			// 
			// iListToolStripMenuItem
			// 
			this.iListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.contributorsListToolStripMenuItem,
									this.eventLoggerToolStripMenuItem});
			this.iListToolStripMenuItem.Name = "iListToolStripMenuItem";
			this.iListToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.iListToolStripMenuItem.Text = "IList";
			// 
			// contributorsListToolStripMenuItem
			// 
			this.contributorsListToolStripMenuItem.Name = "contributorsListToolStripMenuItem";
			this.contributorsListToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.contributorsListToolStripMenuItem.Text = "Contributor\'s List";
			this.contributorsListToolStripMenuItem.ToolTipText = "Customized, draw a frame on every second row";
			this.contributorsListToolStripMenuItem.Click += new System.EventHandler(this.ContributorsListClick);
			// 
			// customizedToolStripMenuItem
			// 
			this.customizedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.contributorsCustomizedToolStripMenuItem,
									this.contributorsSortedByLastnameToolStripMenuItem});
			this.customizedToolStripMenuItem.Name = "customizedToolStripMenuItem";
			this.customizedToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
			this.customizedToolStripMenuItem.Text = "Customized";
			// 
			// contributorsCustomizedToolStripMenuItem
			// 
			this.contributorsCustomizedToolStripMenuItem.Name = "contributorsCustomizedToolStripMenuItem";
			this.contributorsCustomizedToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.contributorsCustomizedToolStripMenuItem.Text = "Contributor\'s Customized";
			this.contributorsCustomizedToolStripMenuItem.Click += new System.EventHandler(this.ContributorsCustomizedClick);
			// 
			// contributorsSortedByLastnameToolStripMenuItem
			// 
			this.contributorsSortedByLastnameToolStripMenuItem.Name = "contributorsSortedByLastnameToolStripMenuItem";
			this.contributorsSortedByLastnameToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.contributorsSortedByLastnameToolStripMenuItem.Text = "Contributor\'s Sorted by Lastname";
			this.contributorsSortedByLastnameToolStripMenuItem.Click += new System.EventHandler(this.ContributorsSortedByLastnameClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnPDF);
			this.groupBox1.Controls.Add(this.btnPrinter);
			this.groupBox1.Controls.Add(this.btnPreviewControl);
			this.groupBox1.Controls.Add(this.btnReportViewer);
			this.groupBox1.Location = new System.Drawing.Point(39, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(600, 50);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Send Report to";
			// 
			// btnPDF
			// 
			this.btnPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPDF.Location = new System.Drawing.Point(448, 19);
			this.btnPDF.Name = "btnPDF";
			this.btnPDF.Size = new System.Drawing.Size(62, 20);
			this.btnPDF.TabIndex = 6;
			this.btnPDF.TabStop = true;
			this.btnPDF.Text = "PDF";
			this.btnPDF.UseVisualStyleBackColor = true;
			// 
			// btnPrinter
			// 
			this.btnPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPrinter.Location = new System.Drawing.Point(308, 15);
			this.btnPrinter.Name = "btnPrinter";
			this.btnPrinter.Size = new System.Drawing.Size(65, 26);
			this.btnPrinter.TabIndex = 5;
			this.btnPrinter.TabStop = true;
			this.btnPrinter.Text = "Printer";
			this.btnPrinter.UseVisualStyleBackColor = true;
			// 
			// btnPreviewControl
			// 
			this.btnPreviewControl.Location = new System.Drawing.Point(146, 19);
			this.btnPreviewControl.Name = "btnPreviewControl";
			this.btnPreviewControl.Size = new System.Drawing.Size(106, 16);
			this.btnPreviewControl.TabIndex = 4;
			this.btnPreviewControl.TabStop = true;
			this.btnPreviewControl.Text = "PreviewControl";
			this.btnPreviewControl.UseVisualStyleBackColor = true;
			// 
			// btnReportViewer
			// 
			this.btnReportViewer.Location = new System.Drawing.Point(20, 19);
			this.btnReportViewer.Name = "btnReportViewer";
			this.btnReportViewer.Size = new System.Drawing.Size(120, 18);
			this.btnReportViewer.TabIndex = 3;
			this.btnReportViewer.TabStop = true;
			this.btnReportViewer.Text = "ReportViewer";
			this.btnReportViewer.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(35, 390);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(247, 22);
			this.label1.TabIndex = 3;
			// 
			// eventLoggerToolStripMenuItem
			// 
			this.eventLoggerToolStripMenuItem.Name = "eventLoggerToolStripMenuItem";
			this.eventLoggerToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.eventLoggerToolStripMenuItem.Text = "EventLogger";
			this.eventLoggerToolStripMenuItem.Click += new System.EventHandler(this.EventLoggerClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(671, 421);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.previewControl1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SharpDevelop Reports Samples";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem eventLoggerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem jACToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaleyByYearWithParametersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mSDESqlExpressToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contributorsSortedByLastnameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contributorsCustomizedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem customizedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contributorsListToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem iListToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem employeesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem missingConnectionStringToolStripMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton btnPDF;
		private System.Windows.Forms.RadioButton btnPrinter;
		private System.Windows.Forms.RadioButton btnReportViewer;
		private System.Windows.Forms.RadioButton btnPreviewControl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ToolStripMenuItem employeesHiredateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pushModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pullModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem formSheetToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private ICSharpCode.Reports.Core.ReportViewer.PreviewControl previewControl1;
		
		
	}
}

