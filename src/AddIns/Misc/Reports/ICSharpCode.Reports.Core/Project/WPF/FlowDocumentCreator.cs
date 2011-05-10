/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.05.2011
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core.Exporter;

using System.Windows.Documents;

namespace ICSharpCode.Reports.Core.WPF
{
	/// <summary>
	/// Description of FlowDocumentCreator.
	/// </summary>
	public class FlowDocumentCreator
	{
		
		ExporterPage page;
		
		public FlowDocumentCreator()
		{
			Console.WriteLine("FlowDocumentCreator :Constructor");
		}
		
	//	http://stackoverflow.com/questions/3671724/wpf-flowdocument-page-break-positioning
		public Block CreatePage (ExporterPage page)
		{
			Console.WriteLine("FlowDocumentCreator :CreatePage");
			this.page = page;
			Paragraph p = new Paragraph();
			
			Run r = new Run("Hallo");
			p.Inlines.Add(r);
			
			return p;
		}
		
	}
}
