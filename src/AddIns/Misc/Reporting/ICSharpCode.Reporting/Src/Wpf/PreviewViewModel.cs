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
using System.Windows.Documents;
using System.Windows.Markup;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.WpfReportViewer
{
	/// <summary>
	/// Description of PreviewViewModel.
	/// </summary>
	public class PreviewViewModel:INotifyPropertyChanged
	{
		
		FixedDocument document ;
		
		public PreviewViewModel(ReportSettings reportSettings, Collection<ExportPage> pages)
		{
			if (pages == null)
				throw new ArgumentNullException("pages");
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			
			Document = CreateFixedDocument(reportSettings);
			
			var wpfExporter = new WpfExporter(pages);
			wpfExporter.Run();
			this.document = wpfExporter.Document;
		}

		FixedDocument CreateFixedDocument(ReportSettings reportSettings)
		{
			var document = new FixedDocument();
			var s = document.DocumentPaginator.PageSize;
			document.DocumentPaginator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,
			                                                              reportSettings.PageSize.Height);
		return document;
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