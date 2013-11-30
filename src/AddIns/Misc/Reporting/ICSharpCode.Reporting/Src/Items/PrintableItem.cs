/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.04.2013
 * Time: 20:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Items
{
	public class PrintableItem : IPrintableObject
	{
		public PrintableItem() {
			ForeColor = Color.Black;
			FrameColor = Color.Black;
			BackColor = Color.White;
		}
		
		public string Name { get; set; }

		public Point Location { get; set; }

		public Size Size { get; set; }

		
		public virtual IExportColumn CreateExportColumn()
		{
			return null;
		}
		
		public Color ForeColor {get;set;}
			
		public Color BackColor {get;set;}
			
		public Color FrameColor {get;set;}
		
		public bool CanGrow {get;set;}
		
		public bool DrawBorder {get;set;}
		
	}
}
