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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of DebugExporter.
	/// </summary>
	public class DebugExporter:BaseExporter
	{
		private DebugVisitor visitor;
		
		public DebugExporter(Collection<IPage> pages):base(pages)
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
