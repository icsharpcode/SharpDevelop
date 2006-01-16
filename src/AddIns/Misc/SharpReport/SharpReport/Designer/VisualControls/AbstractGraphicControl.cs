/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 18.04.2005
 * Time: 17:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SharpReport.Designer
{
	/// <summary>
	/// Base Class for Graphic Contriols like Circle,Rectangle etc
	/// </summary>
	public class AbstractGraphicControl : ReportControlBase {
		int thickness = 1;
		DashStyle dashStyle = DashStyle.Solid;
		
		public AbstractGraphicControl()
		{
		
			InitializeComponent();
			base.DragEnter += new DragEventHandler (OnOver);
		}
		
		void OnOver (object sender, DragEventArgs e) {
			System.Console.WriteLine("AbstractGraphicControl:On Over");
		}
		#region property's
		
		/// <summary>
		/// DasshStyle used in drawing
		/// </summary>
		public DashStyle DashStyle {
			get {
				return dashStyle;
			}
			set {
				dashStyle = value;
				this.Invalidate();
			}
		}
		
		/// <summary>
		/// Thickness of Linme used
		/// </summary>
		public int Thickness {
			get {
				return thickness;
			}
			set {
				thickness = value;
				this.Invalidate();
			}
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
			// AbstractGraphicControl
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "AbstractGraphicControl";
			this.Size = new System.Drawing.Size(292, 266);
		}
		#endregion
	}
}
