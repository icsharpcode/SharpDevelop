/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.04.2013
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of DebugExporter.
	/// </summary>
	class DebugExporter:BaseExporter
	{
		private DebugVisitor visitor;
		
		public DebugExporter(Collection<ExportPage> pages):base(pages)
		{
			visitor = new DebugVisitor();
		}
		
		
		public override void Run () {
			Console.WriteLine();
			Console.WriteLine("Start DebugExporter with {0} Pages ",Pages.Count);
			visitor.Run(Pages);
			Console.WriteLine("Finish DebugVisitor");
			Console.WriteLine();
		}
	}
}
