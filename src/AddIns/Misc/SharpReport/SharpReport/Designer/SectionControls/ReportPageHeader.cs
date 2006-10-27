/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

namespace SharpReport.Designer
{
	/// <summary>
	/// Description of ReportPageHeader.
	/// </summary>
	internal class ReportPageHeader : ReportSectionControlBase
	{
		public event ItemDragDropEventHandler ReportItemsHandling;
		
		public ReportPageHeader()
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
		
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint (e);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ReportPageHeader
			// 
			this.Name = "ReportPageHeader";
			this.Size = new System.Drawing.Size(292, 266);
		}
		#endregion
	}
}
