/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.04.2013
 * Time: 18:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of PrintExporter.
	/// </summary>
	public class PrintExporter:BaseExporter
	{
		private DebugVisitor visitor;
		
		public PrintExporter(Collection<IPage> pages):base(pages)
		{
			visitor = new DebugVisitor();
		}
		
		
		public override void Run () {
			foreach (var page in Pages) {
				ShowDebug(page);
			}
		}
		
		
		void ShowDebug(IExportContainer container)
		{
			var visitor = new DebugVisitor();
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (acceptor != null) {
						Console.WriteLine("--container--");
						acceptor.Accept(visitor);
					}
					ShowDebug(item as IExportContainer);
				} else {
					if (acceptor != null) {
						Console.WriteLine("..Item...");
						acceptor.Accept(visitor);
					}
				}
			}
		}
	}
}
