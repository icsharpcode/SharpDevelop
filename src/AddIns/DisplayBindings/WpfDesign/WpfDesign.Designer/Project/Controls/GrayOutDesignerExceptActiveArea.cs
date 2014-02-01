// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Gray out everything except a specific area.
	/// </summary>
	sealed class GrayOutDesignerExceptActiveArea : FrameworkElement
	{
		Geometry designSurfaceRectangle;
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
			this.IsHitTestVisible = false;
		}
		
		public Brush GrayOutBrush {
			get { return grayOutBrush; }
			set { grayOutBrush = value; }
		}
		
		public Geometry ActiveAreaGeometry {
			get { return activeAreaGeometry; }
			set {
				activeAreaGeometry = value;
				combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, designSurfaceRectangle, activeAreaGeometry);
			}
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(grayOutBrush, null, combinedGeometry);
		}
		
		Rect currentAnimateActiveAreaRectToTarget;
		
		internal void AnimateActiveAreaRectTo(Rect newRect)
		{
			if (newRect.Equals(currentAnimateActiveAreaRectToTarget))
				return;
			activeAreaGeometry.BeginAnimation(
				RectangleGeometry.RectProperty,
				new RectAnimation(newRect, new Duration(new TimeSpan(0,0,0,0,100))),
				HandoffBehavior.SnapshotAndReplace);
			currentAnimateActiveAreaRectToTarget = newRect;
		}
		
		internal static void Start(ref GrayOutDesignerExceptActiveArea grayOut, ServiceContainer services, UIElement activeContainer)
		{
			Debug.Assert(activeContainer != null);
			Start(ref grayOut, services, activeContainer, new Rect(activeContainer.RenderSize));
		}
		
		internal static void Start(ref GrayOutDesignerExceptActiveArea grayOut, ServiceContainer services, UIElement activeContainer, Rect activeRectInActiveContainer)
		{
			Debug.Assert(services != null);
			Debug.Assert(activeContainer != null);
			DesignPanel designPanel = services.GetService<IDesignPanel>() as DesignPanel;
			OptionService optionService = services.GetService<OptionService>();
			if (designPanel != null && grayOut == null && optionService != null && optionService.GrayOutDesignSurfaceExceptParentContainerWhenDragging) {
				grayOut = new GrayOutDesignerExceptActiveArea();
				grayOut.designSurfaceRectangle = new RectangleGeometry(
					new Rect(0, 0, ((Border)designPanel.Child).Child.RenderSize.Width, ((Border)designPanel.Child).Child.RenderSize.Height));
				grayOut.designPanel = designPanel;
				grayOut.adornerPanel = new AdornerPanel();
				grayOut.adornerPanel.Order = AdornerOrder.BehindForeground;
				grayOut.adornerPanel.SetAdornedElement(designPanel.Context.RootItem.View, null);
				grayOut.adornerPanel.Children.Add(grayOut);
				grayOut.ActiveAreaGeometry = new RectangleGeometry(activeRectInActiveContainer, 0, 0, (Transform)activeContainer.TransformToVisual(grayOut.adornerPanel.AdornedElement));
				Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, MaxOpacity);
				designPanel.Adorners.Add(grayOut.adornerPanel);
			}
		}
		
		static readonly TimeSpan animationTime = new TimeSpan(2000000);
		
		static void Animate(Animatable element, DependencyProperty property, double to)
		{
			element.BeginAnimation(property, new DoubleAnimation(to, new Duration(animationTime), FillBehavior.HoldEnd),
			                       HandoffBehavior.SnapshotAndReplace);
		}
		
		internal static void Stop(ref GrayOutDesignerExceptActiveArea grayOut)
		{
			if (grayOut != null) {
				Animate(grayOut.GrayOutBrush, Brush.OpacityProperty, 0);
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
