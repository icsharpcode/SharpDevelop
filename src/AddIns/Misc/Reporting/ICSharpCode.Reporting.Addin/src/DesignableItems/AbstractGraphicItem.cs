/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.04.2014
 * Time: 17:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of AbstractGraphicItem.
	/// </summary>
	public class AbstractGraphicItem:AbstractItem
	{

		float thickness;
		DashStyle dashStyle;

		
		public AbstractGraphicItem()
		{
			Thickness = 1;
			DashStyle = DashStyle.Solid;
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		public override void Draw(System.Drawing.Graphics graphics)
		{
			
		}
		
		[Category("Appearance")]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				dashStyle = value;
				Invalidate();
			}
		}
		
		
		[Category("Appearance")]
		public float Thickness {
			get { return thickness; }
			set {
				thickness = value;
				Invalidate();
			}
		}
	}
}
