/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.04.2013
 * Time: 18:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseTextItem.
	/// </summary>
	public interface  ITextItem:IPrintableObject
	{
		Font Font {get;set;}
		string Text {get;set;}
		bool CanGrow {get;set;}
	}
	
	public class BaseTextItem:PrintableItem,ITextItem
	{
		public BaseTextItem(){
			Name = "BaseTextItem";
			Font = GlobalValues.DefaultFont;
		}
	
		
		public Font Font {get;set;}
		
		public string Text {get;set;}
		
		public bool CanGrow {get;set;}
		
		public override  IExportColumn CreateExportColumn()
		{
			var ex = new ExportText();
			ex.Name = Name;
			ex.Location = Location;
			ex.ForeColor = ForeColor;
			ex.BackColor = BackColor;
			ex.FrameColor = FrameColor;
			ex.Size = Size;
			ex.Font = Font;
			ex.Text = Text;
			return ex;
		}
		
		public override ICSharpCode.Reporting.Arrange.IMeasurementStrategy MeasurementStrategy()
		{
			return new TextBasedMeasurementStrategy();
		}
			
	}
}
