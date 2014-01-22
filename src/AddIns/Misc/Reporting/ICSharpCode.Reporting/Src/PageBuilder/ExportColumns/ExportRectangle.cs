// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing.Drawing2D;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of ExportRectangle.
	/// </summary>
	public class ExportRectangle:ExportColumn,IExportGraphics,IAcceptor
	{
		public ExportRectangle()
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
	}
}
