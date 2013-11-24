/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.04.2013
 * Time: 18:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Markup;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter{
	/// <summary>
	/// Description of PrintExporter.
	/// </summary>
	/// 
	class WpfExporter:BaseExporter {
		
		readonly WpfVisitor visitor;
		
		public WpfExporter(Collection<ExportPage> pages):base(pages){
			visitor = new WpfVisitor();
		}
		
		
		public override void Run () {
			Document = new FixedDocument();
			foreach (var page in Pages) {
				IAcceptor acceptor = page as IAcceptor;
				if (acceptor != null) {
					visitor.Visit(page);
				}
				AddPageToDocument(Document,visitor.FixedPage);
			}
		}
		
		
		static void AddPageToDocument(FixedDocument fixedDocument,FixedPage page){
			PageContent pageContent = new PageContent();
			((IAddChild)pageContent).AddChild(page);
			
			fixedDocument.Pages.Add(pageContent);
		}
		
		public FixedDocument Document {get;private set;}
	}
}
