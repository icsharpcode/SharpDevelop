// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Gray out everything except a specific area.
	/// </summary>
	sealed class GrayOutDesignerExceptActiveArea : FrameworkElement
	{
		//Geometry infiniteRectangle = new RectangleGeometry(new Rect(0, 0, double.PositiveInfinity, double.PositiveInfinity));
		Geometry infiniteRectangle = new RectangleGeometry(new Rect(0, 0, 10000, 10000));
		Geometry activeAreaGeometry;
		Geometry combinedGeometry;
		Brush grayOutBrush;
		
		public GrayOutDesignerExceptActiveArea()
		{
			Color c = SystemColors.GrayTextColor;
			c.R = (byte)Math.Max(0, c.R - 20);
			c.G = (byte)Math.Max(0, c.G - 20);
			c.B = (byte)Math.Max(0, c.B - 20);
			c.A = 30;
			this.GrayOutBrush = new SolidColorBrush(c);
		}
		
		public Brush GrayOutBrush {
			get { return grayOutBrush; }
			set { grayOutBrush = value; }
		}
		
		public Geometry ActiveAreaGeometry {
			get { return activeAreaGeometry; }
			set {
				activeAreaGeometry = value;
				combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, infiniteRectangle, activeAreaGeometry);
			}
		}
		
		public void SetActiveArea(UIElement activeContainer)
		{
			this.ActiveAreaGeometry = new RectangleGeometry(new Rect(activeContainer.RenderSize), 0, 0, (Transform)activeContainer.TransformToVisual(this));
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(grayOutBrush, null, combinedGeometry);
		}
		
		internal static void Start(ref GrayOutDesignerExceptActiveArea grayOut, IDesignPanel designPanel, UIElement activeContainer)
		{
			if (designPanel != null) {
				grayOut = new GrayOutDesignerExceptActiveArea();
				designPanel.MarkerCanvas.Children.Add(grayOut);
				grayOut.SetActiveArea(activeContainer);
			}
		}
		
		internal static void Stop(ref GrayOutDesignerExceptActiveArea grayOut, IDesignPanel designPanel)
		{
			if (grayOut != null) {
				designPanel.MarkerCanvas.Children.Remove(grayOut);
				grayOut = null;
			}
		}
	}
}
