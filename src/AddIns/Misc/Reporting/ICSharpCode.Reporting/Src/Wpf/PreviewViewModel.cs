/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05/04/2013
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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