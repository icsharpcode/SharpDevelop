/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer{
	/// <summary>
	/// Description of ReportDbTextItem.
	/// </summary>

	internal class ReportDbTextControl : ReportTextControl{
		public ReportDbTextControl():base(){
			InitializeComponent();
			this.Size = GlobalValues.PreferedSize;
			base.Name = this.Name;
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea){
			base.Text = this.Text;
			base.OnPaint(pea);
		}
		
		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.BackColor = System.Drawing.Color.White;
			this.Name = "ReportDbTextItem";
			this.Size = new System.Drawing.Size(120, 20);
		}
		#endregion
	}
}
