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

using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.WpfReportViewer.Visitor;

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

		static FixedDocument CreateFixedDocument(ReportSettings reportSettings)
		{
			var document = new FixedDocument();
			document.DocumentPaginator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,
			                                                              reportSettings.PageSize.Height);
		return document;
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