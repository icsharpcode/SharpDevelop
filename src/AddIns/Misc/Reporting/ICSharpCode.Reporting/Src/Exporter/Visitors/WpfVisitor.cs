/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.05.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.Reporting.ExportRenderer;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of WpfVisitor.
	/// </summary>
	/// 
	class WpfVisitor: AbstractVisitor {
		
		private readonly FixedDocumentCreator documentCreator;
		FixedPage fixedPage;
		Canvas currentCanvas;
		
		public WpfVisitor()
		{
			documentCreator = new FixedDocumentCreator();
		}
		
		public override void Visit(ExportPage page)
		{
			Console.WriteLine("WpfVisitor page <{0}>",page.PageInfo.PageNumber);
			fixedPage = FixedDocumentCreator.CreateFixedPage(page);
			FixedPage = fixedPage;
			foreach (var element in page.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		public override void Visit(ExportContainer exportColumn)
		{
			
			Console.WriteLine("\tWpfVisitor <{0}>",exportColumn.Name);
			currentCanvas = documentCreator.CreateContainer(exportColumn);
			CanvasHelper.SetPosition(currentCanvas,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
			fixedPage.Children.Add(currentCanvas);
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			Console.WriteLine("\t\tExpressionVisitor <{0}>",exportColumn.Name);
			var textBlock = documentCreator.CreateTextBlock(exportColumn);
			CanvasHelper.SetPosition(textBlock,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			currentCanvas.Children.Add(textBlock);
		}
		
		public FixedPage FixedPage {get; private set;}
	}
}
