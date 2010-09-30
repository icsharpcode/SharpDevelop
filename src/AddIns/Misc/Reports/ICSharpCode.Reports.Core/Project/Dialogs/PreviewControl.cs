// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Core{
	/// <summary>
	/// Paint Preview Control in Designer-TabPagePreview
	/// currently there is only the first page displayed
	/// </summary>
	public class PreviewControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
		private System.Windows.Forms.PrintPreviewDialog previewDlg;

		public PreviewControl(){
			InitializeComponent();
		}
		
		public void ShowPreview (AbstractRenderer renderer,double zoomFaktor,bool standAlone) {
			if (renderer == null) {
				throw new ArgumentNullException("renderer");
			}
			
			if (renderer.Cancel == false) {
				if (standAlone) {
					this.ShowPreviewWithUserControl (renderer,zoomFaktor);
				}
				else {
					ShowPreviewWithDialog(renderer,zoomFaktor);
				}
			}
		}
		
		private void ShowPreviewWithUserControl (AbstractRenderer renderer,double zoomFaktor) {
			
			System.Drawing.Printing.PrintDocument  doc = renderer.ReportDocument;
			//doc.PrintController = new ExtendedPrintController(new System.Drawing.Printing.PreviewPrintController());
			
//			doc.Print();
			
			printPreviewControl1.InvalidatePreview();
			printPreviewControl1.Document = null;
			printPreviewControl1.Document = doc;

			printPreviewControl1.Zoom = zoomFaktor;
			printPreviewControl1.Document.DocumentName = renderer.ReportSettings.ReportName;
			printPreviewControl1.UseAntiAlias = true;
		}
		
		
		private void ShowPreviewWithDialog (AbstractRenderer renderer,double zoomFaktor) {
			
			System.Drawing.Printing.PrintDocument  doc = renderer.ReportDocument;
			
			printPreviewControl1.InvalidatePreview();
			previewDlg.Document = doc;
			previewDlg.Text	= renderer.ReportSettings.ReportName;
			previewDlg.Text = renderer.ReportSettings.ReportName;
			previewDlg.StartPosition = FormStartPosition.CenterParent;
			previewDlg.PrintPreviewControl.Zoom = zoomFaktor;
			previewDlg.ShowDialog();
		}
		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.previewDlg = new System.Windows.Forms.PrintPreviewDialog();
			this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// previewDlg
			// 
			this.previewDlg.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.previewDlg.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.previewDlg.ClientSize = new System.Drawing.Size(520, 325);
			this.previewDlg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewDlg.Enabled = true;
			this.previewDlg.Location = new System.Drawing.Point(8, 8);
			this.previewDlg.MinimumSize = new System.Drawing.Size(375, 250);
			this.previewDlg.Name = "previewDlg";
			this.previewDlg.TransparencyKey = System.Drawing.Color.Empty;
			this.previewDlg.Visible = false;
			// 
			// printPreviewControl1
			// 
			this.printPreviewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.printPreviewControl1.AutoZoom = false;
			this.printPreviewControl1.BackColor = System.Drawing.SystemColors.Window;
			this.printPreviewControl1.Location = new System.Drawing.Point(8, 8);
			this.printPreviewControl1.Name = "printPreviewControl1";
			this.printPreviewControl1.Size = new System.Drawing.Size(688, 360);
			this.printPreviewControl1.TabIndex = 1;
			this.printPreviewControl1.Zoom = 1.3;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.printPreviewControl1);
			this.panel1.Location = new System.Drawing.Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(704, 384);
			this.panel1.TabIndex = 2;
			// 
			// PaintArea
			// 
			this.Controls.Add(this.panel1);
			this.DockPadding.All = 8;
			this.BackColor = Color.White;
			this.Dock = DockStyle.Fill;
			this.Name = "PaintArea";
			this.Size = new System.Drawing.Size(720, 400);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		
		
	}
}
