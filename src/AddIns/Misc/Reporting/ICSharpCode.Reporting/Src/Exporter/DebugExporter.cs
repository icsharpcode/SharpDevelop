/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.04.2013
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of DebugExporter.
	/// </summary>
	public class DebugExporter
	{
		
		public DebugExporter(Collection<IPage> pages)
		{
			if (pages == null) {
				throw new ArgumentException("pages");
			}
			
			Pages = pages;
		}
	
		
		public void Run () {
			foreach (var page in Pages) {
				ShowDebug(page);
			}
		}
		
	static void ShowDebug(IExportContainer container)
		{
			var visitor = new DebugVisitor();
			foreach (var item in container.ExportedItems) {
				if (item is IExportContainer) {
					var a = item as IAcceptor;
					if (a != null) {
						Console.WriteLine("----");
						a.Accept(visitor);
					}
					ShowDebug(item as IExportContainer);
				} else {
					var b = item as IAcceptor;
					if (b != null) {
						b.Accept(visitor);
						
					}
				}
			}
		}
		
		/*
		static void ShowDebug(IExportContainer container)
		{
			var visitor = new DebugVisitor();
			foreach (var item in container.ExportedItems) {
				if (item is IExportContainer) {
					var a = item as IAcceptor;
					if (a != null) {
						Console.WriteLine("----");
						a.Accept(visitor);
					}
					ShowDebug(item as IExportContainer);
				} else {
					var b = item as IAcceptor;
					if (b != null) {
						b.Accept(visitor);
						
					}
				}
			}
		}
		*/
		public Collection<IPage> Pages {get; private set;}
	}
}
