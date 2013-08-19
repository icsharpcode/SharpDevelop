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
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter
{
	/// <summary>
	/// Description of PrintExporter.
	/// </summary>
	public class WpfExporter:BaseExporter
	{
		WpfVisitor visitor;
		FixedPage fixedPage;
		
		public WpfExporter(Collection<ExportPage> pages):base(pages)
		{
			visitor = new WpfVisitor();
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
					Console.WriteLine("Section {0}",item.Name);
					exportContainer.BackColor = System.Drawing.Color.LightBlue;
				}

				if (exportContainer.Name == "Row") {
					Console.WriteLine(item.Name);
					
				}
				
				if (parentCanvas == null) {
					containerCanvas = CreateContainer(fixedPage,exportContainer);
					Console.WriteLine("Section {0}  at {1} size {2}",item.Name,CanvasHelper.GetPosition(containerCanvas),containerCanvas.DesiredSize);
					fixedPage.Children.Add(containerCanvas);
					parentCanvas = containerCanvas;
				} else {
					containerCanvas = CreateContainer(parentCanvas,exportContainer);
					Console.WriteLine("Row {0}  at {1}",item.Name,CanvasHelper.GetPosition(containerCanvas));
					parentCanvas.Children.Add(containerCanvas);
				}
				
//				Console.WriteLine("canvas at {0}",CanvasHelper.GetPosition(containerCanvas));
				
				foreach (var element in exportContainer.ExportedItems) {
					
					if (!IsContainer(element)) {
						
						var singleItem = CreateSingleEntry(containerCanvas,element);
						
//						Console.WriteLine("TEST  {0} - {1}",CanvasHelper.GetPosition(singleItem),CanvasHelper.GetPosition(containerCanvas));
						containerCanvas.Children.Add(singleItem);
					}
					ShowContainerRecursive(parentCanvas,element);
				}
			}
		}
		
		
		bool IsContainer (IExportColumn column) {
			var container = column as IExportContainer;
			if (container == null) {
				return false;
			}
			return true;
		}
		
		Canvas CreateContainer(UIElement parent,IExportContainer exportContainer)
		{
			var containerAcceptor = exportContainer as IAcceptor;
			containerAcceptor.Accept(visitor);
			var canvas = (Canvas)visitor.UIElement;
			return canvas;
		}

		
		UIElement CreateSingleEntry(UIElement parent, IExportColumn element)
		{
			var v = element as IAcceptor;
			v.Accept(visitor);
			var c = visitor.UIElement;

//			Console.WriteLine("CreateSingleEntry after {0}",CanvasHelper.GetPosition(c));
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
