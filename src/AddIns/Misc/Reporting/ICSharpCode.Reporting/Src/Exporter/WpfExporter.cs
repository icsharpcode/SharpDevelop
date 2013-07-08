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
		FixedPage fixedPage;
		
		public WpfExporter(ReportSettings reportSettings,Collection<ExportPage> pages):base(pages)
		{
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			this.reportSettings = reportSettings;
			visitor = new WpfVisitor(reportSettings);
		}
		

		public override void Run () {
			Document = new FixedDocument();
			
			foreach (var page in Pages) {
				InternalRun(page);
				AddPageToDocument(Document,fixedPage);
			}
		}
		
		void InternalRun(ExportPage page)
		{
			page.Accept(visitor);
			fixedPage = (FixedPage)visitor.UIElement;
			foreach (var item in page.ExportedItems) {
				ShowContainerRecursive(null,item);
			}
		}

		
		void ShowContainerRecursive(Canvas parentCanvas,IExportColumn item)
		{
			var exportContainer = item as IExportContainer;

			Canvas containerCanvas = null;
			
			if (exportContainer != null) {
				
				if (exportContainer.Name =="ReportDetail") {
					Console.WriteLine(item.Name);
				}

				if (exportContainer.Name == "Row") {
					Console.WriteLine(item.Name);
					
				}
				
				if (parentCanvas == null) {
					containerCanvas = CreateContainer(fixedPage,exportContainer);
					Console.WriteLine("Section {0}  at {1}",item.Name,CanvasHelper.GetPosition(containerCanvas));
					fixedPage.Children.Add(containerCanvas);
					parentCanvas = containerCanvas;
				} else {
					containerCanvas = CreateContainer(parentCanvas,exportContainer);
					Console.WriteLine("Row {0}  at {1}",item.Name,CanvasHelper.GetPosition(containerCanvas));
					parentCanvas.Children.Add(containerCanvas);
				}
				
				Console.WriteLine("canvas at {0}",CanvasHelper.GetPosition(containerCanvas));
				
				foreach (var element in exportContainer.ExportedItems) {
					var el = element as IExportContainer;
					if (el == null) {
						var t = CreateSingleEntry(parentCanvas,element);
						containerCanvas.Children.Add(t);
					}									
					ShowContainerRecursive(parentCanvas,element);
				}
			}
		}
		
		
//		http://stackoverflow.com/questions/4523208/wpf-positioning-uielement-on-a-canvas
//http://stackoverflow.com/questions/1123101/changing-position-of-an-element-programmatically-in-wpf
//http://stackoverflow.com/questions/1923697/how-can-i-get-the-position-of-a-child-element-relative-to-a-parent		
		
		
		Canvas CreateContainer(UIElement parent,IExportContainer exportContainer)
		{
			var containerAcceptor = exportContainer as IAcceptor;
			containerAcceptor.Accept(visitor);
			var containerCanvas = (Canvas)visitor.UIElement;
//			Console.WriteLine("CreateContainer bevore {0}",CanvasHelper.GetPosition(containerCanvas));
			CanvasHelper.SetPosition(containerCanvas,
			                         new Point(exportContainer.Location.X,exportContainer.Location.Y));
//			Console.WriteLine("CreateContainer after {0}",CanvasHelper.GetPosition(containerCanvas));
			return containerCanvas;
		}

		
		UIElement CreateSingleEntry(UIElement parent, IExportColumn element)
		{
			var v = element as IAcceptor;
			v.Accept(visitor);
			var c = visitor.UIElement;

//			CanvasHelper.SetLeft(c,element.Location.X);
//			CanvasHelper.SetTop(c,10);
			CanvasHelper.SetPosition(c,new Point(element.Location.X,element.Location.Y));
				Console.WriteLine("CreateSingleEntry after {0}",CanvasHelper.GetPosition(c));
			return c;
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
