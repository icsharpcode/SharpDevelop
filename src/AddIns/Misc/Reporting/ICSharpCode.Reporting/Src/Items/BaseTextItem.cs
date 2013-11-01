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
		string FormatString {get;set;}
		string DataType {get;set;}
		
	}
	
	public class BaseTextItem:PrintableItem,ITextItem
	{
		public BaseTextItem(){
			Name = "BaseTextItem";
			Font = GlobalValues.DefaultFont;
		}
	
		
		public Font Font {get;set;}
		
		public string Text {get;set;}
		
		public string FormatString {get;set;}
		
		string dataType;
		
		public string DataType 
		{
			get {
				if (String.IsNullOrEmpty(this.dataType)) {
					this.dataType = typeof(System.String).ToString();
				}
				return dataType;
			}
			set {
				dataType = value;
			}
		}
		
		
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
			ex.FormatString = FormatString;
			ex.DataType = DataType;
			ex.CanGrow = CanGrow;
			return ex;
		}	
	}
}
