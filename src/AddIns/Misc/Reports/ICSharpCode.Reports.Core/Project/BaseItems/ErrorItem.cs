/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 13.05.2008
 * Zeit: 18:44
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ErrorItem.
	/// </summary>
	public class ErrorItem:BaseTextItem,IExportColumnBuilder
	{
		string errMess;
		
		public ErrorItem():base()
		{
		}
		
		private void SetErrorLayout ()
		{
			base.DrawBorder = true;
			base.ForeColor = Color.Red;
			this.errMess = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                             "Error : <{0}> is missing or obsolete",base.Text);
		}
		
		
		public override void Render(ReportPageEventArgs rpea)
		{	
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			this.SetErrorLayout();
			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			base.DrawFrame(rpea.PrintPageEventArgs.Graphics,b);
			Print (rpea,this.errMess,base.DrawingRectangle);
		}
		
		#region IExportColumnBuilder  implementation
		
		public override BaseExportColumn CreateExportColumn(){
			this.SetErrorLayout();
			TextStyleDecorator st = base.CreateItemStyle();
			ExportText item = new ExportText(st,false);
			item.Text = this.errMess;
			return item;
		}
		
		#endregion
	}
}
