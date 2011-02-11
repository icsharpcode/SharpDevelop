// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Core.Globals
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
