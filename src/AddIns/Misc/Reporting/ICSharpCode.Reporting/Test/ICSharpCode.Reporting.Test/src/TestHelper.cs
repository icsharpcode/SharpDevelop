/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Test
{
	/// <summary>
	/// Description of TestHelper.
	/// </summary>
	public static class TestHelper
	{
		private const string nameSpace = "ICSharpCode.Reporting.Test.src.TestReports.";
		private const string plainReportName = "PlainModel.srd";
		private const string rr = "ReportWithTwoItems.srd";
		
		
		public static string PlainReportFileName{
			get{return nameSpace + plainReportName;}
		}
		
		public static string RepWithTwoItems {
			get {return nameSpace + rr;}
		}
		
		
		public static void ShowDebug(IExportContainer exportContainer)
		{
			var visitor = new DebugVisitor();
			foreach (var item in exportContainer.ExportedItems) {
				var container = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (container != null) {
					if (acceptor != null) {
						Console.WriteLine("----");
						acceptor.Accept(visitor);
					}
					ShowDebug(container);
				} else {
//					var b = item as IAcceptor;
					if (acceptor != null) {
						acceptor.Accept(visitor);
						
					}
				}
			}
		}
	}
}
