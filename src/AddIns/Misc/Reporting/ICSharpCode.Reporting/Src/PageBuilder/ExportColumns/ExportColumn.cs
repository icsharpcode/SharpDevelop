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
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of BaseExportColumn.
	/// </summary>
	public class ExportColumn:IExportColumn
	{
		public ExportColumn() {
			ForeColor = Color.Black;
			FrameColor = Color.Black;
			BackColor = Color.White;
		}
		
		public string Name {get;set;}
		
		public Size Size {get;set;}
		
		public Point Location {get;set;}
		
		public Size DesiredSize {get;set;}
		
		public Color ForeColor {get;set;}
		
		public Color BackColor {get;set;}
		
		public Color FrameColor {get;set;}
		
		public IExportColumn Parent {get;set;}
			
		public bool CanGrow {get;set;}
		
		public bool DrawBorder {get;set;}
		
		public Rectangle DisplayRectangle {
			get {
				return new Rectangle(Location,DesiredSize);
			}
		}
		
		public virtual IArrangeStrategy GetArrangeStrategy ()
		{
			return null;
		}
		
		public virtual IMeasurementStrategy MeasurementStrategy()
		{
			throw new NotImplementedException();
		}
	}
}
