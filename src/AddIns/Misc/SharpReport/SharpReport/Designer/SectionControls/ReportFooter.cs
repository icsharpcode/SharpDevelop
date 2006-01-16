/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer{
	/// <summary>
	/// Description of ReportFooter.
	/// </summary>
	internal class ReportFooter : ReportSectionControlBase
	{
		public event ItemDragDropEventHandler ReportItemsHandling;
		
		public ReportFooter()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			base.ItemDragDrop += new ItemDragDropEventHandler (ItemsChanging);
		}
		
		
		protected void ItemsChanging (object sender,ItemDragDropEventArgs e) {
			if (ReportItemsHandling != null) {
				ReportItemsHandling (this,e);
			}
		}
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint (e);
		}
		
		#endregion
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ReportFooter
			// 
			this.Name = "ReportFooter";
			this.Size = new System.Drawing.Size(292, 266);
		}
		#endregion
	}
}
