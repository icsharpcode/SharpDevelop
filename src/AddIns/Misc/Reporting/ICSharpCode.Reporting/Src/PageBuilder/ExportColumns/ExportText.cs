/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.04.2013
 * Time: 20:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of ExportText.
	/// </summary>
	/// 
	public interface IExportText : IExportColumn
	{
		 Font Font {get;set;}
		 string Text {get;set;}
	}
	
	
	public class ExportText:ExportColumn,IExportText,IAcceptor
	{
		public ExportText()
		{
		}
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this);
		}
		
		public Font Font {get;set;}
		
		
		public string Text {get;set;}
			
	}
}
