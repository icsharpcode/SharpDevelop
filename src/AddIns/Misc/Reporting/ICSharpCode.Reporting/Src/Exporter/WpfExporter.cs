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
			InitFixedPage();
			foreach (var page in Pages) {
				InternalRun(page);
			}
		}

		
		void InitFixedPage()
		{
			fixedPage = new FixedPage();
			fixedPage.Width = reportSettings.PageSize.Width;
			fixedPage.Height = reportSettings.PageSize.Height;
		}
		
		FixedPage fixedPage;
		
		public FixedPage FixedPage {
			get { return fixedPage; }
		}
		
		
		void InternalRun(IExportContainer container)
		{
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
//						var size = new Size(exportContainer.DesiredSize.Width,exportContainer.DesiredSize.Height);
//						canvas.Measure(size);
//						canvas.Arrange(new Rect(new System.Windows.Point(exportContainer.Location.X,exportContainer.Location.Y),size ));
//						canvas.UpdateLayout();
//						var exportArrange = exportContainer.GetArrangeStrategy();
//						exportArrange.Arrange(exportContainer);
					}
//					InternalRun(item as IExportContainer);
				} else {
					if (acceptor != null) {
						Console.WriteLine("..Item...");
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
	}
}
