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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of PrintExporter.
	/// </summary>
	public class WpfExporter:BaseExporter
	{
		private WpfVisitor visitor;
		private ReportSettings reportSettings;
		
		public WpfExporter(ReportSettings reportSettings,Collection<IPage> pages):base(pages)
		{
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			this.reportSettings = reportSettings;
			visitor = new WpfVisitor(reportSettings);
		}
		

		public override void Run () {
			Document = new FixedDocument();
			
			foreach (var page in Pages) {
				var fixedPage = InternalRun(page);
				AddPageToDocument(Document,fixedPage);
			}
		}
		

		FixedPage InternalRun(IExportContainer container)
		{
			
			FixedPage fixedPage = CreateFixedPage();
			Canvas canvas = null ;
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (acceptor != null) {
						acceptor.Accept(visitor);
						canvas = (Canvas)visitor.UIElement;
						fixedPage.Children.Add(canvas);
						foreach (IAcceptor element in exportContainer.ExportedItems) {
							element.Accept(visitor);
							var ui = visitor.UIElement;
							Canvas.SetLeft(ui,((IExportColumn)element).Location.X);
							Canvas.SetTop(ui, ((IExportColumn)element).Location.Y);
							canvas.Children.Add(ui);
						}
					} else {
						if (acceptor != null) {
							acceptor.Accept(visitor);
							var uiElement = visitor.UIElement;
							if (canvas != null) {
								Canvas.SetLeft(uiElement, item.Location.X - exportContainer.Location.X);
								Canvas.SetTop(uiElement, item.Location.Y - exportContainer.Location.Y);
								canvas.Children.Add(uiElement);
							}
							fixedPage.Children.Add(uiElement);
						}
					}
				}
			}
			return fixedPage;
		}

		 FixedPage CreateFixedPage()
		{
			var fixedPage = new FixedPage();
			fixedPage.Width = reportSettings.PageSize.Width;
			fixedPage.Height = reportSettings.PageSize.Height;
			return fixedPage;
		}
		
		
		static void AddPageToDocument(FixedDocument fixedDocument,FixedPage page)
		{
			PageContent pageContent = new PageContent();
			((IAddChild)pageContent).AddChild(page);
			fixedDocument.Pages.Add(pageContent);
		}
		
		public FixedDocument Document {get;private set;}
	}
}
