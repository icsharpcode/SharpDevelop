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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reporting.ExportRenderer;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of WpfVisitor.
	/// </summary>
	internal class WpfVisitor: AbstractVisitor
	{
		private readonly FixedDocumentCreator documentCreator;
		private readonly  ReportSettings reportSettings;
		
		public WpfVisitor(ReportSettings reportSettings)
		{
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			this.reportSettings = reportSettings;
			documentCreator = new FixedDocumentCreator(reportSettings);
		}
		
		public override void Visit(ExportColumn exportColumn)
		{
//			Console.WriteLine("Wpf-Visit ExportColumn {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
		}
		
		
		public override void Visit(ExportContainer exportColumn)
		{
//			Console.WriteLine("Wpf-Visit ExportContainer {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.Location);
			var canvas = documentCreator.CreateContainer(exportColumn);
			UIElement = canvas;
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
//			Console.WriteLine("Wpf-Visit ExportText {0} - {1} - {2}", exportColumn.Name,exportColumn.Size,exportColumn.DesiredSize);
			var textBlock = documentCreator.CreateTextBlock(exportColumn);
			UIElement = textBlock;
		}
		
		
		public UIElement UIElement {get; private set;}
		
	}
}
