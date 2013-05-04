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
		
		private IDocumentPaginatorSource document;
		
		public PreviewViewModel(ReportSettings reportSettings, Collection<IPage> pages)
		{
			this.Pages = pages;
			FixedDocumentRenderer renderer =  new FixedDocumentRenderer(reportSettings,Pages);
			renderer.Start();
			renderer.RenderOutput();
			renderer.End();
//			this.Document = renderer.Document;
		}
		
		
		public Collection<IPage> Pages {get;private set;}
		
		public IDocumentPaginatorSource Document
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