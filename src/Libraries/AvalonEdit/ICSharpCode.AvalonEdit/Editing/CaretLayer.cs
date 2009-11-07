// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
	sealed class CaretLayer : Layer
	{
		bool isVisible;
		Rect caretRectangle;
		
		DoubleAnimationUsingKeyFrames blinkAnimation;
		
		public CaretLayer(TextView textView) : base(textView, KnownLayer.Caret)
		{
			this.IsHitTestVisible = false;
			
			blinkAnimation = new DoubleAnimationUsingKeyFrames();
			blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1, KeyTime.FromPercent(0)));
			blinkAnimation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromPercent(0.5)));
			blinkAnimation.RepeatBehavior = RepeatBehavior.Forever;
		}
		
		public void Show(Rect caretRectangle)
		{
			this.caretRectangle = caretRectangle;
			this.isVisible = true;
			InvalidateVisual();
			StartBlinkAnimation();
		}
		
		public void Hide()
		{
			if (isVisible) {
				isVisible = false;
				StopBlinkAnimation();
				InvalidateVisual();
			}
		}
		
		void StartBlinkAnimation()
		{
			TimeSpan blinkTime = Win32.CaretBlinkTime;
			if (blinkTime.TotalMilliseconds >= 0) {
				BeginAnimation(OpacityProperty, null);
				// duration = 2*blink time
				blinkAnimation.Duration = new Duration(blinkTime + blinkTime);
				BeginAnimation(OpacityProperty, blinkAnimation);
			}
		}
		
		void StopBlinkAnimation()
		{
			BeginAnimation(OpacityProperty, null);
		}
		
		internal Brush CaretBrush;
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (isVisible) {
				Brush caretBrush = this.CaretBrush;
				if (caretBrush == null)
					caretBrush = (Brush)textView.GetValue(TextBlock.ForegroundProperty);
				Rect r = new Rect(caretRectangle.X - textView.HorizontalOffset,
				                  caretRectangle.Y - textView.VerticalOffset,
				                  caretRectangle.Width,
				                  caretRectangle.Height);
				drawingContext.DrawRectangle(caretBrush, null, PixelSnapHelpers.Round(r));
			}
		}
	}
}
