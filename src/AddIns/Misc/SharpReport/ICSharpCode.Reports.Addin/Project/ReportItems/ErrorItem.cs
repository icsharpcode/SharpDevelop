/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 11.05.2008
 * Zeit: 12:57
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ErrorItem.
	/// </summary>
	public class ErrorItem:BaseTextItem
	{
		public ErrorItem()
		{
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw(e.Graphics);
		}
	
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, base.DrawingRectangle);
			}
			
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                         "Error : <{0}> is missing or obsolete",base.Text);
			ICSharpCode.Reports.Core.TextDrawer.DrawString(graphics,s,this.Font,
			                                               new SolidBrush(Color.Red),
			                                               this.ClientRectangle,
			                                               base.StringTrimming,base.ContentAlignment);
			base.DrawControl(graphics,base.DrawingRectangle);
		}
	}
}
