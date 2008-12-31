using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDevelop.XamlDesigner.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner
{
	class PointerTool : Tool
	{
		public static PointerTool Instance = new PointerTool();

		SelectionFrame frame;
		Rect frameRect;

		SelectionCollection Selection
		{
			get { return DesignView.Context.Selection; }
		}

		public override void OnMouseClick(AdvancedDragEventArgs e)
		{
			var result = DesignView.HitTest(e.StartPoint).FirstOrDefault();
			Selection.Auto(result);
		}

		public override void OnDragStarted(AdvancedDragEventArgs e)
		{
			frame = new SelectionFrame();
			DesignView.FeedbackLayer.Children.Add(frame);
		}

		public override void OnDragDelta(AdvancedDragEventArgs e)
		{
			frameRect = new Rect(
				e.StartPoint.X + (e.Delta.X < 0 ? e.Delta.X : 0),
				e.StartPoint.Y + (e.Delta.Y < 0 ? e.Delta.Y : 0),
				Math.Abs(e.Delta.X),
				Math.Abs(e.Delta.Y));

			Canvas.SetLeft(frame, frameRect.X);
			Canvas.SetTop(frame, frameRect.Y);
			frame.Width = frameRect.Width;
			frame.Height = frameRect.Height;
		}

		public override void OnDragCompleted(AdvancedDragEventArgs e)
		{
			DesignView.FeedbackLayer.Children.Remove(frame);
			if (!e.IsCancel) {
				var result = DesignView.HitTest(frameRect);
				Selection.Auto(result);
			}
		}
	}
}
