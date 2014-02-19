// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		const string nameSpace = "ICSharpCode.Reporting.Test.src.TestReports.";
		const string plainReportName = "PlainModel.srd";
		const string withTwoItems = "ReportWithTwoItems.srd";
		const string fromList = "FromList.srd";
		const string groupedList = "GroupedList.srd";
		const string globalsTestReport = "TestForGlobals.srd";
		
		public static string PlainReportFileName{
			get{return nameSpace + plainReportName;}
		}
		
		
		public static string RepWithTwoItems {
			get {return nameSpace + withTwoItems;}
		}
		
		
		public static string ReportFromList {
			get {return nameSpace + fromList;}
		}
		
		public static string GroupedList {
			get {return nameSpace + groupedList;}
		}
		
		
		public static string TestForGlobals {
			get {return nameSpace + globalsTestReport;}
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
					if (acceptor != null) {
						acceptor.Accept(visitor);
						
					}
				}
			}
		}
	}
}
