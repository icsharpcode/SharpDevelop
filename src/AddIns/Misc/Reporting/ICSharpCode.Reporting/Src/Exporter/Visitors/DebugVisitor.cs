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
			base.Visit(page);
		}
		
		
		public override void Visit(ExportContainer exportContainer)
		{
			base.Visit(exportContainer);
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
		}
		
		public override void Visit(ExportLine exportGraphics)
		{
//			base.Visit(exportGraphics);
			Console.WriteLine("Line from {0} size  {1}",exportGraphics.Location,exportGraphics.Size.Width);
		}
	}
}
