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

using ICSharpCode.Reporting.ExportRenderer;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of WpfVisitor.
	/// </summary>
	class WpfVisitor: AbstractVisitor
	{
		private readonly FixedDocumentCreator documentCreator;
		
		public WpfVisitor()
		{
			documentCreator = new FixedDocumentCreator();
		}
		
		
		public override void Visit(ExportPage page)
		{
			UIElement = FixedDocumentCreator.CreateFixedPage(page);
		}
	
		
		public override void Visit(ExportContainer exportColumn)
		{
			var canvas = (Canvas)documentCreator.CreateContainer(exportColumn);
			CanvasHelper.SetPosition(canvas,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			UIElement = canvas;
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			TextBlock textBlock = documentCreator.CreateTextBlock(exportColumn);
			CanvasHelper.SetPosition(textBlock,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			UIElement = textBlock;
		}
		
		
		public UIElement UIElement {get; private set;}
	}
}
