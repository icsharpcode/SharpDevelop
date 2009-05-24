// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	sealed class SelectionLayer : Layer, IWeakEventListener
	{
		readonly TextArea textArea;
		
		public SelectionLayer(TextArea textArea) : base(textArea.TextView, KnownLayer.Selection)
		{
			this.IsHitTestVisible = false;
			
			this.textArea = textArea;
			TextViewWeakEventManager.VisualLinesChanged.AddListener(textView, this);
			TextViewWeakEventManager.ScrollOffsetChanged.AddListener(textView, this);
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextViewWeakEventManager.VisualLinesChanged)
			    || managerType == typeof(TextViewWeakEventManager.ScrollOffsetChanged))
			{
				InvalidateVisual();
				return true;
			}
			return false;
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			
			BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
			foreach (var segment in textArea.Selection.Segments) {
				geoBuilder.AddSegment(textView, segment);
			}
			PathGeometry geometry = geoBuilder.CreateGeometry();
			if (geometry != null) {
				SolidColorBrush lightHighlightBrush = new SolidColorBrush(SystemColors.HighlightColor);
				lightHighlightBrush.Opacity = 0.7;
				lightHighlightBrush.Freeze();
				Pen pen = new Pen(SystemColors.HighlightBrush, 1);
				//pen.LineJoin = PenLineJoin.Round;
				pen.Freeze();
				drawingContext.DrawGeometry(lightHighlightBrush, pen, geometry);
			}
		}
	}
}
