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
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of FormatVisitor.
	/// </summary>
	class FormatVisitor: AbstractVisitor
	{

		
		public override void Visit(ExportContainer exportContainer)
		{
			foreach (var element in exportContainer.ExportedItems) {
				var container = element as ExportContainer;
				if (container != null) {
					Visit(container);
				}
				
				var te = element as ExportText;
				if (te != null) {
					Visit(te);
				}
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine(exportColumn.Text);
			if (!String.IsNullOrEmpty(exportColumn.FormatString)) {
				StandardFormatter.FormatOutput(exportColumn);
			}
		}
	}
}
