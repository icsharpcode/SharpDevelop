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
			foreach (var page in Pages) {
				IAcceptor ac = page as IAcceptor;
				if (ac != null) {
					visitor.Visit(page);
				}
				Console.WriteLine("-----------PageBreak---------");
			}
			Console.WriteLine("Finish DebugVisitor");
			Console.WriteLine();
		}
		
		/*
		void RunInternal(string header,IExportContainer container)
		{
			var leading = header;
			Console.WriteLine();
			Console.WriteLine("{0}{1}",leading,container.Name);
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (exportContainer.ExportedItems.Count > 0) {
						RunInternal(leading = leading + "--",exportContainer);
						acceptor.Accept(visitor);
//						ShowDebug(leading = leading + "--",exportContainer);
						leading = leading.Substring(0,leading.Length -2);
					}
				} else {
					ShowSingleEntry(leading, acceptor);
				}
			}
		}

		
		void ShowSingleEntry(string leading, IAcceptor acceptor)
		{
			if (acceptor != null) {
				acceptor.Accept(visitor);
				leading = leading.Substring(0, leading.Length - 2);
			}
		}
		*/
	}
}
