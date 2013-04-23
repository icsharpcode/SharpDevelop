/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.04.2013
 * Time: 20:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Exporter;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of ExportText.
	/// </summary>
	public class ExportText:ExportColumn,IAcceptor
	{
		public ExportText()
		{
		}
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
