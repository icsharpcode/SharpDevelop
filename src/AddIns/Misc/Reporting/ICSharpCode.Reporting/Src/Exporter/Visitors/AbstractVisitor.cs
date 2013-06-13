/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.04.2013
 * Time: 19:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of AbstractVisitor.
	/// </summary>
	public abstract class AbstractVisitor : IVisitor
	{
		public abstract void Visit(ExportColumn exportColumn);
		public abstract void Visit(ExportContainer exportColumn);
		public abstract void Visit(ExportText exportColumn);
	}
}
