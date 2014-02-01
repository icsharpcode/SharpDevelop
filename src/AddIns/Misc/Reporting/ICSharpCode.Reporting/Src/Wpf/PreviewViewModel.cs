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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Markup;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.ExportRenderer;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.WpfReportViewer
{
	/// <summary>
	/// Description of PreviewViewModel.
	/// </summary>
	public class PreviewViewModel:INotifyPropertyChanged
	{
		
		private FixedDocument document ;
		
		public PreviewViewModel(ReportSettings reportSettings, Collection<IPage> pages)
		{
			if (pages == null)
				throw new ArgumentNullException("pages");
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			Document = new FixedDocument();
			var s = Document.DocumentPaginator.PageSize;
			Document.DocumentPaginator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,reportSettings.PageSize.Height);
			var wpfExporter = new WpfExporter(reportSettings,pages);
			wpfExporter.Run();
			var fixedPage = wpfExporter.FixedPage;
			AddPageToDocument(Document,fixedPage);
		}
		
		static void AddPageToDocument(FixedDocument fixedDocument,FixedPage page)
		{
			var pageContent = new PageContent();
			((IAddChild)pageContent).AddChild(page);
			fixedDocument.Pages.Add(pageContent);
		}
		
		public FixedDocument Document
		{
			get {return document;}
			set {
				this.document = value;
				OnNotifyPropertyChanged ("Document");
				}	
		}
		
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnNotifyPropertyChanged(string num0)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this,new PropertyChangedEventArgs(num0));
			}
		}
	}
}
