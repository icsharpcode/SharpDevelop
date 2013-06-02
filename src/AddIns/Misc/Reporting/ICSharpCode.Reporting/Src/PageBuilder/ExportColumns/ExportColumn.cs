/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of BaseExportColumn.
	/// </summary>
	public class ExportColumn:IExportColumn
	{
		
		public string Name {get;set;}
		
		public Size Size {get;set;}
		
		public Point Location {get;set;}
		
		public virtual IArrangeStrategy GetArrangeStrategy ()
		{
			return null;
		}
		
		public Size DesiredSize {get;set;}
		
		public Color ForeColor {get;set;}
		
		public Color BackColor {get;set;}
		
		public Color FrameColor {get;set;}
		
		public IExportColumn Parent {get;set;}
			
		public bool CanGrow {get;set;}
		
		
		public Rectangle DisplayRectangle {
			get {
				return new Rectangle(Location,Size);
			}
		}
	}
}
