/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.05.2011
 * Time: 21:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.ReportViewer;

namespace ICSharpCode.Reports.Core.WpfReportViewer
{
	
	public interface IWpfReportViewer
	{
		IDocumentPaginatorSource Document {set;}
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
		
		
		public IDocumentPaginatorSource Document {
			set {
				this.DocumentViewer.Document = value;
			}
		}
	
	}
}