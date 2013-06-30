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
				ShowContainerRecursive_2(null,item);
			}
		}

		void ShowContainerRecursive (string parentCanvas,IExportColumn item){
			var exportContainer = item as IExportContainer;
			string name = string.Empty;
			
			Console.WriteLine("perform {0}",parentCanvas);
			
			if (exportContainer != null) {
				
				if (exportContainer.Name =="ReportDetail") {
					Console.WriteLine(item.Name);
				}
//
				if (exportContainer.Name == "Row") {
					Console.WriteLine(item.Name);
				}
				name = item.Name;
//				var containerCanvas = CreateContainer(exportContainer);

//				fixedPage.Children.Add(containerCanvas);
//				Console.WriteLine(fixedPage.Children.Count);
//				Console.WriteLine(containerCanvas.Children.Count);
				
				foreach (var element in exportContainer.ExportedItems) {
					var el = element as IExportContainer;
					if (el == null) {
//						CreateSingleEntry(ref containerCanvas, element);
//						Console.WriteLine(containerCanvas.Children.Count);
						name = element.Name;
					}
					
					ShowContainerRecursive(name,element);
				}
			}
		}
		
		void ShowContainerRecursive_2(Canvas parentCanvas,IExportColumn item)
		{
			var exportContainer = item as IExportContainer;
			
			if (exportContainer != null) {
				
				if (exportContainer.Name =="ReportDetail") {
					Console.WriteLine(item.Name);
				}
//
				if (exportContainer.Name == "Row") {
					Console.WriteLine("\t {0}",item.Name);
				}
				
				foreach (var element in exportContainer.ExportedItems) {
//					Console.WriteLine(element.Name);
					var el = element as IExportContainer;
					if (el == null) {
						Console.WriteLine("\t\t {0}",element.Name);
					}
					ShowContainerRecursive_2(null,exportContainer.ExportedItems[0]);
				}
				
			}
			
			
		}
		
		
		
		void ShowContainerRecursive_1(Canvas parentCanvas,IExportColumn item)
		{
			var exportContainer = item as IExportContainer;
			
			if (exportContainer != null) {
				
				if (exportContainer.Name =="ReportDetail") {
					Console.WriteLine(item.Name);
				}
//
				if (exportContainer.Name == "Row") {
					Console.WriteLine(item.Name);
				}
				
				var containerCanvas = CreateContainer(exportContainer);

				fixedPage.Children.Add(containerCanvas);
				Console.WriteLine(fixedPage.Children.Count);
				Console.WriteLine(containerCanvas.Children.Count);
				
				foreach (var element in exportContainer.ExportedItems) {
					var el = element as IExportContainer;
					if (el == null) {
						CreateSingleEntry(ref containerCanvas, element);
						Console.WriteLine(containerCanvas.Children.Count);
					}
					
					ShowContainerRecursive_1(containerCanvas,element);
				}
			}
		}

		
		Canvas CreateContainer(IExportContainer exportContainer)
		{
			var containerAcceptor = exportContainer as IAcceptor;
			containerAcceptor.Accept(visitor);
			var containerCanvas = (Canvas)visitor.UIElement;
			return containerCanvas;
		}

		
		void CreateSingleEntry(ref Canvas canvas, IExportColumn element)
		{
			var v = element as IAcceptor;
			v.Accept(visitor);
			var c = visitor.UIElement;

			CanvasHelper.SetLeft(c,element.Location.X);
			CanvasHelper.SetTop(c,10);
//			CanvasHelper.SetTop(c,element.Location.Y);
			canvas.AddChild(c);
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
