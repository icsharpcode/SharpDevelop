using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SharpDevelop.XamlDesigner.Controls
{
	public static class PixelSnapper
	{
		static PixelSnapper()
		{
			Application.Current.MainWindow.LayoutUpdated += new EventHandler(SnapHelper_LayoutUpdated);
		}

		static HashSet<WeakReference> elements = new HashSet<WeakReference>();

		public static void SetSnap(UIElement target, bool value)
		{
			elements.Add(new WeakReference(target));
		}

		static void SnapHelper_LayoutUpdated(object sender, EventArgs e)
		{
			foreach (var reference in elements) {
				if (reference.IsAlive) {
					Snap(reference.Target as UIElement);
				}
			}
		}

		static void Snap(UIElement target)
		{
			var ps = PresentationSource.FromVisual(target);
			if (ps == null) return;

			var matrix = (target.TransformToVisual(ps.RootVisual) as MatrixTransform).Matrix;
			Point p = new Point(matrix.OffsetX, matrix.OffsetY);

			double deltaX = Math.Round(p.X) - p.X;
			double deltaY = Math.Round(p.Y) - p.Y;

			if (deltaX != 0 || deltaY != 0) {
				var tr = target.RenderTransform as TranslateTransform;
				if (tr == null) {
					tr = new TranslateTransform();
					target.RenderTransform = tr;
				}
				tr.X = (tr.X + deltaX) - Math.Truncate(tr.X + deltaX);
				tr.Y = (tr.Y + deltaY) - Math.Truncate(tr.Y + deltaY);
			}
		}
	}
}
