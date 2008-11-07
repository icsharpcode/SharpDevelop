// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Gui
{
	sealed class CaretAdorner : FrameworkElement
	{
		TextView textView;
		bool isVisible;
		Rect caretRectangle;
		
		DoubleAnimationUsingKeyFrames blinkAnimation;
		
		public CaretAdorner(TextView textView)
		{
			this.textView = textView;
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
		
		protected override Size MeasureOverride(Size constraint)
		{
			return caretRectangle.Size;
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (isVisible) {
				drawingContext.DrawRectangle(Brushes.Black, null,
				                             new Rect(caretRectangle.X - textView.HorizontalOffset,
				                                      caretRectangle.Y - textView.VerticalOffset,
				                                      caretRectangle.Width,
				                                      caretRectangle.Height));
			}
		}
	}
}
