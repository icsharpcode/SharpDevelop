/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 16.10.2006
 * Time: 22:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ICSharpCode.Reports.Core.ReportViewer
{
	partial class PreviewControl : System.Windows.Forms.UserControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewControl));
			this.panel1 = new System.Windows.Forms.Panel();
			this.drawingPanel = new System.Windows.Forms.Panel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.createPdfMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.firstPageButton = new System.Windows.Forms.ToolStripButton();
			this.backButton = new System.Windows.Forms.ToolStripButton();
			this.numericToolStripTextBox2 = new ICSharpCode.Reports.Core.ReportViewer.NumericToolStripTextBox();
			this.pageInfoLabel = new System.Windows.Forms.ToolStripLabel();
			this.forwardButton = new System.Windows.Forms.ToolStripButton();
			this.lastPageButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.comboZoom = new System.Windows.Forms.ToolStripComboBox();
			this.printButton = new System.Windows.Forms.ToolStripButton();
			this.pdfButton = new System.Windows.Forms.ToolStripButton();
			this.panel1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.AutoScrollMargin = new System.Drawing.Size(5, 5);
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Controls.Add(this.drawingPanel);
			this.panel1.Location = new System.Drawing.Point(5, 33);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(5);
			this.panel1.Size = new System.Drawing.Size(525, 373);
			this.panel1.TabIndex = 3;
			// 
			// drawingPanel
			// 
			this.drawingPanel.BackColor = System.Drawing.SystemColors.Window;
			this.drawingPanel.ContextMenuStrip = this.contextMenuStrip1;
			this.drawingPanel.Location = new System.Drawing.Point(5, 5);
			this.drawingPanel.Name = "drawingPanel";
			this.drawingPanel.Size = new System.Drawing.Size(496, 500);
			this.drawingPanel.TabIndex = 0;
			this.drawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingPanelPaint);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createPdfMenu});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.contextMenuStrip1.Size = new System.Drawing.Size(148, 26);
			this.contextMenuStrip1.Text = "Create PdfFile";
			// 
			// createPdfMenu
			// 
			this.createPdfMenu.Name = "createPdfMenu";
			this.createPdfMenu.Size = new System.Drawing.Size(147, 22);
			this.createPdfMenu.Text = "Create PdfFile";
			this.createPdfMenu.Click += new System.EventHandler(this.PdfButtonClick);
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.firstPageButton,
									this.backButton,
									this.numericToolStripTextBox2,
									this.pageInfoLabel,
									this.forwardButton,
									this.lastPageButton,
									this.toolStripSeparator1,
									this.comboZoom,
									this.printButton,
									this.pdfButton});
			this.toolStrip1.Location = new System.Drawing.Point(5, 5);
			this.toolStrip1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip1.Size = new System.Drawing.Size(520, 25);
			this.toolStrip1.Stretch = true;
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// firstPageButton
			// 
			this.firstPageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.firstPageButton.Image = ((System.Drawing.Image)(resources.GetObject("firstPageButton.Image")));
			this.firstPageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.firstPageButton.Name = "firstPageButton";
			this.firstPageButton.Size = new System.Drawing.Size(23, 22);
			this.firstPageButton.Text = "&&firstPageButton";
			this.firstPageButton.Click += new System.EventHandler(this.FirstPageButtonClick);
			// 
			// backButton
			// 
			this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.backButton.Image = ((System.Drawing.Image)(resources.GetObject("backButton.Image")));
			this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(23, 22);
			this.backButton.Text = "&Back";
			this.backButton.Click += new System.EventHandler(this.BackButtonClick);
			// 
			// numericToolStripTextBox2
			// 
			this.numericToolStripTextBox2.AllowSpace = false;
			this.numericToolStripTextBox2.BackColor = System.Drawing.SystemColors.Window;
			this.numericToolStripTextBox2.Name = "numericToolStripTextBox2";
			this.numericToolStripTextBox2.Size = new System.Drawing.Size(50, 25);
			// 
			// pageInfoLabel
			// 
			this.pageInfoLabel.Name = "pageInfoLabel";
			this.pageInfoLabel.Size = new System.Drawing.Size(0, 22);
			// 
			// forwardButton
			// 
			this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.forwardButton.Image = ((System.Drawing.Image)(resources.GetObject("forwardButton.Image")));
			this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new System.Drawing.Size(23, 22);
			this.forwardButton.Text = "&Forward";
			this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
			// 
			// lastPageButton
			// 
			this.lastPageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lastPageButton.Image = ((System.Drawing.Image)(resources.GetObject("lastPageButton.Image")));
			this.lastPageButton.Name = "lastPageButton";
			this.lastPageButton.Size = new System.Drawing.Size(23, 22);
			this.lastPageButton.Text = "&&LastPage";
			this.lastPageButton.Click += new System.EventHandler(this.LastPageButtonClick);
			
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// comboZoom
			// 
			this.comboZoom.Name = "comboZoom";
			this.comboZoom.Size = new System.Drawing.Size(80, 25);
			this.comboZoom.SelectedIndexChanged += new System.EventHandler(this.comboZoomSelectedIndexChange);
			// 
			// printButton
			// 
			this.printButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.printButton.Image = ((System.Drawing.Image)(resources.GetObject("printButton.Image")));
			this.printButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.printButton.Name = "printButton";
			this.printButton.Size = new System.Drawing.Size(23, 22);
			this.printButton.Text = "&Print";
			this.printButton.Click += new System.EventHandler(this.PrintButton);
			// 
			// pdfButton
			// 
			this.pdfButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pdfButton.Image = ((System.Drawing.Image)(resources.GetObject("pdfButton.Image")));
			this.pdfButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.pdfButton.Name = "pdfButton";
			this.pdfButton.Size = new System.Drawing.Size(23, 22);
			this.pdfButton.Text = "toolStripButton1";
			this.pdfButton.Click += new System.EventHandler(this.PdfButtonClick);
			// 
			// PreviewControl
			// 
			this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.panel1);
			this.Name = "PreviewControl";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(530, 411);
			this.panel1.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private NumericToolStripTextBox numericToolStripTextBox2;
		private System.Windows.Forms.ToolStripLabel pageInfoLabel;
		private System.Windows.Forms.ToolStripButton pdfButton;
		private System.Windows.Forms.ToolStripButton lastPageButton;
		private System.Windows.Forms.ToolStripButton firstPageButton;
		private System.Windows.Forms.ToolStripMenuItem createPdfMenu;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripButton forwardButton;
		private System.Windows.Forms.ToolStripButton printButton;
		private System.Windows.Forms.ToolStripComboBox comboZoom;
		private System.Windows.Forms.Panel drawingPanel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton backButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStrip toolStrip1;

	}
}
