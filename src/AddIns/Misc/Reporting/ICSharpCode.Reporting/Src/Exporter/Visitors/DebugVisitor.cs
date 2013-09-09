/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	
//	http://www.remondo.net/visitor-pattern-example-csharp/
//	http://www.codeproject.com/Articles/42240/Visitor-Design-Pattern
//	http://www.remondo.net/strategy-pattern-example-csharp/
	
	
	public class DebugVisitor : AbstractVisitor
	{
		public override void Visit(ExportPage page)
		{
			Console.WriteLine("<Page> {0} - {1}  Items {2}",
			                 page.Name,page.Location,page.BackColor);
		}
		
		
		public override void Visit(ExportColumn exportColumn)
		{
			Console.WriteLine("Visit ExportColumn {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
		}
		
		
		
		public override void Visit(ExportContainer exportColumn)
		{
			Console.WriteLine();
//			Console.WriteLine("<ExportContainer> {0} - {1}  Items {2}",
//			                  exportColumn.Name,exportColumn.Location,exportColumn.BackColor);
				Console.WriteLine("{0} - {1}  Items {2}",
			                  exportColumn.Name,exportColumn.Location,exportColumn.BackColor);
		}
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine("<ExportText> {0} - {1}", exportColumn.Text,exportColumn.Location);
		}
		
		
	}

}
