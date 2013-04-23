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

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of Visitor.
	/// </summary>
	/// 

	public abstract class Visitor : IVisitor
	{
		public abstract void Visit(ExportColumn exportColumn);
		public abstract void Visit(ExportContainer exportColumn);
		public abstract void Visit(ExportText exportColumn);
	}

	
//	http://www.remondo.net/visitor-pattern-example-csharp/
//	http://www.codeproject.com/Articles/42240/Visitor-Design-Pattern
//	http://www.remondo.net/strategy-pattern-example-csharp/
	
	
	public class DebugVisitor : Visitor
	{
		public override void Visit(ExportColumn exportColumn)
		{
			Console.WriteLine("Visit ExportColumn {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
		}
		
		
		
		public override void Visit(ExportContainer exportColumn)
		{
			Console.WriteLine("Visit ExportContainer {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
		}
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine("Visit ExportText {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
		}
	}

}
