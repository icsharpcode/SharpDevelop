/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	public interface IVisitor
	{
		void Visit(ExportColumn exportColumn);
		void Visit(ExportContainer exportColumn);
		void Visit(ExportText exportColumn);
	}
}
