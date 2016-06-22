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
using System.Drawing.Drawing2D;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of ExportGraphics.
	/// </summary>
	/// 
	public interface IExportGraphics:IExportColumn {
		int Thickness {get;set;}
		DashStyle DashStyle {get;set;}
		LineCap StartLineCap {get;set;}
		LineCap EndLineCap {get;set;}
	}
	
	
	public class ExportLine:ExportColumn,IExportGraphics,IAcceptor
	{
		public ExportLine()
		{
		}
		
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this);
		}
		
		
		public override ICSharpCode.Reporting.Arrange.IMeasurementStrategy MeasurementStrategy()
		{
			throw new NotImplementedException();
		}
		
		public int Thickness {get;set;}
		
		public DashStyle DashStyle {get;set;}
		
		public LineCap StartLineCap {get;set;}
		
		public LineCap EndLineCap {get;set;}
		
		public Point FromPoint {get;set;}
		
		public Point ToPoint {get;set;}
	}
}
