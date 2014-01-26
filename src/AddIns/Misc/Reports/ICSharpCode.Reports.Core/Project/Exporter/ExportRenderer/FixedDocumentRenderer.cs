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
using System.Windows.Markup;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.WPF;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of FixedDocumentRenderer.
	/// </summary>
	public class FixedDocumentRenderer:BaseExportRenderer
	{
		private PagesCollection pages;
		private ReportSettings reportSettings;
		private FixedDocument document ;
		private FixedDocumentCreator docCreator;
	
	
		public static FixedDocumentRenderer CreateInstance (ReportSettings reportSettings,PagesCollection pages)
		{
			var instance = new FixedDocumentRenderer(reportSettings,pages);
			return instance;
		}
		
		
		private FixedDocumentRenderer(ReportSettings reportSettings,PagesCollection pages):base(pages)
		{
			this.pages  = pages;
			this.reportSettings = reportSettings;
			this.docCreator = new FixedDocumentCreator();
		}
		
		
		
		#region overrides
		
		public override void Start()
		{
			base.Start();
			document = new FixedDocument();
			

			// point 	 595x842
			
			// 827/1169

//			A4 paper is 210mm x 297mm
			//8.2 inch x 11.6 inch
			//1240 px x 1754 px
			/*
		iTextSharp uses a default of 72 pixels per inch.
			792 would be 11", or the height of a standard Letter size paper." +
			595 would be 8.264",
		which is the standard width of A4 size paper.
			Using 595 x 792 as the page size would be a cheap and dirty way
			to ensure that you could print on either A4 or Letter
			without anything getting cut off. –
			 */
			
			docCreator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,reportSettings.PageSize.Height);
			document.DocumentPaginator.PageSize = docCreator.PageSize;
		}
		
	
		
		public override void RenderOutput()
		{
			base.RenderOutput();
			
			foreach (var page in pages)
			{
				FixedPage fixedPage = docCreator.CreatePage (page);
				FixedDocumentCreator.ArrangePage(document.DocumentPaginator.PageSize,fixedPage);
				AddPageToDocument(document,fixedPage);
			}
			Document = document;
		}
		
	
		static void AddPageToDocument(FixedDocument fixedDocument,FixedPage page)
		{
			PageContent pageContent = new PageContent();
			((IAddChild)pageContent).AddChild(page);
			fixedDocument.Pages.Add(pageContent);
		}
		
		
		public override void End()
		{
			base.End();
		}
			
		public IDocumentPaginatorSource Document {get;private set;}
		
		#endregion
	}
}
