/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.05.2011
 * Time: 21:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSharpCode.Reports.Core.WpfReportViewer
{
	
	public interface IWpfReportViewer
	{
		IDocumentPaginatorSource Document {set;}
		void SetBinding (PreviewViewModel model);
	}
	/// <summary>
	/// Interaction logic for WpfReportViewer.xaml
	/// </summary>
	
	public partial class WpfReportViewer : UserControl,IWpfReportViewer
	{
		public WpfReportViewer()
		{
			InitializeComponent();
		}
		
		public void SetBinding (PreviewViewModel model)
		{
			this.DataContext = model;
		}
		
		
		public IDocumentPaginatorSource Document {
			set {
				this.DocumentViewer.Document = value;
			}
		}
	}
}