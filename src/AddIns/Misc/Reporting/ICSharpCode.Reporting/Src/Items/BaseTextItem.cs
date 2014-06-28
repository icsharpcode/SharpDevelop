// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows;
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
		ContentAlignment ContentAlignment {get;set;}
		TextAlignment TextAlignment {get;set;}
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
		
		[Obsolete ("Use TextAlignment")]
		public ContentAlignment ContentAlignment {get;set;}
		
		public TextAlignment TextAlignment {get;set;}


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
			ex.ContentAlignment = ContentAlignment;
			ex.TextAlignment = TextAlignment;
			ex.DataType = DataType;
			ex.CanGrow = CanGrow;
			ex.DrawBorder = DrawBorder;
			return ex;
		}	
	}
}
