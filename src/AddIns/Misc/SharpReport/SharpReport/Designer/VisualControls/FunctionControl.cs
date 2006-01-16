/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 19.04.2005
 * Time: 22:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SharpReport.Designer
{
	/// <summary>
	/// Description of PageNumber.
	/// </summary>
	internal class FunctionControl : ReportControlBase {
		string functionValue;
		
		public FunctionControl()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
		}
		
		#region Overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics g = e.Graphics;

			StringFormat f = base.StringFormat;
			f.Alignment = base.StringAlignment;
			g.DrawString(this.Text + functionValue,
			             this.Font, 
			             new SolidBrush(this.ForeColor),
			             new Rectangle(0, 0, this.Width - 1, this.Height - 1),
			             f);
		}
		
		/// <summary>
		/// text is the TitlePart of an function like 'PageNr : '
		/// </summary>
		public override string Text{
			get { return base.Text; }
			set { base.Text = value; }
		}
		
		/// <summary>
		/// FunctionValues give's the flexible part of the Function
		/// </summary>
		public string FunctionValue {
			get {
				return functionValue;
			}
			set {
				functionValue = value;
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
			// PageNumber
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "PageNumber";
			this.Size = new System.Drawing.Size(120, 20);
		}
		#endregion
	}
}
