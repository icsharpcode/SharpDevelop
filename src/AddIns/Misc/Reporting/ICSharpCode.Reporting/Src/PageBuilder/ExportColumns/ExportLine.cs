// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
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
		
	}
}
