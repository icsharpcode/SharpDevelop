/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 21.12.2008
 * Zeit: 12:10
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of CreateGraphic.
	/// </summary>
	public sealed class CreateGraphicObject
	{
		private CreateGraphicObject ()
		{
		}
		
		
		public static Graphics FromPrinter (PageSettings page)
		{
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			Graphics test = page.PrinterSettings.CreateMeasurementGraphics();
			return test;
		}
		
		
		public static Graphics FromSize (Size size){
			if (size == null) {
				throw new ArgumentNullException("size");
			}
			Bitmap b = new Bitmap(size.Width,size.Height);
			Graphics g = Graphics.FromImage(b);
			return g;
		}
	}
}
