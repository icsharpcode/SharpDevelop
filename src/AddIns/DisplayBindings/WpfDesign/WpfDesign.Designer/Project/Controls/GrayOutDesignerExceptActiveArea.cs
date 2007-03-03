// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Gray out everything except a specific area.
	/// </summary>
	sealed class GrayOutDesignerExceptActiveArea : FrameworkElement
	{
		//Geometry infiniteRectangle = new RectangleGeometry(new Rect(0, 0, double.PositiveInfinity, double.PositiveInfinity));
		Geometry infiniteRectangle = new RectangleGeometry(new Rect(-100, -100, 10000, 10000));
		Geometry activeAreaGeometry;
		Geometry combinedGeometry;
		Brush grayOutBrush;
		AdornerPanel adornerPanel;
		IDesignPanel designPanel;
		const double MaxOpacity = 0.3;
		
		public GrayOutDesignerExceptActiveArea()
		{
			this.GrayOutBrush = new SolidColorBrush(SystemColors.GrayTextColor);
			this.GrayOutBrush.Opacity = MaxOpacity;
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
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(grayOutBrush, null, combinedGeometry);
		}
		
		internal static void Start(ref GrayOutDesignerExceptActiveArea grayOut, IDesignPanel designPanel, UIElement activeContainer)
		{
			Debug.Assert(activeContainer != null);
			if (designPanel != null && grayOut == null) {
				grayOut = new GrayOutDesignerExceptActiveArea();
				grayOut.designPanel = designPanel;
				grayOut.adornerPanel = new AdornerPanel();
				grayOut.adornerPanel.Order = AdornerOrder.BehindForeground;
				grayOut.adornerPanel.SetAdornedElement(designPanel.Context.RootItem.View, null);
				grayOut.adornerPanel.Children.Add(grayOut);
				grayOut.ActiveAreaGeometry = new RectangleGeometry(new Rect(activeContainer.RenderSize), 0, 0, (Transform)activeContainer.TransformToVisual(grayOut.adornerPanel.AdornedElement));
				Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, 0, MaxOpacity);
				designPanel.Adorners.Add(grayOut.adornerPanel);
			}
		}
		
		static readonly TimeSpan animationTime = new TimeSpan(2000000);
		
		static void Animate(Animatable element, DependencyProperty property, double from, double to)
		{
			element.BeginAnimation(property, new DoubleAnimation(from, to, new Duration(animationTime), FillBehavior.Stop));
		}
		
		internal static void Stop(ref GrayOutDesignerExceptActiveArea grayOut)
		{
			if (grayOut != null) {
				Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, MaxOpacity, 0);
				IDesignPanel designPanel = grayOut.designPanel;
				AdornerPanel adornerPanelToRemove = grayOut.adornerPanel;
				DispatcherTimer timer = new DispatcherTimer();
				timer.Interval = animationTime;
				timer.Tick += delegate {
					timer.Stop();
					designPanel.Adorners.Remove(adornerPanelToRemove);
				};
				timer.Start();
				grayOut = null;
			}
		}
	}
}
