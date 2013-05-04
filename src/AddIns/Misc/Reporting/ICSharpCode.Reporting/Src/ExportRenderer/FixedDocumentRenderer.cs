/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05/04/2013
 * Time: 19:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Windows.Documents;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.ExportRenderer
{
	/// <summary>
	/// Description of FixedDocumentRenderer.
	/// </summary>
	public class FixedDocumentRenderer
	{
		ReportSettings reportSettings;
			private FixedDocument document ;
//		private FixedDocumentCreator docCreator;
		
		public FixedDocumentRenderer(ReportSettings reportSettings,Collection<IPage> pages)
		{
			if (pages == null)
				throw new ArgumentNullException("pages");
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			this.reportSettings = reportSettings;
			Pages = pages;
			Console.WriteLine("FixedDocumentRenderer with {0} pages ",Pages.Count);
		}
		
		
		public  void Start()
		{
			Console.WriteLine("FixedDocumentrenderer - Start");
			document = new FixedDocument();
//			docCreator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,reportSettings.PageSize.Height);
//			document.DocumentPaginator.PageSize = docCreator.PageSize;
		}
		
		public  void RenderOutput(){
			Console.WriteLine("FixedDocumentrenderer - RenderOutput");
		}
		
		public  void End()
		{
			Console.WriteLine("FixedDocumentrenderer - End");
		}
		
		public Collection<IPage> Pages {get;private set;}
	}
}
