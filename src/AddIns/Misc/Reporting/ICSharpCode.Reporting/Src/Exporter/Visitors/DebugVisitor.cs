/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	
//	http://www.remondo.net/visitor-pattern-example-csharp/
//	http://www.codeproject.com/Articles/42240/Visitor-Design-Pattern
//	http://www.remondo.net/strategy-pattern-example-csharp/
	
	
	class DebugVisitor : AbstractVisitor
	{
		
		public override void Visit(ExportPage page)
		{
			Console.WriteLine();
			Console.WriteLine("<Page> {0} - {1}  Items {2}",
			                 page.Name,page.Location,page.BackColor);
			base.Visit(page);
		}
		
		
		public override void Visit(ExportContainer exportContainer)
		{
			Console.WriteLine();
			Console.WriteLine("\t{0} - {1}  Items {2}",
			                  exportContainer.Name,exportContainer.Location,exportContainer.BackColor);
			Console.WriteLine("\thas Child {0}",exportContainer.ExportedItems.Count);
			base.Visit(exportContainer);
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine("\t\t<ExportText> {0} - {1}", exportColumn.Text,exportColumn.Location);
		}
	}
}
